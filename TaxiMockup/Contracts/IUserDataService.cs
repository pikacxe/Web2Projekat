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
        /// <returns></returns>
        Task<UserInfoDTO> GetAsync(Guid id);
        /// <summary>
        /// Gets all users in system
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<UserInfoDTO>> GetAllAsync();
        /// <summary>
        /// Gets all unverified users
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<UserInfoDTO>> GetAllUnverifiedAsync();
        /// <summary>
        /// Verrifies user
        /// </summary>
        /// <param name="userId">Id of user to verify</param>
        /// <returns></returns>
        Task VerifyUser(Guid userId);
        /// <summary>
        /// Bans user
        /// </summary>
        /// <param name="userId">Id of user to ban</param>
        /// <returns></returns>
        Task BanUser(Guid userId);
        /// <summary>
        /// Validates login parameteres
        /// </summary>
        /// <param name="userLoginDTO">Login parameters</param>
        /// <returns></returns>
        Task<bool> ValidateLoginParams(UserLoginDTO userLoginDTO);
        /// <summary>
        /// Registers new user
        /// </summary>
        /// <param name="registerUserDTO">Registration parameters</param>
        /// <returns></returns>
        Task RegisterNewUser(RegisterUserDTO registerUserDTO);
        /// <summary>
        /// Gets user state for given user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<UserStateDTO> GetUserState(Guid id);
        /// <summary>
        /// Changes user password
        /// </summary>
        /// <param name="userPasswordChangeDTO"></param>
        /// <returns></returns>
        Task ChangeUserPassword(UserPasswordChangeDTO userPasswordChangeDTO);
    }
}
