using Common.DTO;
using Microsoft.AspNetCore.Mvc;
using Contracts;
using Microsoft.AspNetCore.Authorization;

namespace TaxiWebAPI.Controllers
{
    [Route("api/rides")]
    [ApiController]
    public class RideController : ControllerBase
    {
        private readonly ILogger<RideController> _logger;
        private readonly IRideDataService _proxy;

        public RideController(ILogger<RideController> logger, IRideDataService proxy)
        {
            _logger = logger;
            _proxy = proxy;
        }

        // GET rides/
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Get()
        {
            try
            {
                var rides = await _proxy.GetAllRidesAsync();
                return Ok(rides);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        // GET rides/pending
        [HttpGet]
        [Route("pending")]
        [Authorize(Roles = "Driver")]
        public async Task<ActionResult> GetPending()
        {
            try
            {
                var rides = await _proxy.GetPendingRidesAsync();
                return Ok(rides);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        // GET /rides/:id/history
        [HttpGet]
        [Route("{id}/history")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> GetCompletedRidesForUser(Guid id)
        {
            try
            {
                var rides = await _proxy.GetCompletedRidesUserAsync(id);
                return Ok(rides);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        // GET /rides/:id/finished
        [HttpGet]
        [Route("{id}/finished")]
        [Authorize(Roles = "Driver")]
        public async Task<ActionResult> GetCompletedRidesForDriver(Guid id)
        {
            try
            {
                var rides = await _proxy.GetCompletedRidesDriverAsync(id);
                return Ok(rides);
            }
            catch
            {
                return StatusCode(500);
            }
        }

        // POST /rides/request
        [HttpPost]
        [Route("request")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> RequestRide(ProposedRideRequest proposedRide)
        {
            try
            {
                await _proxy.RequestRideAsync(proposedRide);
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Invalid data");
            }
            catch
            {
                return StatusCode(500);
            }
            return NoContent();
        }

        // PATCH /rides/accept
        [HttpPatch]
        [Route("accept")]
        [Authorize(Roles = "Driver")]
        public async Task<ActionResult> AcceptRide(AcceptRideRequest acceptedRide)
        {
            try
            {
                await _proxy.AcceptRideAsync(acceptedRide);
                return Ok();
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Invalid data");
            }
            catch (KeyNotFoundException)
            {
                return BadRequest("Ride not found");
            }
            catch
            {
                return StatusCode(500);
            }
        }

        // PATCH /rides/finish
        [HttpPatch]
        [Route("finish")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult> FinishRide(FinishedRideRequest finishedRideDTO)
        {
            try
            {
                await _proxy.FinishRideAsync(finishedRideDTO);
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Invalid data");
            }
            catch (KeyNotFoundException)
            {
                return BadRequest("Ride not found");
            }
            catch
            {
                return StatusCode(500);
            }
            return Ok();
        }


    }
}
