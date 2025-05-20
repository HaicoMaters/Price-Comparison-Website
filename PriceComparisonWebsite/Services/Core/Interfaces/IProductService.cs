using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using PriceComparisonWebsite.Models;

namespace PriceComparisonWebsite.Services.Interfaces
{
    /// <summary>
    /// Service for managing product-related operations
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Gets all products in the system
        /// </summary>
        Task<IEnumerable<Product>> GetAllProducts();

        /// <summary>
        /// Gets all products with specified query options
        /// </summary>
        Task<IEnumerable<Product>> GetAllProducts(QueryOptions<Product> queryOptions);

        /// <summary>
        /// Gets a specific product by its ID
        /// </summary>
        Task<Product> GetProductById(int productId, QueryOptions<Product> queryOptions);

        /// <summary>
        /// Adds a new product to the system
        /// </summary>
        Task AddProduct(Product product);

        /// <summary>
        /// Updates an existing product's information
        /// </summary>
        /// <returns>The updated product</returns>
        Task<Product> UpdateProduct(Product product);

        /// <summary>
        /// Deletes a product from the system
        /// </summary>
        Task DeleteProduct(int productId);

        /// <summary>
        /// Gets all products in a specific category
        /// </summary>
        Task<IEnumerable<Product>> GetProductsByCategoryId(int categoryId, QueryOptions<Product> queryOptions);

        /// <summary>
        /// Recalculates and updates the cheapest price for a product
        /// </summary>
        Task RecalculateCheapestPrice(int productId);

        /// <summary>
        /// Sets up pagination for a list of products
        /// </summary>
        List<Product> SetupPagination(IEnumerable<Product> allProducts, int pageNumber, ViewDataDictionary viewData);

        /// <summary>
        /// Records a new price point in the product's price history
        /// </summary>
        Task RecordPriceHistory(int productId, decimal price);

        /// <summary>
        /// Retrieves the price history for a product
        /// </summary>
        Task<IEnumerable<ProductPriceHistory>> GetPriceHistory(int productId);
    }
}