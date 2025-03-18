using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Price_Comparison_Website.Models;

namespace Price_Comparison_Website.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategories();
        Task<Category> GetCategoryById(int categoryId, QueryOptions<Category> queryOptions);
    }
}