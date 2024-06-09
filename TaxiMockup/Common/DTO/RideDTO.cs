using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Common.DTO
{
    [DataContract]
    public class RideStateRequest
    {
        [DataMember]
        public Guid RideId { get; set; }
        [DataMember]
        public RideState RideState { get; set; }
    }
    [DataContract]
    public class RideInfo
    {
        [DataMember]
        public Guid Id { get; set; }
        [DataMember]
        public Guid PassengerId { get; set; }
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
        public double DriverETA { get; set; }
        [DataMember]
        public RideState RideState { get; set; }
        [DataMember]
        public int Rating { get; set; }
        [DataMember]
        public string? CreatedAt { get; set; }
        [DataMember]
        public string? UpdatedAt { get; set; }
        [DataMember]
        public string? FinishedAt { get; set; }

    }

    [DataContract]
    public class ProposedRideRequest
    {
        [DataMember]
        [Required(ErrorMessage = "Passenger id is required")]
        public Guid PassengerId { get; set; }
        [DataMember]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Start destination is required")]
        public string? StartDestination { get; set; }
        [DataMember]
        [Required(AllowEmptyStrings = false, ErrorMessage = "End destination is required")]
        public string? EndDestination { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Price is required")]
        [Range(0, double.MaxValue)]
        public double Price { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Estimation ride duration is required")]
        [Range(0, double.MaxValue)]
        public double EstRideDuration { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Driver ETA is required")]
        [Range(0, double.MaxValue)]
        public double DriverETA { get; set; }
        [DataMember]
        public string PassengerName { get; set; } = string.Empty;
        [DataMember]
        public string ConnectionId { get; set; } = string.Empty;
    }
    [DataContract]
    public class AvailableRideResponse
    {
        [DataMember]
        public Guid RideId { get; set; }
        [DataMember]
        public string? StartDestination { get; set; }
        [DataMember]
        public string? EndDestination { get; set; }
        [DataMember]
        public string PassengerName { get; set; } = string.Empty;
    }
    [DataContract]
    public class CompletedRideInfoResponse
    {
        [DataMember]
        public Guid RideId { get; set; }
        [DataMember]
        public Guid PassengerId { get; set; }
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
    public class AcceptRideRequest
    {
        [DataMember]
        [Required(ErrorMessage = "Ride id is required")]
        public Guid RideId { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Driver id is required")]
        public Guid DriverID { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Driver ETA is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Driver ETA must be greate than zero")]
        public double DriverETA { get; set; }
        [DataMember]
        public string ConnectionId { get; set; } = string.Empty;
    }
    [DataContract]
    public class FinishedRideRequest
    {
        [DataMember]
        [Required(ErrorMessage = "Ride id is required")]
        public Guid RideId { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Passenger id is required")]
        public Guid PassengerId { get; set; }
        [DataMember]
        [Range(1, 10, ErrorMessage = "Rating must be on a scale 1 to 10")]
        public int Rating { get; set; }
    }

    [DataContract]
    public class RideInProgressInfo
    {
        [DataMember]
        public Guid RideId { get; set; }
        [DataMember]
        public Guid PassengerId { get; set; }
        [DataMember]
        public double DriverETA { get; set; }
        [DataMember]
        public double RideDuration { get; set; }
    }
}
