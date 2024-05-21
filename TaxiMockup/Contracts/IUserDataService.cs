using Microsoft.ServiceFabric.Services.Remoting;
using Common.DTO;


namespace Contracts
{
    public interface IUserDataService : IService
    {
        /// <summary>
        /// Gets user by user id
        /// </summary>
        /// <param name="id"></param>        
        /// <exception cref="KeyNotFoundException"/>
        /// <returns></returns>
        Task<UserInfo> GetAsync(Guid id, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets all users in system
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<UserInfo>> GetAllAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets all unverified users
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<UserInfo>> GetAllUnverifiedAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Verrifies user
        /// </summary>
        /// <param name="userId">Id of user to verify</param>
        /// <exception cref="KeyNotFoundException"/>
        /// <returns></returns>
        Task VerifyUserAsync(Guid userId, CancellationToken cancellationToken = default);
        /// <summary>
        /// Bans user
        /// </summary>
        /// <param name="userId">Id of user to ban</param>
        /// <exception cref="KeyNotFoundException"/>
        /// <returns></returns>
        Task BanUserAsync(Guid userId, CancellationToken cancellationToken = default);
        /// <summary>
        /// Validates login parameteres
        /// </summary>
        /// <param name="userLoginDTO">Login parameters</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="KeyNotFoundException"/>
        /// <returns></returns>
        Task<AuthResponse> ValidateLoginParamsAsync(UserLoginRequest userLoginDTO, CancellationToken cancellationToken = default);
        /// <summary>
        /// Registers new user
        /// </summary>
        /// <param name="registerUserDTO">Registration parameters</param>
        /// <exception cref="ArgumentException"/>
        /// <returns></returns>
        Task<Guid> RegisterNewUserAsync(RegisterUserRequest registerUserDTO, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets user state for given user
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="KeyNotFoundException"/>
        /// <returns></returns>
        Task<UserStateResponse> GetUserStateAsync(Guid id, CancellationToken cancellationToken = default);
        /// <summary>
        /// Changes user password
        /// </summary>
        /// <param name="userPasswordChangeDTO"></param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="KeyNotFoundException"/>
        /// <returns></returns>
        Task ChangeUserPasswordAsync(UserPasswordChangeRequest userPasswordChangeDTO, CancellationToken cancellationToken = default);
        /// <summary>
        /// Updates user
        /// </summary>
        /// <param name="updateUserDTO"></param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="KeyNotFoundException"/>
        /// <returns></returns>
        Task UpdateUserAsync(UpdateUserReques updateUserDTO, CancellationToken cancellationToken = default);
        /// <summary>
        /// Deletes user
        /// </summary>
        /// <param name="id">Id of user to delete</param>
        /// <exception cref="KeyNotFoundException"/>
        /// <returns></returns>
        Task DeleteUserAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
