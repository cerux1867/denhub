using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Denhub.Chat.Processor.Caches;
using Denhub.Chat.Processor.Tests.Utils;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Denhub.Chat.Processor.Tests {
    public class InMemoryEmoteCacheTests {
        private const string BttvGlobalResponse = "[{\"id\":\"54fa8f1401e468494b85b537\",\"code\":\":tf:\",\"imageType\":\"png\",\"userId\":\"5561169bd6b9d206222a8c19\"},{\"id\":\"54fa8fce01e468494b85b53c\",\"code\":\"CiGrip\",\"imageType\":\"png\",\"userId\":\"5561169bd6b9d206222a8c19\"}]";
        private const string BttvChannelResponse = "{\"id\":\"5a78315ff730010d194c1f0c\",\"bots\":[\"esfandbot\"],\"channelEmotes\":[{\"id\":\"5b490907cf46791f8491f69f\",\"code\":\"cmonBrother\",\"imageType\":\"png\",\"userId\":\"5a78315ff730010d194c1f0c\"},{\"id\":\"5b25a3a85ea9d964f40020e0\",\"code\":\"gachiESFAND\",\"imageType\":\"gif\",\"userId\":\"5a78315ff730010d194c1f0c\"}],\"sharedEmotes\":[{\"id\":\"56c2cff2d9ec6bf744247bf1\",\"code\":\"KKool\",\"imageType\":\"gif\",\"user\":{\"id\":\"559c3b94a9d6c1121305ff4e\",\"name\":\"garych\",\"displayName\":\"garych\",\"providerId\":\"30393449\"}},{\"id\":\"5b35ca08392c604c5fd81874\",\"code\":\"HYPERCLAP\",\"imageType\":\"gif\",\"user\":{\"id\":\"55c94879fc8fb4101a3b8a60\",\"name\":\"alfurin\",\"displayName\":\"Alfurin\",\"providerId\":\"41528856\"}}]}";
        private const string FfzResponse = "{\"room\":{\"_id\":293868,\"twitch_id\":38746172,\"id\":\"esfandtv\",\"is_group\":false,\"display_name\":\"EsfandTV\",\"set\":293880,\"moderator_badge\":null,\"vip_badge\":null,\"mod_urls\":null,\"user_badges\":{},\"user_badge_ids\":{},\"css\":null},\"sets\":{\"293880\":{\"id\":293880,\"_type\":1,\"icon\":null,\"title\":\"Channel: EsfandTV\",\"css\":null,\"emoticons\":[{\"id\":270930,\"name\":\"widepeepoHappy\",\"height\":19,\"width\":80,\"public\":true,\"hidden\":false,\"modifier\":false,\"offset\":null,\"margins\":null,\"css\":null,\"owner\":{\"_id\":306186,\"name\":\"black__tic_tac\",\"display_name\":\"black__tic_tac\"},\"urls\":{\"1\":\"//cdn.frankerfacez.com/emote/270930/1\",\"2\":\"//cdn.frankerfacez.com/emote/270930/2\",\"4\":\"//cdn.frankerfacez.com/emote/270930/4\"},\"status\":1,\"usage_count\":116970,\"created_at\":\"2018-08-04T02:40:31.258Z\",\"last_updated\":\"2018-08-04T04:02:23.759Z\"},{\"id\":214681,\"name\":\"monkaW\",\"height\":32,\"width\":32,\"public\":true,\"hidden\":false,\"modifier\":false,\"offset\":null,\"margins\":null,\"css\":null,\"owner\":{\"_id\":102274,\"name\":\"voparos_\",\"display_name\":\"voparoS_\"},\"urls\":{\"1\":\"//cdn.frankerfacez.com/emote/214681/1\",\"2\":\"//cdn.frankerfacez.com/emote/214681/2\",\"4\":\"//cdn.frankerfacez.com/emote/214681/4\"},\"status\":1,\"usage_count\":104693,\"created_at\":\"2017-09-26T23:24:43.345Z\",\"last_updated\":\"2017-09-27T05:56:47.462Z\"}]}}}";
        private readonly ILogger<InMemoryEmoteCache> _loggerMock;

        public InMemoryEmoteCacheTests() {
            _loggerMock = new Mock<ILogger<InMemoryEmoteCache>>().Object;
        }
        
        [Fact]
        public async Task GetChannelCachedAsync_NotCached_ListOfEmotes() {
            var mockMsgHandler = HttpClientTestUtils.ConstructSequencedMockHttpMessageHandler(new List<HttpStatusCode> {
                HttpStatusCode.OK,
                HttpStatusCode.OK,
                HttpStatusCode.OK
            }, new List<StringContent> {
                new(BttvGlobalResponse),
                new(BttvChannelResponse),
                new(FfzResponse)
            });
            var cache = new InMemoryEmoteCache(_loggerMock, new HttpClient(mockMsgHandler.Object));

            var emotes = await cache.GetChannelCachedAsync(123);
            
            Assert.Equal(8, emotes.Count);
        }
        
        [Fact]
        public async Task GetChannelCachedAsync_NotCachedNoFfzEmotes_ListOfEmotes() {
            var mockMsgHandler = HttpClientTestUtils.ConstructSequencedMockHttpMessageHandler(new List<HttpStatusCode> {
                HttpStatusCode.OK,
                HttpStatusCode.OK,
                HttpStatusCode.NotFound
            }, new List<StringContent> {
                new(BttvGlobalResponse),
                new(BttvChannelResponse),
                new("")
            });
            var cache = new InMemoryEmoteCache(_loggerMock, new HttpClient(mockMsgHandler.Object));

            var emotes = await cache.GetChannelCachedAsync(123);
            
            Assert.Equal(6, emotes.Count);
        }
        
        [Fact]
        public async Task GetChannelCachedAsync_Cached_ListOfEmotes() {
            var mockMsgHandler = HttpClientTestUtils.ConstructSequencedMockHttpMessageHandler(new List<HttpStatusCode> {
                HttpStatusCode.OK,
                HttpStatusCode.OK,
                HttpStatusCode.OK
            }, new List<StringContent> {
                new(BttvGlobalResponse),
                new(BttvChannelResponse),
                new(FfzResponse)
            });
            var cache = new InMemoryEmoteCache(_loggerMock, new HttpClient(mockMsgHandler.Object));

            var emotes = await cache.GetChannelCachedAsync(123);
            var emotesCached = await cache.GetChannelCachedAsync(123);
            
            Assert.Equal(8, emotes.Count);
            Assert.Equal(8, emotesCached.Count);
        }
        
        [Fact]
        public async Task GetChannelCachedAsync_MalformedResponse_Throws() {
            var mockMsgHandler = HttpClientTestUtils.ConstructMockHttpMessageHandler(HttpStatusCode.OK,
                new StringContent(
                    "{\"room\":{\"_id\":293868,\"twitch_id\":38746172,\"id\":\"esfandtv\",\"is_group\":false,\"display_name\":\"EsfandTV\",\"set\":293880,\"moderator_badge\":null,\"vip_badge\":null,\"mod_urls\":null,\"user_badges\":{},\"user_badge_ids\":{},\"css\":null},\"test\":{\"293880\":{\"id\":293880,\"_type\":1,\"icon\":null,\"title\":\"Channel: EsfandTV\",\"css\":null,\"emoticons\":[{\"id\":270930,\"name\":\"widepeepoHappy\",\"height\":19,\"width\":80,\"public\":true,\"hidden\":false,\"modifier\":false,\"offset\":null,\"margins\":null,\"css\":null,\"owner\":{\"_id\":306186,\"name\":\"black__tic_tac\",\"display_name\":\"black__tic_tac\"},\"urls\":{\"1\":\"//cdn.frankerfacez.com/emote/270930/1\",\"2\":\"//cdn.frankerfacez.com/emote/270930/2\",\"4\":\"//cdn.frankerfacez.com/emote/270930/4\"},\"status\":1,\"usage_count\":116970,\"created_at\":\"2018-08-04T02:40:31.258Z\",\"last_updated\":\"2018-08-04T04:02:23.759Z\"},{\"id\":214681,\"name\":\"monkaW\",\"height\":32,\"width\":32,\"public\":true,\"hidden\":false,\"modifier\":false,\"offset\":null,\"margins\":null,\"css\":null,\"owner\":{\"_id\":102274,\"name\":\"voparos_\",\"display_name\":\"voparoS_\"},\"urls\":{\"1\":\"//cdn.frankerfacez.com/emote/214681/1\",\"2\":\"//cdn.frankerfacez.com/emote/214681/2\",\"4\":\"//cdn.frankerfacez.com/emote/214681/4\"},\"status\":1,\"usage_count\":104693,\"created_at\":\"2017-09-26T23:24:43.345Z\",\"last_updated\":\"2017-09-27T05:56:47.462Z\"}]}}}"));
            var cache = new InMemoryEmoteCache(_loggerMock, new HttpClient(mockMsgHandler.Object));

            await Assert.ThrowsAnyAsync<Exception>(async () => {
                await cache.GetChannelCachedAsync(123);
            });
        }
    }
}