using System;
using System.Threading.Tasks;
using Denhub.API.Models;
using Denhub.Common.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Denhub.API.Repositories {
    public class ChatLogsRepository : IChatLogsRepository {
        private readonly IMongoCollection<TwitchChatMessage> _collection;

        public ChatLogsRepository(IOptions<MongoDbOptions> options, IMongoClient mongoClient) {
            var db = mongoClient.GetDatabase(options.Value.DatabaseName);
            if (db == null) {
                throw new ArgumentException($"Database with name {options.Value.DatabaseName} does not exist");
            }

            _collection = db.GetCollection<TwitchChatMessage>(options.Value.ChatLogsCollectionName);
            if (_collection == null) {
                throw new ArgumentException($"Collection with name {options.Value.ChatLogsCollectionName} does not exist");
            }
        }
        
        /// <inheritdoc />
        public Task<IMongoQueryable<TwitchChatMessage>> GetAllAsync() {
            return Task.FromResult(_collection.AsQueryable());
        }

        /// <inheritdoc />
        public async Task InsertAsync(TwitchChatMessage item) {
            await _collection.InsertOneAsync(item);
        }
    }
}