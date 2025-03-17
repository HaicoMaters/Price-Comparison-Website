using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Price_Comparison_Website.Models;
using Price_Comparison_Website.Services.Interfaces;

namespace Price_Comparison_Website.Services.Implementations
{
    public class ProductService : IProductService
    {
        public readonly IRepository<Product> _products;
        public readonly IRepository<PriceListing> _priceListings;
        public readonly ILogger<ProductService> _logger;

        public ProductService(IRepository<Product> products, IRepository<PriceListing> priceListings, ILogger<ProductService> logger)
        {
            _products = products;
            _priceListings = priceListings;
            _logger = logger;
        }

        public async Task AddProduct(Product product)
        {
            try
            {
                await _products.AddAsync(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ProductService.AddProduct()");
                throw;
            }
        }

        public async Task DeleteProduct(int productId)
        {
            try{
                await _products.DeleteAsync(productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ProductService.DeleteProduct()");
                throw;
            }
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            try{
                return await _products.GetAllAsync();
            }
            catch(Exception ex){
                _logger.LogError(ex, "Error in ProductService.GetAllProducts()");
                throw;
            }
        }

        public async Task<Product> GetProductById(int productId)
        {
            try{
                return await _products.GetByIdAsync(productId, new QueryOptions<Product>());
            }
            catch(Exception ex){
                _logger.LogError(ex, "Error in ProductService.GetProductById()");
                throw;
            }
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryId(int categoryId)
        {
            try{
                return await _products.GetAllByIdAsync(categoryId, "CategoryId", new QueryOptions<Product>());
            }
            catch(Exception ex){
                _logger.LogError(ex, "Error in ProductService.GetProductsByCategoryId()");
                throw;
            }
        }

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
				product.CheapestPrice = cheapestPrice;
				await _products.UpdateAsync(product);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to recalculate cheapest price. ProductId: {ProductId}", productId);
                throw;
			}
        }

        public async Task<Product> UpdateProduct(Product product, int catId)
        {
            try{
                var exsistingProduct = await _products.GetByIdAsync(product.ProductId, new QueryOptions<Product>());
                exsistingProduct.Name = product.Name;
                exsistingProduct.Description = product.Description;
                exsistingProduct.CategoryId = catId;

                await _products.UpdateAsync(exsistingProduct);
                return exsistingProduct;
            }
            catch(Exception ex){
                _logger.LogError(ex, "Error in ProductService.UpdateProduct()");
                throw;
            }
        }
    }
}