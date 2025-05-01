using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PriceComparisonWebsite.Models;

namespace PriceComparisonWebsite.Services
{
    public class AdminService : IAdminService
    {
        private readonly IRepository<Product> _products;
        private readonly IRepository<PriceListing> _priceListings;
        private readonly IRepository<Vendor> _vendors;
        private readonly ILogger<AdminService> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminService(
            IRepository<Product> products,
            IRepository<PriceListing> priceListings,
            IRepository<Vendor> vendors, ILogger<AdminService> logger, 
            UserManager<ApplicationUser> userManager
            )
        {
            _products = products;
            _priceListings = priceListings;
            _vendors = vendors;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<IEnumerable<PriceListing>> GetAllPriceListingsAsync()
        {
            return await _priceListings.GetAllAsync();
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _products.GetAllAsync();
        }

        public async Task<IEnumerable<Vendor>> GetAllVendorsAsync()
        {
            return await _vendors.GetAllAsync();
        }

        public async Task<int> GetTotalUsersAsync()
        {
            try
            {
                var count = await _userManager.Users.CountAsync();
                _logger.LogInformation("Retrieved total user count: {Count}", count);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting total user count");
                return 0;
            }
        }
    }
}