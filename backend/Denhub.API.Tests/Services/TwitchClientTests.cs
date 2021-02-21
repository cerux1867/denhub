using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Denhub.API.Models;
using Denhub.API.Models.Twitch;
using Denhub.API.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Xunit;

namespace Denhub.API.Tests.Services {
    public class TwitchClientTests {
        private readonly Mock<ILogger<TwitchClient>> _logger;
        private readonly Mock<IOptions<TwitchClientSettings>> _optionsMock;

        public TwitchClientTests() {
            _logger = new Mock<ILogger<TwitchClient>>();
            _optionsMock = new Mock<IOptions<TwitchClientSettings>>();
            _optionsMock.SetupGet(m => m.Value).Returns(new TwitchClientSettings {
                Token = "test",
                ClientId = "test"
            });
        }

        [Theory]
        [InlineData(TwitchVideoItemType.All, "{\"data\":[{\"id\":\"234482848\",\"user_id\":\"67955580\",\"user_login\":\"chewiemelodies\",\"user_name\":\"ChewieMelodies\",\"title\":\"-\",\"description\":\"\",\"created_at\":\"2018-03-02T20:53:41Z\",\"published_at\":\"2018-03-02T20:53:41Z\",\"url\":\"https://www.twitch.tv/videos/234482848\",\"thumbnail_url\":\"https://static-cdn.jtvnw.net/s3_vods/bebc8cba2926d1967418_chewiemelodies_27786761696_805342775/thumb/thumb0-%{width}x%{height}.jpg\",\"viewable\":\"public\",\"view_count\":142,\"language\":\"en\",\"type\":\"archive\",\"duration\":\"3h8m33s\"}],\"pagination\":{\"cursor\":\"eyJiIjpudWxsLCJhIjoiMTUwMzQ0MTc3NjQyNDQyMjAwMCJ9\"}}")]
        [InlineData(TwitchVideoItemType.Archive, "{\"data\":[{\"id\":\"234482848\",\"user_id\":\"67955580\",\"user_login\":\"chewiemelodies\",\"user_name\":\"ChewieMelodies\",\"title\":\"-\",\"description\":\"\",\"created_at\":\"2018-03-02T20:53:41Z\",\"published_at\":\"2018-03-02T20:53:41Z\",\"url\":\"https://www.twitch.tv/videos/234482848\",\"thumbnail_url\":\"https://static-cdn.jtvnw.net/s3_vods/bebc8cba2926d1967418_chewiemelodies_27786761696_805342775/thumb/thumb0-%{width}x%{height}.jpg\",\"viewable\":\"public\",\"view_count\":142,\"language\":\"en\",\"type\":\"archive\",\"duration\":\"3h8m33s\"}],\"pagination\":{\"cursor\":\"eyJiIjpudWxsLCJhIjoiMTUwMzQ0MTc3NjQyNDQyMjAwMCJ9\"}}")]
        [InlineData(TwitchVideoItemType.Highlight, "{\"data\":[{\"id\":\"234482848\",\"user_id\":\"67955580\",\"user_login\":\"chewiemelodies\",\"user_name\":\"ChewieMelodies\",\"title\":\"-\",\"description\":\"\",\"created_at\":\"2018-03-02T20:53:41Z\",\"published_at\":\"2018-03-02T20:53:41Z\",\"url\":\"https://www.twitch.tv/videos/234482848\",\"thumbnail_url\":\"https://static-cdn.jtvnw.net/s3_vods/bebc8cba2926d1967418_chewiemelodies_27786761696_805342775/thumb/thumb0-%{width}x%{height}.jpg\",\"viewable\":\"public\",\"view_count\":142,\"language\":\"en\",\"type\":\"highlight\",\"duration\":\"3h8m33s\"}],\"pagination\":{\"cursor\":\"eyJiIjpudWxsLCJhIjoiMTUwMzQ0MTc3NjQyNDQyMjAwMCJ9\"}}")]
        [InlineData(TwitchVideoItemType.Upload, "{\"data\":[{\"id\":\"234482848\",\"user_id\":\"67955580\",\"user_login\":\"chewiemelodies\",\"user_name\":\"ChewieMelodies\",\"title\":\"-\",\"description\":\"\",\"created_at\":\"2018-03-02T20:53:41Z\",\"published_at\":\"2018-03-02T20:53:41Z\",\"url\":\"https://www.twitch.tv/videos/234482848\",\"thumbnail_url\":\"https://static-cdn.jtvnw.net/s3_vods/bebc8cba2926d1967418_chewiemelodies_27786761696_805342775/thumb/thumb0-%{width}x%{height}.jpg\",\"viewable\":\"public\",\"view_count\":142,\"language\":\"en\",\"type\":\"upload\",\"duration\":\"3h8m33s\"}],\"pagination\":{\"cursor\":\"eyJiIjpudWxsLCJhIjoiMTUwMzQ0MTc3NjQyNDQyMjAwMCJ9\"}}")]
        public async Task GetVideosByUserIdAsync_VideosExist_ResponseWithListOfVods(TwitchVideoItemType type, string jsonString) {
            var messageHandlerMock = new Mock<HttpMessageHandler>();
            var httpClientMock = new Mock<HttpClient>(messageHandlerMock.Object);
            messageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage {
                    Content = new StringContent(jsonString, Encoding.UTF8, "application/json"),
                    StatusCode = HttpStatusCode.OK
                })
                .Verifiable();
            var client = new TwitchClient(_logger.Object, httpClientMock.Object, _optionsMock.Object);

