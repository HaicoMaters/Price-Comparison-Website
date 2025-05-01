using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using PriceComparisonWebsite.Services.Utilities;

namespace PriceComparisonWebsite.Tests.Services.Utilities
{
    public class RetryHandlerTests
    {
        private readonly RetryHandler _retryHandler;
        private readonly Mock<ILogger<RetryHandler>> _loggerMock;

        public RetryHandlerTests()
        {
            _loggerMock = new Mock<ILogger<RetryHandler>>();
            _retryHandler = new RetryHandler(_loggerMock.Object);
        }

        // -------------------------------------------- ShouldRetry ----------------------------------------------
        
        [Theory]
        [InlineData(HttpStatusCode.TooManyRequests, true)]
        [InlineData(HttpStatusCode.ServiceUnavailable, true)]
        [InlineData(HttpStatusCode.GatewayTimeout, true)]
        [InlineData(HttpStatusCode.RequestTimeout, true)]
        [InlineData(HttpStatusCode.NotFound, false)]
        [InlineData(HttpStatusCode.BadRequest, false)]
        [InlineData(HttpStatusCode.InternalServerError, false)]
        public void ShouldRetry_WithDifferentStatusCodes_ReturnsExpectedResult(HttpStatusCode statusCode, bool expected)
        {
            // Arrange
            var ex = new HttpRequestException("Test", null, statusCode);
            
            // Act
            var result = _retryHandler.ShouldRetry(ex, 1, 3);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ShouldRetry_WhenAttemptEqualsMaxRetries_ReturnsFalse()
        {
            // Arrange
            var ex = new HttpRequestException("Test", null, HttpStatusCode.TooManyRequests);
            
            // Act
            var result = _retryHandler.ShouldRetry(ex, 3, 3);

            // Assert
            Assert.False(result);
        }

        // -------------------------------------------- ExecuteWithRetryAsync ------------------------------------

        [Fact]
        public async Task ExecuteWithRetryAsync_WhenSuccessful_ShouldNotRetry()
        {
            // Arrange
            int attempts = 0;
            var operation = new Func<Task<string>>(() => {
                attempts++;
                return Task.FromResult("Success");
            });

            // Act
            var result = await _retryHandler.ExecuteWithRetryAsync(operation, "Test operation");

            // Assert
            Assert.Equal(1, attempts);
            Assert.Equal("Success", result);
        }

        [Fact]
        public async Task ExecuteWithRetryAsync_WhenFailsWithRetryableError_ShouldRetryAndEventuallySucceed()
        {
            // Arrange
            int attempts = 0;
            var operation = new Func<Task<string>>(() => {
                attempts++;
                if (attempts < 3)
                {
                    throw new HttpRequestException("Test", null, HttpStatusCode.TooManyRequests);
                }
                return Task.FromResult("Success");
            });

            // Act
            var result = await _retryHandler.ExecuteWithRetryAsync(operation, "Test operation");

            // Assert
            Assert.Equal(3, attempts);
            Assert.Equal("Success", result);
             _loggerMock.Verify(x => x.Log( LogLevel.Warning,It.IsAny<EventId>(),It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(), It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                    Times.Exactly(2));
        }

        [Fact]
        public async Task ExecuteWithRetryAsync_WhenFailsWithNonRetryableError_ShouldNotRetry()
        {
            // Arrange
            int attempts = 0;
            var operation = new Func<Task<string>>(() => {
                attempts++;
                throw new HttpRequestException("Test", null, HttpStatusCode.NotFound);
            });

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => 
                _retryHandler.ExecuteWithRetryAsync(operation, "Test operation"));
            Assert.Equal(1, attempts);
        }

        [Fact]
        public async Task ExecuteWithRetryAsync_WhenExceedsMaxRetries_ShouldThrowException()
        {
            // Arrange
            int attempts = 0;
            var operation = new Func<Task<string>>(() => {
                attempts++;
                throw new HttpRequestException("Test", null, HttpStatusCode.TooManyRequests);
            });

            // Act & Assert
            var ex = await Assert.ThrowsAsync<HttpRequestException>(() => 
                _retryHandler.ExecuteWithRetryAsync(operation, "Test operation", 3));
            Assert.Equal(4, attempts); // Initial attempt + 3 retries
            Assert.Equal("Test", ex.Message);
        }

        [Fact]
        public async Task ExecuteWithRetryAsync_WithVoidOperation_ShouldHandleRetriesCorrectly()
        {
            // Arrange
            int attempts = 0;
            var operation = new Func<Task>(() => {
                attempts++;
                if (attempts < 3)
                {
                    throw new HttpRequestException("Test", null, HttpStatusCode.TooManyRequests);
                }
                return Task.CompletedTask;
            });

            // Act
            await _retryHandler.ExecuteWithRetryAsync(operation, "Test operation");

            // Assert
            Assert.Equal(3, attempts);
            _loggerMock.Verify(x => x.Log( LogLevel.Warning,It.IsAny<EventId>(),It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(), It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                    Times.Exactly(2));
        }
    }
}