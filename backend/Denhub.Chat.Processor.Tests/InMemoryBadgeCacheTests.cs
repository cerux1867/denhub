using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Denhub.Chat.Processor.Caches;
using Denhub.Chat.Processor.Models;
using Denhub.Chat.Processor.Tests.Utils;
using Denhub.Common.Models;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Denhub.Chat.Processor.Tests {
    public class InMemoryBadgeCacheTests {
        private const string GlobalJsonResponse = "{\"badge_sets\":{\"1979-revolution_1\":{\"versions\":{\"1\":{\"image_url_1x\":\"https://static-cdn.jtvnw.net/badges/v1/7833bb6e-d20d-48ff-a58d-67fe827a4f84/1\",\"image_url_2x\":\"https://static-cdn.jtvnw.net/badges/v1/7833bb6e-d20d-48ff-a58d-67fe827a4f84/2\",\"image_url_4x\":\"https://static-cdn.jtvnw.net/badges/v1/7833bb6e-d20d-48ff-a58d-67fe827a4f84/3\",\"description\":\"1979 Revolution\",\"title\":\"1979 Revolution\",\"click_action\":\"visit_url\",\"click_url\":\"https://www.twitch.tv/directory/game/1979%20Revolution/details\",\"last_updated\":null}}},\"60-seconds_1\":{\"versions\":{\"1\":{\"image_url_1x\":\"https://static-cdn.jtvnw.net/badges/v1/1e7252f9-7e80-4d3d-ae42-319f030cca99/1\",\"image_url_2x\":\"https://static-cdn.jtvnw.net/badges/v1/1e7252f9-7e80-4d3d-ae42-319f030cca99/2\",\"image_url_4x\":\"https://static-cdn.jtvnw.net/badges/v1/1e7252f9-7e80-4d3d-ae42-319f030cca99/3\",\"description\":\"60 Seconds!\",\"title\":\"60 Seconds!\",\"click_action\":\"visit_url\",\"click_url\":\"https://www.twitch.tv/directory/game/60%20Seconds!/details\",\"last_updated\":null}}},\"sub-gifter\":{\"versions\":{\"1\":{\"image_url_1x\":\"https://static-cdn.jtvnw.net/badges/v1/f1d8486f-eb2e-4553-b44f-4d614617afc1/1\",\"image_url_2x\":\"https://static-cdn.jtvnw.net/badges/v1/f1d8486f-eb2e-4553-b44f-4d614617afc1/2\",\"image_url_4x\":\"https://static-cdn.jtvnw.net/badges/v1/f1d8486f-eb2e-4553-b44f-4d614617afc1/3\",\"description\":\"Has gifted a subscription to another viewer in this community\",\"title\":\"Sub Gifter\",\"click_action\":\"none\",\"click_url\":\"\",\"last_updated\":null}}},\"bits\":{\"versions\":{\"1\":{\"image_url_1x\":\"https://static-cdn.jtvnw.net/badges/v1/73b5c3fb-24f9-4a82-a852-2f475b59411c/1\",\"image_url_2x\":\"https://static-cdn.jtvnw.net/badges/v1/73b5c3fb-24f9-4a82-a852-2f475b59411c/2\",\"image_url_4x\":\"https://static-cdn.jtvnw.net/badges/v1/73b5c3fb-24f9-4a82-a852-2f475b59411c/3\",\"description\":\" \",\"title\":\"cheer 1\",\"click_action\":\"visit_url\",\"click_url\":\"https://bits.twitch.tv\",\"last_updated\":null}}}}}";
        private const string ChannelJsonResponse = "{\"badge_sets\":{\"subscriber\":{\"versions\":{\"0\":{\"image_url_1x\":\"https://static-cdn.jtvnw.net/badges/v1/f733d157-3ba8-464d-894b-e1b174f499f1/1\",\"image_url_2x\":\"https://static-cdn.jtvnw.net/badges/v1/f733d157-3ba8-464d-894b-e1b174f499f1/2\",\"image_url_4x\":\"https://static-cdn.jtvnw.net/badges/v1/f733d157-3ba8-464d-894b-e1b174f499f1/3\",\"description\":\"Subscriber\",\"title\":\"Subscriber\",\"click_action\":\"subscribe_to_channel\",\"click_url\":\"\",\"last_updated\":null},\"2000\":{\"image_url_1x\":\"https://static-cdn.jtvnw.net/badges/v1/ab637299-dabc-4c3e-8504-6ae534de643d/1\",\"image_url_2x\":\"https://static-cdn.jtvnw.net/badges/v1/ab637299-dabc-4c3e-8504-6ae534de643d/2\",\"image_url_4x\":\"https://static-cdn.jtvnw.net/badges/v1/ab637299-dabc-4c3e-8504-6ae534de643d/3\",\"description\":\"Subscriber\",\"title\":\"Subscriber\",\"click_action\":\"subscribe_to_channel\",\"click_url\":\"\",\"last_updated\":null}}}}}";
        private readonly ILogger<InMemoryBadgeCache> _loggerMock;
        
        public InMemoryBadgeCacheTests() {
            _loggerMock = new Mock<ILogger<InMemoryBadgeCache>>().Object;
        }
        
        [Fact]
        public async Task GetChannelCachedAsync_NotCached_GlobalAndChannelBadges() {
            var mockMsgHandler = HttpClientTestUtils.ConstructSequencedMockHttpMessageHandler(new List<HttpStatusCode> {
                HttpStatusCode.OK,
                HttpStatusCode.OK
            }, new List<StringContent> {
                new(GlobalJsonResponse),
                new(ChannelJsonResponse)
            });
            var cache = new InMemoryBadgeCache(_loggerMock, new HttpClient(mockMsgHandler.Object));

            var badges = await cache.GetChannelCachedAsync(123);
            
            Assert.Equal(6, badges.Count);
            Assert.Equal(BadgeType.Subscriber, badges[4].Type);
        }

        [Fact]
        public async Task GetChannelCachedAsync_Cached_GlobalAndChannelBadges() {
            var mockMsgHandler = HttpClientTestUtils.ConstructSequencedMockHttpMessageHandler(new List<HttpStatusCode> {
                HttpStatusCode.OK,
                HttpStatusCode.OK
            }, new List<StringContent> {
                new(GlobalJsonResponse),
                new(ChannelJsonResponse)
            });
            var cache = new InMemoryBadgeCache(_loggerMock, new HttpClient(mockMsgHandler.Object));

            var badges = await cache.GetChannelCachedAsync(123);
            var badgesCached = await cache.GetChannelCachedAsync(123);
            
            Assert.Equal(6, badges.Count);
            Assert.Equal(6, badgesCached.Count);
            Assert.Equal(BadgeType.Subscriber, badges[4].Type);
            Assert.Equal(BadgeType.Subscriber, badgesCached[4].Type);
        }

        [Fact]
        public async Task GetChannelCachedAsync_MalformedBadges_Throws() {
            var mockMsgHandler = HttpClientTestUtils.ConstructMockHttpMessageHandler(HttpStatusCode.OK,
                new StringContent(
                    "{\"badge_sets\":{\"bits\":{\"temp\":{\"1\":{\"image_url_1x\":\"https://static-cdn.jtvnw.net/badges/v1/73b5c3fb-24f9-4a82-a852-2f475b59411c/1\",\"image_url_2x\":\"https://static-cdn.jtvnw.net/badges/v1/73b5c3fb-24f9-4a82-a852-2f475b59411c/2\",\"image_url_4x\":\"https://static-cdn.jtvnw.net/badges/v1/73b5c3fb-24f9-4a82-a852-2f475b59411c/3\",\"description\":\" \",\"title\":\"cheer 1\",\"click_action\":\"visit_url\",\"click_url\":\"https://bits.twitch.tv\",\"last_updated\":null}}}}}"));
            var cache = new InMemoryBadgeCache(_loggerMock, new HttpClient(mockMsgHandler.Object));

            await Assert.ThrowsAnyAsync<Exception>(async () => {
                await cache.GetChannelCachedAsync(123);
            });
        }
    }
}