using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceComparisonWebsite.Models;

namespace PriceComparisonWebsite.Services.Interfaces
{
    public interface IPriceListingService
    {
        Task<IEnumerable<PriceListing>> GetPriceListingsByProductId(int productId, QueryOptions<PriceListing> queryOptions);
        Task<IEnumerable<PriceListing>> GetPriceListingsByVendorId(int vendorId, QueryOptions<PriceListing> queryOptions);
        Task<IEnumerable<PriceListing>> GetAllPriceListings(QueryOptions<PriceListing> queryOptions);
        Task<IEnumerable<PriceListing>> GetAllPriceListings();
        Task<PriceListing> GetPriceListingById(int priceListingId, QueryOptions<PriceListing> queryOptions);
        Task DeletePriceListing(int priceListingId);
        Task AddPriceListing(PriceListing priceListing);
        Task UpdatePriceListing(PriceListing priceListing);
        Task UpdateCheapestPrice(int productId, decimal newPrice);
      
    }
}