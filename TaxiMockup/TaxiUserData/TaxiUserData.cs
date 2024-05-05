using System.Fabric;
using Contracts;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Common.Repository;
using Common.Entities;
using Common.DTO;
using Common;

namespace TaxiUserData
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class TaxiUserData : StatelessService, IUserDataService
    {
        private readonly IRepository<User> _repo; 
        public TaxiUserData(StatelessServiceContext context, IRepository<User> repo)
            : base(context)
        {
            _repo = repo;
        }

        #region User service methods
        public async Task ChangeUserPassword(UserPasswordChangeDTO userPasswordChangeDTO)
        {
            var existingUser = await _repo.GetAsync(userPasswordChangeDTO.UserId) as User;
            if (existingUser == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            // TODO: Better password validation with hashing (bcrypt?)
            if (existingUser.Password != userPasswordChangeDTO.OldPassword)
            {
                throw new Exception("Incorrect old password!");
            }
            existingUser.Password = userPasswordChangeDTO.NewPassword;
            await _repo.UpdateAsync(existingUser);
        }

        public async Task<bool> ValidateLoginParams(UserLoginDTO userLoginDTO)
        {
            var existingUser = await _repo.GetAsync(x => x.Email == userLoginDTO.Email || x.Username == userLoginDTO.Username) as User;
            if (existingUser == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            // TODO: Better password validation with hashing (bcrypt?)
            if(existingUser.Password == userLoginDTO.Password)
            {
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<UserInfoDTO>> GetAllAsync()
        {
           var users = await _repo.GetAllAsync();
            List<UserInfoDTO> result = new List<UserInfoDTO>();
            foreach (var user in users)
            {
                result.Add(user.AsInfoDTO());
            }
            return result;
        }

        public async Task<IEnumerable<UserInfoDTO>> GetAllUnverifiedAsync()
        {
            var users = await _repo.GetAllAsync(x => x.UserState == UserState.Unverified);
            List<UserInfoDTO> result = new List<UserInfoDTO>();
            foreach (var user in users)
            {
                result.Add(user.AsInfoDTO());
            }
            return result;
        }

        public async Task<UserInfoDTO> GetAsync(Guid id)
        {
            var user = await _repo.GetAsync(id) as User;
            if (user == null)
            {
                throw new KeyNotFoundException("User not found!");
            }
            return user.AsInfoDTO();
        }

        public async Task<UserStateDTO> GetUserState(Guid id)
        {
            var existingUser = await _repo.GetAsync(id) as User;
            if (existingUser == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            UserStateDTO userStateDTO = new UserStateDTO
            {
                UserId = id,
                UserState = existingUser.UserState
            };
            return userStateDTO;
        }

        public async Task RegisterNewUser(RegisterUserDTO registerUserDTO)
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
                UserPictures = registerUserDTO.UserPicture,
                UserType = registerUserDTO.UserType,
                UserState = registerUserDTO.UserType == UserType.Driver ? UserState.Unverified : UserState.Default,
                _CreatedAt = DateTimeOffset.UtcNow
            };
            await _repo.CreateAsync(user);
        }

        public async Task VerifyUser(Guid userId)
        {
            var existingUser = await _repo.GetAsync(userId) as User;
            if (existingUser == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            existingUser.UserState = UserState.Verified;
            existingUser._VerifiedAt = DateTimeOffset.UtcNow;
            await _repo.UpdateAsync(existingUser);
        }
        public async Task BanUser(Guid userId)
        {
            var existingUser = await _repo.GetAsync(userId) as User;
            if (existingUser == null)
            {
                throw new KeyNotFoundException("User not found");
            }
            existingUser.UserState = UserState.Denied;
            await _repo.UpdateAsync(existingUser);
        }
        #endregion

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return this.CreateServiceRemotingInstanceListeners();
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            long iterations = 0;

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
