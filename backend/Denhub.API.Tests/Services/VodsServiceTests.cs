using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Denhub.API.Models;
using Denhub.API.Models.Twitch;
using Denhub.API.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Denhub.API.Tests.Services {
    public class VodsServiceTests {
        private readonly Mock<ILogger<VodsService>> _logger;

        public VodsServiceTests() {
            _logger = new Mock<ILogger<VodsService>>();
        }

        [Fact]
        public async Task GetVideosByUserIdAsync_TwitchVodsForChannel_ListOfVods() {
            var vodRepoMock = new Mock<IVodRepository>();
            var twitchClientMock = new Mock<ITwitchClient>();
            vodRepoMock.Setup(m => m.GetOrFetchVodsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new List<CommonVodModel> {
                new() {
                    Title = "test"
                },
                new() {
                    Title = "test"
                },
                new() {
                    Title = "test"
                }
            });
            var service = new VodsService(_logger.Object, vodRepoMock.Object, twitchClientMock.Object);

            var result = (await service.GetAllByIdAsync(1)).ToList();

            Assert.Equal(3, result.Count);
        }

        [Fact]
        public async Task GetVideosByUserIdAsync_ChannelDoesntExist_EmptyList() {
            var vodRepoMock = new Mock<IVodRepository>();
            var twitchClientMock = new Mock<ITwitchClient>();
            vodRepoMock.Setup(m => m.GetOrFetchVodsAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new List<CommonVodModel>());
            var service = new VodsService(_logger.Object, vodRepoMock.Object, twitchClientMock.Object);

            var result = (await service.GetAllByIdAsync(1)).ToList();

            Assert.Empty(result);
        }
        
        [Fact]
        public async Task GetTwitchChannelIdByChannelNameAsync_ChannelExists_ChannelId() {
            var vodRepoMock = new Mock<IVodRepository>();
            var twitchClientMock = new Mock<ITwitchClient>();
            twitchClientMock.Setup(m => m.GetUsersAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(
                new TwitchResponseModel<IEnumerable<TwitchUserItem>> {
                    Data = new[] {
                        new TwitchUserItem {
                            Id = "1"
                        }
                    },
                    Pagination = new TwitchPagination()
                });
            var service = new VodsService(_logger.Object, vodRepoMock.Object, twitchClientMock.Object);

            var result = await service.GetChannelIdByChannelNameAsync("test");

            Assert.Equal(1, result);
        }
        
        [Fact]
        public async Task GetTwitchChannelIdByChannelNameAsync_ChannelDoesNotExist_MinusOne() {
            var vodRepoMock = new Mock<IVodRepository>();
            var twitchClientMock = new Mock<ITwitchClient>();
            twitchClientMock.Setup(m => m.GetUsersAsync(It.IsAny<IEnumerable<string>>())).ReturnsAsync(
                new TwitchResponseModel<IEnumerable<TwitchUserItem>> {
                    Data = Array.Empty<TwitchUserItem>(),
                    Pagination = new TwitchPagination()
                });
            var service = new VodsService(_logger.Object, vodRepoMock.Object, twitchClientMock.Object);

            var result = await service.GetChannelIdByChannelNameAsync("test");

            Assert.Equal(-1, result);
        }
    }
}