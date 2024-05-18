using Common;
using Common.DTO;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
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
            try
            {
                return Ok(await _proxy.GetAllAsync());
            }
            catch
            {
                return StatusCode(500);
            }
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
            catch (AggregateException)
            {
                return NotFound("User not found");
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }

        // GET /users/unverified
        [HttpGet]
        [Route("unverified")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> UnverifiedUsers()
        {
            try
            {
                return Ok(await _proxy.GetAllUnverifiedAsync());
            }
            catch
            {
                return StatusCode(500);
            }
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
            catch (AggregateException)
            {
                // TODO better handling
                return NotFound("Invalid email or password");
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
            catch (AggregateException)
            {
                // TODO better handling
                return NotFound("User not found");
            }
            catch
            {
                return StatusCode(500);
            }
        }


        // POST /users/register
        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<ActionResult> Register(RegisterUserRequest registerUserDTO)
        {
            try
            {
                await _proxy.RegisterNewUserAsync(registerUserDTO);
                return NoContent();
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Invalid data");
            }
            catch
            {
                return StatusCode(500);
            }
        }

        // PUT /users/update
        [HttpPut("/{id}")]
        [Route("update")]
        [Authorize]
        public async Task<ActionResult> UpdateUser(Guid id, UserInfo userInfoDTO)
        {
            if(id != userInfoDTO.Id)
            {
                return Unauthorized("User ids does not match");
            }
            var requestUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (requestUserId == null)
            {
                return BadRequest("You can only change own password");
            }
            if (requestUserId != id.ToString())
            {
                return BadRequest("You can only change own password");
            }
            try
            {
                await _proxy.UpdateUserAsync(userInfoDTO);
                return NoContent();
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Invalid data");
            }
            catch (AggregateException)
            {
                // TODO better handling
                return NotFound("User not found");
            }
            catch
            {
                return StatusCode(500);
            }
        }

        // PATCH /users/:id/change-password
        [Route("{id}/change-password")]
        [HttpPatch]
        [Authorize]
        public async Task<ActionResult> ChangeUserPassword(Guid id, UserPasswordChangeRequest request)
        {
            if(id != request.UserId)
            {
                return Unauthorized("User ids do not match");
            }
            var requestUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(requestUserId == null)
            {
                return BadRequest("You can only change own password");
            }
            if(requestUserId != id.ToString())
            {
                return BadRequest("You can only change own password");
            }
            try
            {
                await _proxy.ChangeUserPasswordAsync(request);
                return NoContent();
            }
            catch (AggregateException)
            {
                return NotFound("User not found");
            }
            catch
            {
                return StatusCode(500);
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
                return NoContent();
            }
            catch (AggregateException)
            {
                // TODO better handling
                return NotFound("User not found");
            }
            catch
            {
                return StatusCode(500);
            }
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
                return NoContent();
            }
            catch (AggregateException)
            {
                // TODO better handling
                return NotFound("User not found");
            }
            catch
            {
                return StatusCode(500);
            }
        }


        // DELETE /users/:id
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _proxy.DeleteUserAsync(id);
                return NoContent();
            }
            catch (AggregateException)
            {
                // TODO better handling
                return NotFound("User not found");
            }
            catch
            {
                return StatusCode(500);
            }
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
