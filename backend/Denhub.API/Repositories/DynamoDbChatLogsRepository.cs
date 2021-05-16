using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Denhub.API.Utils;
using Denhub.Common.Models;
using Microsoft.Extensions.Configuration;

namespace Denhub.API.Repositories {
    public class DynamoDbChatLogsRepository : IChatLogsRepository {
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly string _tableName;
        private readonly string _indexName;

        public DynamoDbChatLogsRepository(IConfiguration config, IAmazonDynamoDB dynamoClient) {
            _dynamoDbClient = dynamoClient;
            _tableName = config.GetValue("Database:DynamoDB:TableName", "ChatMessages");
            _indexName = config.GetValue("Database:DynamoDB:IndexName", "ChannelAtTime");
        }

        public async Task<(string, IEnumerable<TwitchChatMessageBackend>)> GetByChannelIdAsync(long channelId,
            bool isAscending, DateTime timestamp,
            string paginationToken = null, int limit = 100) {
            var queryRequest = new QueryRequest {
                IndexName = _indexName,
                TableName = _tableName,
                KeyConditionExpression = "ChannelId = :channelId AND #t >= :time",
                ScanIndexForward = isAscending,
                ExpressionAttributeValues = new Dictionary<string, AttributeValue> {
                    {
                        ":channelId", new AttributeValue {
                            N = channelId.ToString()
                        }
                    }, {
                        ":time", new AttributeValue {
                            N = new DateTimeOffset(timestamp).ToUnixTimeMilliseconds().ToString()
                        }
                    }
                },
                ExpressionAttributeNames = new Dictionary<string, string> {
                    {"#t", "Timestamp"}
                },
                Limit = limit
            };

            if (!string.IsNullOrEmpty(paginationToken)) {
                var decodedCursor = PaginationUtils.ConvertFromBase64(paginationToken);
                var decodedSplit = decodedCursor.Split(",");
                queryRequest.ExclusiveStartKey = new Dictionary<string, AttributeValue> {
                    {
                        "MessageId", new AttributeValue(decodedSplit[0])
                    }, {
                        "ChannelId", new AttributeValue {
                            N = decodedSplit[1]
                        }
                    }, {
                        "Timestamp", new AttributeValue {
                            N = decodedSplit[2]
                        }
                    }
                };
            }

            var queryResult = await _dynamoDbClient.QueryAsync(queryRequest);
            var context = new DynamoDBContext(_dynamoDbClient);
            var documents = queryResult.Items.Select(Document.FromAttributeMap).ToList();

            if (queryResult.LastEvaluatedKey.Count >= 3) {
                queryResult.LastEvaluatedKey.TryGetValue("MessageId", out var cursorMessageId);
                queryResult.LastEvaluatedKey.TryGetValue("Timestamp", out var cursorTimestamp);
                queryResult.LastEvaluatedKey.TryGetValue("ChannelId", out var cursorChannelId);
                return (PaginationUtils.ConvertToBase64($"{cursorMessageId.S},{cursorChannelId.N},{cursorTimestamp.N}"),
                    context.FromDocuments<TwitchChatMessageBackend>(documents));
            }

            return (null, context.FromDocuments<TwitchChatMessageBackend>(documents));
        }
    }
}