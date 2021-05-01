using System.Collections.Generic;
using System.Threading.Tasks;
using Denhub.Chat.Processor.Models;

namespace Denhub.Chat.Processor.Caches {
    public interface IEmoteCache {
        public Task<List<CachedEmote>> GetChannelCachedAsync(long channelId);
    }
}