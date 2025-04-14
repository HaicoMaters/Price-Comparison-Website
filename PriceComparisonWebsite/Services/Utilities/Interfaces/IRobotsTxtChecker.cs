using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceComparisonWebsite.Services.Utilities.Interfaces
{
    public interface IRobotsTxtChecker
    {
        Task<bool> CheckRobotsTxt(Uri url);
        Task CacheRobotsTxtFile(Uri url);
        bool CheckIfRobotsTxtFileIsCached(string domain);
    }
}