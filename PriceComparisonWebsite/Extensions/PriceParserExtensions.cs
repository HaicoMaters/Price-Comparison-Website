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
            // HTTP Client registration
            services.AddHttpClient<IScraperHttpClient, ScraperHttpClient>();

            // Singleton services 
            services.AddSingleton<IContentCompressor, ContentCompressor>();
            services.AddSingleton<IFileSystemWrapper, FileSystemWrapper>();
            services.AddSingleton<IRetryHandler, RetryHandler>();
            
            // Scoped services
            services.AddScoped<IPriceScraperService, PriceScraperService>();
            services.AddScoped<IRobotsTxtChecker, RobotsTxtChecker>();
            services.AddScoped<IScraperRateLimiter, ScraperRateLimiter>();
            services.AddScoped<IScraperStatusService, ScraperStatusService>();
            services.AddScoped<IScraperLogService, ScraperLogService>();

            // Background service
            services.AddHostedService<PriceScraperBackgroundService>();

            // Price parsers as transient
            services.AddTransient<IPriceParser, AmazonPriceParser>();
            services.AddTransient<IPriceParser, NeweggPriceParser>();

            // Parser factory as singleton
            services.AddSingleton<IPriceParserFactory>(sp =>
            {
                var parsers = sp.GetServices<IPriceParser>().ToList();
                return new PriceParserFactory(parsers);
            });

            return services;
        }
    }
}