namespace Common.Entities
{
    public class Ride : IEntity
    {
        public Guid Id { get; set; }
        public Guid PassengerId { get; set; }
        public Guid DriverId { get; set; }
        public string? PassengerName { get; set; } = string.Empty;
        public string? StartDestination { get; set; } = string.Empty;
        public string? EndDestination { get; set; } = string.Empty;
        public double Price { get; set; }
        public double RideDuration { get; set; }
        public double DriverETA { get; set; }
        public RideState RideState { get; set; }
        public int Rating { get; set; }
        public DateTimeOffset? _CreatedAt { get; set; }
        public DateTimeOffset? _UpdatedAt { get; set; }
        public DateTimeOffset? _FinishedAt { get; set; }

    }
}
