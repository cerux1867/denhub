using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Denhub.API.Controllers;
using Denhub.API.Models;
using Denhub.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Denhub.API.Tests.Controllers {
    public class VodsControllersTests {
        [Fact]
        public async Task GetAllByChannelNameAsync_ChannelExistsVodsExist_Status200Ok() {
            var vodsServiceMock = new Mock<IVodsService>();
            vodsServiceMock.Setup(m => m.GetChannelIdByChannelNameAsync(It.IsAny<string>())).ReturnsAsync(1);
            vodsServiceMock.Setup(m => m.GetAllByIdAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new List<CommonVodModel> {
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
            var controller = new VodsController(vodsServiceMock.Object);

            var result = Assert.IsType<OkObjectResult>(await controller.GetAllByChannelNameAsync("test", new VodsParameters()));
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);
            var value = Assert.IsAssignableFrom<IEnumerable<CommonVodModel>>(result.Value);
            Assert.Equal(3, value.Count());
        }
        
        [Fact]
        public async Task GetAllByChannelNameAsync_ChannelDoesNotExist_Status404NotFound() {
            var vodsServiceMock = new Mock<IVodsService>();
            vodsServiceMock.Setup(m => m.GetChannelIdByChannelNameAsync(It.IsAny<string>())).ReturnsAsync(-1);
            var controller = new VodsController(vodsServiceMock.Object);

            var result = Assert.IsType<NotFoundResult>(await controller.GetAllByChannelNameAsync("test", new VodsParameters()));
            Assert.Equal(StatusCodes.Status404NotFound, result.StatusCode);
        }
    }
}