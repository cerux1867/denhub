using System.Text.Json;
using System.Threading.Tasks;
using Denhub.Chat.Collector.Models;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Denhub.Chat.Collector {
    public class RedisChatMessageQueue : IChatMessageAsyncQueue {
        private readonly IConnectionMultiplexer _redisClient;
        private readonly IOptions<RedisSettings> _options;

        public RedisChatMessageQueue(IOptions<RedisSettings> options, IConnectionMultiplexer redisClient) {
            _redisClient = redisClient;
            _options = options;
        }
        
        public async Task EnqueueAsync(UnprocessedChatMessage message) {
            await _redisClient.GetDatabase()
                .ListLeftPushAsync(_options.Value.QueueListKey, JsonSerializer.Serialize(message));
        }
    }
}