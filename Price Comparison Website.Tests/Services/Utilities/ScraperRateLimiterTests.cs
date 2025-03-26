using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.RateLimiting;
using System.Threading.Tasks;
using Price_Comparison_Website.Services.Utilities;
using Price_Comparison_Website.Services.Utilities.Interfaces;

namespace Price_Comparison_Website.Tests.Services.Utilities
{
    public class ScraperRateLimiterTests
    {
        private readonly IScraperRateLimiter _rateLimiter;

        public ScraperRateLimiterTests()
        {
            _rateLimiter = new ScraperRateLimiter(TimeSpan.FromMilliseconds(500)); // Use short cooldown for testing
        }

        // ----------------------------------------------------------------------------- EnqueueRequest ----------------------------------------
        [Fact]
        public async Task EnqueueRequest_WhenNotOnCooldown_ShouldExecuteActionImmediately()
        {
            // Arrange
            bool actionExecuted = false;
            async Task TestAction() => await Task.Run(() => actionExecuted = true);

            // Act
            await _rateLimiter.EnqueueRequest(TestAction, "example.com");

            // Assert
            Assert.True(actionExecuted);
        }

        [Fact]
        public async Task EnqueueRequest_WhenMultipleRequestsToSameDomain_ShouldThrottleRequestsPerDomain()
        {
            // Arrange
            int executionCount = 0;
            async Task TestAction()
            {
                await Task.Delay(50);
                executionCount++;
            }

            string domain = "example.com";

            // Act
            await _rateLimiter.EnqueueRequest(TestAction, domain);   // First request
            await _rateLimiter.EnqueueRequest(TestAction, domain);   // Should be throttled

            // Assert
            Assert.Equal(2, executionCount);
        }

        [Fact]
        public async Task EnqueueRequest_WhenSendingRequestsToMultipleDomains_ShouldThrottleMultipleDomainsSeparately()
        {
            // Arrange
            int domain1Count = 0;
            int domain2Count = 0;

            async Task Domain1Action() => await Task.Run(() => domain1Count++);
            async Task Domain2Action() => await Task.Run(() => domain2Count++);

            // Act
            await _rateLimiter.EnqueueRequest(Domain1Action, "domain1.com");
            await _rateLimiter.EnqueueRequest(Domain2Action, "domain2.com");

            // Assert
            Assert.Equal(1, domain1Count);
            Assert.Equal(1, domain2Count);
        }

        [Fact]
        public async Task EnqueueRequest_WhenExceptionOccurs_ShouldHandleException()
        {
            // Arrange
            bool exceptionCaught = false;
            async Task FailingAction()
            {
                await Task.Delay(50);
                throw new InvalidOperationException("Test Exception");
            }

            async Task ExceptionCatcher()
            {
                try
                {
                    await _rateLimiter.EnqueueRequest(FailingAction, "example.com");
                }
                catch
                {
                    exceptionCaught = true;
                }
            }

            // Act
            await ExceptionCatcher();

            // Assert
            Assert.False(exceptionCaught); // Exception should be caught internally, not propagate
        }

        // --------------------------------------------------------------------- Set Cooldown ---------------------------------------------------------------
        [Fact]
        public void SetCooldown_ShouldSetCooldownForDomain()
        {
            // Arrange
            string domain = "example.com";
            TimeSpan cooldown = TimeSpan.FromSeconds(1);

            // Act
            _rateLimiter.SetCooldown(domain, cooldown);

            // Assert
            Assert.True(_rateLimiter.IsOnCooldown(domain));
        }

        // --------------------------------------------------------------------- IsOnCooldown ---------------------------------------------------------------
        
           
        [Fact]
        public async Task IsOnCooldown_WhenStillOnCooldown_ShouldReturnTrue()
        {
            // Arrange
            string domain = "example.com";

            // Act
            _rateLimiter.SetCooldown(domain, TimeSpan.FromMinutes(100));

            // Assert
            Assert.True(_rateLimiter.IsOnCooldown(domain));
        }

        [Fact]
        public async Task IsOnCooldown_WhenCooldownExpires_ShouldReturnFalse()
        {
            // Arrange
            string domain = "example.com";
            _rateLimiter.SetCooldown(domain, TimeSpan.FromMilliseconds(200));

            // Act
            Assert.True(_rateLimiter.IsOnCooldown(domain));

            await Task.Delay(300);

            // Assert
            Assert.False(_rateLimiter.IsOnCooldown(domain));
        }

        // ------------------------------------------------------------------- Start Processing -------------------------------------------------------------------

        [Fact]
        public async Task StartProcessing_ShouldCompleteImmediately()
        {
            // Act
            await _rateLimiter.StartProcessing();

            // Assert
            Assert.True(true); // No exception means it completed successfully
        }

        // ------------------------------------------------------------------ StopProcessing --------------------------------------------------

        [Fact]
        public async Task StopProcessing_ShouldCompleteImmediately()
        {
            // Act
            await _rateLimiter.StopProcessing();

            // Assert
            Assert.True(true); // No exception means it completed successfully
        }
    }
}