using Common.DTO;
using Common.Entities;
using Common;
using Contracts;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;
using TaxiUserData.Settings;
using TaxiUserData.Helpers;
using System.Linq;

[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]

namespace TaxiUserData
{
    internal class UserServiceImpl : IUserDataService, IUserRideService
    {
        private readonly string _dictName;
        private readonly string _queueName;
        private readonly IReliableStateManager StateManager;
        private MailHelper _mailClient;
        private readonly TimeSpan timeout = TimeSpan.FromSeconds(5);

        public UserServiceImpl(string dictName, string queueName, IReliableStateManager stateManager, MailHelper mailClient)
        {
            _dictName = dictName;
            _queueName = queueName;
            StateManager = stateManager;
            _mailClient = mailClient;
        }

        #region User service methods
        public async Task ChangeUserPasswordAsync(UserPasswordChangeRequest userPasswordChangeDTO, CancellationToken cancellationToken)
        {
            if (userPasswordChangeDTO == null)
            {
                throw new ArgumentNullException(nameof(userPasswordChangeDTO));
            }
            User existingUser;
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(tx, _dictName, timeout);
                var user = await users.TryGetValueAsync(tx, userPasswordChangeDTO.UserId, timeout, cancellationToken);
                if (user.HasValue)
                {
                    existingUser = user.Value;
                    // TODO: Better password validation with hashing (bcrypt?)
                    if (existingUser.Password != userPasswordChangeDTO.OldPassword)
                    {
                        throw new ArgumentException("Incorrect old password!");
                    }
                    existingUser.Password = userPasswordChangeDTO.NewPassword;
                    existingUser._UpdatedAt = DateTimeOffset.UtcNow;
                    await users.AddOrUpdateAsync(tx, existingUser.Id, existingUser, (key, value) => existingUser, timeout, cancellationToken);
                    await tx.CommitAsync();
                }
                else
                {
                    throw new KeyNotFoundException("User not found");
                }
            }
            await QueueDataForLaterProcessingAsync(existingUser, cancellationToken);
        }
        public async Task<AuthResponse> ValidateLoginParamsAsync(UserLoginRequest userLoginDTO, CancellationToken cancellationToken)
        {
            if (userLoginDTO == null)
            {
                throw new ArgumentNullException();
            }
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(tx, _dictName, timeout);
                var enumerable = await users.CreateEnumerableAsync(tx);
                var enumerator = enumerable.GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(cancellationToken))
                {
                    var currentUser = enumerator.Current.Value;
                    if (currentUser.Email == userLoginDTO.Email)
                    {
                        // TODO update check to Bcrypt validate
                        if (currentUser.Password == userLoginDTO.Password)
                        {
                            AuthResponse authResponse = new AuthResponse()
                            {
                                UserId = currentUser.Id.ToString(),
                                ProfileImage = currentUser.UserPicture,
                                Username = currentUser.Username,
                                UserRole = currentUser.UserType.ToString()
                            };
                            enumerator.Dispose();
                            return authResponse;
                        }
                    }
                }
                throw new KeyNotFoundException(nameof(User));
            }
        }
        public async Task<IEnumerable<UserInfo>> GetAllAsync(CancellationToken cancellationToken)
        {
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(tx, _dictName, timeout);
                var enumerable = await users.CreateEnumerableAsync(tx);
                var enumerator = enumerable.GetAsyncEnumerator();
                List<UserInfo> result = new();
                while (await enumerator.MoveNextAsync(cancellationToken))
                {
                    var currentUser = enumerator.Current.Value;
                    if (currentUser.UserType == UserType.Admin)
                        continue;
                    result.Add(currentUser.AsInfoDTO());
                }
                enumerator.Dispose();
                return result;
            }
        }
        public async Task<IEnumerable<UserInfo>> GetAllUnverifiedAsync(CancellationToken cancellationToken)
        {
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(tx, _dictName, timeout);
                var enumerable = await users.CreateEnumerableAsync(tx);
                var enumerator = enumerable.GetAsyncEnumerator();
                List<UserInfo> result = new();
                while (await enumerator.MoveNextAsync(cancellationToken))
                {
                    var currentUser = enumerator.Current.Value;
                    if (currentUser.UserState == UserState.Unverified)
                    {
                        result.Add(currentUser.AsInfoDTO());
                    }
                }
                enumerator.Dispose();
                return result;
            }
        }
        public async Task<UserInfo> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(tx, _dictName, timeout);
                var user = await users.TryGetValueAsync(tx, id, timeout, cancellationToken);
                if (user.HasValue)
                {
                    var existingUser = user.Value;
                    return existingUser.AsInfoDTO();
                }
                else
                {
                    throw new KeyNotFoundException("User not found");
                }
            }
        }
        public async Task<UserStateResponse> GetUserStateAsync(Guid id, CancellationToken cancellationToken)
        {
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(tx, _dictName, timeout);
                var user = await users.TryGetValueAsync(tx, id, timeout, cancellationToken);
                if (user.HasValue)
                {
                    var existingUser = user.Value;
                    return existingUser.AsStateDTO();
                }
                else
                {
                    throw new KeyNotFoundException("User not found");
                }
            }
        }
        public async Task<Guid> RegisterNewUserAsync(RegisterUserRequest registerUserDTO, CancellationToken cancellationToken)
        {
            if (registerUserDTO == null)
            {
                throw new ArgumentNullException(nameof(registerUserDTO));
            }
            // Check if user with provided username or email already exists
            User? user = null;
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(tx, _dictName, timeout);
                if (await userAlreadyExists(registerUserDTO, users, tx, cancellationToken))
                {
                    throw new ArgumentException("User already exists");
                }
                user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = registerUserDTO.Username,
                    Email = registerUserDTO.Email,
                    Password = registerUserDTO.Password,
                    Address = registerUserDTO.Address,
                    DateOfBirth = registerUserDTO.DateOfBirth,
                    Fullname = registerUserDTO.Fullname,
                    UserPicture = registerUserDTO.UserPicture,
                    UserType = registerUserDTO.UserType,
                    UserState = registerUserDTO.UserType == UserType.Driver ? UserState.Unverified : UserState.Default,
                    _CreatedAt = DateTimeOffset.UtcNow
                };
                await users.AddAsync(tx, user.Id, user, timeout, cancellationToken);
                await tx.CommitAsync();
            }
            if (user != null)
            {
                await QueueDataForLaterProcessingAsync(user, cancellationToken);
            }
            return user != null ? user.Id : Guid.Empty;
        }

        private async Task<bool> userAlreadyExists(RegisterUserRequest registerUserDTO, IReliableDictionary<Guid, User> users, ITransaction tx, CancellationToken cancellationToken)
        {
            var enumerable = await users.CreateEnumerableAsync(tx);
            var enumerator = enumerable.GetAsyncEnumerator();
            while (await enumerator.MoveNextAsync(cancellationToken))
            {
                var currentUser = enumerator.Current.Value;
                if (currentUser.Username.Equals( registerUserDTO.Username,StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
                if (currentUser.Email.Equals(registerUserDTO.Email,StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public async Task UpdateUserAsync(UpdateUserRequest updateUserDTO, CancellationToken cancellationToken)
        {
            if (updateUserDTO == null)
            {
                throw new ArgumentNullException(nameof(updateUserDTO));
            }
            User existingUser;
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(tx, _dictName, timeout);
                var user = await users.TryGetValueAsync(tx, updateUserDTO.Id, timeout, cancellationToken);
                if (user.HasValue)
                {
                    existingUser = user.Value;
                    existingUser.Username = updateUserDTO.Username ?? existingUser.Username;
                    existingUser.Email = updateUserDTO.Email ?? existingUser.Email;
                    existingUser.Address = updateUserDTO.Address ?? existingUser.Address;
                    existingUser.DateOfBirth = updateUserDTO.DateOfBirth ?? existingUser.DateOfBirth;
                    existingUser.Fullname = updateUserDTO.Fullname ?? existingUser.Fullname;
                    existingUser.UserPicture = updateUserDTO.UserPicture ?? existingUser.UserPicture;
                    existingUser._UpdatedAt = DateTimeOffset.UtcNow;
                    await users.AddOrUpdateAsync(tx, existingUser.Id, existingUser, (key, value) => existingUser, timeout, cancellationToken);
                    await tx.CommitAsync();
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }
            await QueueDataForLaterProcessingAsync(existingUser, cancellationToken);
        }
        public async Task DeleteUserAsync(Guid id, CancellationToken cancellationToken)
        {
            User user;
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(tx, _dictName, timeout);
                var res = await users.TryRemoveAsync(tx, id, timeout, cancellationToken);
                if (!res.HasValue)
                {
                    throw new KeyNotFoundException(nameof(User));
                }
                user = res.Value;
                user.Username = "toDelete";
                await tx.CommitAsync();
            }
            await QueueDataForLaterProcessingAsync(user, cancellationToken);
        }
        public async Task VerifyUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            User existingUser;
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(tx, _dictName, timeout);
                var user = await users.TryGetValueAsync(tx, userId, timeout, cancellationToken);
                if (user.HasValue)
                {
                    existingUser = user.Value;
                    if (existingUser.UserType != UserType.Driver)
                    {
                        throw new ArgumentException("Only drivers can be verified");
                    }
                    if (existingUser.UserState == UserState.Verified)
                    {
                        throw new ArgumentException("Driver is already verified");
                    }
                    existingUser.UserState = UserState.Verified;
                    existingUser._VerifiedAt = DateTimeOffset.UtcNow;
                    existingUser._UpdatedAt = DateTimeOffset.UtcNow;
                    await users.AddOrUpdateAsync(tx, userId, existingUser, (key, value) => existingUser, timeout, cancellationToken);
                    await tx.CommitAsync();
                }
                else
                {
                    throw new KeyNotFoundException(nameof(User));
                }
            }
            //await _mailClient.SendVerificationMailAsync(existingUser.Email, cancellationToken);
            await QueueDataForLaterProcessingAsync(existingUser, CancellationToken.None);
        }
        public async Task BanUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            User existingUser;
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(tx, _dictName, timeout);
                var user = await users.TryGetValueAsync(tx, userId, timeout, cancellationToken);
                if (user.HasValue)
                {
                    existingUser = user.Value;
                    if (existingUser.UserType != UserType.Driver)
                    {
                        throw new ArgumentException("Only drivers can be banned");
                    }
                    if (existingUser.UserState == UserState.Denied)
                    {
                        throw new ArgumentException("Driver is already banned");
                    }
                    existingUser.UserState = UserState.Denied;
                    existingUser._VerifiedAt = null;
                    existingUser._UpdatedAt = DateTimeOffset.UtcNow;
                    await users.AddOrUpdateAsync(tx, userId, existingUser, (key, value) => existingUser, timeout, cancellationToken);
                    await tx.CommitAsync();
                }
                else
                {
                    throw new KeyNotFoundException(nameof(User));
                }
            }
            await QueueDataForLaterProcessingAsync(existingUser, cancellationToken);
        }
        #endregion

        #region User-Ride service methods
        public async Task<bool> DriverExistsAndVerifiedAsync(Guid id, CancellationToken cancellationToken)
        {
            var driver = await GetAsync(id, cancellationToken);
            return driver.UserType == UserType.Driver && driver.UserState == UserState.Verified;
        }
        public async Task<bool> CheckPasengerTypeAsync(Guid id, CancellationToken cancellationToken)
        {
            var passenger = await GetAsync(id, cancellationToken);
            return passenger.UserType == UserType.User;
        }
        #endregion

        private async Task QueueDataForLaterProcessingAsync(User data, CancellationToken cancellationToken)
        {
            using (var tx = StateManager.CreateTransaction())
            {
                var myQueue = await StateManager.GetOrAddAsync<IReliableQueue<User>>(tx, _queueName, timeout);
                await myQueue.EnqueueAsync(tx, data, timeout, cancellationToken);
                await tx.CommitAsync();
            }
        }
    }
}
