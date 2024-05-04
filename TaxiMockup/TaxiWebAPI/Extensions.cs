using Common.Settings;
using Contracts;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;

namespace TaxiWebAPI
{
    public static class Extensions
    {
        public static IServiceCollection AddRideDataService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(serviceProvider =>
            {
                var serviceSettings = configuration.GetSection(nameof(RideDataServiceSettings)).Get<RideDataServiceSettings>();
                if(serviceSettings == null)
                {
                    throw new ArgumentNullException(nameof(serviceSettings));
                }
                var partKey = new ServicePartitionKey(serviceSettings.PartitionKey);
                IRideDataService proxy = ServiceProxy.Create<IRideDataService>(new Uri(serviceSettings.ConnectionString), partKey);
                return proxy;
            });
            return services;
        }
    }
}
