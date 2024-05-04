using Common;
using Common.Entities;
using Common.Repository;
using Common.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting;
using Contracts;
using Microsoft.ServiceFabric.Services.Client;
using System.Linq.Expressions;
using ZstdSharp.Unsafe;

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

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            var rides = await _proxy.GetAllRidesAsync();
            return Ok(rides);
        }

        [HttpGet]
        [Route("pending")]
        public async Task<ActionResult> GetPending()
        {
            var rides = await _proxy.GetPendingRidesAsync();
            return Ok(rides);
        }

        [HttpGet]
        [Route("{id}/history")]
        public async Task<ActionResult> GetCompletedRidesForUser(Guid id)
        {
            var rides = await _proxy.GetCompletedRidesUserAsync(id);
            return Ok(rides);
        }

        [HttpGet]
        [Route("{id}/finished")]
        public async Task<ActionResult> GetCompletedRidesForDriver(Guid id)
        {
            var rides = await _proxy.GetCompletedRidesDriverAsync(id);
            return Ok(rides);
        }

        [HttpPost]
        [Route("request")]
        public async Task<ActionResult> RequestRide(ProposedRideDTO proposedRide)
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

        [HttpPatch]
        [Route("accept")]
        public async Task<ActionResult> AcceptRide(AcceptRideDTO acceptedRide)
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

        [HttpPatch]
        [Route("finish")]
        public async Task<ActionResult> FinishRide(FinishedRideDTO finishedRideDTO)
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
