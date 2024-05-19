﻿using Common.DTO;
using Common.Entities;
using Common;
using Contracts;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;

[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]

namespace TaxiUserData
{
    internal class UserServiceImpl : IUserDataService, IUserRideService
    {
        private readonly string _dictName;
        private readonly string _queueName;
        private readonly IReliableStateManager StateManager;

        public UserServiceImpl(string dictName, string queueName, IReliableStateManager stateManager)
        {
            _dictName = dictName;
            _queueName = queueName;
            StateManager = stateManager;
        }

        #region User service methods
        public async Task ChangeUserPasswordAsync(UserPasswordChangeRequest userPasswordChangeDTO)
        {
            if (userPasswordChangeDTO == null)
            {
                throw new ArgumentNullException(nameof(userPasswordChangeDTO));
            }
            var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(_dictName);
            User existingUser;
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var user = await users.TryGetValueAsync(tx, userPasswordChangeDTO.UserId);
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
                    await users.AddOrUpdateAsync(tx, existingUser.Id, existingUser, (key, value) => existingUser);
                    await tx.CommitAsync();
                }
                else
                {
                    throw new KeyNotFoundException("User not found");
                }
            }
            await QueueDataForLaterProcessingAsync(existingUser, CancellationToken.None);
        }
        public async Task<AuthResponse> ValidateLoginParamsAsync(UserLoginRequest userLoginDTO)
        {
            if (userLoginDTO == null)
            {
                throw new ArgumentNullException();
            }
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
                            enumerator.Dispose();
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
        public async Task<Guid> RegisterNewUserAsync(RegisterUserRequest registerUserDTO)
        {
            if (registerUserDTO == null)
            {
                throw new ArgumentNullException(nameof(registerUserDTO));
            }
            var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(_dictName);
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
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                await users.AddAsync(tx, user.Id, user);
                await tx.CommitAsync();
            }
            await QueueDataForLaterProcessingAsync(user, CancellationToken.None);
            return user.Id;
        }
        public async Task UpdateUserAsync(UpdateUserReques updateUserDTO)
        {
            if (updateUserDTO == null)
            {
                throw new ArgumentNullException(nameof(updateUserDTO));
            }
            var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(_dictName);
            User existingUser;
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var user = await users.TryGetValueAsync(tx, updateUserDTO.Id);
                if (user.HasValue)
                {
                    existingUser = user.Value;
                    existingUser.Username = updateUserDTO.Username;
                    existingUser.Email = updateUserDTO.Email;
                    existingUser.Address = updateUserDTO.Address;
                    existingUser.DateOfBirth = updateUserDTO.DateOfBirth;
                    existingUser.Fullname = updateUserDTO.Fullname;
                    existingUser.UserPicture = updateUserDTO.UserPicture;
                    existingUser._UpdatedAt = DateTimeOffset.UtcNow;
                    await users.AddOrUpdateAsync(tx, existingUser.Id, existingUser, (key, value) => existingUser);
                    await tx.CommitAsync();
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }
            await QueueDataForLaterProcessingAsync(existingUser, CancellationToken.None);
        }
        public async Task DeleteUserAsync(Guid id)
        {
            var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(_dictName);
            User user;
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var res = await users.TryRemoveAsync(tx, id);
                if (!res.HasValue)
                {
                    throw new KeyNotFoundException(nameof(User));
                }
                user = res.Value;
                user.Username = "toDelete";
                await tx.CommitAsync();
            }
            await QueueDataForLaterProcessingAsync(user, CancellationToken.None);
        }
        public async Task VerifyUserAsync(Guid userId)
        {
            var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(_dictName);
            User existingUser;
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var user = await users.TryGetValueAsync(tx, userId);
                if (user.HasValue)
                {
                    existingUser = user.Value;
                    existingUser.UserState = UserState.Verified;
                    existingUser._VerifiedAt = DateTimeOffset.UtcNow;
                    existingUser._UpdatedAt = DateTimeOffset.UtcNow;
                    await users.AddOrUpdateAsync(tx, userId, existingUser, (key, value) => existingUser);
                    await tx.CommitAsync();
                }
                else
                {
                    throw new KeyNotFoundException(nameof(User));
                }
            }
            await QueueDataForLaterProcessingAsync(existingUser, CancellationToken.None);
        }
        public async Task BanUserAsync(Guid userId)
        {
            var users = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(_dictName);
            User existingUser;
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var user = await users.TryGetValueAsync(tx, userId);
                if (user.HasValue)
                {
                    existingUser = user.Value;
                    existingUser.UserState = UserState.Denied;
                    existingUser._VerifiedAt = null;
                    existingUser._UpdatedAt = DateTimeOffset.UtcNow;
                    await users.AddOrUpdateAsync(tx, userId, existingUser, (key, value) => existingUser);
                    await tx.CommitAsync();
                }
                else
                {
                    throw new KeyNotFoundException(nameof(User));
                }
            }
            await QueueDataForLaterProcessingAsync(existingUser, CancellationToken.None);
        }
        #endregion

        #region User-Ride service methods
        public async Task<bool> DriverExistsAndVerifiedAsync(Guid id)
        {
            var driver = await GetAsync(id);
            return driver.UserType == UserType.Driver && driver.UserState == UserState.Verified;
        }
        public async Task<bool> CheckPasengerTypeAsync(Guid id)
        {
            var passenger = await GetAsync(id);
            return passenger.UserType == UserType.User;
        }
        #endregion

        private async Task QueueDataForLaterProcessingAsync(User data, CancellationToken cancellationToken)
        {
            var myQueue = await StateManager.GetOrAddAsync<IReliableQueue<User>>(_queueName);
            using (var tx = StateManager.CreateTransaction())
            {
                await myQueue.EnqueueAsync(tx, data);
                await tx.CommitAsync();
            }
        }
    }
}
