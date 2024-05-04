using System.Diagnostics;
using Common.Entities;
using Common.MongoDB;
using Common.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Runtime;

namespace TaxiRideStateful
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
                // Create service collection
                IServiceCollection services = new ServiceCollection();

                // Configure services
                ConfigureServices(services);

                // Build the service provider
                var serviceProvider = services.BuildServiceProvider();

                // The ServiceManifest.XML file defines one or more service type names.
                // Registering a service maps a service type name to a .NET type.
                // When Service Fabric creates an instance of this service type,
                // an instance of the class is created in this host process.

                var repo = serviceProvider.GetService<IRepository<Ride>>();
                if(repo == null)
                {
                    Trace.WriteLine($"Unable to initialize repository!");
                    throw new Exception("Unable to initilize repository!");
                }

                ServiceRuntime.RegisterServiceAsync("TaxiRideStatefulType",
                    context => new TaxiRideStateful(context, repo)).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(TaxiRideStateful).Name);

                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            // Load configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            services.AddMongo(configuration);
            services.AddMongoRepository<Ride>("Rides");
        }
    }
}
