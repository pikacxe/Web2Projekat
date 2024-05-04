using Common.Entities;
using Common.Repository;
using Common.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace Common.MongoDB
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
            if(serviceSettings == null || mongoDbSettings == null) 
            {
                throw new ArgumentNullException();
            }
            var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
            var database = mongoClient.GetDatabase(serviceSettings.ServiceName);

            // Register database instance as singleton
            services.AddSingleton(database);

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
                if(database == null)
                {
                    throw new ArgumentNullException(nameof(database));
                }
                return new MongoRepository<T>(database, collectionName);
            });
            return services;
        }
    }
}
