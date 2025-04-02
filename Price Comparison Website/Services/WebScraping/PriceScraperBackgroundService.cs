using System;
using System.Collections.Generic;
using System.Linq;
using Price_Comparison_Website.Models;
using Price_Comparison_Website.Services.WebScraping;
using Price_Comparison_Website.Services.WebScraping.Interfaces;


public class PriceScraperBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PriceScraperBackgroundService> _logger;
    private readonly static int MINSTOCHECK = 5;
    private readonly static int HOURSTOWAITFORAUTOMATICUPDATES = 12;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(MINSTOCHECK);
    private readonly TimeSpan _scrapeThreshold = TimeSpan.FromHours(HOURSTOWAITFORAUTOMATICUPDATES);

    public PriceScraperBackgroundService(IServiceProvider serviceProvider, ILogger<PriceScraperBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using (var scope = _serviceProvider.CreateScope())
                {
                    var scraperService = scope.ServiceProvider.GetRequiredService<IPriceScraperService>();
                    var scraperStatusService = scope.ServiceProvider.GetRequiredService<IScraperStatusService>();

                    var lastUpdate = await scraperStatusService.GetLastUpdateTime();
                    var timeSinceLastUpdate = DateTime.Now - lastUpdate;

                    if (timeSinceLastUpdate >= _scrapeThreshold)
                    {
                        _logger.LogInformation("Doing an automatic update of prices starting scraping.");
                        await scraperService.UpdateAllListings();
                        //await scraperStatusService.UpdateLastUpdateTime(); this is done within the main service so manual updates also count
                    }
                    else
                    {
                        _logger.LogInformation($"Skipping scrape. Last update was {timeSinceLastUpdate.TotalMinutes:F2} minutes ago, automatic updates are done every {HOURSTOWAITFORAUTOMATICUPDATES} hours.");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while running price scraper.");
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }
}