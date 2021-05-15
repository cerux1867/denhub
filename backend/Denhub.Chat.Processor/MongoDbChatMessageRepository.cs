using System.Threading.Tasks;
using Denhub.Chat.Processor.Models;
using Denhub.Common.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace Denhub.Chat.Processor {
    public class MongoDbChatMessageRepository : IChatMessageRepository {
        private readonly IMongoCollection<TwitchChatMessageBackend> _collection;
        
        public MongoDbChatMessageRepository(IConfiguration config, IMongoClient mongoClient) {
            var dbName = config.GetValue("Database:MongoDB:DatabaseName", "denhub");
            var collectionName = config.GetValue("Database:MongoDB:CollectionName", "chat_messages");
            
            var db = mongoClient.GetDatabase(dbName);
            _collection = db.GetCollection<TwitchChatMessageBackend>(collectionName);
        }
        
        public async Task AddAsync(TwitchChatMessageBackend messageBackend) {
            await _collection.InsertOneAsync(messageBackend);
        }
    }
}