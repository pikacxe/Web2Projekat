using Common.Settings;
using Contracts;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TaxiWebAPI.Settings;

namespace TaxiWebAPI
{
    public static class Extensions
    {
        public static IServiceCollection AddRideDataServiceFactory(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(serviceProvider =>
            {
                var serviceSettings = configuration.GetSection(nameof(RideDataServiceSettings)).Get<RideDataServiceSettings>();
                if(serviceSettings == null)
                {
                    throw new ApplicationException("Ride data service settings not set");
                }
                return serviceSettings;
            });
            return services;
        }

        public static IServiceCollection AddUserDataServiceFactory(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient(serviceProvider =>
            {
                var serviceSettings = configuration.GetSection(nameof(UserDataServiceSettings)).Get<UserDataServiceSettings>();
                if (serviceSettings == null)
                {
                    throw new ApplicationException("User data service settings not set");
                }
                return serviceSettings;
            });
            return services;
        }

        public static IServiceCollection AddJwtSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(serviceProvider =>
            {
                var jwtSettings = configuration.GetSection(nameof(JwtTokenSettings)).Get<JwtTokenSettings>();
                if(jwtSettings == null)
                {
                    throw new ApplicationException("Jwt token settings not set");
                }
                return jwtSettings;
            });
            return services;
        }
    }
}
