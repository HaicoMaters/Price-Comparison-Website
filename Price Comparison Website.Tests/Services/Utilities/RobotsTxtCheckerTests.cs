using Moq;
using Moq.Protected;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Price_Comparison_Website.Services.Utilities;
using Price_Comparison_Website.Services.Utilities.Interfaces;

namespace Price_Comparison_Website.Tests.Services.Utilities
{
    public class RobotsTxtCheckerTests : IDisposable
    {
        private readonly Mock<ILogger<RobotsTxtChecker>> _mockLogger;
        private readonly Mock<IFileSystemWrapper> _mockFileSystem;
        private readonly Mock<HttpMessageHandler> _mockHandler;
        private readonly HttpClient _httpClient;
        private readonly RobotsTxtChecker _robotsChecker;
        private const string CacheFolder = "RobotsCache";

        public RobotsTxtCheckerTests()
        {
            _mockLogger = new Mock<ILogger<RobotsTxtChecker>>();
            _mockFileSystem = new Mock<IFileSystemWrapper>();
            _mockHandler = new Mock<HttpMessageHandler>();

            _httpClient = new HttpClient(_mockHandler.Object);

            _robotsChecker = new RobotsTxtChecker(
                _httpClient,
                _mockLogger.Object,
                _mockFileSystem.Object,
                cacheFolder: CacheFolder
            );
        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }

        // --------------------------------------------------- CacheRobotsTxtFile ---------------------------------------------------------------------------------------------

        [Fact]
        public async Task CacheRobotsTxtFile_WhenFileDoesNotExist_ShouldSaveToCacheFolder()
        {
            // Arrange
            var testUrl = new Uri("https://example.com");
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("User-agent: *\nDisallow: /private/\n")
            };

            _mockHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            ).ReturnsAsync(responseMessage);

            string expectedFilePath = Path.Combine(CacheFolder, "example.com_robots.txt");

            _mockFileSystem.Setup(fs => fs.FileExists(expectedFilePath)).Returns(false);

            // Act
            await _robotsChecker.CacheRobotsTxtFile(testUrl);

            // Assert
            _mockFileSystem.Verify(fs => fs.WriteAllTextAsync(expectedFilePath, It.IsAny<string>()), Times.Once);

