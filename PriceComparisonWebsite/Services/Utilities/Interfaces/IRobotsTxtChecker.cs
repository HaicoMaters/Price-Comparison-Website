using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceComparisonWebsite.Services.Utilities.Interfaces
{
    /// <summary>
    /// Manages robots.txt file checking and caching
    /// </summary>
    public interface IRobotsTxtChecker
    {
        /// <summary>
        /// Checks if a URL is allowed by the site's robots.txt rules
        /// </summary>
        Task<bool> CheckRobotsTxt(Uri url);

        /// <summary>
        /// Downloads and caches a site's robots.txt file
        /// </summary>
        Task CacheRobotsTxtFile(Uri url);

        /// <summary>
        /// Checks if a robots.txt file is cached for a domain
        /// </summary>
        bool CheckIfRobotsTxtFileIsCached(string domain);
    }
}