using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PriceComparisonWebsite.Models;
using PriceComparisonWebsite.Services.Interfaces;
using Microsoft.Extensions.Logging;

namespace PriceComparisonWebsite.Services.Implementations
{
    /// <inheritdoc />
    public class PriceListingService : IPriceListingService
    {
        private readonly IRepository<PriceListing> _priceListings;
        private readonly ILogger<PriceListingService> _logger;
        private readonly INotificationService _notificationService;
        private readonly IProductService _productService;

        public PriceListingService(
            IRepository<PriceListing> priceListings,
            ILogger<PriceListingService> logger,
            INotificationService notificationService,
            IProductService productService
         )
        {
            _priceListings = priceListings;
            _logger = logger;
            _notificationService = notificationService;
            _productService = productService;
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task<IEnumerable<PriceListing>> GetPriceListingsByVendorId(int vendorId, QueryOptions<PriceListing> queryOptions)
        {
            try
            {
                return await _priceListings.GetAllByIdAsync(vendorId, "VendorId", queryOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get pricelistings with vendorId");
                throw;
            }
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public async Task UpdateCheapestPrice(int productId, decimal newPrice)
        {
            try
            {
                var existingProduct = await _productService.GetProductById(productId, new QueryOptions<Product>());
                if (existingProduct == null)
                    throw new InvalidOperationException("Product not found");

                decimal oldPrice = existingProduct.CheapestPrice;
                if (newPrice < existingProduct.CheapestPrice || existingProduct.CheapestPrice == 0)
                {
                    // Send Notification to Users with item on wishlist if price drops
                    await _notificationService.CreateProductPriceDropNotifications(productId, existingProduct.Name, newPrice, oldPrice);
                }
                await _productService.RecalculateCheapestPrice(productId); // Recalculate cheapest price after updating
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update cheapest price. ProductId: {ProductId}", productId);
                throw new InvalidOperationException("Failed to update cheapest price", ex);
            }
        }
    }
}