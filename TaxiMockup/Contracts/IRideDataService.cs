using Microsoft.ServiceFabric.Services.Remoting;
using Common.DTO;
using Common.Entities;

namespace Contracts
{
    public interface IRideDataService : IService
    {
        /// <summary>
        /// Requests new ride
        /// </summary>
        /// <param name="proposedRide">Parameters for requesting ride</param>
        /// <returns></returns>
        Task RequestRideAsync(ProposedRideDTO proposedRide);
        /// <summary>
        /// Accepts pending ride
        /// </summary>
        /// <param name="acceptRideDTO">Parameters for accepting ride</param>
        /// <returns></returns>
        Task AcceptRideAsync(AcceptRideDTO acceptRideDTO);
        /// <summary>
        /// Finishes ride in progress
        /// </summary>
        /// <param name="finishRideDTO">Parameters for finishing ride</param>
        /// <returns></returns>
        Task FinishRideAsync(FinishedRideDTO finishRideDTO);
        /// <summary>
        /// Gets all rides
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<IEnumerable<Ride>> GetAllRidesAsync();
        /// <summary>
        /// Gets all rides
        /// </summary>
        /// <param name="filter">Filter to filter rides by</param>
        /// <returns></returns>
        Task<IEnumerable<CompletedRideInfoDTO>> GetCompletedRidesUserAsync(Guid userId);
        /// <summary>
        /// Gets completed rides for given driver
        /// </summary>
        /// <param name="driverId"></param>
        /// <returns></returns>
        Task<IEnumerable<CompletedRideInfoDTO>> GetCompletedRidesDriverAsync(Guid driverId);
        /// <summary>
        /// Gets all pending rides
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<AvailableRideDTO>> GetPendingRidesAsync();
    }
}
