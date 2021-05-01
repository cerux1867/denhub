using System.Collections.Generic;
using System.Threading.Tasks;
using Denhub.Chat.Processor.Caches;
using Denhub.Chat.Processor.Models;
using Denhub.Chat.Processor.Processors;
using Moq;
using Xunit;

namespace Denhub.Chat.Processor.Tests {
    public class BadgeProcessorTests {
        [Fact]
        public async Task ProcessAsync_RawBadges_ProcessedBadgeList() {
            var mockBadgeCache = new Mock<IBadgeCache>();
            mockBadgeCache.Setup(m => m.GetChannelCachedAsync(It.IsAny<long>())).ReturnsAsync(new List<TwitchBadge> {
                new() {
                    Name = "subscriber",
                    Version = "0"
                },
                new() {
                    Name = "bits",
                    Version = "0"
                }
            });
            var processor = new BadgeProcessor(mockBadgeCache.Object);

            var badges = await processor.ProcessAsync(123, new List<string> {
                "subscriber/0",
                "bits/0"
            });
            
            Assert.Equal(2, badges.Count);
        }

        [Fact]
        public async Task ProcessAsync_NoBadges_EmptyList() {
            var mockBadgeCache = new Mock<IBadgeCache>();
            mockBadgeCache.Setup(m => m.GetChannelCachedAsync(It.IsAny<long>())).ReturnsAsync(new List<TwitchBadge> {
                new() {
                    Name = "subscriber",
                    Version = "0"
                },
                new() {
                    Name = "bits",
                    Version = "0"
                }
            });
            var processor = new BadgeProcessor(mockBadgeCache.Object);

            var badges = await processor.ProcessAsync(123, new List<string>());
            
            Assert.Empty(badges);
        }
    }
}