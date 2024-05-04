using System.Runtime.Serialization;

namespace Common.DTO
{
    [DataContract]
    public class RideStateDTO
    {
        [DataMember]
        public Guid RideId { get; set; }
        [DataMember]
        public RideState RideState { get; set; }
    }
    [DataContract]
    public class ProposedRideDTO
    {
        [DataMember]
        public Guid RideId { get; set; } = Guid.Empty;
        [DataMember]
        public Guid passengerId { get; set; }
        [DataMember]
        public string? StartDestination { get; set; }
        [DataMember]
        public string? EndDestination { get; set; }
        [DataMember]
        public double Price { get; set; }
        [DataMember]
        public double EstRideDuration { get; set; }
        [DataMember]
        public double DriverETA { get; set; }
    }
    [DataContract]
    public record AvailableRideDTO
    {
        [DataMember]
        public Guid RideId { get; set; }
        [DataMember]
        public string? StartDestination { get; set; }
        [DataMember]
        public string? EndDestination { get; set; }
    }
    [DataContract]
    public record CompletedRideInfoDTO
    {
        [DataMember]
        public Guid RideId { get; set; }
        [DataMember]
        public Guid UserId { get; set; }
        [DataMember]
        public Guid DriverId { get; set; }
        [DataMember]
        public string? StartDestination { get; set; }
        [DataMember]
        public string? EndDestination { get; set; }
        [DataMember]
        public double Price { get; set; }
        [DataMember]
        public double RideDuration { get; set; }
        [DataMember]
        public int Rating { get; set; }
    }
    
    [DataContract]
    public record AcceptRideDTO
    {
        [DataMember]
        public Guid RideId { get; set; }
        [DataMember]
        public Guid driverID { get; set; }
        [DataMember]
        public double DriverETA { get; set; }
    }
    [DataContract]
    public record FinishedRideDTO
    {
        [DataMember]
        public Guid RideId { get; set; }
        [DataMember]
        public int Rating { get; set; }
    }
}
