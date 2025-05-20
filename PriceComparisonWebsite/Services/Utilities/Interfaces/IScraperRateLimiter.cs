using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceComparisonWebsite.Services.Utilities.Interfaces
{
    /// <summary>
    /// Manages rate limiting for web scraping requests
    /// </summary>
    public interface IScraperRateLimiter
    {
        /// <summary>
        /// Queues a request for execution with rate limiting
        /// </summary>
        /// <param name="requestFunc">The request function to execute</param>
        /// <param name="domain">The domain being requested</param>
        Task EnqueueRequest(Func<Task> requestFunc, string domain);

        /// <summary>
        /// Sets a cooldown period for a specific domain
        /// </summary>
        void SetCooldown(string domain, TimeSpan cooldown);

        /// <summary>
        /// Checks if a domain is currently on cooldown
        /// </summary>
        bool IsOnCooldown(string domain);

        /// <summary>
        /// Starts the rate limiter processing
        /// </summary>
        Task StartProcessing();

        /// <summary>
        /// Stops the rate limiter processing
        /// </summary>
        Task StopProcessing();
    }
}