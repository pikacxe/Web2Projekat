using Common.DTO;
using Microsoft.AspNetCore.Mvc;
using Contracts;
using Microsoft.AspNetCore.Authorization;
using Common.Settings;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.AspNetCore.SignalR;
using TaxiWebAPI.Hubs;

namespace TaxiWebAPI.Controllers
{
    [Route("api/rides")]
    [ApiController]
    public class RideController : ControllerBase
    {
        private readonly RideDataServiceSettings _rideServiceSettings;
        private readonly ServiceProxyFactory _proxyFactory;
        private readonly IHubContext<RideHub, IRideChat> _hubContext;
        private readonly Uri _rideServiceUri;
        public RideController(IHubContext<RideHub, IRideChat> hubContext,ServiceProxyFactory serviceProxyFactory, RideDataServiceSettings rideDataServiceSettings)
        {
            _hubContext = hubContext;
            _proxyFactory = serviceProxyFactory;
            _rideServiceSettings = rideDataServiceSettings;
            _rideServiceUri = new Uri(_rideServiceSettings.ConnectionString);
        }

        // GET rides/
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Get()
        {
            try
            {
                var _proxy = CreateProxy();
                var rides = await _proxy.GetAllRidesAsync();
                return Ok(rides);
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
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
                var _proxy = CreateProxy();
                var rides = await _proxy.GetPendingRidesAsync();
                return Ok(rides);
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
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
                var _proxy = CreateProxy();
                var rides = await _proxy.GetCompletedRidesUserAsync(id);
                return Ok(rides);
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
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
                var _proxy = CreateProxy();
                var rides = await _proxy.GetCompletedRidesDriverAsync(id);
                return Ok(rides);
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
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
                var _proxy = CreateProxy();
                var result = await _proxy.RequestRideAsync(proposedRide);
                // Broadcast the new ride to all drivers
                await _hubContext.Clients.Group("drivers").NewRideRequest(new AvailableRideResponse()
                {
                    RideId = result,
                    EndDestination = proposedRide.EndDestination,
                    PassengerName = proposedRide.PassengerName,
                    StartDestination = proposedRide.StartDestination
                });
                await _hubContext.Groups.AddToGroupAsync(proposedRide.ConnectionId, result.ToString());
                return StatusCode(201, new { id = result });
            }
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.InnerExceptions)
                {
                    if (innerEx is ArgumentException)
                    {
                        return BadRequest("Invalid data");
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

        // PATCH /rides/accept
        [HttpPatch]
        [Route("accept")]
        [Authorize(Roles = "Driver")]
        public async Task<ActionResult> AcceptRide(AcceptRideRequest acceptedRide)
        {
            try
            {
                var _proxy = CreateProxy();
                var ride = await _proxy.AcceptRideAsync(acceptedRide);
                await _hubContext.Groups.AddToGroupAsync(acceptedRide.ConnectionId, acceptedRide.RideId.ToString());
                var payload = new RideInProgressInfo()
                {
                    RideId = ride.Id,
                    DriverETA = ride.DriverETA,
                    PassengerId = ride.PassengerId,
                    RideDuration = ride.RideDuration
                };
                await _hubContext.Clients.Group(ride.Id.ToString()).RideAccepted(payload);
                return NoContent();
            }
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.InnerExceptions)
                {
                    if (innerEx is KeyNotFoundException)
                    {
                        return NotFound("The requested ride was not found.");
                    }
                    else if (innerEx is ArgumentException)
                    {
                        return BadRequest("Invalid data");
                    }
                    else if (innerEx is ArgumentNullException)
                    {
                        return BadRequest("Invalid data");
                    }
                    // Add more specific exceptions as needed.
                }
                // Add more specific exceptions as needed.
                // If none of the inner exceptions are handled specifically, return a generic server error.
                return StatusCode(500, "An error occurred while processing your request.");
            }
            catch
            {
                return StatusCode(500, "An error occurred while processing your request.");
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
                var _proxy = CreateProxy();
                await _proxy.FinishRideAsync(finishedRideDTO);
                return NoContent();
            }
            catch (AggregateException ex)
            {
                foreach (var innerEx in ex.InnerExceptions)
                {
                    if (innerEx is KeyNotFoundException)
                    {
                        return NotFound("The requested ride was not found.");
                    }
                    else if (innerEx is ArgumentException)
                    {
                        return BadRequest("Invalid data");
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

        private IRideDataService CreateProxy()
        {
            ServicePartitionKey key = new ServicePartitionKey(_rideServiceSettings.PartitionKey);
            return _proxyFactory.CreateServiceProxy<IRideDataService>(_rideServiceUri, key);
        }
    }
}
