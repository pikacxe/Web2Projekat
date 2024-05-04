using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Entities
{
    public class Ride : IEntity
    {
        [Required]
        public Guid Id { get; set; }
        public Guid PassengerId { get; set; }
        public Guid DriverId { get; set; }
        [Required]
        public string? StartDestination { get; set; } = string.Empty;
        [Required]
        public string? EndDestination { get; set; } = string.Empty;
        [Range(0, int.MaxValue)]
        public double Price { get; set; }
        [Range(0, int.MaxValue)]
        public double RideDuration { get; set; }
        [Range(0, int.MaxValue)]
        public double DriverETA { get; set; }
        public RideState RideState { get; set; }
        [Range(0, 10)]
        public int Rating { get; set; }

        public DateTimeOffset? _CreatedAt { get; set; }
        public DateTimeOffset? _UpdatedAt { get; set; }
        public DateTimeOffset? _FinishedAt { get; set; }

    }
}
