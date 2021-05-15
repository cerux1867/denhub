using System.Collections.Generic;
using Denhub.Common.Models;

namespace Denhub.API.Models {
    public class TwitchChatMessagesResult {
        public IEnumerable<TwitchChatMessagePublic> ChatMessages { get; set; }
        public string PaginationCursor { get; set; }
    }
}