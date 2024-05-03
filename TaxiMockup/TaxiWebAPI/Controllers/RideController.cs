using Common;
using Common.Entities;
using Common.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TaxiWebAPI.Controllers
{
    [Route("api/rides")]
    [ApiController]
    public class RideController : ControllerBase
    {
        private readonly ILogger<RideController> _logger;

        public RideController(ILogger<RideController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public ActionResult Get()
        {
            return Ok("Sanity check");
        }

    }
}
