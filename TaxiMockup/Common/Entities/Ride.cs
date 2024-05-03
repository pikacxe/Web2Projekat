using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities
{
    public class Ride : IEntity
    {
        public Guid Id { get; set; }
        public Guid PassangerId { get; set; }
        public Guid DriverId { get; set; }
        public string? StartDestination { get; set; }
        public string? EndDestination { get; set; }

        public double? Price { get; set; }
        public TimeSpan EstRideDuration { get; set; }
        public TimeSpan RideDuration { get; set; }
        public TimeSpan DriverETA { get; set; }
        public RideState RideState { get; set; }
        public int Rating { get; set; }

    }
}
