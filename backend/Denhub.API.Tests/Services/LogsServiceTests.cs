using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Denhub.API.Models.Twitch;
using Denhub.API.Repositories;
using Denhub.API.Results;
using Denhub.API.Services;
using Denhub.Common.Models;
using Moq;
using Xunit;

namespace Denhub.API.Tests.Services {
    public class LogsServiceTests {
        [Fact]
        public async Task GetByChannelAsync_UsingIdInitialRequest_SuccessResultWithListOfChatMessagesWithPagination() {
            var chatLogsRepoMock = new Mock<IChatLogsRepository>();
            var twitchClientMock = new Mock<ITwitchClient>();
            chatLogsRepoMock.Setup(m =>
                    m.GetByChannelIdAsync(It.IsAny<long>(), It.IsAny<bool>(),It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync((
                    "bla-blabla-test",
                    new List<TwitchChatMessageBackend> {
                        new() {
                            MessageId = Guid.NewGuid().ToString()
                        },
                        new() {
                            MessageId = Guid.NewGuid().ToString()
                        }
                    }
                ));
            var service = new LogsService(chatLogsRepoMock.Object, twitchClientMock.Object);

            var result = await service.GetByChannelAsync(123, DateTime.Now, "asc");
            
            Assert.Equal(ResultType.Ok, result.Type);
            Assert.Equal(2, result.Value.ChatMessages.Count());
            Assert.Equal("bla-blabla-test", result.Value.PaginationCursor);
        }
        
        [Fact]
        public async Task GetByChannelAsync_UsingNameInitialRequest_SuccessResultWithListOfChatMessagesWithPagination() {
            var chatLogsRepoMock = new Mock<IChatLogsRepository>();
            var twitchClientMock = new Mock<ITwitchClient>();
            twitchClientMock.Setup(m => m.GetUsersAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(new TwitchResponseModel<IEnumerable<TwitchUserItem>> {
                    Data = new List<TwitchUserItem> {
                        new() {
                            Id = "123"
                        }
                    }
                });
            chatLogsRepoMock.Setup(m =>
                    m.GetByChannelIdAsync(It.IsAny<long>(), It.IsAny<bool>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync((
                    "bla-blabla-test",
                    new List<TwitchChatMessageBackend> {
                        new() {
                            MessageId = Guid.NewGuid().ToString()
                        },
                        new() {
                            MessageId = Guid.NewGuid().ToString()
                        }
                    }
                ));
            var service = new LogsService(chatLogsRepoMock.Object, twitchClientMock.Object);

            var result = await service.GetByChannelAsync("test", DateTime.Now, "asc");
            
            Assert.Equal(ResultType.Ok, result.Type);
            Assert.Equal(2, result.Value.ChatMessages.Count());
            Assert.Equal("bla-blabla-test", result.Value.PaginationCursor);
        }
        
        [Fact]
        public async Task GetByChannelAsync_UsingAmbiguousNameInitialRequest_NotFoundResult() {
            var chatLogsRepoMock = new Mock<IChatLogsRepository>();
            var twitchClientMock = new Mock<ITwitchClient>();
            twitchClientMock.Setup(m => m.GetUsersAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(new TwitchResponseModel<IEnumerable<TwitchUserItem>> {
                    Data = new List<TwitchUserItem> {
                        new() {
                            Id = "123"
                        },
                        new() {
                            Id = "123"
                        }
                    }
                });
            chatLogsRepoMock.Setup(m =>
                    m.GetByChannelIdAsync(It.IsAny<long>(), It.IsAny<bool>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync((
                    "bla-blabla-test",
                    new List<TwitchChatMessageBackend> {
                        new() {
                            MessageId = Guid.NewGuid().ToString()
                        },
                        new() {
                            MessageId = Guid.NewGuid().ToString()
                        }
                    }
                ));
            var service = new LogsService(chatLogsRepoMock.Object, twitchClientMock.Object);

            var result = await service.GetByChannelAsync("test", DateTime.Now, "asc");
            
            Assert.Equal(ResultType.NotFound, result.Type);
        }
        
        [Fact]
        public async Task GetByChannelAsync_NonExistingChannelNameNameInitialRequest_NotFoundResult() {
            var chatLogsRepoMock = new Mock<IChatLogsRepository>();
            var twitchClientMock = new Mock<ITwitchClient>();
            twitchClientMock.Setup(m => m.GetUsersAsync(It.IsAny<IEnumerable<string>>()))
                .ReturnsAsync(new TwitchResponseModel<IEnumerable<TwitchUserItem>> {
                    Data = new List<TwitchUserItem>()
                });
            chatLogsRepoMock.Setup(m =>
                    m.GetByChannelIdAsync(It.IsAny<long>(), It.IsAny<bool>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync((
                    "bla-blabla-test",
                    new List<TwitchChatMessageBackend> {
                        new() {
                            MessageId = Guid.NewGuid().ToString()
                        },
                        new() {
                            MessageId = Guid.NewGuid().ToString()
                        }
                    }
                ));
            var service = new LogsService(chatLogsRepoMock.Object, twitchClientMock.Object);

            var result = await service.GetByChannelAsync("test", DateTime.Now, "asc");
            
            Assert.Equal(ResultType.NotFound, result.Type);
        }
    }
}