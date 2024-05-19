using Common.DTO;
using Common.Entities;
using Common;
using Contracts;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Data;
using Common.Settings;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;


[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]

namespace TaxiRideData
{
    internal class RideServiceImpl : IRideDataService
    {
        private readonly string _dictName;
        private readonly string _queueName;
        private readonly IReliableStateManager StateManager;
        private readonly UserDataServiceSettings _serviceSettings;
        private readonly Uri _serviceUri;
        public RideServiceImpl(string dictName,string  queueName, IReliableStateManager stateManager, UserDataServiceSettings userServiceSettings)
        {
            _dictName = dictName;
            _queueName = queueName;
            StateManager = stateManager;
            _serviceSettings = userServiceSettings;
            _serviceUri = new Uri(_serviceSettings.ConnectionString);
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
        public async Task<Guid> RequestRideAsync(ProposedRideRequest proposedRide)
        {
            if (proposedRide == null)
            {
                throw new ArgumentNullException(nameof(proposedRide));
            }
            // TODO Check that passenger is not of type driver
            //var proxy = ServiceProxy.Create<IUserRideService>(_serviceUri, new ServicePartitionKey(1));
            //var res = await proxy.CheckPasengerTypeAsync(proposedRide.PassengerId);
            //if (!res)
            //{
            //    throw new ArgumentException("User requesting ride can not be a Driver");
            //}
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
            return newRide.Id;
        }
        public async Task AcceptRideAsync(AcceptRideRequest acceptRideDTO)
        {
            if (acceptRideDTO == null)
            {
                throw new ArgumentNullException(nameof(acceptRideDTO));
            }
            // TODO Check that driver is verified before procceding
            //var proxy = ServiceProxy.Create<IUserRideService>(_serviceUri, new ServicePartitionKey(1));
            //var res = await proxy.DriverExistsAndVerifiedAsync(acceptRideDTO.DriverID);
            //if (!res)
            //{
            //    throw new ArgumentException("Driver must be verified to accept rides");
            //}
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
            // TODO Check that driver is verified before procceding
            //var proxy = ServiceProxy.Create<IUserRideService>(_serviceUri, new ServicePartitionKey(1));
            //var res = await proxy.CheckPasengerTypeAsync(finishedRideDTO.PassengerId);
            //ServiceEventSource.Current.ServiceMessage(this.Context, $"Passenger is of type user: {res}");
            //if (!res)
            //{
            //    throw new ArgumentException("User finishing ride can not be a Driver");
            //}
            var rides = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, Ride>>(_dictName);
            Ride finishedRide;
            using (ITransaction tx = StateManager.CreateTransaction())
            {

                var ride = await rides.TryGetValueAsync(tx, finishedRideDTO.RideId);
                if (ride.HasValue)
                {
                    finishedRide = ride.Value;
                    if (finishedRide.PassengerId != finishedRideDTO.PassengerId)
                    {
                        throw new ArgumentException("Passenger Ids do not match");
                    }
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
        private async Task QueueDataForLaterProcessingAsync(Ride data, CancellationToken cancellationToken)
        {
            var myQueue = await StateManager.GetOrAddAsync<IReliableQueue<Ride>>(_queueName);
            using (var tx = StateManager.CreateTransaction())
            {
                await myQueue.EnqueueAsync(tx, data);
                await tx.CommitAsync();
            }
        }
    }
}
