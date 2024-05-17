using Common.DTO;
using Contracts;
using Microsoft.AspNetCore.Mvc;

namespace TaxiWebAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserDataService _proxy;

        public UserController(ILogger<UserController> logger, IUserDataService proxy)
        {
            _logger = logger;
            _proxy = proxy;
        }

        // GET /users/
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _proxy.GetAllAsync());
        }

        // GET /users/:id
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(Guid id)
        {
            try
            {
                var user = await _proxy.GetAsync(id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // GET /users/unverified
        [HttpGet]
        [Route("unverified")]
        public async Task<ActionResult> UnverifiedUsers()
        {
            return Ok(await _proxy.GetAllUnverifiedAsync());
        }

        // POST /users/login
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login(UserLoginRequest login)
        {
            try
            {
                await _proxy.ValidateLoginParamsAsync(login);
                // TODO: get JWT token
                return Ok("Debug token");

            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to login: {ex.Message}");
            }
        }

        // GET /users/:id/state
        [HttpGet]
        [Route("{id}/state")]
        public async Task<ActionResult> GetUserState(Guid id)
        {
            try
            {
                var state = await _proxy.GetUserStateAsync(id);
                return Ok(state);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        // POST /users/register
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register(RegisterUserRequest registerUserDTO)
        {

            await _proxy.RegisterNewUserAsync(registerUserDTO);
            return Ok();

        }

        // PUT /users/update
        [HttpPut]
        [Route("update")]
        public async Task<ActionResult> UpdateUser(UserInfo userInfoDTO)
        {
            try
            {
                await _proxy.UpdateUserAsync(userInfoDTO);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Update failed: {ex.Message}");    
            }
        }

        // PATCH /users/:id/verify
        [Route("{id}/verify")]
        [HttpPatch]
        public async Task<ActionResult> VerifyUser(Guid id)
        {
            try
            {
                await _proxy.VerifyUserAsync(id);
            }
            catch(Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
            return NoContent();
        }

        // PATCH /users/:id/ban
        [Route("{id}/ban")]
        [HttpPatch]
        public async Task<ActionResult> BanUser(Guid id)
        {
            try
            {
                await _proxy.BanUserAsync(id);
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
            return NoContent();
        }


        // DELETE /users/:id
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _proxy.DeleteUserAsync(id);
            return Ok();
        }

    }
}
