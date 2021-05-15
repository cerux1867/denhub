using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Denhub.API.Models;
using Denhub.API.Repositories;
using Denhub.API.Services;
using Denhub.Common.Models;
using Moq;
using Xunit;

namespace Denhub.API.Tests.Services {
    public class LogsServiceTests {
        [Fact]
        public async Task GetByChannelIdAsync_InitialRequest_ListOfChatMessagesWithPagination() {
            var chatLogsRepoMock = new Mock<IChatLogsRepository>();
            chatLogsRepoMock.Setup(m =>
                    m.GetByChannelIdAsync(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>()))
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
            var service = new LogsService(chatLogsRepoMock.Object);

            var result = await service.GetByChannelIdAsync(123, DateTime.Now);
            
            Assert.Equal(2, result.ChatMessages.Count());
            Assert.Equal("bla-blabla-test", result.PaginationCursor);
        }
    }
}