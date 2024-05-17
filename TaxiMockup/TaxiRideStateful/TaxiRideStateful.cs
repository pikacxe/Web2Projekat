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

namespace TaxiRideStateful
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class TaxiRideStateful : StatefulService, IRideDataService
    {
        private readonly string _dictName = "rides";
        public TaxiRideStateful(StatefulServiceContext context)
            : base(context)
        {
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
            try
            {
                using (ITransaction tx = StateManager.CreateTransaction())
                {
                    var rides = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, Ride>>(_dictName);
                    await rides.AddAsync(tx, newRide.Id, newRide);
                    await tx.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex);
            }
        }

        public async Task AcceptRideAsync(AcceptRideRequest acceptRideDTO)
        {
            if (acceptRideDTO == null)
            {
                throw new ArgumentNullException(nameof(acceptRideDTO));
            }
            var rides = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, Ride>>(_dictName);
            using (ITransaction tx = StateManager.CreateTransaction())
            {

                var ride = await rides.TryGetValueAsync(tx, acceptRideDTO.RideId);
                if (ride.HasValue)
                {
                    Ride acceptedRide = ride.Value;
                    acceptedRide.DriverId = acceptRideDTO.DriverID;
                    acceptedRide.DriverETA = acceptedRide.DriverETA;
                    acceptedRide.RideState = RideState.InProgress;
                    await rides.AddOrUpdateAsync(tx,acceptedRide.Id,acceptedRide,(r1,r2)=>r2);
                    await tx.CommitAsync();
                }
                else
                {
                    throw new KeyNotFoundException(nameof(acceptRideDTO));
                }
            }
        }

        public async Task FinishRideAsync(FinishedRideRequest finishedRideDTO)
        {
            if (finishedRideDTO == null)
            {
                throw new ArgumentNullException(nameof(finishedRideDTO));
            }
            var rides = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, Ride>>(_dictName);
            using (ITransaction tx = StateManager.CreateTransaction())
            {

                var ride = await rides.TryGetValueAsync(tx, finishedRideDTO.RideId);
                if (ride.HasValue)
                {
                    Ride finishedRide = ride.Value;
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
        //protected override async Task RunAsync(CancellationToken cancellationToken)
        //{

        //    var rides = await this.StateManager.GetOrAddAsync<IReliableDictionary<Guid, Ride>>(_dictName);

        //    while (true)
        //    {
        //        cancellationToken.ThrowIfCancellationRequested();

        //        using (var tx = this.StateManager.CreateTransaction())
        //        {
        //            // TODO: Save state to db

        //            // If an exception is thrown before calling CommitAsync, the transaction aborts, all changes are 
        //            // discarded, and nothing is saved to the secondary replicas.
        //            await tx.CommitAsync();
        //        }

        //        await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
        //    }
        //}
    }
}
