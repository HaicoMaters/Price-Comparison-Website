using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Price_Comparison_Website.Models;

namespace Price_Comparison_Website.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProducts();
        Task<IEnumerable<Product>> GetAllProducts(QueryOptions<Product> queryOptions);
        Task<Product> GetProductById(int productId, QueryOptions<Product> queryOptions);
        Task AddProduct(Product product);
        Task<Product> UpdateProduct(Product product, int catId);
        Task DeleteProduct(int productId);
        Task<IEnumerable<Product>> GetProductsByCategoryId(int categoryId ,QueryOptions<Product> queryOptions);
        Task RecalculateCheapestPrice(int productId);
        List<Product> SetupPagination(IEnumerable<Product> allProducts, int pageNumber, ViewDataDictionary viewData);
    }
}