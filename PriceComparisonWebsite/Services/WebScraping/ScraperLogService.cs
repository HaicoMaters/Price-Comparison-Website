using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PriceComparisonWebsite.Hubs;
using PriceComparisonWebsite.Services.WebScraping.Interfaces;

namespace PriceComparisonWebsite.Services.WebScraping
{
    /// <inheritdoc />
    public class ScraperLogService : IScraperLogService
    {
        private readonly IHubContext<ScraperHub> _hubContext;

        public ScraperLogService(IHubContext<ScraperHub> hubContext)
        {
            _hubContext = hubContext;
        }

        /// <inheritdoc />
        public async Task SendLogAsync(string message)
        {
            await _hubContext.Clients.All.SendAsync("ReceiveScraperLog", message);
        }
    }
}