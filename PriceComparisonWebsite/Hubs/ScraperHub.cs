using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace PriceComparisonWebsite.Hubs
{
    public class ScraperHub : Hub
    {
        public async Task SendScraperLog(string message)
        {
            await Clients.All.SendAsync("ReceiveScraperLog", message);
        }
    }
}