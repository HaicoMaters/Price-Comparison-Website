using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceComparisonWebsite.Services.WebScraping.Interfaces
{
    /// <summary>
    /// Service for real-time logging of scraping operations
    /// </summary>
    public interface IScraperLogService
    {
        /// <summary>
        /// Sends a log message to connected clients
        /// </summary>
        /// <param name="message">The message to log</param>
        Task SendLogAsync(string message);
    }
}