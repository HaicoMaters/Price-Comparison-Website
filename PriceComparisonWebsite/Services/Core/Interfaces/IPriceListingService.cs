using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceComparisonWebsite.Models;

namespace PriceComparisonWebsite.Services.Interfaces
{
    /// <summary>
    /// Service for managing price listings and price updates
    /// </summary>
    public interface IPriceListingService
    {
        /// <summary>
        /// Gets all price listings for a specific product
        /// </summary>
        Task<IEnumerable<PriceListing>> GetPriceListingsByProductId(int productId, QueryOptions<PriceListing> queryOptions);

        /// <summary>
        /// Gets all price listings from a specific vendor
        /// </summary>
        Task<IEnumerable<PriceListing>> GetPriceListingsByVendorId(int vendorId, QueryOptions<PriceListing> queryOptions);

        /// <summary>
        /// Gets all price listings with specified query options
        /// </summary>
        Task<IEnumerable<PriceListing>> GetAllPriceListings(QueryOptions<PriceListing> queryOptions);

        /// <summary>
        /// Gets all price listings
        /// </summary>
        Task<IEnumerable<PriceListing>> GetAllPriceListings();

        /// <summary>
        /// Gets a specific price listing by its ID
        /// </summary>
        Task<PriceListing> GetPriceListingById(int priceListingId, QueryOptions<PriceListing> queryOptions);

        /// <summary>
        /// Deletes a price listing
        /// </summary>
        Task DeletePriceListing(int priceListingId);

        /// <summary>
        /// Adds a new price listing
        /// </summary>
        Task AddPriceListing(PriceListing priceListing);

        /// <summary>
        /// Updates an existing price listing
        /// </summary>
        Task UpdatePriceListing(PriceListing priceListing);

        /// <summary>
        /// Updates the cheapest price for a product and sends notifications if price drops
        /// </summary>
        Task UpdateCheapestPrice(int productId, decimal newPrice);
    }
}