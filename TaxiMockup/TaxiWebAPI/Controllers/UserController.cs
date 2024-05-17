using Common;
using Common.DTO;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaxiWebAPI.Settings;

namespace TaxiWebAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserDataService _proxy;
        private readonly JwtTokenSettings _jwtSettings;

        public UserController(ILogger<UserController> logger, IUserDataService proxy, JwtTokenSettings jwtSettings)
        {
            _logger = logger;
            _proxy = proxy;
            _jwtSettings = jwtSettings;
        }

        // GET /users/
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Get()
        {
            return Ok(await _proxy.GetAllAsync());
        }

        // GET /users/:id
        [HttpGet("{id}")]
        [Authorize]
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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UnverifiedUsers()
        {
            return Ok(await _proxy.GetAllUnverifiedAsync());
        }

        // POST /users/login
        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult> Login(UserLoginRequest login)
        {
            try
            {
                var response = await _proxy.ValidateLoginParamsAsync(login);
                if (response == null)
                {
                    return StatusCode(500);
                }
                response.Token = CreateToken(response);
                return Ok(response);
            }
            catch (KeyNotFoundException)
            {
                return BadRequest("Invalid login data");
            }
            catch
            {
                return StatusCode(500);
            }
        }

        // GET /users/:id/state
        [HttpGet]
        [Route("{id}/state")]
        [Authorize]
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
        [AllowAnonymous]
        public async Task<ActionResult> Register(RegisterUserRequest registerUserDTO)
        {

            await _proxy.RegisterNewUserAsync(registerUserDTO);
            return Ok();

        }

        // PUT /users/update
        [HttpPut]
        [Route("update")]
        [Authorize]
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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> VerifyUser(Guid id)
        {
            try
            {
                await _proxy.VerifyUserAsync(id);
            }
            catch (Exception ex)
            {
                return BadRequest($"{ex.Message}");
            }
            return NoContent();
        }

        // PATCH /users/:id/ban
        [Route("{id}/ban")]
        [HttpPatch]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _proxy.DeleteUserAsync(id);
            return Ok();
        }

        private string CreateToken(AuthResponse response)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, response.UserID),
                    new Claim(ClaimTypes.Role, response.UserRole)
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,

            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
