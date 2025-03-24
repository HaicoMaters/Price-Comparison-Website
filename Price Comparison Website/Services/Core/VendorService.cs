using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Price_Comparison_Website.Models;
using Price_Comparison_Website.Services.Interfaces;

namespace Price_Comparison_Website.Services.Implementations
{
    public class VendorService : IVendorService
    {
        public readonly IRepository<Vendor> _vendorRepository;
        public readonly ILogger<VendorService> _logger;

        public VendorService(IRepository<Vendor> vendors, ILogger<VendorService> logger)
        {
            _vendorRepository = vendors;
            _logger = logger;
        }

        public async Task AddVendorAsync(Vendor vendor)
        {
            try
            {
                await _vendorRepository.AddAsync(vendor);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error adding vendor");
                throw;
            }
        }

        public async Task DeleteVendorAsync(int vendorId)
        {
            try
            {
                var vendor = await _vendorRepository.GetByIdAsync(vendorId, new QueryOptions<Vendor>());
                if (vendor != null)
                {
                    await _vendorRepository.DeleteAsync(vendorId);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error deleting vendor");
                throw;
            }
        }

        public async Task<IEnumerable<Vendor>> GetAllVendorsAsync()
        {
            try
            {
                return await _vendorRepository.GetAllAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting all vendors");
                throw;
            }
        }

        public async Task<IEnumerable<Vendor>> GetAllVendorsAsync(QueryOptions<Vendor> queryOptions)
        {
            try
            {
                return await _vendorRepository.GetAllAsync(queryOptions);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting all vendors");
                throw;
            }
        }

        public async Task<Vendor> GetVendorByIdAsync(int vendorId)
        {
            try
            {
                return await _vendorRepository.GetByIdAsync(vendorId, new QueryOptions<Vendor>());
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error getting vendor by id");
                throw;
            }
        }

        public async Task UpdateVendorAsync(Vendor vendor)
        {
            try
            {
                var existingVendor = await _vendorRepository.GetByIdAsync(vendor.VendorId, new QueryOptions<Vendor>());

                existingVendor.VendorUrl = vendor.VendorUrl;
                existingVendor.VendorLogoUrl = vendor.VendorLogoUrl;
                existingVendor.Name = vendor.Name;
                existingVendor.SupportsAutomaticUpdates = vendor.SupportsAutomaticUpdates;

                await _vendorRepository.UpdateAsync(existingVendor);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error updating vendor");
                throw;
            }
        }
        
        public List<Vendor> SetupPagination(IEnumerable<Vendor> allVendors, int pageNumber, ViewDataDictionary viewData)
        {
            int pageSize = 12; // Number of products per page

            viewData["PageNumber"] = pageNumber;
            viewData["TotalPages"] = (int)Math.Ceiling(allVendors.Count() / (double)pageSize);

            return allVendors
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();
        }
    }
}