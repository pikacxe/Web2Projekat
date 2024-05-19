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
        Task<UserInfo> GetAsync(Guid id);
        /// <summary>
        /// Gets all users in system
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<UserInfo>> GetAllAsync();
        /// <summary>
        /// Gets all unverified users
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<UserInfo>> GetAllUnverifiedAsync();
        /// <summary>
        /// Verrifies user
        /// </summary>
        /// <param name="userId">Id of user to verify</param>
        /// <exception cref="KeyNotFoundException"/>
        /// <returns></returns>
        Task VerifyUserAsync(Guid userId);
        /// <summary>
        /// Bans user
        /// </summary>
        /// <param name="userId">Id of user to ban</param>
        /// <exception cref="KeyNotFoundException"/>
        /// <returns></returns>
        Task BanUserAsync(Guid userId);
        /// <summary>
        /// Validates login parameteres
        /// </summary>
        /// <param name="userLoginDTO">Login parameters</param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="KeyNotFoundException"/>
        /// <returns></returns>
        Task<AuthResponse> ValidateLoginParamsAsync(UserLoginRequest userLoginDTO);
        /// <summary>
        /// Registers new user
        /// </summary>
        /// <param name="registerUserDTO">Registration parameters</param>
        /// <exception cref="ArgumentException"/>
        /// <returns></returns>
        Task<Guid> RegisterNewUserAsync(RegisterUserRequest registerUserDTO);
        /// <summary>
        /// Gets user state for given user
        /// </summary>
        /// <param name="id"></param>
        /// <exception cref="KeyNotFoundException"/>
        /// <returns></returns>
        Task<UserStateResponse> GetUserStateAsync(Guid id);
        /// <summary>
        /// Changes user password
        /// </summary>
        /// <param name="userPasswordChangeDTO"></param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="KeyNotFoundException"/>
        /// <returns></returns>
        Task ChangeUserPasswordAsync(UserPasswordChangeRequest userPasswordChangeDTO);
        /// <summary>
        /// Updates user
        /// </summary>
        /// <param name="updateUserDTO"></param>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="KeyNotFoundException"/>
        /// <returns></returns>
        Task UpdateUserAsync(UpdateUserReques updateUserDTO);
        /// <summary>
        /// Deletes user
        /// </summary>
        /// <param name="id">Id of user to delete</param>
        /// <exception cref="KeyNotFoundException"/>
        /// <returns></returns>
        Task DeleteUserAsync(Guid id);
    }
}
