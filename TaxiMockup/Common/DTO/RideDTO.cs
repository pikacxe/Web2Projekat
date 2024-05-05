using System.ComponentModel.DataAnnotations;
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
    public record AvailableRideDTO
    {
        [DataMember]
        [Required(ErrorMessage = "Ride id is required")]
        public Guid RideId { get; set; }
        [DataMember]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Start destination is required")]
        public string? StartDestination { get; set; }
        [DataMember]
        [Required(AllowEmptyStrings = false, ErrorMessage = "End destination is required")]
        public string? EndDestination { get; set; }
    }
    [DataContract]
    public record CompletedRideInfoDTO
    {
        [DataMember]
        [Required(ErrorMessage = "Ride id is required")]
        public Guid RideId { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Passenger id is required")]
        public Guid PassengerId { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Driver id is required")]
        public Guid DriverId { get; set; }
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
        [Required(ErrorMessage = "Ride duration is required")]
        [Range(0, double.MaxValue)]
        public double RideDuration { get; set; }
        [DataMember]
        [Required(ErrorMessage = "Rating is required")]
        [Range(0, 10)]
        public int Rating { get; set; }
    }

    [DataContract]
    public record AcceptRideDTO
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
    public record FinishedRideDTO
    {
        [DataMember]
        public Guid RideId { get; set; }
        [DataMember]
        public int Rating { get; set; }
    }
}
