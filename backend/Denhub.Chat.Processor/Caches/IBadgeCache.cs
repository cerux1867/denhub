using System.Collections.Generic;
using System.Threading.Tasks;
using Denhub.Chat.Processor.Models;
using Denhub.Common.Models;

namespace Denhub.Chat.Processor.Caches {
    public interface IBadgeCache {
        Task<List<TwitchBadge>> GetChannelCachedAsync(long channelId);
    }
}