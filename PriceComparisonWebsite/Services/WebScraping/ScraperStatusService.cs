using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceComparisonWebsite.Models;
using PriceComparisonWebsite.Services.WebScraping.Interfaces;

namespace PriceComparisonWebsite.Services.WebScraping
{
    public class ScraperStatusService : IScraperStatusService
    {
        private readonly IRepository<ScraperStatus> _scraperStatus;
        private readonly ILogger<ScraperStatusService> _logger;

        public ScraperStatusService(IRepository<ScraperStatus> scraperStatus, ILogger<ScraperStatusService> logger)
        {
            _scraperStatus = scraperStatus;
            _logger = logger;
        }

        public async Task<DateTime> GetLastUpdateTime()
        {
            try
            {
                var time = await _scraperStatus.GetByIdAsync(1, new QueryOptions<ScraperStatus>());
                return time?.LastUpdated ?? DateTime.MinValue;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving last update time.");
                throw;
            }
        }

        public async Task UpdateLastUpdateTime()
        {
            try
            {
                var time = await _scraperStatus.GetByIdAsync(1, new QueryOptions<ScraperStatus>());
                if (time == null)
                {
                    time = new ScraperStatus { Id = 1, LastUpdated = DateTime.Now };
                    await _scraperStatus.AddAsync(time);
                }
                else
                {
                    time.LastUpdated = DateTime.Now;
                    await _scraperStatus.UpdateAsync(time);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating last update time.");
                throw;
            }
        }
    }
}