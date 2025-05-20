using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceComparisonWebsite.Models
{
    /// <summary>
    /// Represents the status of the web scraper
    /// </summary>
    public class ScraperStatus
    {
        public int Id { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}