using Common;
using Common.Entities;
using Common.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;
using TaxiWebAPI.DTOs;

namespace TaxiWebAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly IRepository<User> _repository;

        public UserController(ILogger<UserController> logger, IRepository<User> repository)
        {
            _logger = logger;
            _repository = repository;
        }

        // GET /users/
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            return Ok(await _repository.GetAllAsync());
        }

        // GET /users/:id
        [HttpGet("{id}")]
        public async Task<ActionResult> Get(Guid id)
        {
            var user = await _repository.GetAsync(id);
            if (user == null)
            {
                return NotFound("User not found");
            }
            return Ok(user);
        }

        // GET /users/unverified
        [HttpGet]
        [Route("unverified")]
        public async Task<ActionResult> UnverifiedUsers()
        {
            return Ok(await _repository.GetAllAsync(x => x.UserType == UserType.Driver && x.UserState == UserState.Default));
        }

        // POST /users/login
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult> Login(UserLoginDTO login)
        {
            var user = await _repository.GetAsync(x => x.Username == login.Username);
            if (user == null)
            {
                return NotFound();
            }
            // TODO: verify password

            return Ok("temp token");
        }

        // GET /users/:id/state
        [HttpGet]
        [Route("/state")]
        public async Task<ActionResult> GetUserState(Guid id)
        {
            var user = await _repository.GetAsync(x => x.Id == id);
            if(user == null)
            {
                return NotFound();
            }
            return Ok(((User)user).UserState);
        }


        // POST /users/register
        [HttpPost]
        [Route("register")]
        public async Task<ActionResult> Register(User user)
        {
            if (user == null)
            {
                return BadRequest();
            }
            await _repository.CreateAsync(user);
            return Ok();
        }

        // PUT /users/:id
        [HttpPut("{id}")]
        public async Task<ActionResult> Put(Guid id, User user)
        {
            try
            {
                await _repository.UpdateAsync(user);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError($"[ERROR] {ex.Message}");
            }
            return BadRequest();
        }
        
        // PATCH /users/:id/verify
        [Route("{id}/verify")]
        [HttpPatch]
        public async Task<ActionResult> VerifyUser(Guid id)
        {
            var user = await _repository.GetAsync(x => x.Id == id);
            if(user == null)
            {
                return BadRequest();
            }
            User updated = (User)user;
            updated.UserState = UserState.Verified;
            await _repository.UpdateAsync(updated);
            return NoContent();
        }


        // DELETE /users/:id
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            await _repository.DeleteAsync(id);
            return Ok();
        }

    }
}
