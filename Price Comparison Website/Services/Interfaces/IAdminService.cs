using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Price_Comparison_Website.Models;

namespace Price_Comparison_Website.Services
{
    public interface IAdminService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<PriceListing>> GetAllPriceListingsAsync();
        Task<IEnumerable<Vendor>> GetAllVendorsAsync();
    }
}