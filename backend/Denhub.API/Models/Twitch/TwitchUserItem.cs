using System;

namespace Denhub.API.Models.Twitch {
    public record TwitchUserItem {
        public string Id { get; set; }
        public string Login { get; set; }
        public string DisplayName { get; set; }
        public Uri ProfileImageUrl { get; set; }
    }
}