using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Price_Comparison_Website.Models;

namespace Price_Comparison_Website.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProducts();
        Task<Product> GetProductById(int productId);
        Task AddProduct(Product product);
        Task<Product> UpdateProduct(Product product, int catId);
        Task DeleteProduct(int productId);
        Task<IEnumerable<Product>> GetProductsByCategoryId(int categoryId);
        Task RecalculateCheapestPrice(int productId);
    }
}