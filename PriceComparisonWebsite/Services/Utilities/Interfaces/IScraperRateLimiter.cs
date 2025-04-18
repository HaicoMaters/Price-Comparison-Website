using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceComparisonWebsite.Services.Utilities.Interfaces
{
    public interface IScraperRateLimiter
    {
        Task EnqueueRequest(Func<Task> requestFunc, string domain);
        void SetCooldown(string domain, TimeSpan cooldown);
        bool IsOnCooldown(string domain);
        Task StartProcessing();
        Task StopProcessing();
    }
}