using Common.DTO;
using Common.Entities;
using Common;
using Contracts;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Data;
using Common.Settings;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Client;


[assembly: FabricTransportServiceRemotingProvider(RemotingListenerVersion = RemotingListenerVersion.V2_1, RemotingClientVersion = RemotingClientVersion.V2_1)]

namespace TaxiRideData
{
    internal class RideServiceImpl : IRideDataService
    {
        private readonly string _dictName;
        private readonly string _queueName;
        private readonly IReliableStateManager StateManager;
        private readonly ServiceProxyFactory _proxyFactory;
        private readonly UserDataServiceSettings _serviceSettings;
        private readonly Uri _serviceUri;
        private readonly TimeSpan timeout = TimeSpan.FromSeconds(5);
        public RideServiceImpl(string dictName, string queueName, IReliableStateManager stateManager, UserDataServiceSettings userServiceSettings, ServiceProxyFactory proxyFactory)
        {
            _dictName = dictName;
            _queueName = queueName;
            StateManager = stateManager;
            _serviceSettings = userServiceSettings;
            _serviceUri = new Uri(_serviceSettings.ConnectionString);
            _proxyFactory = proxyFactory;
        }

        #region Ride service methods
        public async Task<IEnumerable<Ride>> GetAllRidesAsync(CancellationToken cancellationToken)
        {
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var rides = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, Ride>>(tx, _dictName, timeout);
                List<Ride> list = new();
                var enumerable = await rides.CreateEnumerableAsync(tx);
                var enumerator = enumerable.GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(cancellationToken))
                {
                    list.Add(enumerator.Current.Value);
                }
                enumerator.Dispose();
                return list;
            }
        }
        public async Task<IEnumerable<CompletedRideInfoResponse>> GetCompletedRidesUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var rides = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, Ride>>(tx, _dictName, timeout);
                List<CompletedRideInfoResponse> result = new();
                var enumerable = await rides.CreateEnumerableAsync(tx);
                var enumerator = enumerable.GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(cancellationToken))
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
        public async Task<IEnumerable<CompletedRideInfoResponse>> GetCompletedRidesDriverAsync(Guid driverId, CancellationToken cancellationToken)
        {
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var rides = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, Ride>>(tx, _dictName, timeout);
                List<CompletedRideInfoResponse> result = new();
                var enumerable = await rides.CreateEnumerableAsync(tx);
                var enumerator = enumerable.GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(cancellationToken))
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
        public async Task<IEnumerable<AvailableRideResponse>> GetPendingRidesAsync(CancellationToken cancellationToken)
        {
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var rides = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, Ride>>(tx, _dictName, timeout);
                List<AvailableRideResponse> result = new();
                var enumerable = await rides.CreateEnumerableAsync(tx);
                var enumerator = enumerable.GetAsyncEnumerator();
                while (await enumerator.MoveNextAsync(cancellationToken))
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
        public async Task<Guid> RequestRideAsync(ProposedRideRequest proposedRide, CancellationToken cancellationToken)
        {
            if (proposedRide == null)
            {
                throw new ArgumentNullException(nameof(proposedRide));
            }
            var proxy = CreateProxy();
            var res = await proxy.CheckPasengerTypeAsync(proposedRide.PassengerId);
            if (!res)
            {
                throw new ArgumentException("User requesting ride can not be a Driver");
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
                var rides = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, Ride>>(tx, _dictName, timeout);
                await rides.AddAsync(tx, newRide.Id, newRide, timeout, cancellationToken);
                await tx.CommitAsync();
            }
            await QueueDataForLaterProcessingAsync(newRide, cancellationToken);
            return newRide.Id;
        }
        public async Task AcceptRideAsync(AcceptRideRequest acceptRideDTO, CancellationToken cancellationToken)
        {
            if (acceptRideDTO == null)
            {
                throw new ArgumentNullException(nameof(acceptRideDTO));
            }
            var proxy = CreateProxy();
            var res = await proxy.DriverExistsAndVerifiedAsync(acceptRideDTO.DriverID);
            if (!res)
            {
                throw new ArgumentException("Driver must exist and be verified to accept rides");
            }
            Ride acceptedRide;
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var rides = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, Ride>>(tx, _dictName, timeout);
                var ride = await rides.TryGetValueAsync(tx, acceptRideDTO.RideId);
                if (ride.HasValue)
                {
                    acceptedRide = ride.Value;
                    acceptedRide.DriverId = acceptRideDTO.DriverID;
                    acceptedRide.DriverETA = acceptedRide.DriverETA;
                    acceptedRide.RideState = RideState.InProgress;
                    await rides.AddOrUpdateAsync(tx, acceptedRide.Id, acceptedRide, (r1, r2) => r2, timeout, cancellationToken);
                    await tx.CommitAsync();
                }
                else
                {
                    throw new KeyNotFoundException(nameof(acceptRideDTO));
                }
            }
            await QueueDataForLaterProcessingAsync(acceptedRide, cancellationToken);
        }
        public async Task FinishRideAsync(FinishedRideRequest finishedRideDTO, CancellationToken cancellationToken)
        {
            if (finishedRideDTO == null)
            {
                throw new ArgumentNullException(nameof(finishedRideDTO));
            }
            var proxy = CreateProxy();
            var res = await proxy.CheckPasengerTypeAsync(finishedRideDTO.PassengerId);
            if (!res)
            {
                throw new ArgumentException("User finishing ride can not be a Driver");
            }
            Ride finishedRide;
            using (ITransaction tx = StateManager.CreateTransaction())
            {
                var rides = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, Ride>>(tx, _dictName, timeout);
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
                    await rides.AddOrUpdateAsync(tx, finishedRide.Id, finishedRide, (r1, r2) => r2, timeout, cancellationToken);
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
            using (var tx = StateManager.CreateTransaction())
            {
                var myQueue = await StateManager.GetOrAddAsync<IReliableQueue<Ride>>(tx, _queueName, timeout);
                await myQueue.EnqueueAsync(tx, data, timeout, cancellationToken);
                await tx.CommitAsync();
            }
        }

        private IUserRideService CreateProxy()
        {
            ServicePartitionKey key = new ServicePartitionKey(1);
            return _proxyFactory.CreateServiceProxy<IUserRideService>(_serviceUri, key);
        }
    }
}
