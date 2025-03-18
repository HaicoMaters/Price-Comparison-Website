using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Price_Comparison_Website.Models;

namespace Price_Comparison_Website.Services.Interfaces
{
    public interface IVendorService
    {
        Task<IEnumerable<Vendor>> GetAllVendorsAsync();
        Task<IEnumerable<Vendor>> GetAllVendorsAsync(QueryOptions<Vendor> queryOptions);
        Task<Vendor> GetVendorByIdAsync(int vendorId);
        Task AddVendorAsync(Vendor vendor);
        Task UpdateVendorAsync(Vendor vendor);
        Task DeleteVendorAsync(int vendorId);
    }
}