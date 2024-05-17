using Common.DTO;
using Microsoft.AspNetCore.Mvc;
using Contracts;

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
        public async Task<ActionResult> Get()
        {
            var rides = await _proxy.GetAllRidesAsync();
            return Ok(rides);
        }

        // GET rides/pending
        [HttpGet]
        [Route("pending")]
        public async Task<ActionResult> GetPending()
        {
            var rides = await _proxy.GetPendingRidesAsync();
            return Ok(rides);
        }

        // GET /rides/:id/history
        [HttpGet]
        [Route("{id}/history")]
        public async Task<ActionResult> GetCompletedRidesForUser(Guid id)
        {
            var rides = await _proxy.GetCompletedRidesUserAsync(id);
            return Ok(rides);
        }

        // GET /rides/:id/finished
        [HttpGet]
        [Route("{id}/finished")]
        public async Task<ActionResult> GetCompletedRidesForDriver(Guid id)
        {
            var rides = await _proxy.GetCompletedRidesDriverAsync(id);
            return Ok(rides);
        }

        // POST /rides/request
        [HttpPost]
        [Route("request")]
        public async Task<ActionResult> RequestRide(ProposedRideRequest proposedRide)
        {
            try
            {
                await _proxy.RequestRideAsync(proposedRide);
            }
            catch
            {
                return BadRequest("Key not found!");
            }
            return NoContent();
        }

        // PATCH /rides/accept
        [HttpPatch]
        [Route("accept")]
        public async Task<ActionResult> AcceptRide(AcceptRideRequest acceptedRide)
        {
            try
            {
                await _proxy.AcceptRideAsync(acceptedRide);
            }
            catch
            {
                return BadRequest("Key not found");
            }
            return Ok();
        }

        // PATCH /rides/finish
        [HttpPatch]
        [Route("finish")]
        public async Task<ActionResult> FinishRide(FinishedRideRequest finishedRideDTO)
        {
            try
            {
                await _proxy.FinishRideAsync(finishedRideDTO);
            }
            catch
            {
                return BadRequest("Key not found");
            }
            return Ok();
        }


    }
}