            _mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Caching new robots.txt")),
                It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once
            );

            _mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Overwriting existing robots.txt")),
                It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Never
            );
        }


        [Fact]
        public async Task CacheRobotsTxtFile_WhenFileExists_ShouldOverwriteFile()
        {
            // Arrange
            var testUrl = new Uri("https://example.com");
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("User-agent: *\nDisallow: /private/\n")
            };

            _mockHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            ).ReturnsAsync(responseMessage);

            string expectedFilePath = Path.Combine(CacheFolder, "example.com_robots.txt");

            _mockFileSystem.Setup(fs => fs.FileExists(expectedFilePath)).Returns(true);

            // Act
            await _robotsChecker.CacheRobotsTxtFile(testUrl);

            // Assert
            _mockFileSystem.Verify(fs => fs.WriteAllTextAsync(expectedFilePath, It.IsAny<string>()), Times.Once);

            _mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Overwriting existing robots.txt")),
                It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once
            );

            _mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Caching new robots.txt")),
                It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Never
            );
        }

        [Fact]
        public async Task CacheRobotsTxtFile_WhenInvalidResponse_ShouldNotCacheFile()
        {
            // Arrange
            var testUrl = new Uri("https://example.com");

            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("NotFound")
            };

            _mockHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            ).ReturnsAsync(responseMessage);

            string expectedFilePath = Path.Combine(CacheFolder, "example.com_robots.txt");

            _mockFileSystem.Setup(fs => fs.FileExists(expectedFilePath)).Returns(false);

            // Act
            await _robotsChecker.CacheRobotsTxtFile(testUrl);

            // Assert
            _mockFileSystem.Verify(fs => fs.WriteAllTextAsync(expectedFilePath, It.IsAny<string>()), Times.Never);

            _mockLogger.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Failed to download robots.txt")),
                It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once
            );
        }

        [Fact]
        public async Task CacheRobotsTxtFile_WhenExceptionOccurs_ShouldLogError()
        {
            // Arrange
            var testUrl = new Uri("https://example.com");

            _mockHandler.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            ).ThrowsAsync(new HttpRequestException("Network error"));

            string expectedFilePath = Path.Combine(CacheFolder, "example.com_robots.txt");

            _mockFileSystem.Setup(fs => fs.FileExists(expectedFilePath)).Returns(false);

            // Act
            await _robotsChecker.CacheRobotsTxtFile(testUrl);

            // Assert
            _mockFileSystem.Verify(fs => fs.WriteAllTextAsync(expectedFilePath, It.IsAny<string>()), Times.Never);

            _mockLogger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error caching robots.txt")),
                It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once
            );
        }

        // --------------------------------------------------- CheckIfRobotsTxtFileIsCached --------------------------------------------------------------------------------

        [Fact]
        public void CheckIfRobotsTxtFileIsCached_WhenValidCache_ShouldReturnTrue()
        {
            // Arrange
            var domain = "example.com";
            var filePath = Path.Combine(CacheFolder, $"{domain}_robots.txt");

            _mockFileSystem.Setup(fs => fs.FileExists(filePath)).Returns(true);
            _mockFileSystem.Setup(fs => fs.GetLastWriteTime(filePath)).Returns(DateTime.Now.AddHours(-1));

            // Act
            var result = _robotsChecker.CheckIfRobotsTxtFileIsCached(domain);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void CheckIfRobotsTxtFileIsCached_WhenExpiredCache_ShouldReturnFalse()
        {
            // Arrange
            var domain = "example.com";
            var filePath = Path.Combine(CacheFolder, $"{domain}_robots.txt");

            _mockFileSystem.Setup(fs => fs.FileExists(filePath)).Returns(true);
            _mockFileSystem.Setup(fs => fs.GetLastWriteTime(filePath)).Returns(DateTime.Now.AddHours(-26));

            // Act
            var result = _robotsChecker.CheckIfRobotsTxtFileIsCached(domain);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CheckIfRobotsTxtFileIsCached_WhenNoCachedFile_ShouldReturnFalse()
        {
            // Arrange
            var domain = "example.com";
            var filePath = Path.Combine(CacheFolder, $"{domain}_robots.txt");

            _mockFileSystem.Setup(fs => fs.FileExists(filePath)).Returns(false);

            // Act
            var result = _robotsChecker.CheckIfRobotsTxtFileIsCached(domain);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void CheckIfRobotsTxtFileIsCached_WhenDomainIsNullOrEmpty_ShouldReturnFalse()
        {
            // Act
            var resultWithNull = _robotsChecker.CheckIfRobotsTxtFileIsCached(null);
            var resultWithEmpty = _robotsChecker.CheckIfRobotsTxtFileIsCached("");

            // Assert
            Assert.False(resultWithNull);
            Assert.False(resultWithEmpty);

            _mockLogger.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Domain is null or empty.")),
                   It.IsAny<Exception>(), It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)), Times.Exactly(2)
           );
        }

        [Fact]
        public void CheckIfRobotsTxtFileIsCached_WhenExceptionOccurs_ShouldReturnFalse()
        {
            // Arrange
            var domain = "example.com";
            var filePath = Path.Combine("RobotsCache", $"{domain}_robots.txt");

            _mockFileSystem.Setup(fs => fs.FileExists(filePath)).Returns(true);

            _mockFileSystem.Setup(fs => fs.GetLastWriteTime(filePath))
                .Throws(new IOException("File exception"));

            // Act
            var result = _robotsChecker.CheckIfRobotsTxtFileIsCached(domain);

            // Assert
            Assert.False(result);

            _mockLogger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error checking cache for robots.txt")),
                It.IsAny<IOException>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once
            );
        }



        // --------------------------------------------------- CheckRobotsTxt -------------------------------------------------------------------------------------------------

        [Fact]
        public async Task CheckRobotsTxt_WhenAllowedPath_ShouldReturnTrue()
        {
            // Arrange
            var url = new Uri("https://example.com/allowed-path");
            string domain = "example.com";
            string robotsContent = "User-agent: *\nAllow: /allowed-path\nDisallow: /private/";
            var filePath = Path.Combine(CacheFolder, $"{domain}_robots.txt");

            _mockFileSystem.Setup(fs => fs.FileExists(filePath)).Returns(true);
            _mockFileSystem.Setup(fs => fs.ReadAllTextAsync(filePath)).ReturnsAsync(robotsContent);
            _mockFileSystem.Setup(fs => fs.GetLastWriteTime(filePath)).Returns(DateTime.Now.AddHours(-1)); // CheckIfRobotsTxtFileIsCached() should return true

            // Act
            var result = await _robotsChecker.CheckRobotsTxt(url);

            // Assert
            Assert.True(result);

            _mockFileSystem.Verify(fs => fs.FileExists(filePath), Times.Exactly(2));
            _mockFileSystem.Verify(fs => fs.GetLastWriteTime(filePath), Times.Once);
            _mockFileSystem.Verify(fs => fs.ReadAllTextAsync(filePath), Times.Once);

            _mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(" => Allowed")),
                It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once
            );
        }

        [Fact]
        public async Task CheckRobotsTxt_WhenDisallowedPath_ShouldReturnFalse()
        {
            // Arrange
            var url = new Uri("https://example.com/private/Test");
            string domain = "example.com";
            string robotsContent = "User-agent: *\nAllow: /allowed-path\nDisallow: /private/";
            var filePath = Path.Combine(CacheFolder, $"{domain}_robots.txt");

            _mockFileSystem.Setup(fs => fs.FileExists(filePath)).Returns(true);
            _mockFileSystem.Setup(fs => fs.ReadAllTextAsync(filePath)).ReturnsAsync(robotsContent);
            _mockFileSystem.Setup(fs => fs.GetLastWriteTime(filePath)).Returns(DateTime.Now.AddHours(-1)); // CheckIfRobotsTxtFileIsCached() should return true

            // Act
            var result = await _robotsChecker.CheckRobotsTxt(url);

            // Assert
            Assert.False(result);

            _mockFileSystem.Verify(fs => fs.FileExists(filePath), Times.Exactly(2));
            _mockFileSystem.Verify(fs => fs.GetLastWriteTime(filePath), Times.Once);
            _mockFileSystem.Verify(fs => fs.ReadAllTextAsync(filePath), Times.Once);

            _mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(" => Disallowed")),
                It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once
            );
        }


        [Fact]
        public async Task CheckRobotsTxt_WhenNoRobotsTxtAfterTriedAdding_ShouldReturnFalse()
        {
            // Arrange
            var url = new Uri("https://example.com/any-path");
            string domain = "example.com";
            var filePath = Path.Combine(CacheFolder, $"{domain}_robots.txt");

            _mockFileSystem.Setup(fs => fs.FileExists(filePath)).Returns(false);

            // Simulate a invalid robots.txt download
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("User-agent: *\nDisallow: /private/")
            };

            _mockHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",ItExpr.IsAny<HttpRequestMessage>(),ItExpr.IsAny<CancellationToken>()
                ).ReturnsAsync(responseMessage);

            // Act
            var result = await _robotsChecker.CheckRobotsTxt(url);

            // Assert
            Assert.False(result);

            _mockFileSystem.Verify(fs => fs.FileExists(filePath), Times.Exactly(2));
        }

        [Fact]
        public async Task CheckRobotsTxt_WhenMalformedRobotsTxt_ShouldReturnTrue()
        {
            // Arrange
            var url = new Uri("https://example.com/private/");
            string domain = "example.com";
            string robotsContent = "User-agent: *\nAllow: /allowed-path\nDisallow /private/"; // Missing colon after Disallow
            var filePath = Path.Combine(CacheFolder, $"{domain}_robots.txt");

            _mockFileSystem.Setup(fs => fs.FileExists(filePath)).Returns(true);
            _mockFileSystem.Setup(fs => fs.ReadAllTextAsync(filePath)).ReturnsAsync(robotsContent);
            _mockFileSystem.Setup(fs => fs.GetLastWriteTime(filePath)).Returns(DateTime.Now.AddHours(-1)); // CheckIfRobotsTxtFileIsCached() should return true

            // Act
            var result = await _robotsChecker.CheckRobotsTxt(url);

            // Assert
            Assert.True(result);

            _mockFileSystem.Verify(fs => fs.FileExists(filePath), Times.Exactly(2));
            _mockFileSystem.Verify(fs => fs.GetLastWriteTime(filePath), Times.Once);
            _mockFileSystem.Verify(fs => fs.ReadAllTextAsync(filePath), Times.Once);

            _mockLogger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(" => Allowed")),
                It.IsAny<Exception>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once
            );
        }

        [Fact]
        public async Task CheckRobotsTxt_WhenExceptionOccurs_ShouldReturnFalse()
        {
            // Arrange
            var url = new Uri("https://example.com/error-path");
            string domain = "example.com";
            var filePath = Path.Combine(CacheFolder, $"{domain}_robots.txt");

            _mockFileSystem.Setup(fs => fs.FileExists(filePath)).Throws(new IOException("File error"));

            // Simulate a invalid robots.txt download
            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.NotFound,
                Content = new StringContent("User-agent: *\nDisallow: /private/")
            };

            _mockHandler.Protected().Setup<Task<HttpResponseMessage>>("SendAsync",ItExpr.IsAny<HttpRequestMessage>(),ItExpr.IsAny<CancellationToken>()
                ).ReturnsAsync(responseMessage);

            // Act
            var result = await _robotsChecker.CheckRobotsTxt(url);

            // Assert
            Assert.False(result);

            _mockFileSystem.Verify(fs => fs.FileExists(filePath), Times.Exactly(2));

            _mockLogger.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error checking robots.txt")),
                It.IsAny<IOException>(), It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Once
            );
        }
    }
}

