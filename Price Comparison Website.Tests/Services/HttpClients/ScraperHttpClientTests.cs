using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Moq.Protected;
using Price_Comparison_Website.Services.HttpClients;

namespace Price_Comparison_Website.Tests.Services.HttpClients
{
    public class ScraperHttpClientTests
    {

        private ScraperHttpClient CreateScraperHttpClient(HttpResponseMessage response)
        {
            var handlerMock = new Mock<HttpMessageHandler>();

            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(response);

            var httpClient = new HttpClient(handlerMock.Object);

            return new ScraperHttpClient(httpClient);
        }

        [Fact]
        public async Task SendRequestAsync_WhenSucceeds_ShouldReturnResponse()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Success response")
            };

            var client = CreateScraperHttpClient(response);

            var uri = new Uri("https://example.com");

            // Act
            var result = await client.SendRequestAsync(uri, HttpMethod.Get);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            var content = await result.Content.ReadAsStringAsync();
            Assert.Equal("Success response", content);
        }

        [Fact]
        public async Task SendRequestAsync_WhenFailure_ShouldThrowException()
        {
            // Arrange
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Bad request")
            };

            var client = CreateScraperHttpClient(response);

            var uri = new Uri("https://example.com");

            // Act & Assert
            var exception = await Assert.ThrowsAsync<HttpRequestException>(async () =>
            {
                await client.SendRequestAsync(uri, HttpMethod.Get);
            });

            Assert.Contains("Response status code does not indicate success", exception.Message);
        }
    }
}
