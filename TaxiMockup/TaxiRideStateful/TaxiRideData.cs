using System.Fabric;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using Contracts;
using Common.Entities;
using Common.DTO;
using Common;
using System.Diagnostics;
using Microsoft.ServiceFabric.Data;
using Common.Repository;

namespace TaxiRideData
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class TaxiRideData : StatefulService, IRideDataService
    {
        private readonly string _dictName = "ridesDictionary";
        private readonly string _queueName = "ridesQueue";
        private readonly IRepository<Ride> _repo;
        private readonly int seedPeriod;
        public TaxiRideData(StatefulServiceContext context, IRepository<Ride> repo, int seedPeriod)
            : base(context)
        {
            _repo = repo;
            this.seedPeriod = seedPeriod;
        }
        #region Ride service methods
        public async Task<IEnumerable<Ride>> GetAllRidesAsync()
        {
            var rides = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, Ride>>(_dictName);
            List<Ride> list = new();
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var enumerable = await rides.CreateEnumerableAsync(tx);
                var enumerator = enumerable.GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    list.Add(enumerator.Current.Value);
                }
                enumerator.Dispose();
            }
            return list;
        }

        public async Task<IEnumerable<CompletedRideInfoResponse>> GetCompletedRidesUserAsync(Guid userId)
        {
            var rides = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, Ride>>(_dictName);
            List<CompletedRideInfoResponse> result = new();
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var enumerable = await rides.CreateEnumerableAsync(tx);
                var enumerator = enumerable.GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    var currentRide = enumerator.Current.Value;
                    if (currentRide.PassengerId == userId && currentRide.RideState == RideState.Finished)
                    {
                        result.Add(currentRide.AsInfoDTO());
                    }
                }
                enumerator.Dispose();
                return result;
            }
        }

        public async Task<IEnumerable<CompletedRideInfoResponse>> GetCompletedRidesDriverAsync(Guid driverId)
        {
            var rides = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, Ride>>(_dictName);
            List<CompletedRideInfoResponse> result = new();
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var enumerable = await rides.CreateEnumerableAsync(tx);
                var enumerator = enumerable.GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    var currentRide = enumerator.Current.Value;
                    if (currentRide.DriverId == driverId && currentRide.RideState == RideState.Finished)
                    {
                        result.Add(currentRide.AsInfoDTO());
                    }
                }
                enumerator.Dispose();
                return result;
            }
        }

        public async Task<IEnumerable<AvailableRideResponse>> GetPendingRidesAsync()
        {
            var rides = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, Ride>>(_dictName);
            List<AvailableRideResponse> result = new();
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var enumerable = await rides.CreateEnumerableAsync(tx);
                var enumerator = enumerable.GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(CancellationToken.None))
                {
                    var currentRide = enumerator.Current.Value;
                    if (currentRide.RideState == RideState.Pending)
                    {
                        result.Add(currentRide.AsAvailableRideDTO());
                    }
                }
                enumerator.Dispose();
                return result;
            }
        }

        public async Task RequestRideAsync(ProposedRideRequest proposedRide)
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
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var rides = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, Ride>>(_dictName);
                await rides.AddAsync(tx, newRide.Id, newRide);
                await tx.CommitAsync();
            }
            await QueueDataForLaterProcessingAsync(newRide, CancellationToken.None);
        }

        public async Task AcceptRideAsync(AcceptRideRequest acceptRideDTO)
        {
            if (acceptRideDTO == null)
            {
                throw new ArgumentNullException(nameof(acceptRideDTO));
            }
            var rides = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, Ride>>(_dictName);
            Ride acceptedRide;
            using (ITransaction tx = StateManager.CreateTransaction())
            {

                var ride = await rides.TryGetValueAsync(tx, acceptRideDTO.RideId);
                if (ride.HasValue)
                {
                    acceptedRide = ride.Value;
                    acceptedRide.DriverId = acceptRideDTO.DriverID;
                    acceptedRide.DriverETA = acceptedRide.DriverETA;
                    acceptedRide.RideState = RideState.InProgress;
                    await rides.AddOrUpdateAsync(tx, acceptedRide.Id, acceptedRide, (r1, r2) => r2);
                    await tx.CommitAsync();
                }
                else
                {
                    throw new KeyNotFoundException(nameof(acceptRideDTO));
                }
            }
            await QueueDataForLaterProcessingAsync(acceptedRide, CancellationToken.None);
        }

        public async Task FinishRideAsync(FinishedRideRequest finishedRideDTO)
        {
            if (finishedRideDTO == null)
            {
                throw new ArgumentNullException(nameof(finishedRideDTO));
            }
            var rides = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, Ride>>(_dictName);
            Ride finishedRide;
            using (ITransaction tx = StateManager.CreateTransaction())
            {

                var ride = await rides.TryGetValueAsync(tx, finishedRideDTO.RideId);
                if (ride.HasValue)
                {
                    finishedRide = ride.Value;
                    finishedRide.RideState = RideState.Finished;
                    finishedRide.Rating = finishedRideDTO.Rating;
                    finishedRide._FinishedAt = DateTimeOffset.Now;
                    await rides.AddOrUpdateAsync(tx, finishedRide.Id, finishedRide, (r1, r2) => r2);
                    await tx.CommitAsync();
                }
                else
                {
                    throw new KeyNotFoundException(nameof(finishedRideDTO));
                }
            }
            await QueueDataForLaterProcessingAsync(finishedRide, CancellationToken.None);
        }

        #endregion
        #region Data seeding methods
        private async Task SeedDataFromMongoDBAsync(CancellationToken cancellationToken)
        {
            var data = await _repo.GetAllAsync();
            await SeedDataToServiceFabricAsync(data, cancellationToken);
        }
        private async Task SeedDataToServiceFabricAsync(IEnumerable<Ride> data, CancellationToken cancellationToken)
        {
            var myDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, Ride>>(_dictName);
            using (var tx = StateManager.CreateTransaction())
            {
                foreach (var item in data)
                {
                    await myDictionary.AddOrUpdateAsync(tx, item.Id, item, (key, value) => item);
                }

                await tx.CommitAsync();
            }
        }
        private async Task QueueDataForLaterProcessingAsync(Ride data, CancellationToken cancellationToken)
        {
            var myQueue = await StateManager.GetOrAddAsync<IReliableQueue<Ride>>(_queueName);
            using (var tx = StateManager.CreateTransaction())
            {
                await myQueue.EnqueueAsync(tx, data);
                await tx.CommitAsync();
            }
        }
        private async Task ProcessQueuedDataAsync(CancellationToken cancellationToken)
        {
            var myQueue = await StateManager.GetOrAddAsync<IReliableQueue<Ride>>(_queueName);
            while (!cancellationToken.IsCancellationRequested)
            {
                using (var tx = StateManager.CreateTransaction())
                {
                    var result = await myQueue.TryDequeueAsync(tx, TimeSpan.FromSeconds(5), cancellationToken);
                    if (result.HasValue)
                    {
                        Ride ride = result.Value;
                        ServiceEventSource.Current.ServiceMessage(this.Context, $"======== Item from '{_queueName}': {ride.Id} ========");
                        if (ride.StartDestination == "toDelete")
                        {
                            await _repo.DeleteAsync(ride.Id);
                        }
                        else
                        {
                            await _repo.UpdateAsync(ride);
                        }
                        await tx.CommitAsync();
                    }
                    else
                    {
                        break;
                    }
                }
            }
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
            // Read data from MongoDB at startup
            await SeedDataFromMongoDBAsync(cancellationToken);

            // Example loop to periodically process queued data (if any)
            while (!cancellationToken.IsCancellationRequested)
            {
                await ProcessQueuedDataAsync(cancellationToken);
                await Task.Delay(TimeSpan.FromSeconds(seedPeriod), cancellationToken);
            }
        }
    }
}
