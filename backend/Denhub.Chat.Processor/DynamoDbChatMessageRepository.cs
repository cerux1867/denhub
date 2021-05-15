using System.Text.Json;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using Denhub.Chat.Processor.Models;
using Denhub.Common.Models;
using Microsoft.Extensions.Configuration;

namespace Denhub.Chat.Processor {
    public class DynamoDbChatMessageRepository : IChatMessageRepository {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly string _tableName;

        public DynamoDbChatMessageRepository(IConfiguration config, IAmazonDynamoDB dynamoDbClient) {
            _dynamoDbClient = dynamoDbClient;
            _tableName = config.GetValue("Database:DynamoDB:TableName", "ChatMessages");
        }

        public async Task AddAsync(TwitchChatMessageBackend messageBackend) {
            var serialisedMsg = JsonSerializer.Serialize(messageBackend);
            await _dynamoDbClient.PutItemAsync(_tableName,
                Document.FromJson(serialisedMsg).ToAttributeMap());
        }
    }
}