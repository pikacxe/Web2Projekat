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
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        /// <returns></returns>
        Task<Guid> RequestRideAsync(ProposedRideRequest proposedRide, CancellationToken cancellationToken = default);
        /// <summary>
        /// Accepts pending ride
        /// </summary>
        /// <param name="acceptRideDTO">Parameters for accepting ride</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="KeyNotFoundException"/>
        /// <returns></returns>
        Task<RideInfo> AcceptRideAsync(AcceptRideRequest acceptRideDTO, CancellationToken cancellationToken = default);
        /// <summary>
        /// Finishes ride in progress
        /// </summary>
        /// <param name="finishRideDTO">Parameters for finishing ride</param>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="ArgumentException"/>
        /// <exception cref="KeyNotFoundException"/>
        /// <returns></returns>
        Task FinishRideAsync(FinishedRideRequest finishRideDTO, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets all rides
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<RideInfo>> GetAllRidesAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets all rides
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<CompletedRideInfoResponse>> GetCompletedRidesUserAsync(Guid userId, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets completed rides for given driver
        /// </summary>
        /// <param name="driverId"></param>
        /// <returns></returns>
        Task<IEnumerable<CompletedRideInfoResponse>> GetCompletedRidesDriverAsync(Guid driverId, CancellationToken cancellationToken = default);
        /// <summary>
        /// Gets all pending rides
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<AvailableRideResponse>> GetPendingRidesAsync(CancellationToken cancellationToken = default);
    }
}
