using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using PriceComparisonWebsite.Models;

namespace PriceComparisonWebsite.Services.Interfaces
{
    /// <summary>
    /// Service for managing vendor-related operations
    /// </summary>
    public interface IVendorService
    {
        /// <summary>
        /// Gets all vendors in the system
        /// </summary>
        Task<IEnumerable<Vendor>> GetAllVendorsAsync();

        /// <summary>
        /// Gets all vendors with specified query options for filtering and ordering
        /// </summary>
        /// <param name="queryOptions">The query options to apply</param>
        Task<IEnumerable<Vendor>> GetAllVendorsAsync(QueryOptions<Vendor> queryOptions);

        /// <summary>
        /// Gets a specific vendor by their ID
        /// </summary>
        /// <param name="vendorId">The ID of the vendor to retrieve</param>
        Task<Vendor> GetVendorByIdAsync(int vendorId);

        /// <summary>
        /// Adds a new vendor to the system
        /// </summary>
        /// <param name="vendor">The vendor to add</param>
        Task AddVendorAsync(Vendor vendor);

        /// <summary>
        /// Updates an existing vendor's information
        /// </summary>
        /// <param name="vendor">The vendor with updated information</param>
        Task UpdateVendorAsync(Vendor vendor);

        /// <summary>
        /// Deletes a vendor from the system
        /// </summary>
        /// <param name="vendorId">The ID of the vendor to delete</param>
        Task DeleteVendorAsync(int vendorId);

        /// <summary>
        /// Sets up pagination for a list of vendors
        /// </summary>
        /// <param name="allVendors">Complete list of vendors to paginate</param>
        /// <param name="pageNumber">The requested page number</param>
        /// <param name="viewData">ViewData dictionary for storing pagination information</param>
        /// <returns>A list of vendors for the requested page</returns>
        List<Vendor> SetupPagination(IEnumerable<Vendor> allVendors, int pageNumber, ViewDataDictionary viewData);
    }
}