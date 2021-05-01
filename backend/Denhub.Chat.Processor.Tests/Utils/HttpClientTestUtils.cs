using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace Denhub.Chat.Processor.Tests.Utils {
    public static class HttpClientTestUtils {
        public static Mock<HttpMessageHandler> ConstructMockHttpMessageHandler(HttpStatusCode statusCode, StringContent content) {
            var mockHandler = new Mock<HttpMessageHandler>();
            mockHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage {
                    StatusCode = statusCode,
                    Content = content
                });
            return mockHandler;
        }

        public static Mock<HttpMessageHandler> ConstructSequencedMockHttpMessageHandler(
            List<HttpStatusCode> statusCodes, List<StringContent> contents) {
            var mockHandler = new Mock<HttpMessageHandler>();
            var setup = mockHandler
                .Protected()
                .SetupSequence<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>());
            for (var i = 0; i < statusCodes.Count; i++) {
                setup.ReturnsAsync(new HttpResponseMessage {
                    Content = contents[i],
                    StatusCode = statusCodes[i]
                });
            }

            return mockHandler;
        }
    }
}