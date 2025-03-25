using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Price_Comparison_Website.Services.WebScraping.Interfaces
{
    public interface IPriceScraperService
    {
        Task UpdateAllListings();
        Task<bool> UpdateListing(int id);
        Task FilterUsingRobotsTxt(Dictionary<Uri, int> uris);
        Task<List<int>> GetVendorIdsThatSupportScraping();
    }
}