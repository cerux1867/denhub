using System.Threading;
using System.Threading.Tasks;
using Denhub.Chat.Processor.Models;
using Denhub.Common.Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace Denhub.Chat.Processor.Tests {
    public class MongoDbChatMessageRepositoryTests {
        [Fact]
        public async Task AddAsync_ConfiguredDbNameAndCollection_AddsToCollection() {
            var mockedMongoClient = new Mock<IMongoClient>();
            var mockConfig = new Mock<IConfiguration>();
            var mockSection = new Mock<IConfigurationSection>();
            mockSection.SetupSequence(m => m.Value)
                .Returns("denhub")
                .Returns("chat_messages");
            mockConfig.Setup(m => m.GetSection(It.IsAny<string>())).Returns(mockSection.Object);
            var mockedCollection = new Mock<IMongoCollection<TwitchChatMessage>>();
            mockedCollection.Setup(m => m.InsertOneAsync(It.IsAny<TwitchChatMessage>(), It.IsAny<InsertOneOptions>(),
                It.IsAny<CancellationToken>()));
            var mockedDb = new Mock<IMongoDatabase>();
            mockedDb.Setup(m =>
                    m.GetCollection<TwitchChatMessage>(It.IsAny<string>(), It.IsAny<MongoCollectionSettings>()))
                .Returns(mockedCollection.Object);
            mockedMongoClient.Setup(m => m.GetDatabase(It.IsAny<string>(), It.IsAny<MongoDatabaseSettings>()))
                .Returns(mockedDb.Object);
            var repo = new MongoDbChatMessageRepository(mockConfig.Object, mockedMongoClient.Object);

            await repo.AddAsync(new TwitchChatMessage {
                Message = "test"
            });
            
            mockedCollection.Verify(m => m.InsertOneAsync(It.IsAny<TwitchChatMessage>(), It.IsAny<InsertOneOptions>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}