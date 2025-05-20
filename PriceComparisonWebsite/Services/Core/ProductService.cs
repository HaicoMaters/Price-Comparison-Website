using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using PriceComparisonWebsite.Models;
using PriceComparisonWebsite.Services.Interfaces;

namespace PriceComparisonWebsite.Services.Implementations
{
    /// <inheritdoc />
    public class ProductService : IProductService
    {
        public readonly IRepository<Product> _products;
        public readonly IRepository<PriceListing> _priceListings;
        public readonly ILogger<ProductService> _logger;
        private readonly IRepository<ProductPriceHistory> _priceHistory;

        public ProductService(IRepository<Product> products, IRepository<PriceListing> priceListings, IRepository<ProductPriceHistory> priceHistory, ILogger<ProductService> logger)
        {
            _products = products;
            _priceListings = priceListings;
            _priceHistory = priceHistory;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task AddProduct(Product product)
        {
            try
            {
                await _products.AddAsync(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add product");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task DeleteProduct(int productId)
        {
            try{
                await _products.DeleteAsync(productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete product");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            try{
                return await _products.GetAllAsync();
            }
            catch(Exception ex){
                _logger.LogError(ex, "Failed to get products");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Product>> GetAllProducts(QueryOptions<Product> queryOptions)
        {
            try{
                return await _products.GetAllAsync(queryOptions);
            }
            catch(Exception ex){
                _logger.LogError(ex, "Failed to get products");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<Product> GetProductById(int productId, QueryOptions<Product> queryOptions)
        {
            try{
                return await _products.GetByIdAsync(productId, queryOptions);
            }
            catch(Exception ex){
                _logger.LogError(ex, "Failed to get products by id");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Product>> GetProductsByCategoryId(int categoryId, QueryOptions<Product> queryOptions)
        {
            try{
                return await _products.GetAllByIdAsync(categoryId, "CategoryId", queryOptions);
            }
            catch(Exception ex){
                _logger.LogError(ex, "Failed to get products by category id");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task RecalculateCheapestPrice(int productId)
        {
            try
			{
				var product = await _products.GetByIdAsync(productId, new QueryOptions<Product>());
				if (product == null)
					throw new InvalidOperationException("Product not found");

				var listings = await _priceListings.GetAllByIdAsync(productId, "ProductId", new QueryOptions<PriceListing>());
				
				// If no listings exist, set cheapest price to 0
				if (!listings.Any())
				{
					product.CheapestPrice = 0;
					await _products.UpdateAsync(product);
					return;
				}

				decimal cheapestPrice = listings.Min(l => l.DiscountedPrice);
				if (product.CheapestPrice != cheapestPrice && cheapestPrice > 0)
                {
                    await RecordPriceHistory(productId, cheapestPrice);
                }
				product.CheapestPrice = cheapestPrice;
				await _products.UpdateAsync(product);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to recalculate cheapest price. ProductId: {ProductId}", productId);
                throw;
			}
        }

        /// <inheritdoc />
        public async Task<Product> UpdateProduct(Product product)
        {
            try{
                var exsistingProduct = await _products.GetByIdAsync(product.ProductId, new QueryOptions<Product>());
                exsistingProduct.Name = product.Name;
                exsistingProduct.Description = product.Description;
                exsistingProduct.CategoryId = product.CategoryId;

                await _products.UpdateAsync(exsistingProduct);
                return exsistingProduct;
            }
            catch(Exception ex){
                _logger.LogError(ex, "Failed to update product, ProductId: {ProductId}", product.ProductId);
                throw;
            }
        }

        /// <inheritdoc />
        public List<Product> SetupPagination(IEnumerable<Product> allProducts, int pageNumber, ViewDataDictionary viewData)
        {
            int pageSize = 12; // Number of products per page

            viewData["PageNumber"] = pageNumber;
            viewData["TotalPages"] = (int)Math.Ceiling(allProducts.Count() / (double)pageSize);

            return allProducts
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
        }

        /// <inheritdoc />
        public async Task RecordPriceHistory(int productId, decimal price)
        {
            try
            {
                var history = new ProductPriceHistory
                {
                    ProductId = productId,
                    Price = price,
                    Timestamp = DateTime.Now
                };
                await _priceHistory.AddAsync(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to record price history");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ProductPriceHistory>> GetPriceHistory(int productId)
        {
            try
            {
                var options = new QueryOptions<ProductPriceHistory>
                {
                    OrderBy = h => h.Timestamp,
                    Where = h => h.ProductId == productId
                };
                return await _priceHistory.GetAllAsync(options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get price history");
                throw;
            }
        }
    }
}