using System.Collections.Generic;

namespace Denhub.Common.Models {
    public class TwitchChatMessageBase {
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