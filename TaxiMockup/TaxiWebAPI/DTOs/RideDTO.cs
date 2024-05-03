using Common;

namespace TaxiWebAPI.DTOs
{
    public record RideStateDTO(Guid RideId, RideState RideState);
    public record ProposedRideDTO(string StartDestination, string EndDestination, double Price, TimeSpan EstRideDuration, TimeSpan DriverETA);
}
