using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Denhub.API.Controllers;
using Denhub.API.Models;
using Denhub.API.Results;
using Denhub.API.Services;
using Denhub.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Denhub.API.Tests.Controllers {
    public class LogsControllerTests {
        [Fact]
        public async Task GetByChannelIdAsync_DefaultValues_Status200Ok() {
            var serviceMock = new Mock<ILogsService>();
            serviceMock.Setup(m =>
                    m.GetByChannelAsync(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(Result.Ok(new TwitchChatMessagesResult {
                    ChatMessages = new List<TwitchChatMessagePublic> {
                        new(),
                        new(),
                        new()
                    },
                    PaginationCursor = "123"
                }));
            var controller = new LogsController(serviceMock.Object);

            var response = await controller.GetByChannelIdAsync(123, new DateTime(), null);
            var result = Assert.IsType<OkObjectResult>(response.Result);
            var responseValue = Assert.IsAssignableFrom<ValueResult<TwitchChatMessagesResult>>(result.Value);
            
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(3, responseValue.Value.ChatMessages.Count());
        }
        
        [Fact]
        public async Task GetByChannelIdAsync_PaginationCursor_Status200Ok() {
            var serviceMock = new Mock<ILogsService>();
            serviceMock.Setup(m =>
                    m.GetByChannelAsync(It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(Result.Ok(new TwitchChatMessagesResult {
                    ChatMessages = new List<TwitchChatMessagePublic> {
                        new(),
                        new(),
                        new()
                    },
                    PaginationCursor = "123"
                }));
            var controller = new LogsController(serviceMock.Object);

            var response = await controller.GetByChannelIdAsync(123, new DateTime(), "123");
            var result = Assert.IsType<OkObjectResult>(response.Result);
            var responseValue = Assert.IsAssignableFrom<ValueResult<TwitchChatMessagesResult>>(result.Value);
            
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(3, responseValue.Value.ChatMessages.Count());
        }
        
        [Fact]
        public async Task GetByChannelNameAsync_ValidName_Status200Ok() {
            var serviceMock = new Mock<ILogsService>();
            serviceMock.Setup(m =>
                    m.GetByChannelAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(Result.Ok(new TwitchChatMessagesResult {
                    ChatMessages = new List<TwitchChatMessagePublic> {
                        new(),
                        new(),
                        new()
                    },
                    PaginationCursor = "123"
                }));
            var controller = new LogsController(serviceMock.Object);

            var response = await controller.GetByChannelNameAsync("test", new DateTime(), "123");
            var result = Assert.IsType<OkObjectResult>(response.Result);
            var responseValue = Assert.IsAssignableFrom<ValueResult<TwitchChatMessagesResult>>(result.Value);
            
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            Assert.Equal(3, responseValue.Value.ChatMessages.Count());
        }
        
        [Fact]
        public async Task GetByChannelNameAsync_InvalidName_Status404NotFound() {
            var serviceMock = new Mock<ILogsService>();
            serviceMock.Setup(m =>
                    m.GetByChannelAsync(It.IsAny<string>(), It.IsAny<DateTime>(), It.IsAny<string>(), It.IsAny<int>()))
                .ReturnsAsync(Result.NotFound<TwitchChatMessagesResult>(default));
            var controller = new LogsController(serviceMock.Object);

            var response = await controller.GetByChannelNameAsync("test", new DateTime(), "123");
            var result = Assert.IsType<NotFoundObjectResult>(response.Result);

            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }
    }
}