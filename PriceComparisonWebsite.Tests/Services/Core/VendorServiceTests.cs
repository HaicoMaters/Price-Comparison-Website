using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using PriceComparisonWebsite.Services.Implementations;

namespace PriceComparisonWebsite.Tests.Services
{
    public class VendorServiceTests
    {
        readonly IVendorService _vendorService;
        public readonly Mock<IRepository<Vendor>> _vendorRepositoryMock;
        public readonly Mock<ILogger<VendorService>> _loggerMock;

        public VendorServiceTests(){
            _vendorRepositoryMock = new Mock<IRepository<Vendor>>();
            _loggerMock = new Mock<ILogger<VendorService>>();
            _vendorService = new VendorService(_vendorRepositoryMock.Object, _loggerMock.Object);
        }

        // ------------------------------------------------- Setup Pagination ------------------------------------------------
          [Fact]
        public void SetupPagination_WithLessThan12ProductAndPage1_ShouldReturnListOfAllProducts()
        {
            // Arrange
            var vendors = Enumerable.Range(1, 10)
                .Select(i => new Vendor { VendorId = i })
                .ToList();

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());

            // Act
            var pagedVendors = _vendorService.SetupPagination(vendors, 1, viewData);

            // Assert
            Assert.NotNull(pagedVendors);
            Assert.Equal(10, pagedVendors.Count);

            Assert.Equal(Enumerable.Range(1, 10), pagedVendors.Select(v => v.VendorId));
        }


        [Fact]
        public void SetupPagination_WithMoreThan12ProductsAndPage1_ShouldReturnListOfFirst12()
        {
            // Arrange
            var vendors = Enumerable.Range(1, 46)
                .Select(i => new Vendor { VendorId = i })
                .ToList();

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());

            // Act
            var pagedVendors = _vendorService.SetupPagination(vendors, 1, viewData);

            // Assert
            Assert.NotNull(pagedVendors);
            Assert.Equal(12, pagedVendors.Count);

            Assert.Equal(Enumerable.Range(1, 12), pagedVendors.Select(v => v.VendorId));
        }


        [Fact]
        public void SetupPagination_WithMoreThan12ProductsAndPage2_ShouldReturnListOfProductAfterTheFirst12()
        {
            // Arrange
            var vendors = Enumerable.Range(1, 46)
                .Select(i => new Vendor { VendorId = i })
                .ToList();

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());

            // Act
            var pagedVendors = _vendorService.SetupPagination(vendors, 4, viewData);

            // Assert
            Assert.NotNull(pagedVendors);
            Assert.Equal(10, pagedVendors.Count);

            Assert.Equal(Enumerable.Range(37, 10), pagedVendors.Select(v => v.VendorId));
        }


        [Fact]
        public void SetupPagination_ShouldSetupViewDataCorrectly()
        {
            // Arrange
            var vendors = Enumerable.Range(1, 46)
                .Select(i => new Vendor { VendorId = i })
                .ToList();

            var viewData = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());

            // Act
            var pagedVendors = _vendorService.SetupPagination(vendors, 2, viewData);

            // Assert
            Assert.Equal(2, viewData["PageNumber"]);
            Assert.Equal(4, viewData["TotalPages"]);

            Assert.NotNull(pagedVendors);
            Assert.Equal(12, pagedVendors.Count);

            Assert.Equal(Enumerable.Range(13, 12), pagedVendors.Select(v => v.VendorId));
        }
    }
}