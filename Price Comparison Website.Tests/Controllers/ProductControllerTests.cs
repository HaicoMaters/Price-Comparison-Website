
using Microsoft.Identity.Client;
using Moq.Protected;

namespace Price_Comparison_Website.Tests.Controllers
{
    public class ProductControllerTests
    {
        private readonly Mock<ICategoryService> _categoryServiceMock;
        private readonly Mock<IProductService> _productServiceMock;
        private readonly Mock<IVendorService> _vendorServiceMock;
        private readonly Mock<IPriceListingService> _priceListingServiceMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<ILogger<ProductController>> _loggerMock;
        private readonly ProductController _productController;

        public ProductControllerTests()
        {
            _categoryServiceMock = new Mock<ICategoryService>();
            _productServiceMock = new Mock<IProductService>();
            _vendorServiceMock = new Mock<IVendorService>();
            _priceListingServiceMock = new Mock<IPriceListingService>();
            _userServiceMock = new Mock<IUserService>();
            _loggerMock = new Mock<ILogger<ProductController>>();

            // Mocking UserManager<ApplicationUser>
            var store = new Mock<IUserStore<ApplicationUser>>();

            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                store.Object, null, null, null, null, null, null, null, null
            );

            _productController = new ProductController(_categoryServiceMock.Object, _productServiceMock.Object,
            _vendorServiceMock.Object, _userServiceMock.Object, _priceListingServiceMock.Object,
            _userManagerMock.Object, _loggerMock.Object);
        }

        // ---------------------------------------- Index ----------------------------------------------------------
        [Fact]
        public async Task Index_WithPageNumber_ShouldAskForCorrectPaginatedProducts(){
            // Arrange

            // Act

            // Assert
        }
        
        [Fact]
        public async Task Index_WithCategoryIdOf0_ShouldReturnViewWithAllPaginatedProducts(){

        }

        [Fact]
        public async Task Index_WithCategoryId_ShouldReturnViewWithAllPaginatedProductsWithThatCategoryId(){

        }

        [Fact]
        public async Task Index_WithSearchQuery_ShouldReturnViewWithAllPaginatedProductsContainingNameInSearchQuery(){

        }

        [Fact]
        public async Task Index_WithSearchQuery_ShouldReturnViewWithAllPaginatedProductsContainingDescriptionInSearchQuery(){

        }

        [Fact]
        public async Task Index_WithSearchQuery_ShouldReturnViewWithAllPaginatedProductsContainingNameOrDescriptionInSearchQuery(){

        }

        [Fact]
        public async Task Index_WithAuthenticatedUserWithWishlist_ShouldReturnViewWithWishlistItemsInViewBag(){

        }

        [Fact]
        public async Task Index_ShouldUpdateViewDataWithCategoryIdAndSearchQuery(){

        }

        [Fact]
        public async Task Index_WhenInvalidOperationExceptionOccurs_ShouldReturnBadRequest(){

        }

        [Fact]
        public async Task Index_WhenOtherExceptionOccurs_ShouldReturnErrorCode500(){

        }

        // --------------------------------------------------- Add Edit [HTTP GET] ------------------------------------------


        // -------------------------------------------------- Add Edit [HTTP POST] ------------------------------------------



        // -------------------------------------------------- Delete --------------------------------------------------------



        // -------------------------------------------------- ViewProduct ----------------------------------------------------


        // -------------------------------------------------- Update Wishlist ------------------------------------------------
    }
}