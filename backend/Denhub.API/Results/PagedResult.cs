using System.Collections.Generic;
using Denhub.Common.Models;

namespace Denhub.API.Results {
    public class PagedResult {
        public List<TwitchChatMessage> ChatMessages { get; set; }
        public PaginationMetadata Metadata { get; set; }
    }
}