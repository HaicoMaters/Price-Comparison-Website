using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceComparisonWebsite.Services.HttpClients;
using PriceComparisonWebsite.Services.Utilities;
using PriceComparisonWebsite.Services.Utilities.Interfaces;
using PriceComparisonWebsite.Services.WebScraping;
using PriceComparisonWebsite.Services.WebScraping.Interfaces;
using PriceComparisonWebsite.Services.WebScraping.Parsers;
using PriceComparisonWebsite.Services.WebScraping.Parsers.Interfaces;

namespace PriceComparisonWebsite.Extensions
{

    public static class PriceParserExtensions
    {
        public static IServiceCollection AddPriceParsers(this IServiceCollection services)
        {
            services.AddHttpClient<IScraperHttpClient, ScraperHttpClient>(); // Add the http client

            services.AddScoped<IPriceScraperService, PriceScraperService>();
            services.AddScoped<IRobotsTxtChecker, RobotsTxtChecker>();
            services.AddScoped<IFileSystemWrapper, FileSystemWrapper>();
            services.AddScoped<IScraperRateLimiter, ScraperRateLimiter>();
            services.AddScoped<IRetryHandler, RetryHandler>();

            services.AddScoped<IScraperStatusService, ScraperStatusService>();
            services.AddHostedService<PriceScraperBackgroundService>();
            services.AddScoped<IScraperLogService, ScraperLogService>();


            // Add individual parsers
            services.AddTransient<IPriceParser, AmazonPriceParser>();
            services.AddTransient<IPriceParser, NeweggPriceParser>();

            // Register the factory as a singleton with all parsers injected
            services.AddSingleton<IPriceParserFactory, PriceParserFactory>(sp =>
            {
                var parsers = sp.GetServices<IPriceParser>().ToList();  // Retrieve all registered parsers
                return new PriceParserFactory(parsers);
            });

            return services;
        }
    }
}