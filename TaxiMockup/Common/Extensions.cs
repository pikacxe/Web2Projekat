using Common.Entities;
using Common.MongoDB;
using Common.Repository;
using Common.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Common
{
    public static class Extensions
    {
        /// <summary>
        /// Registers mongoDb service for communicating with database
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddMongo(this IServiceCollection services, IConfiguration configuration)
        {
            BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
            BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String));

            var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
            var mongoDbSettings = configuration.GetSection(nameof(MongoDBSettings)).Get<MongoDBSettings>();
            if (serviceSettings == null || mongoDbSettings == null)
            {
                throw new ApplicationException(nameof(configuration));
            }
            if (serviceSettings.isValid && mongoDbSettings.isValid)
            {
                var mongoClient = new MongoClient($"{mongoDbSettings.ConnectionString}/{serviceSettings.ServiceName}");
                var database = mongoClient.GetDatabase(serviceSettings.ServiceName);

                // Register database instance as singleton
                services.AddSingleton(database);

            }
            else
            {
                throw new ApplicationException("Database connection settings invalid");
            }
            return services;
        }

        /// <summary>
        /// Registers mongoDb repository for given collection
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="services"></param>
        /// <param name="collectionName"></param>
        /// <returns></returns>
        public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services, string collectionName)
            where T : IEntity
        {
            services.AddSingleton<IRepository<T>>(ServiceProvider =>
            {
                var database = ServiceProvider.GetService<IMongoDatabase>();
                if (database == null)
                {
                    throw new FieldAccessException(nameof(database));
                }
                return new MongoRepository<T>(database, collectionName);
            });
            return services;
        }

        /// <summary>
        /// Registers service connections settings for UserDataService
        /// </summary>
        /// <param name="services"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        public static IServiceCollection AddUserDataServiceSettings(this IServiceCollection services, IConfiguration configuration)
        {
            var serviceSettings = configuration.GetSection(nameof(UserDataServiceSettings)).Get<UserDataServiceSettings>();
            if (serviceSettings == null || !serviceSettings.isValid)
            {
                throw new ApplicationException("User data service settings not set");
            }
            services.AddSingleton(serviceProvider =>
            {
                return serviceSettings;
            });
            return services;
        }
        public static IServiceCollection AddServiceProxyFactory(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(serviceProvider =>
            {
                var serviceProxyFactory = new ServiceProxyFactory(
                (callbackClient) =>
                {
                    var settings = new FabricTransportRemotingSettings();
                    settings.UseWrappedMessage = true;
                    return new FabricTransportServiceRemotingClientFactory(settings);
                });
                return serviceProxyFactory;
            });
            return services;
        }
    }
}
