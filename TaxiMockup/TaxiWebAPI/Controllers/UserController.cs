using Common;
using Common.DTO;
using Common.Settings;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaxiWebAPI.Hubs;
using TaxiWebAPI.Settings;

namespace TaxiWebAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserDataServiceSettings _userServiceSettings;
        private readonly JwtTokenSettings _jwtSettings;
        private readonly Uri _serviceUri;
        private readonly ServiceProxyFactory _serviceProxyFactory;
        private readonly IHubContext<RideHub, IRideChat> _hubContext;

        public UserController(IHubContext<RideHub, IRideChat> hubContext,ServiceProxyFactory factory,UserDataServiceSettings serviceSettings , JwtTokenSettings jwtSettings)
        {
            _hubContext = hubContext;
            _jwtSettings = jwtSettings;
            _userServiceSettings = serviceSettings;
            _serviceProxyFactory = factory;
            _serviceUri = new Uri(_userServiceSettings.ConnectionString);
        }

        // GET /users/
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Get()
        {
            try
            {
                var _proxy = CreateProxy(Guid.NewGuid());
                return Ok(await _proxy.GetAllAsync());
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // GET /users/:id
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult> Get(Guid id)
        {
            try
            {
                var _proxy = CreateProxy(id);
                var user = await _proxy.GetAsync(id);
                return Ok(user);
            }
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.InnerExceptions)
                {
                    if (innerEx is KeyNotFoundException)
                    {
                        return NotFound("The requested user was not found.");
                    }
                    else if (innerEx is ArgumentNullException)
                    {
                        return BadRequest("Invalid data");
                    }
                    // Add more specific exceptions as needed.
                }
                // If none of the inner exceptions are handled specifically, return a generic server error.
                return StatusCode(500, "An error occurred while processing your request.");
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
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
                var _proxy = CreateProxy(Guid.NewGuid());
                return Ok(await _proxy.GetAllUnverifiedAsync());
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
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
                var _proxy = CreateProxy(Guid.NewGuid());
                var response = await _proxy.ValidateLoginParamsAsync(login);
                if (response == null)
                {
                    return StatusCode(500);
                }
                response.Token = CreateToken(response);
                return Ok(response);
            }
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.InnerExceptions)
                {
                    if (innerEx is KeyNotFoundException)
                    {
                        return NotFound("The requested user was not found.");
                    }
                    else if (innerEx is ArgumentNullException)
                    {
                        return BadRequest("Invalid data");
                    }
                    // Add more specific exceptions as needed.
                }
                // If none of the inner exceptions are handled specifically, return a generic server error.
                return StatusCode(500, "An error occurred while processing your request.");
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
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
                var _proxy = CreateProxy(id);
                var state = await _proxy.GetUserStateAsync(id);
                return Ok(state);
            }
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.InnerExceptions)
                {
                    if (innerEx is KeyNotFoundException)
                    {
                        return NotFound("The requested user was not found.");
                    }
                    else if (innerEx is ArgumentNullException)
                    {
                        return BadRequest("Invalid data");
                    }
                    // Add more specific exceptions as needed.
                }
                // If none of the inner exceptions are handled specifically, return a generic server error.
                return StatusCode(500, "An error occurred while processing your request.");
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
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
                var _proxy = CreateProxy(Guid.NewGuid());
                var id = await _proxy.RegisterNewUserAsync(registerUserDTO);
                return StatusCode(201,new { id = id });
            }
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.InnerExceptions)
                {
                    if (innerEx is KeyNotFoundException)
                    {
                        return NotFound("The requested user was not found.");
                    }
                    else if (innerEx is ArgumentNullException)
                    {
                        return BadRequest("Invalid data");
                    }
                    else if(innerEx is ArgumentException)
                    {
                        return BadRequest("User already exists");
                    }
                    // Add more specific exceptions as needed.
                }
                // If none of the inner exceptions are handled specifically, return a generic server error.
                return StatusCode(500, "An error occurred while processing your request.");
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PUT /users/:id/update
        [HttpPatch]
        [Route("{id}/update")]
        [Authorize]
        public async Task<ActionResult> UpdateUser(Guid id, UpdateUserRequest updateUserDTO)
        {
            if(id != updateUserDTO.Id)
            {
                return Unauthorized("User ids does not match");
            }
            var requestUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (requestUserId == null || requestUserId != id.ToString())
            {
                return BadRequest("You can only change own password");
            }
            if (requestUserId != id.ToString())
            {
                return BadRequest("You can only change own password");
            }
            try
            {
                var _proxy = CreateProxy(id);
                await _proxy.UpdateUserAsync(updateUserDTO);
                return NoContent();
            }
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.InnerExceptions)
                {
                    if (innerEx is KeyNotFoundException)
                    {
                        return NotFound("The requested user was not found.");
                    }
                    else if (innerEx is ArgumentNullException)
                    {
                        return BadRequest("Invalid data");
                    }
                    // Add more specific exceptions as needed.
                }
                // If none of the inner exceptions are handled specifically, return a generic server error.
                return StatusCode(500, "An error occurred while processing your request.");
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        // PATCH /users/:id/change-password
        [Route("{id}/change-password")]
        [HttpPatch]
        [Authorize]
        public async Task<ActionResult> ChangeUserPassword(Guid id, UserPasswordChangeRequest request)
        {
            var requestUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(requestUserId == null || requestUserId != id.ToString())
            {
                return BadRequest("You can only change own password");
            }
            if(requestUserId != id.ToString())
            {
                return BadRequest("You can only change own password");
            }
            try
            {
                var _proxy = CreateProxy(id);
                await _proxy.ChangeUserPasswordAsync(request);
                return NoContent();
            }
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.InnerExceptions)
                {
                    if (innerEx is KeyNotFoundException)
                    {
                        return NotFound("The requested user was not found.");
                    }
                    else if (innerEx is ArgumentNullException)
                    {
                        return BadRequest("Invalid data");
                    }
                    // Add more specific exceptions as needed.
                }
                // If none of the inner exceptions are handled specifically, return a generic server error.
                return StatusCode(500, "An error occurred while processing your request.");
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
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
                var _proxy = CreateProxy(id);
                await _proxy.VerifyUserAsync(id);
                return NoContent();
            }
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.InnerExceptions)
                {
                    if (innerEx is KeyNotFoundException)
                    {
                        return NotFound("The requested user was not found.");
                    }
                    else if (innerEx is ArgumentNullException)
                    {
                        return BadRequest("Invalid data");
                    }
                    else if (innerEx is ArgumentException)
                    {
                        return BadRequest(innerEx.Message);
                    }
                    // Add more specific exceptions as needed.
                }
                // If none of the inner exceptions are handled specifically, return a generic server error.
                return StatusCode(500, "An error occurred while processing your request.");
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
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
                var _proxy = CreateProxy(id);
                await _proxy.BanUserAsync(id);
                return NoContent();
            }
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.InnerExceptions)
                {
                    if (innerEx is KeyNotFoundException)
                    {
                        return NotFound("The requested user was not found.");
                    }
                    else if (innerEx is ArgumentNullException)
                    {
                        return BadRequest("Invalid data");
                    }
                    else if(innerEx is ArgumentException)
                    {
                        return BadRequest(innerEx.Message);
                    }
                    // Add more specific exceptions as needed.
                }
                // If none of the inner exceptions are handled specifically, return a generic server error.
                return StatusCode(500, "An error occurred while processing your request.");
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        // DELETE /users/:id
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var _proxy = CreateProxy(id);
                await _proxy.DeleteUserAsync(id);
                return NoContent();
            }
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.InnerExceptions)
                {
                    if (innerEx is KeyNotFoundException)
                    {
                        return NotFound("The requested user was not found.");
                    }
                    else if (innerEx is ArgumentNullException)
                    {
                        return BadRequest("Invalid data");
                    }
                    // Add more specific exceptions as needed.
                }
                // If none of the inner exceptions are handled specifically, return a generic server error.
                return StatusCode(500, "An error occurred while processing your request.");
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        private string CreateToken(AuthResponse response)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key ?? throw new ApplicationException("bad JWT settings"));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, response.UserId ?? throw new ArgumentException("Invalid data")),
                    new Claim(ClaimTypes.Role, response.UserRole ?? throw new ArgumentException("Invalid data"))
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,

            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private IUserDataService CreateProxy(Guid id)
        {
            // Get a hash code for the GUID
            int hash = id.GetHashCode();

            // Ensure positive values and evenly distribute keys in the range of 0-10
            long key = (hash & 0x7FFFFFFF) % 10; // & 0x7FFFFFFF is used to ensure the hash code is non-negative

            ServicePartitionKey partKey = new ServicePartitionKey(1);
            return _serviceProxyFactory.CreateServiceProxy<IUserDataService>(_serviceUri, partKey);
        }

    }
}
