using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriceComparisonWebsite.Services.WebScraping.Interfaces
{
    public interface IScraperStatusService
    {
        Task<DateTime> GetLastUpdateTime();
        Task UpdateLastUpdateTime();

    }
}