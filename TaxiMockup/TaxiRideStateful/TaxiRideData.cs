using System.Fabric;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Common.Entities;
using Common.Repository;
using Common.Settings;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;

namespace TaxiRideData
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class TaxiRideData : StatefulService
    {
        private readonly string _dictName = "ridesDictionary";
        private readonly string _queueName = "ridesQueue";
        private readonly IRepository<Ride> _repo;
        private readonly UserDataServiceSettings _serviceSettings;
        private readonly int seedPeriod;
        public TaxiRideData(StatefulServiceContext context, IRepository<Ride> repo,UserDataServiceSettings serviceSettings, int seedPeriod)
            : base(context)
        {
            _repo = repo;
            _serviceSettings = serviceSettings;
            this.seedPeriod = seedPeriod;
        }
 
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
            return
            [
                new ServiceReplicaListener(context =>
                {
                    var settings = new FabricTransportRemotingListenerSettings();
                    settings.ExceptionSerializationTechnique = FabricTransportRemotingListenerSettings.ExceptionSerialization.Default;
                    settings.UseWrappedMessage = true;
                    return new FabricTransportServiceRemotingListener(
                        context,
                        new RideServiceImpl(_dictName,_queueName, StateManager, _serviceSettings ),
                        settings);
                })
            ];
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
