using System.Threading.Tasks;
using Denhub.Chat.Processor.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Denhub.Chat.Processor {
    public class MongoDbChatMessageRepository : IChatMessageRepository {
        private readonly IMongoCollection<TwitchChatMessage> _collection;
        
        public MongoDbChatMessageRepository(IConfiguration config, IMongoClient mongoClient) {
            var dbName = config.GetValue("Database:MongoDB:DatabaseName", "denhub");
            var collectionName = config.GetValue("Database:MongoDB:CollectionName", "chat_messages");
            
            var db = mongoClient.GetDatabase(dbName);
            _collection = db.GetCollection<TwitchChatMessage>(collectionName);
        }
        
        public async Task AddAsync(TwitchChatMessage message) {
            await _collection.InsertOneAsync(message);
        }
    }
}