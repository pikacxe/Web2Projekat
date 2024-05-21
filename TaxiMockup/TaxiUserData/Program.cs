using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Runtime;
using Microsoft.Extensions.Configuration;
using Common.Repository;
using Common.Entities;
using Common;
using TaxiUserData.Helpers;

namespace TaxiUserData
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
                IRepository<User> repo = serviceProvider.GetService<IRepository<User>>() ?? throw new Exception("Database connection settings missing");
                MailHelper mailHelper = serviceProvider.GetService<MailHelper>() ?? throw new Exception("Mail client setting missing");
                // Ensure single admin is created
                EnsureAdminCreation(repo).Wait();

                ServiceRuntime.RegisterServiceAsync("TaxiUserDataType",
                    context => new TaxiUserData(context, repo,mailHelper, seedPeriod)).GetAwaiter().GetResult();

                ServiceEventSource.Current.ServiceTypeRegistered(Process.GetCurrentProcess().Id, typeof(TaxiUserData).Name);

                // Prevents this host process from terminating so services keep running.
                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception e)
            {
                ServiceEventSource.Current.ServiceHostInitializationFailed(e.ToString());
                throw;
            }
        }

        private static async Task EnsureAdminCreation(IRepository<User> repo)
        {
            User? exists = await repo.GetAsync(u => u.UserType == Common.UserType.Admin) as User;
            if (exists == null)
            {
                User admin = new User()
                {
                    Id = new Guid("d725738c-70ea-4573-9944-fad093c1e8e0"),
                    Email = "admin@admin.com",
                    Username = "admin",
                    Password = "password",
                    UserType = UserType.Admin,
                    _CreatedAt = DateTime.Now,
                };
                await repo.CreateAsync(admin);
            }
        }

        private static IServiceProvider SetupServices(IConfiguration configuration)
        {
            IServiceCollection services = new ServiceCollection();
            services.AddMongo(configuration);
            services.AddMongoRepository<User>("User");
            services.AddMailClient(configuration);

            return services.BuildServiceProvider();
        }

        private static IConfiguration SetupConfiguration()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional:false)
                .Build();
            return configuration;
        }
    }
}
