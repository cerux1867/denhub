using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Denhub.Chat.Processor.Caches;
using Denhub.Chat.Processor.Models;
using Denhub.Common.Models;

namespace Denhub.Chat.Processor.Processors {
    public class BadgeProcessor : IBadgeProcessor {
        private readonly IBadgeCache _badgeCache;

        public BadgeProcessor(IBadgeCache badgeCache) {
            _badgeCache = badgeCache;
        }
        
        public async Task<List<TwitchBadge>> ProcessAsync(long channelId, List<string> badgeList) {
            var processedBadges = new List<TwitchBadge>();
            var channelBadges = await _badgeCache.GetChannelCachedAsync(channelId);
            foreach (var badge in badgeList) {
                var splitBadge = badge.Split("/");
                var processedBadge =
                    channelBadges.FirstOrDefault(b => b.Name == splitBadge[0] && b.Version == splitBadge[1]);
                processedBadges.Add(processedBadge);
            }

            return processedBadges;
        }
    }
}