using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using PriceComparisonWebsite.Services.Utilities.Interfaces;

namespace PriceComparisonWebsite.Services.Utilities
{
    /// <inheritdoc />
    public class RetryHandler : IRetryHandler
    {

        private readonly ILogger<RetryHandler> _logger;

        public RetryHandler(ILogger<RetryHandler> logger)
        {
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, string operationName, int maxRetries = 3)
        {
            for (int i = 0; i <= maxRetries; i++)
            {
                try
                {
                    return await operation();
                }
                catch (HttpRequestException ex) when (ShouldRetry(ex, i, maxRetries))
                {
                    await HandleRetry(operationName, i, maxRetries, ex);
                }
                catch (Exception ex) when (i < maxRetries && !(ex is HttpRequestException))
                {
                    await HandleRetry(operationName, i, maxRetries, ex);
                }
            }

            throw new Exception($"Operation {operationName} failed after {maxRetries} retries");
        }

        /// <inheritdoc />
        public async Task ExecuteWithRetryAsync(Func<Task> operation, string operationName, int maxRetries = 3)
        {
            for (int i = 0; i <= maxRetries; i++)
            {
                try
                {
                    await operation();
                    return;
                }
                catch (HttpRequestException ex) when (ShouldRetry(ex, i, maxRetries))
                {
                    await HandleRetry(operationName, i, maxRetries, ex);
                }
                catch (Exception ex) when (i < maxRetries && !(ex is HttpRequestException))
                {
                    await HandleRetry(operationName, i, maxRetries, ex);
                }
            }

            throw new Exception($"Operation {operationName} failed after {maxRetries} retries");
        }

        /// <inheritdoc />
        public bool ShouldRetry(HttpRequestException ex, int attemptNumber, int maxRetries) // Status codes where delay should happen i.e. not retring if 404
        {
            if (attemptNumber >= maxRetries) return false;

            if (ex.StatusCode.HasValue)
            {
                switch (ex.StatusCode.Value)
                {
                    case HttpStatusCode.TooManyRequests:
                    case HttpStatusCode.ServiceUnavailable:
                    case HttpStatusCode.GatewayTimeout:
                    case HttpStatusCode.RequestTimeout:
                        return true;
                    default:
                        return false;
                }
            }

            return true; // Retry network-related errors
        }

        /// <inheritdoc />
        public async Task HandleRetry(string operationName, int attemptNumber, int maxRetries, Exception ex)
        {
            double delay = Math.Pow(2, attemptNumber) * 1000; // Exponential backoff increse delay more the more attempts
            _logger.LogWarning(ex,
                "Attempt {AttemptNumber}/{MaxRetries} for {OperationName} failed. Retrying in {Delay}ms...",
                attemptNumber + 1, maxRetries, operationName, delay);
            
            await Task.Delay((int)delay);
        }
    }
}