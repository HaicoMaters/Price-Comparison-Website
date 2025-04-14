using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceComparisonWebsite.Models;

namespace PriceComparisonWebsite.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategories();
        Task<Category> GetCategoryById(int categoryId, QueryOptions<Category> queryOptions);
    }
}