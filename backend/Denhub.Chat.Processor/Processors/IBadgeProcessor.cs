using System.Collections.Generic;
using System.Threading.Tasks;
using Denhub.Chat.Processor.Models;

namespace Denhub.Chat.Processor.Processors {
    public interface IBadgeProcessor {
        public Task<List<TwitchBadge>> ProcessAsync(long channelId, List<string> badgeList);
    }
}