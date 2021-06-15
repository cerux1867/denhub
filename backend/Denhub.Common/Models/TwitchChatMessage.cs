using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Denhub.Common.Models {
    public class TwitchChatMessage {
        [BsonId]
        [JsonIgnore]
        public ObjectId Id { get; set; }
        [BsonIgnore]
        [JsonIgnore]
        public string[] RawBadges { get; set; }

        [BsonIgnore]
        [JsonPropertyName("timestamp")]
        public DateTime UtcTimestamp => DateTimeOffset.FromUnixTimeMilliseconds(Timestamp).DateTime;
        [JsonIgnore]
        public long Timestamp { get; set; }

        public string MessageId { get; set; }
        public long UserId { get; set; }
        public string UserDisplayName { get; set; }
        public long ChannelId { get; set; }
        public string ChannelDisplayName { get; set; }
        public string UserColor { get; set; }
        public List<TwitchBadge> Badges { get; set; }
        public string Message { get; set; }
        public List<TwitchEmote> Emotes { get; set; }
    }
}