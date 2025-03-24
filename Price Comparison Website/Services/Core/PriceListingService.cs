using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Price_Comparison_Website.Models;
using Price_Comparison_Website.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace Price_Comparison_Website.Services.Implementations
{
    public class PriceListingService : IPriceListingService
    {
        private readonly IRepository<PriceListing> _priceListings;
        private readonly ILogger<PriceListingService> _logger;

        public PriceListingService(IRepository<PriceListing> priceListings, ILogger<PriceListingService> logger)
        {
            _priceListings = priceListings;
            _logger = logger;
        }

        public async Task AddPriceListing(PriceListing priceListing)
        {
            try
            {
                await _priceListings.AddAsync(priceListing);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add pricelisting");
                throw;
            }

        }

        public async Task DeletePriceListing(int priceListingId)
        {
            try
            {
                await _priceListings.DeleteAsync(priceListingId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete pricelisting");
                throw;
            }
        }

        public async Task<IEnumerable<PriceListing>> GetAllPriceListings(QueryOptions<PriceListing> queryOptions)
        {
            try
            {
                return await _priceListings.GetAllAsync(queryOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get pricelistings");
                throw;
            }
        }

        public async Task<IEnumerable<PriceListing>> GetAllPriceListings()
        {
            try
            {
                return await _priceListings.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get pricelistings");
                throw;
            }
        }

        public async Task<PriceListing> GetPriceListingById(int priceListingId, QueryOptions<PriceListing> queryOptions)
        {
            try
            {
                return await _priceListings.GetByIdAsync(priceListingId, queryOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to get pricelisting with id: {priceListingId}");
                throw;
            }
        }

        public async Task<IEnumerable<PriceListing>> GetPriceListingsByProductId(int productId, QueryOptions<PriceListing> queryOptions)
        {
            try
            {
                return await _priceListings.GetAllByIdAsync(productId, "ProductId", queryOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get pricelistings with prodId");
                throw;
            }
        }

        public async Task<IEnumerable<PriceListing>> GetPriceListingsByVendorId(int vendorId, QueryOptions<PriceListing> queryOptions)
        {
            try
            {
                return await _priceListings.GetAllByIdAsync(vendorId, "ProductId", queryOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get pricelistings with prodId");
                throw;
            }
        }

        public async Task UpdatePriceListing(PriceListing priceListing)
        {
            try
            {
                var existingListing = await GetPriceListingById(priceListing.PriceListingId, new QueryOptions<PriceListing>());

                existingListing.PurchaseUrl = priceListing.PurchaseUrl;
                existingListing.Price = priceListing.Price;
                existingListing.DiscountedPrice = priceListing.DiscountedPrice;
                existingListing.VendorId = priceListing.VendorId;
                existingListing.DateListed = DateTime.Now;

                await _priceListings.UpdateAsync(existingListing);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update pricelistings");
                throw;
            }
        }
    }
}