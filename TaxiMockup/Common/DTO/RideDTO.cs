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
    public class ProposedRideRequest
    {
        [DataMember]
        [Required(ErrorMessage = "Ride id is required")]
        public Guid RideId { get; set; }
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
    }
    [DataContract]
    public record AvailableRideResponse
    {
        [DataMember]
        public Guid RideId { get; set; }
        [DataMember]
        public string? StartDestination { get; set; }
        [DataMember]
        public string? EndDestination { get; set; }
    }
    [DataContract]
    public record CompletedRideInfoResponse
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
    public record AcceptRideRequest
    {
        [DataMember]
        [Required(ErrorMessage = "Ride id is required")]
        public Guid RideId { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Driver id is required")]
        public Guid DriverID { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Driver ETA is required")]
        [Range(0, double.MaxValue)]
        public double DriverETA { get; set; }
    }
    [DataContract]
    public record FinishedRideRequest
    {
        [DataMember]
        [Required(ErrorMessage ="Ride id is required")]
        public Guid RideId { get; set; }
        [DataMember]
        public int Rating { get; set; }
    }
}
