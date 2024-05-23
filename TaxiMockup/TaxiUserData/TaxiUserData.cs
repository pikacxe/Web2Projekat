using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Common.Repository;
using Common.Entities;
using Microsoft.ServiceFabric.Data.Collections;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Common.Settings;
using TaxiUserData.Helpers;

namespace TaxiUserData
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class TaxiUserData : StatefulService
    {
        private readonly string _dictName = "usersDictionary";
        private readonly string _queueName = "usersQueue";
        private readonly IRepository<User> _repo;
        private readonly MailHelper _mailHelper;
        private readonly int seedPeriod;
        private readonly TimeSpan timeout = TimeSpan.FromSeconds(10); // timeout for realiable collections
        public TaxiUserData(StatefulServiceContext context, IRepository<User> repo, MailHelper mailHelper, int seedPeriod)
            : base(context)
        {
            _repo = repo;
            _mailHelper = mailHelper;
            this.seedPeriod = seedPeriod;
        }


        #region Data seeding methods
        private async Task SeedDataFromMongoDBAsync(CancellationToken cancellationToken)
        {
            var data = await _repo.GetAllAsync(cancellationToken);
            await SeedDataToServiceFabricAsync(data, cancellationToken);
        }
        private async Task SeedDataToServiceFabricAsync(IEnumerable<User> data, CancellationToken cancellationToken)
        {
            var myDictionary = await StateManager.GetOrAddAsync<IReliableDictionary<Guid, User>>(_dictName);
            using (var tx = StateManager.CreateTransaction())
            {
                foreach (var item in data)
                {
                    await myDictionary.AddOrUpdateAsync(tx, item.Id, item, (key, value) => item, timeout, cancellationToken);
                }

                await tx.CommitAsync();
            }
        }
        private async Task ProcessQueuedDataAsync(CancellationToken cancellationToken)
        {
                var myQueue = await StateManager.GetOrAddAsync<IReliableQueue<User>>(_queueName);
            using (var tx = StateManager.CreateTransaction())
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var result = await myQueue.TryDequeueAsync(tx, TimeSpan.FromSeconds(5), cancellationToken);
                    if (result.HasValue)
                    {
                        User user = result.Value;
                        ServiceEventSource.Current.ServiceMessage(this.Context, $"======== Item from '{_queueName}': {user.Id} ========");
                        if (user.Username == "toDelete")
                        {
                            await _repo.DeleteAsync(user.Id);
                        }
                        else
                        {
                            await _repo.UpdateAsync(user);
                        }
                    }
                    else
                    {
                        break;
                    }
                }
                await tx.CommitAsync();
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
                        new UserServiceImpl(_dictName,_queueName, StateManager, _mailHelper),
                        settings);
                })
            ];
        }

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            try
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
            catch (Exception ex)
            {
                ServiceEventSource.Current.ServiceMessage(Context, ex.Message);
            }
        }
    }
}
