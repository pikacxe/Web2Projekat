using Common.Entities;
using Common.Repository;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace Common.MongoDB
{
    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly IMongoCollection<T> dbCollection;

        private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;
        private readonly ReplaceOptions replaceOptions = new ReplaceOptions()
        {
            IsUpsert = true,
        };
        private readonly InsertOneOptions insertOneOptions = new InsertOneOptions()
        {
            BypassDocumentValidation = false
        };

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            dbCollection = database.GetCollection<T>(collectionName);
        }

        public async Task CreateAsync(T entity, CancellationToken cancellationToken)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            await dbCollection.InsertOneAsync(entity,insertOneOptions,cancellationToken);
        }

        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);
            var res = await dbCollection.DeleteOneAsync(filter,cancellationToken);
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await dbCollection.Find(filterBuilder.Empty).ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyCollection<T>> GetAllAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken)
        {
            return await dbCollection.Find(filter).ToListAsync(cancellationToken);
        }

        public async Task<IEntity> GetAsync(Guid id, CancellationToken cancellationToken)
        {
            FilterDefinition<T> filter = filterBuilder.Eq(entity => entity.Id, id);
            return await dbCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEntity> GetAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken)
        {
            return await dbCollection.Find(filter).FirstOrDefaultAsync(cancellationToken);
        }

        public async Task UpdateAsync(T entity, CancellationToken cancellationToken)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            
            FilterDefinition<T> filter = filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
            await dbCollection.ReplaceOneAsync(filter, entity, replaceOptions,cancellationToken);
        }
    }
}
