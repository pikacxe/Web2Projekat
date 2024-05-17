using System.Fabric;
using Contracts;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Common.Repository;
using Common.Entities;
using Common.DTO;
using Common;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Data;

namespace TaxiUserData
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class TaxiUserData : StatefulService, IUserDataService
    {
        private readonly string _dictName = "users";
        public TaxiUserData(StatefulServiceContext context)
            : base(context)
        {
        }

        #region User service methods
        public async Task ChangeUserPasswordAsync(UserPasswordChangeRequest userPasswordChangeDTO)
        {
            var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(_dictName);
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var user = await users.TryGetValueAsync(tx, userPasswordChangeDTO.UserId);
                if (user.HasValue)
                {
                    var existingUser = user.Value;
                    // TODO: Better password validation with hashing (bcrypt?)
                    if (existingUser.Password != userPasswordChangeDTO.OldPassword)
                    {
                        throw new ArgumentException("Incorrect old password!");
                    }
                    existingUser.Password = userPasswordChangeDTO.NewPassword;
                    existingUser._UpdatedAt = DateTimeOffset.UtcNow;
                    await users.AddOrUpdateAsync(tx, existingUser.Id, existingUser, (u1, u2) => u2);
                    await tx.CommitAsync();
                }
                else
                {
                    throw new KeyNotFoundException("User not found");
                }
            }
        }

        public async Task<AuthResponse> ValidateLoginParamsAsync(UserLoginRequest userLoginDTO)
        {
            var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(_dictName);
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var enumerable = await users.CreateEnumerableAsync(tx);
                var enumerator = enumerable.GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    var currentUser = enumerator.Current.Value;
                    if (currentUser.Email == userLoginDTO.Email)
                    {
                        // TODO update check to Bcrypt validate
                        if (currentUser.Password == userLoginDTO.Password)
                        {
                            AuthResponse authResponse = new AuthResponse()
                            {
                                UserID = currentUser.Id.ToString(),
                                ProfileImage = currentUser.UserPicture,
                                Username = currentUser.Username,
                                UserRole = currentUser.UserType.ToString()
                            };
                            return authResponse;
                        }
                    }
                }
                throw new KeyNotFoundException(nameof(User));
            }
        }

        public async Task<IEnumerable<UserInfo>> GetAllAsync()
        {
            var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(_dictName);
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var enumerable = await users.CreateEnumerableAsync(tx);
                var enumerator = enumerable.GetAsyncEnumerator();
                List<UserInfo> result = new();
                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    var currentUser = enumerator.Current.Value;
                    result.Add(currentUser.AsInfoDTO());
                }
                enumerator.Dispose();
                return result;
            }
        }

        public async Task<IEnumerable<UserInfo>> GetAllUnverifiedAsync()
        {
            var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(_dictName);
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var enumerable = await users.CreateEnumerableAsync(tx);
                var enumerator = enumerable.GetAsyncEnumerator();
                List<UserInfo> result = new();
                while (await enumerator.MoveNextAsync(CancellationToken.None))
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

        public async Task<UserInfo> GetAsync(Guid id)
        {
            var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(_dictName);
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var user = await users.TryGetValueAsync(tx, id);
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

        public async Task<UserStateResponse> GetUserStateAsync(Guid id)
        {
            var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(_dictName);
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var user = await users.TryGetValueAsync(tx, id);
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

        public async Task RegisterNewUserAsync(RegisterUserRequest registerUserDTO)
        {
            if (registerUserDTO == null)
            {
                throw new ArgumentNullException(nameof(registerUserDTO));
            }
            var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(_dictName);
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                // TODO: validate data
                User user = new User
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
                await users.AddAsync(tx, user.Id, user);
                await tx.CommitAsync();
            }
        }
        public async Task UpdateUserAsync(UserInfo userDTO)
        {
            if (userDTO == null)
            {
                throw new ArgumentNullException(nameof(userDTO));
            }
            var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(_dictName);
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var user = await users.TryGetValueAsync(tx, userDTO.Id);
                if (user.HasValue)
                {
                    var existingUser = user.Value;
                    existingUser.Username = userDTO.Username;
                    existingUser.Email = userDTO.Email;
                    existingUser.Address = userDTO.Address;
                    existingUser.DateOfBirth = userDTO.DateOfBirth;
                    existingUser.Fullname = userDTO.Fullname;
                    existingUser.UserPicture = userDTO.UserPicture;
                    existingUser._UpdatedAt = DateTimeOffset.UtcNow;
                    await users.AddOrUpdateAsync(tx, existingUser.Id, existingUser, (u1, u2) => u2);
                    await tx.CommitAsync();
                }
            }
        }

        public async Task DeleteUserAsync(Guid id)
        {
            var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(_dictName);
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var res = await users.TryRemoveAsync(tx, id);
                if (!res.HasValue)
                {
                    throw new KeyNotFoundException(nameof(User));
                }
                await tx.CommitAsync();
            }
        }


        public async Task VerifyUserAsync(Guid userId)
        {
            var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(_dictName);
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var user = await users.TryGetValueAsync(tx, userId);
                if (user.HasValue)
                {
                    var existingUser = user.Value;
                    existingUser.UserState = UserState.Verified;
                    existingUser._VerifiedAt = DateTimeOffset.UtcNow;
                    existingUser._UpdatedAt = DateTimeOffset.UtcNow;
                    await users.AddOrUpdateAsync(tx,userId,existingUser,(u1, u2) => u2);
                    await tx.CommitAsync();
                }
                else
                {
                    throw new KeyNotFoundException(nameof(User));
                }
            }
        }
        public async Task BanUserAsync(Guid userId)
        {
            var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(_dictName);
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var user = await users.TryGetValueAsync(tx, userId);
                if (user.HasValue)
                {
                    var existingUser = user.Value;
                    existingUser.UserState = UserState.Denied;
                    existingUser._VerifiedAt = null;
                    existingUser._UpdatedAt = DateTimeOffset.UtcNow;
                    await users.AddOrUpdateAsync(tx, userId, existingUser, (u1, u2) => u2);
                    await tx.CommitAsync();
                }
                else
                {
                    throw new KeyNotFoundException(nameof(User));
                }
            }
        }
        #endregion

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        //protected override async Task RunAsync(CancellationToken cancellationToken)
        //{
        //    // TODO: Replace the following sample code with your own logic 
        //    //       or remove this RunAsync override if it's not needed in your service.

        //    while (true)
        //    {
        //        cancellationToken.ThrowIfCancellationRequested();

        //        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        //    }
        //}
    }
}
