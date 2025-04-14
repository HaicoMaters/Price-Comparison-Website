using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceComparisonWebsite.Models;

namespace PriceComparisonWebsite.Services
{
    public class AdminService : IAdminService
    {
        private readonly IRepository<Product> _products;
        private readonly IRepository<PriceListing> _priceListings;
        private readonly IRepository<Vendor> _vendors;
        private readonly ILogger<AdminService> _logger;

        public AdminService(IRepository<Product> products,
            IRepository<PriceListing> priceListings,
            IRepository<Vendor> vendors, ILogger<AdminService> logger)
        {
            _products = products;
            _priceListings = priceListings;
            _vendors = vendors;
            _logger = logger;
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
    }
}