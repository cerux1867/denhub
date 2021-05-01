using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Denhub.Chat.Processor.Models {
    public record TwitchChatMessage {
        [BsonId]
        [JsonIgnore]
        public ObjectId Id { get; set; }
        public string MessageId { get; set; }
        public DateTime Timestamp { get; set; }
        public long UserId { get; set; }
        public string UserDisplayName { get; set; }
        public long ChannelId { get; set; }
        public string ChannelDisplayName { get; set; }
        public string UserColor { get; set; }
        [BsonIgnore]
        [JsonIgnore]
        public string[] RawBadges { get; set; }
        public List<TwitchBadge> Badges { get; set; }
        public string Message { get; set; }
        public List<TwitchEmote> Emotes { get; set; }
    }
}