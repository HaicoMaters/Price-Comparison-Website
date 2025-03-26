using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Price_Comparison_Website.Services.Utilities.Interfaces;

namespace Price_Comparison_Website.Services.Utilities
{
    public class ScraperRateLimiter : IScraperRateLimiter
    {
        private readonly Dictionary<string, DateTime> _cooldowns = new();
        private readonly TimeSpan _cooldownDuration = TimeSpan.FromSeconds(2);

        public ScraperRateLimiter(TimeSpan cooldownDuration)
        {
            _cooldownDuration = cooldownDuration;
        }

        public async Task EnqueueRequest(Func<Task> requestFunc, string domain)
        {
            while (IsOnCooldown(domain))
            {
                await Task.Delay(100);  // Wait if on cooldown
            }

            try
            {
                await requestFunc();
                SetCooldown(domain, _cooldownDuration);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error for {domain}: {ex.Message}");
            }
        }

        public void SetCooldown(string domain, TimeSpan cooldown)
        {
            _cooldowns[domain] = DateTime.UtcNow.Add(cooldown);
        }

        public bool IsOnCooldown(string domain)
        {
            return _cooldowns.TryGetValue(domain, out var expiry) && DateTime.UtcNow < expiry;
        }

        public Task StartProcessing() => Task.CompletedTask;
        public Task StopProcessing() => Task.CompletedTask;
    }
}
