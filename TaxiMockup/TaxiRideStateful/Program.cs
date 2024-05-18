using System.Diagnostics;
using Common.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Runtime;
using Common.MongoDB;
using Common.Repository;
using Microsoft.ServiceFabric.Services.Communication.Client;

namespace TaxiRideData
{
    internal static class Program
    {
        /// <summary>
        /// This is the entry point of the service host process.
        /// </summary>
        private static void Main()
        {
            try
            {
                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.

                // Setup configuration
                IConfiguration configuration = SetupConfiguration();

                // Register services
                IServiceProvider serviceProvider = SetupServices(configuration);
                int seedPeriod;
                int.TryParse(configuration.GetSection("SeedPeriod").Value, out seedPeriod);
                IRepository<Ride> repo = serviceProvider.GetService<IRepository<Ride>>() ?? throw new Exception("Database connection missing");

                ServiceRuntime.RegisterServiceAsync("TaxiRideDataType",
                    context => new TaxiRideData(context, repo, seedPeriod)).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(TaxiRideData).Name);

                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }

        private static IServiceProvider SetupServices(IConfiguration configuration)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddMongo(configuration);
            services.AddMongoRepository<Ride>("Ride");

            return services.BuildServiceProvider();
        }

        private static IConfiguration SetupConfiguration()
        {
            var settingsPath = Path.Combine(
            FabricRuntime.GetActivationContext().GetCodePackageObject("Code").Path,
            "appsettings.json");

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(settingsPath)
                .Build();
            return configuration;
        }
    }
}