            var response = await client.GetVideosByUserIdAsync(1, null, 100, type);
            var list = response.Data.ToList();
            
            Assert.Single(list);
        }
        
        [Fact]
        public async Task GetVideosByUserIdAsync_ApiError_Throws() {
            var messageHandlerMock = new Mock<HttpMessageHandler>();
            var httpClientMock = new Mock<HttpClient>(messageHandlerMock.Object);
            messageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage {
                    StatusCode = HttpStatusCode.BadRequest
                })
                .Verifiable();
            var client = new TwitchClient(_logger.Object, httpClientMock.Object, _optionsMock.Object);

            await Assert.ThrowsAsync<Exception>(async () => await client.GetVideosByUserIdAsync(1));
        }
        
        [Fact]
        public async Task GetVideosByUserIdAsync_NoVideos_ResponseWithEmptyVods() {
            var messageHandlerMock = new Mock<HttpMessageHandler>();
            var httpClientMock = new Mock<HttpClient>(messageHandlerMock.Object);
            messageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage {
                    Content = new StringContent(@"{""data"":[],""pagination"":{}}"),
                    StatusCode = HttpStatusCode.OK
                })
                .Verifiable();
            var client = new TwitchClient(_logger.Object, httpClientMock.Object, _optionsMock.Object);

            var response = await client.GetVideosByUserIdAsync(1);
            var list = response.Data.ToList();
            
            Assert.Empty(list);
        }
        
        [Fact]
        public async Task GetUsersAsync_ExistsByLoginName_ListWithUser() {
            var messageHandlerMock = new Mock<HttpMessageHandler>();
            var httpClientMock = new Mock<HttpClient>(messageHandlerMock.Object);
            const string jsonContent = "{\"data\":[{\"id\":\"141981764\",\"login\":\"twitchdev\",\"display_name\":\"TwitchDev\",\"type\":\"\",\"broadcaster_type\":\"partner\",\"description\":\"Supporting third-party developers building Twitch integrations from chatbots to game integrations.\",\"profile_image_url\":\"https://static-cdn.jtvnw.net/jtv_user_pictures/8a6381c7-d0c0-4576-b179-38bd5ce1d6af-profile_image-300x300.png\",\"offline_image_url\":\"https://static-cdn.jtvnw.net/jtv_user_pictures/3f13ab61-ec78-4fe6-8481-8682cb3b0ac2-channel_offline_image-1920x1080.png\",\"view_count\":5980557,\"email\":\"not-real@email.com\",\"created_at\":\"2016-12-14T20:32:28.894263Z\"}]}";
            messageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage {
                    Content = new StringContent(jsonContent),
                    StatusCode = HttpStatusCode.OK
                })
                .Verifiable();
            var client = new TwitchClient(_logger.Object, httpClientMock.Object, _optionsMock.Object);

            var response = await client.GetUsersAsync(new [] { "twitchdev" });
            var list = response.Data.ToList();
            
            Assert.Single(list);
        }
        
        [Fact]
        public async Task GetUsersAsync_TooManyLoginNames_Throws() {
            var httpClientMock = new Mock<HttpClient>();
            var client = new TwitchClient(_logger.Object, httpClientMock.Object, _optionsMock.Object);
            var loginNames = new List<string>();
            for (var i = 0; i < 200; i++) {
                loginNames.Add("login_name");
            }

            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await client.GetUsersAsync(loginNames));
        }
        
        [Fact]
        public async Task GetUsersAsync_ErrorOnApi_Throws() {
            var messageHandlerMock = new Mock<HttpMessageHandler>();
            var httpClientMock = new Mock<HttpClient>(messageHandlerMock.Object);
            messageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage {
                    StatusCode = HttpStatusCode.BadRequest
                })
                .Verifiable();
            var client = new TwitchClient(_logger.Object, httpClientMock.Object, _optionsMock.Object);

            await Assert.ThrowsAsync<Exception>(async () => await client.GetUsersAsync(new[] {"twitchdev"}));
        }
    }
}