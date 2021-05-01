using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Denhub.Chat.Processor.Models;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace Denhub.Chat.Processor.Tests {
    public class DynamoDbChatMessageRepositoryTests {
        [Fact]
        public async Task AddAsync_ConfiguredTableName_AddsToTable() {
            var mockedDynamoDbClient = new Mock<IAmazonDynamoDB>();
            mockedDynamoDbClient.Setup(m => m.PutItemAsync(It.IsAny<string>(),
                It.IsAny<Dictionary<string, AttributeValue>>(), It.IsAny<ReturnValue>(),
                It.IsAny<CancellationToken>()));
            var mockConfig = new Mock<IConfiguration>();
            var mockSection = new Mock<IConfigurationSection>();
            mockSection.Setup(m => m.Value).Returns("test");
            mockConfig.Setup(m => m.GetSection(It.IsAny<string>())).Returns(mockSection.Object);
            var repo = new DynamoDbChatMessageRepository(mockConfig.Object, mockedDynamoDbClient.Object);

            await repo.AddAsync(new TwitchChatMessage {
                Message = "test"
            });
            
            mockedDynamoDbClient.Verify(m => m.PutItemAsync("test", It.IsAny<Dictionary<string,AttributeValue>>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}