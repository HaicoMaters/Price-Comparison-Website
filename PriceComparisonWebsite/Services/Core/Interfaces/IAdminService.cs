using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceComparisonWebsite.Models;

namespace PriceComparisonWebsite.Services
{
    /// <summary>
    /// Service for administrative operations and statistics
    /// </summary>
    public interface IAdminService
    {
        /// <summary>
        /// Gets all products in the system
        /// </summary>
        Task<IEnumerable<Product>> GetAllProductsAsync();

        /// <summary>
        /// Gets all price listings in the system
        /// </summary>
        Task<IEnumerable<PriceListing>> GetAllPriceListingsAsync();

        /// <summary>
        /// Gets all vendors in the system
        /// </summary>
        Task<IEnumerable<Vendor>> GetAllVendorsAsync();

        /// <summary>
        /// Gets the total number of users in the system
        /// </summary>
        Task<int> GetTotalUsersAsync();
    }
}