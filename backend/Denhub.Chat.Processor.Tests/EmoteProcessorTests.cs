using System.Collections.Generic;
using System.Threading.Tasks;
using Denhub.Chat.Processor.Caches;
using Denhub.Chat.Processor.Models;
using Denhub.Chat.Processor.Processors;
using Denhub.Common.Models;
using Moq;
using Xunit;

namespace Denhub.Chat.Processor.Tests {
    public class EmoteProcessorTests {
        [Fact]
        public async Task EnrichWithExternalEmotesAsync_MessageWithRawEmotes_EnrichedMessage() {
            var msg = new TwitchChatMessage {
                Message = "YEP TEST YEP TEST",
                Emotes = new List<TwitchEmote>()
            };
            var cacheMock = new Mock<IEmoteCache>();
            cacheMock.Setup(m => m.GetChannelCachedAsync(It.IsAny<long>())).ReturnsAsync(new List<CachedEmote> {
                new() {
                    Name = "YEP",
                    EmotePlatform = EmotePlatform.BetterTTV
                }
            });
            var processor = new EmoteProcessor(cacheMock.Object);

            var enrichedMessage = await processor.EnrichWithExternalEmotesAsync(msg);
            
            Assert.Equal(2, enrichedMessage.Emotes.Count);
        }

        [Fact]
        public async Task EnrichWithExternalEmotesAsync_MessageWithNoEmotes_EnrichedMessage() {
            var msg = new TwitchChatMessage {
                Message = "TEST TEST",
                Emotes = new List<TwitchEmote>()
            };
            var cacheMock = new Mock<IEmoteCache>();
            cacheMock.Setup(m => m.GetChannelCachedAsync(It.IsAny<long>())).ReturnsAsync(new List<CachedEmote> {
                new() {
                    Name = "YEP",
                    EmotePlatform = EmotePlatform.BetterTTV
                }
            });
            var processor = new EmoteProcessor(cacheMock.Object);

            var enrichedMessage = await processor.EnrichWithExternalEmotesAsync(msg);
            
            Assert.Empty(enrichedMessage.Emotes);
        }
    }
}