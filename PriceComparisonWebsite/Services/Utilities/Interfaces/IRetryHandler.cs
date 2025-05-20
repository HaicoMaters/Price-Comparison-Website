using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceComparisonWebsite.Services.Utilities.Interfaces
{
    /// <summary>
    /// Handles retry logic for operations that may fail temporarily
    /// </summary>
    public interface IRetryHandler
    {
        /// <summary>
        /// Executes an operation with retry logic
        /// </summary>
        /// <typeparam name="T">The type of result returned by the operation</typeparam>
        /// <param name="operation">The operation to execute</param>
        /// <param name="operationName">Name of the operation for logging</param>
        /// <param name="maxRetries">Maximum number of retry attempts</param>
        /// <returns>The result of the operation</returns>
        Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation, string operationName, int maxRetries = 3);

        /// <summary>
        /// Executes an operation with retry logic that returns no result
        /// </summary>
        /// <param name="operation">The operation to execute</param>
        /// <param name="operationName">Name of the operation for logging</param>
        /// <param name="maxRetries">Maximum number of retry attempts</param>
        Task ExecuteWithRetryAsync(Func<Task> operation, string operationName, int maxRetries = 3);

        /// <summary>
        /// Determines if an operation should be retried based on the exception
        /// </summary>
        bool ShouldRetry(HttpRequestException ex, int attemptNumber, int maxRetries);

        /// <summary>
        /// Handles the retry attempt including logging and delay calculation
        /// </summary>
        Task HandleRetry(string operationName, int attemptNumber, int maxRetries, Exception ex);
    }
}