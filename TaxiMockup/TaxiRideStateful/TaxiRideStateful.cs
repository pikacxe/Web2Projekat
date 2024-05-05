using System.Fabric;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Contracts;
using Common.Entities;
using Common.DTO;
using Common.Repository;
using Common;
using System.Diagnostics;

namespace TaxiRideStateful
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class TaxiRideStateful : StatefulService, IRideDataService
    {
        private readonly IRepository<Ride> _repo;
        public TaxiRideStateful(StatefulServiceContext context, IRepository<Ride> repository)
            : base(context)
        {
            _repo = repository;
        }
        #region Ride service methods
        public async Task<IEnumerable<Ride>> GetAllRidesAsync()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<IEnumerable<CompletedRideInfoDTO>> GetCompletedRidesUserAsync(Guid userId)
        {
            var completedRides = await _repo.GetAllAsync(ride => ride.RideState == Common.RideState.Finished
                                                && ride.PassengerId == userId);
            if (completedRides.Count == 0)
            {
                return Enumerable.Empty<CompletedRideInfoDTO>();
            }

            List<CompletedRideInfoDTO> completedRideInfoDTOs = new List<CompletedRideInfoDTO>();
            foreach (var ride in completedRides)
            {
                completedRideInfoDTOs.Add(ride.AsInfoDTO());
            }
            return completedRideInfoDTOs;
        }

        public async Task<IEnumerable<CompletedRideInfoDTO>> GetCompletedRidesDriverAsync(Guid driverId)
        {
            var completedRides = await _repo.GetAllAsync(ride => ride.RideState == Common.RideState.Finished
                                                && ride.DriverId == driverId);
            if (completedRides.Count == 0)
            {
                return Enumerable.Empty<CompletedRideInfoDTO>();
            }

            List<CompletedRideInfoDTO> completedRideInfoDTOs = new List<CompletedRideInfoDTO>();
            foreach (var ride in completedRides)
            {
                completedRideInfoDTOs.Add(ride.AsInfoDTO());
            }
            return completedRideInfoDTOs;
        }

        public async Task<IEnumerable<AvailableRideDTO>> GetPendingRidesAsync()
        {
            var rides = await _repo.GetAllAsync(ride => ride.RideState == RideState.Pending);

            if (rides.Count == 0)
            {
                return Enumerable.Empty<AvailableRideDTO>();
            }

            List<AvailableRideDTO> availableRideDTOs = new List<AvailableRideDTO>();
            foreach (var ride in rides)
            {
                availableRideDTOs.Add(ride.AsAvailableRideDTO());
            }
            return availableRideDTOs;
        }

        public async Task RequestRideAsync(ProposedRideDTO proposedRide)
        {
            if (proposedRide == null)
            {
                throw new ArgumentNullException(nameof(proposedRide));
            }
            Ride newRide = new Ride
            {
                Id = Guid.NewGuid(),
                PassengerId = proposedRide.PassengerId,
                StartDestination = proposedRide.StartDestination,
                EndDestination = proposedRide.EndDestination,
                Price = proposedRide.Price,
                RideDuration = proposedRide.EstRideDuration,
                RideState = RideState.Pending,
                _CreatedAt = DateTimeOffset.Now,
                _UpdatedAt = DateTimeOffset.Now,
            };
            ServiceEventSource.Current.ServiceMessage(this.Context, $"Ride[{newRide.Id},{newRide.StartDestination},{newRide.EndDestination}]");
            try
            {
                await _repo.CreateAsync(newRide);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);

            }
        }

        public async Task AcceptRideAsync(AcceptRideDTO acceptRideDTO)
        {
            if(acceptRideDTO == null)
            {
                throw new ArgumentNullException(nameof(acceptRideDTO));
            }
            var ride = await _repo.GetAsync(acceptRideDTO.RideId);
            if (ride == null)
            {
                throw new KeyNotFoundException(nameof(acceptRideDTO));
            }
            Ride acceptedRide = (Ride)ride;
            acceptedRide.DriverId = acceptRideDTO.DriverID;
            acceptedRide.DriverETA = acceptedRide.DriverETA;
            acceptedRide.RideState = RideState.InProgress;
            await _repo.UpdateAsync(acceptedRide);
        }

        public async Task FinishRideAsync(FinishedRideDTO finishedRideDTO)
        {
            if (finishedRideDTO == null)
            {
                throw new ArgumentNullException(nameof(finishedRideDTO));
            }
            var ride = await _repo.GetAsync(finishedRideDTO.RideId);
            if (ride == null)
            {
                throw new KeyNotFoundException(nameof(finishedRideDTO));
            }
            Ride finishedRide = (Ride)ride;
            finishedRide.RideState = RideState.Finished;
            finishedRide.Rating = finishedRideDTO.Rating;
            finishedRide._FinishedAt = DateTimeOffset.Now;
            await _repo.UpdateAsync(finishedRide);
        }

        #endregion

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following sample code with your own logic 
            //       or remove this RunAsync override if it's not needed in your service.

            var myDictionary = await this.StateManager.GetOrAddAsync<IReliableDictionary<string, long>>("myDictionary");

            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                using (var tx = this.StateManager.CreateTransaction())
                {
                    var result = await myDictionary.TryGetValueAsync(tx, "Counter");

                    ServiceEventSource.Current.ServiceMessage(this.Context, "Current Counter Value: {0}",
                        result.HasValue ? result.Value.ToString() : "Value does not exist.");

                    await myDictionary.AddOrUpdateAsync(tx, "Counter", 0, (key, value) => ++value);

                    // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
                    // discarded, and nothing is saved to the secondary replicas.
                    await tx.CommitAsync();
                }

                await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
            }
        }
    }
}
