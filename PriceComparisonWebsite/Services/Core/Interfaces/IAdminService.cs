using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceComparisonWebsite.Models;

namespace PriceComparisonWebsite.Services
{
    public interface IAdminService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<PriceListing>> GetAllPriceListingsAsync();
        Task<IEnumerable<Vendor>> GetAllVendorsAsync();
        Task<int> GetTotalUsersAsync();
    }
}