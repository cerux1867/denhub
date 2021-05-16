using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Denhub.API.Repositories;
using Denhub.API.Utils;
using Moq;
using Xunit;
using Microsoft.Extensions.Configuration;

namespace Denhub.API.Tests.Repositories {
    public class DynamoDbChatLogsRepositoryTests {
        [Fact]
        public async Task GetByChannelIdAsync_NoCursorAllItems_ListOfChatMessages() {
            var amazonClientMock = new Mock<IAmazonDynamoDB>();
            amazonClientMock.Setup(m => m.QueryAsync(It.IsAny<QueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new QueryResponse {
                    Items = new List<Dictionary<string, AttributeValue>> {
                        new(),
                        new()
                    }
                });
            var configMock = new Mock<IConfiguration>();
            var sectionMock = new Mock<IConfigurationSection>();
            sectionMock.SetupSequence(m => m.Value)
                .Returns("Test")
                .Returns("Test");
            configMock.Setup(m => m.GetSection(It.Is<string>(k => k == "Database:DynamoDB:TableName")))
                .Returns(sectionMock.Object);
            configMock.Setup(m => m.GetSection(It.Is<string>(k => k == "Database:DynamoDB:IndexName")))
                .Returns(sectionMock.Object);
            var repo = new DynamoDbChatLogsRepository(configMock.Object, amazonClientMock.Object);

            var (_, chatMessages) = await repo.GetByChannelIdAsync(123, DateTime.UtcNow);
            
            Assert.Equal(2, chatMessages.Count());
        }
        
        [Fact]
        public async Task GetByChannelIdAsync_NoCursorPagedItems_ListOfChatMessages() {
            var amazonClientMock = new Mock<IAmazonDynamoDB>();
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            amazonClientMock.Setup(m => m.QueryAsync(It.IsAny<QueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new QueryResponse {
                    Items = new List<Dictionary<string, AttributeValue>> {
                        new(),
                        new()
                    },
                    LastEvaluatedKey = new Dictionary<string, AttributeValue> {
                        {"MessageId", new AttributeValue("Test")}, 
                        {
                            "ChannelId", new AttributeValue {
                                N = "123"
                            }
                        },
                        {"Timestamp", new AttributeValue {
                            N = now
                        }}
                    }
                });
            var configMock = new Mock<IConfiguration>();
            var sectionMock = new Mock<IConfigurationSection>();
            sectionMock.SetupSequence(m => m.Value)
                .Returns("Test")
                .Returns("Test");
            configMock.Setup(m => m.GetSection(It.Is<string>(k => k == "Database:DynamoDB:TableName")))
                .Returns(sectionMock.Object);
            configMock.Setup(m => m.GetSection(It.Is<string>(k => k == "Database:DynamoDB:IndexName")))
                .Returns(sectionMock.Object);
            var repo = new DynamoDbChatLogsRepository(configMock.Object, amazonClientMock.Object);

            var (cursor, chatMessages) = await repo.GetByChannelIdAsync(123, DateTime.UtcNow);
            
            Assert.Equal(2, chatMessages.Count());
            var decoded = PaginationUtils.ConvertFromBase64(cursor);
            Assert.Equal($"Test,123,{now}", decoded);
        }
        
        [Fact]
        public async Task GetByChannelIdAsync_Cursor_ListOfChatMessages() {
            var amazonClientMock = new Mock<IAmazonDynamoDB>();
            var now = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
            amazonClientMock.Setup(m => m.QueryAsync(It.IsAny<QueryRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new QueryResponse {
                    Items = new List<Dictionary<string, AttributeValue>> {
                        new(),
                        new()
                    },
                    LastEvaluatedKey = new Dictionary<string, AttributeValue> {
                        {"MessageId", new AttributeValue("Test")}, 
                        {
                            "ChannelId", new AttributeValue {
                                N = "123"
                            }
                        },
                        {"Timestamp", new AttributeValue {
                            N = now
                        }}
                    } 
                });
            var configMock = new Mock<IConfiguration>();
            var sectionMock = new Mock<IConfigurationSection>();
            sectionMock.SetupSequence(m => m.Value)
                .Returns("Test")
                .Returns("Test");
            configMock.Setup(m => m.GetSection(It.Is<string>(k => k == "Database:DynamoDB:TableName")))
                .Returns(sectionMock.Object);
            configMock.Setup(m => m.GetSection(It.Is<string>(k => k == "Database:DynamoDB:IndexName")))
                .Returns(sectionMock.Object);
            var repo = new DynamoDbChatLogsRepository(configMock.Object, amazonClientMock.Object);

            var (cursor, chatMessages) = await repo.GetByChannelIdAsync(123, DateTime.UtcNow, PaginationUtils.ConvertToBase64($"Test,123,{now}"));
            
            Assert.Equal(2, chatMessages.Count());
            var decoded = PaginationUtils.ConvertFromBase64(cursor);
            Assert.Equal($"Test,123,{now}", decoded);
        }
    }
}