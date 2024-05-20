using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace TaxiCDN
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance.
    /// </summary>
    internal sealed class TaxiCDN : StatelessService
    {
        public TaxiCDN(StatelessServiceContext context)
            : base(context)
        { }

        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return
            [
                new ServiceInstanceListener(context =>
                {
                    var settings = new FabricTransportRemotingListenerSettings();
                    settings.ExceptionSerializationTechnique = FabricTransportRemotingListenerSettings.ExceptionSerialization.Default;
                    settings.UseWrappedMessage = true;
                    return new FabricTransportServiceRemotingListener(
                        context,
                        new CDNServiceImpl(),
                        settings);
                })
            ];
        }
    }
}
