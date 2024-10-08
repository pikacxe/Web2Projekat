﻿using Common.Settings;
using Contracts;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using TaxiWebAPI.Settings;

namespace TaxiWebAPI
{
    public static class Extensions
    {
        public static IServiceCollection AddRideDataServiceSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(serviceProvider =>
            {
                var serviceSettings = configuration.GetSection(nameof(RideDataServiceSettings)).Get<RideDataServiceSettings>();
                if(serviceSettings == null || !serviceSettings.isValid)
                {
                    throw new ApplicationException("Ride data service settings not set");
                }
                return serviceSettings;
            });
            return services;
        }
        public static IServiceCollection AddCDNServiceSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(serviceProvider =>
            {
                var serviceSettings = configuration.GetSection(nameof(CDNServiceSettings)).Get<CDNServiceSettings>();
                if (serviceSettings == null || !serviceSettings.isValid)
                {
                    throw new ApplicationException("CDN service settings not set");
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
                if(jwtSettings == null || !jwtSettings.isValid)
                {
                    throw new ApplicationException("Jwt token settings not set");
                }
                return jwtSettings;
            });
            return services;
        }
    }
}
