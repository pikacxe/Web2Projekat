using Common.DTO;

namespace TaxiWebAPI.Hubs
{
    public interface IRideChat
    {
        Task SendMessage(string sender, string message);
        Task NewRideRequest(AvailableRideResponse availableRide);
        Task RideAccepted(RideInProgressInfo rideInfo);
    }
}
