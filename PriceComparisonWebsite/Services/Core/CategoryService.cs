using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceComparisonWebsite.Models;
using PriceComparisonWebsite.Services.Interfaces;

namespace PriceComparisonWebsite.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        public readonly IRepository<Category> _categoryRepository;
        public readonly ILogger<CategoryService> _logger;

        public CategoryService(IRepository<Category> categoryRepositor, ILogger<CategoryService> logger)
        {
            _categoryRepository = categoryRepositor;
            _logger = logger;
        }

        public async Task<IEnumerable<Category>> GetAllCategories()
        {
            try{
                return await _categoryRepository.GetAllAsync();
            }
            catch(Exception ex){
                _logger.LogError(ex, "Failed to get categories");
                throw;
            }
        }

        public async Task<Category> GetCategoryById(int categoryId, QueryOptions<Category> queryOptions)
        {
            try{
                return await _categoryRepository.GetByIdAsync(categoryId,queryOptions);
            }
            catch(Exception ex){
                _logger.LogError(ex, "Failed to get category by id");
                throw;
            }
        }
    }
}