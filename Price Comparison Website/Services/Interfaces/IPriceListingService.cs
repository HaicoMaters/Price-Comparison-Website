using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Price_Comparison_Website.Models;

namespace Price_Comparison_Website.Services.Interfaces
{
    public interface IPriceListingService
    {
        Task<IEnumerable<PriceListing>> GetPriceListingsByProductId(int productId);
        Task<IEnumerable<PriceListing>> GetAllPriceListings();
        Task<PriceListing> GetPriceListingById(int priceListingId);
        Task DeletePriceListing(int priceListingId);
        Task AddPriceListing(PriceListing priceListing);
        Task UpdatePriceListing(PriceListing priceListing);
      
    }
}