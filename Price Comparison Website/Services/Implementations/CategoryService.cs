using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Price_Comparison_Website.Models;
using Price_Comparison_Website.Services.Interfaces;

namespace Price_Comparison_Website.Services.Implementations
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
                _logger.LogError(ex, "Error in CategoryService.GetAllCategories()");
                throw;
            }
        }

        public async Task<Category> GetCategoryById(int categoryId)
        {
            try{
                return await _categoryRepository.GetByIdAsync(categoryId, new QueryOptions<Category>());
            }
            catch(Exception ex){
                _logger.LogError(ex, "Error in CategoryService.GetCategoryById()");
                throw;
            }
        }
    }
}