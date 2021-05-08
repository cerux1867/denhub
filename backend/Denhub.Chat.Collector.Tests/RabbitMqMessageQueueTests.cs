using System;
using System.Threading.Tasks;
using Denhub.Common;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RabbitMQ.Client;
using Xunit;

namespace Denhub.Chat.Collector.Tests {
    public class RabbitMqMessageQueueTests {
        [Fact]
        public async Task EnqueueAsync_ValidMessage_SerializesAndLeftPush() {
            var msg = new UnprocessedChatMessage {
                RawChatMessage = "random chat string",
                TimeReceived = DateTime.UtcNow
            };
            var connectionMock = new Mock<IConnection>();
            var optionsMock = new Mock<IOptions<QueueSettings>>();
            optionsMock.SetupGet(m => m.Value).Returns(new QueueSettings {
                ConnectionString = "test",
                ExchangeName = "test",
                QueueName = "test"
            });
            var channelMock = new Mock<IModel>();
            channelMock.Setup(m => m.CreateBasicProperties()).Returns(new Mock<IBasicProperties>().Object);
            connectionMock.Setup(m => m.CreateModel()).Returns(channelMock.Object);
            var loggerMock = new Mock<ILogger<RabbitMqQueue>>();
            var queue = new RabbitMqQueue(loggerMock.Object, optionsMock.Object, connectionMock.Object);

            await queue.EnqueueAsync(msg);
            
            channelMock.Verify(m => m.BasicPublish(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<IBasicProperties>(), It.IsAny<ReadOnlyMemory<byte>>()), Times.Once);
        }
    }
}