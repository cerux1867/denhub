using System;
using System.Threading.Tasks;
using Denhub.Chat.Collector.Models;
using Microsoft.Extensions.Options;
using Moq;
using StackExchange.Redis;
using Xunit;

namespace Denhub.Chat.Collector.Tests {
    public class RedisChatMessageQueueTests {
        [Fact]
        public async Task EnqueueAsync_ValidMessage_SerializesAndLeftPush() {
            var msg = new UnprocessedChatMessage {
                RawChatMessage = "random chat string",
                TimeReceived = DateTime.UtcNow
            };
            var multiplexerMock = new Mock<IConnectionMultiplexer>();
            var redisDbMock = new Mock<IDatabase>();
            var redisOptionsMock = new Mock<IOptions<RedisSettings>>();
            redisOptionsMock.Setup(m => m.Value).Returns(new RedisSettings {
                ConfigString = "localhost:6379",
                QueueListKey = "denhub_chat_messages_queue"
            });
            multiplexerMock.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(redisDbMock.Object);
            var queue = new RedisChatMessageQueue(redisOptionsMock.Object, multiplexerMock.Object);

            await queue.EnqueueAsync(msg);
            
            redisDbMock.Verify(m => m.ListLeftPushAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<When>(), It.IsAny<CommandFlags>()), Times.Once);
        }
    }
}