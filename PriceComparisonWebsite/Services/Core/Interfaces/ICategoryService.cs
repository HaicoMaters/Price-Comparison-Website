using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceComparisonWebsite.Models;

namespace PriceComparisonWebsite.Services.Interfaces
{
    /// <summary>
    /// Service for managing product categories
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// Gets all product categories
        /// </summary>
        Task<IEnumerable<Category>> GetAllCategories();

        /// <summary>
        /// Gets a specific category by its ID
        /// </summary>
        Task<Category> GetCategoryById(int categoryId, QueryOptions<Category> queryOptions);
    }
}