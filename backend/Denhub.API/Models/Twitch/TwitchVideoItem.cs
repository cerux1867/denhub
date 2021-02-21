using System;
using System.Text.Json.Serialization;
using Denhub.API.Converters;

namespace Denhub.API.Models.Twitch {
    public record TwitchVideoItem {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime PublishedAt { get; set; }
        public string ThumbnailUrl { get; set; }
        public int ViewCount { get; set; }
        [JsonConverter(typeof(TwitchDurationTimespanConverter))]
        public TimeSpan Duration { get; set; }
        public TwitchVideoItemType Type { get; set; }
    }
}