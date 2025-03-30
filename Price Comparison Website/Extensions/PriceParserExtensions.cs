using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Price_Comparison_Website.Services.HttpClients;
using Price_Comparison_Website.Services.WebScraping;
using Price_Comparison_Website.Services.WebScraping.Interfaces;
using Price_Comparison_Website.Services.WebScraping.Parsers;
using Price_Comparison_Website.Services.WebScraping.Parsers.Interfaces;

namespace Price_Comparison_Website.Extensions
{

    public static class PriceParserExtensions
    {
        public static IServiceCollection AddPriceParsers(this IServiceCollection services)
        {
            services.AddHttpClient<IScraperHttpClient, ScraperHttpClient>(); // Add the http client

            // Add individual parsers
            services.AddTransient<IPriceParser, AmazonPriceParser>();

            // Register the factory as a singleton with all parsers injected
            services.AddSingleton<IPriceParserFactory>(sp =>
            {
                var parsers = sp.GetServices<IPriceParser>();  // Retrieve all registered parsers
                return new PriceParserFactory(parsers);
            });

            return services;
        }
    }
}