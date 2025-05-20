using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace PriceComparisonWebsite.Hubs
{
    /// <summary>
    /// SignalR hub for real-time scraper log communication
    /// </summary>
    public class ScraperHub : Hub
    {
        /// <summary>
        /// Broadcasts a scraper log message to all connected clients
        /// </summary>
        /// <param name="message">The log message to broadcast</param>
        public async Task SendScraperLog(string message)
        {
            await Clients.All.SendAsync("ReceiveScraperLog", message);
        }
    }
}