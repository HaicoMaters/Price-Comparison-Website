using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Price_Comparison_Website.Services.Utilities.Interfaces;

namespace Price_Comparison_Website.Services.Utilities
{
    public class RateLimiter : IRateLimiter
    {
        public Task EnqueueRequest(Func<Task> requestFunc, string domain)
        {
            throw new NotImplementedException();
        }

        public bool IsOnCooldown(string domain)
        {
            throw new NotImplementedException();
        }

        public void SetCooldown(string domain, TimeSpan cooldown)
        {
            throw new NotImplementedException();
        }

        public Task StartProcessing()
        {
            throw new NotImplementedException();
        }

        public Task StopProcessing()
        {
            throw new NotImplementedException();
        }
    }
}