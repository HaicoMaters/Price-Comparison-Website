using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using PriceComparisonWebsite.Models;

namespace PriceComparisonWebsite.Services.Interfaces
{
    public interface IVendorService
    {
        Task<IEnumerable<Vendor>> GetAllVendorsAsync();
        Task<IEnumerable<Vendor>> GetAllVendorsAsync(QueryOptions<Vendor> queryOptions);
        Task<Vendor> GetVendorByIdAsync(int vendorId);
        Task AddVendorAsync(Vendor vendor);
        Task UpdateVendorAsync(Vendor vendor);
        Task DeleteVendorAsync(int vendorId);
        List<Vendor> SetupPagination(IEnumerable<Vendor> allVendors, int pageNumber, ViewDataDictionary viewData);
    }
}