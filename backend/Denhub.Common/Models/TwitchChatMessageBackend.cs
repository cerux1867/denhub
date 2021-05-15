using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Denhub.Common.Models {
    public class TwitchChatMessageBackend : TwitchChatMessageBase {
        [BsonId]
        [JsonIgnore]
        public ObjectId Id { get; set; }
        [BsonIgnore]
        [JsonIgnore]
        public string[] RawBadges { get; set; }
        public long Timestamp { get; set; }
    }
}