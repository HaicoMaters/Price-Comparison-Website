using System.Net;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using PriceComparisonWebsite.Services.HttpClients;
using Xunit;

namespace PriceComparisonWebsite.Tests.Services.HttpClients
{
    public class ApiHttpClientTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;

        public ApiHttpClientTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _httpClientFactoryMock = new Mock<IHttpClientFactory>();

            _configurationMock.Setup(x => x["LocalhostUrl"]).Returns("http://localhost:5000");
            _configurationMock.Setup(x => x["InternalApi:Key"]).Returns("test-key");
        }
        
        // --------------------------------------------- Constructor ---------------------------------------------

        [Fact]
        public void Constructor_ShouldConfigureClientCorrectly()
        {
            // Arrange
            var httpClient = new HttpClient();
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            // Act
            var apiClient = new ApiHttpClient(_httpClientFactoryMock.Object, _configurationMock.Object);

            // Assert
            Assert.Equal(new Uri("http://localhost:5000"), httpClient.BaseAddress);
            Assert.Contains(new MediaTypeWithQualityHeaderValue("application/json"), httpClient.DefaultRequestHeaders.Accept);
            Assert.Contains("true", httpClient.DefaultRequestHeaders.GetValues("X-Internal-Client"));
            Assert.Contains("test-key", httpClient.DefaultRequestHeaders.GetValues("X-Internal-Auth"));
        }

        // --------------------------------------------- SendAsync ---------------------------------------------

        [Fact]
        public async Task SendAsync_WithContent_ShouldSerializeAndSendContent()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(handlerMock.Object);
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var content = new { test = "value" };
            var endpoint = "api/test";

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => 
                        req.Method == HttpMethod.Post && 
                        req.RequestUri.ToString().EndsWith(endpoint)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var apiClient = new ApiHttpClient(_httpClientFactoryMock.Object, _configurationMock.Object);

            // Act
            var result = await apiClient.SendAsync(HttpMethod.Post, endpoint, content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Content != null),
                ItExpr.IsAny<CancellationToken>()
            );
        }

        [Fact]
        public async Task SendAsync_WithoutContent_ShouldSendRequestWithoutBody()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(handlerMock.Object);
            _httpClientFactoryMock.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);

            var endpoint = "api/test";

            handlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req => 
                        req.Method == HttpMethod.Get && 
                        req.RequestUri.ToString().EndsWith(endpoint)),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK));

            var apiClient = new ApiHttpClient(_httpClientFactoryMock.Object, _configurationMock.Object);

            // Act
            var result = await apiClient.SendAsync(HttpMethod.Get, endpoint);

            // Assert
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            handlerMock.Protected().Verify(
                "SendAsync",
                Times.Once(),
                ItExpr.Is<HttpRequestMessage>(req => req.Content == null),
                ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}
