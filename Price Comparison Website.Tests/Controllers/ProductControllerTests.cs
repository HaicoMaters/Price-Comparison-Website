
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
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
        public async Task Index_WithPageNumber_ShouldAskForCorrectPaginatedProducts()
        {
            // Arrange
            var products = new Product[]{
                new Product{ProductId = 1, CategoryId = 2, Name = "Test", Description = "Test"}
            };

            _productServiceMock.Setup(ps => ps.GetAllProducts()).ReturnsAsync(products);

            // Act
            await _productController.Index(pageNumber: 2);

            // Assert
            _productServiceMock.Verify(p => p.SetupPagination(It.IsAny<IEnumerable<Product>>(), 2, It.IsAny<ViewDataDictionary>()), Times.Once);
        }

        [Fact]
        public async Task Index_WithCategoryIdOf0_ShouldReturnViewWithAllPaginatedProducts()
        {
            // Arrange
            var products = new List<Product>{
                new Product{ProductId = 1, CategoryId = 2, Name = "Test", Description = "Test"},
                new Product{ProductId = 2, CategoryId = 3, Name = "Test", Description = "Test"},
                new Product{ProductId = 3, CategoryId = 1, Name = "Test", Description = "Test"},
                new Product{ProductId = 4, CategoryId = 4, Name = "Test", Description = "Test"},
                new Product{ProductId = 5, CategoryId = 2, Name = "Test", Description = "Test"}
            };

            _productServiceMock.Setup(ps => ps.GetAllProducts())
                .ReturnsAsync(products);

            _productServiceMock.Setup(ps => ps.SetupPagination(It.IsAny<IEnumerable<Product>>(), 1, It.IsAny<ViewDataDictionary>()))
                .Returns(products);

            //  Mock unauthenticated user
            var user = new ClaimsPrincipal(new ClaimsIdentity());  // No claims, unauthenticated
            _productController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _productController.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<List<Product>>(result.Model);
            Assert.Equal(products, model);

            _productServiceMock.Verify(p => p.SetupPagination(It.IsAny<IEnumerable<Product>>(), 1, It.IsAny<ViewDataDictionary>()), Times.Once);
            _productServiceMock.Verify(p => p.GetAllProducts(), Times.Once);
        }

        [Fact]
        public async Task Index_WithCategoryId_ShouldReturnViewWithAllPaginatedProductsWithThatCategoryId()
        {
            // Arrange
            var products = new List<Product>{
                new Product{ProductId = 1, CategoryId = 2, Name = "Test", Description = "Test"},
                new Product{ProductId = 2, CategoryId = 3, Name = "Test", Description = "Test"},
                new Product{ProductId = 3, CategoryId = 1, Name = "Test", Description = "Test"},
                new Product{ProductId = 4, CategoryId = 4, Name = "Test", Description = "Test"},
                new Product{ProductId = 5, CategoryId = 2, Name = "Test", Description = "Test"}
            };

            _productServiceMock.Setup(ps => ps.GetProductsByCategoryId(2, It.IsAny<QueryOptions<Product>>()))
                .ReturnsAsync(new List<Product> { products[0], products[4] });

            _productServiceMock.Setup(ps => ps.SetupPagination(It.IsAny<IEnumerable<Product>>(), 1, It.IsAny<ViewDataDictionary>()))
                .Returns(new List<Product> { products[0], products[4] });

            //  Mock unauthenticated user
            var user = new ClaimsPrincipal(new ClaimsIdentity());  // No claims, unauthenticated
            _productController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _productController.Index(catId: 2) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<List<Product>>(result.Model);
            Assert.Equal(2, model.Count);
            Assert.Equal(products[0], model[0]);
            Assert.Equal(products[4], model[1]);

            _productServiceMock.Verify(p => p.SetupPagination(It.IsAny<IEnumerable<Product>>(), 1, It.IsAny<ViewDataDictionary>()), Times.Once);
            _productServiceMock.Verify(p => p.GetProductsByCategoryId(2, It.IsAny<QueryOptions<Product>>()), Times.Once);
        }

        [Fact]
        public async Task Index_WithSearchQuery_ShouldReturnViewWithAllPaginatedProductsContainingNameInSearchQuery()
        {
            // Arrange
            var products = new List<Product>{
                new Product{ProductId = 1, CategoryId = 2, Name = "Test 1", Description = "NOTHINGINCOMMON"},
                new Product{ProductId = 2, CategoryId = 3, Name = "Test", Description = "NOTHINGINCOMMON"},
                new Product{ProductId = 3, CategoryId = 1, Name = "Cest", Description = "NOTHINGINCOMMON"},
                new Product{ProductId = 4, CategoryId = 4, Name = "Test 4", Description = "NOTHINGINCOMMON"},
                new Product{ProductId = 5, CategoryId = 2, Name = "Sest", Description = "NOTHINGINCOMMON"}
            };

            _productServiceMock.Setup(ps => ps.GetAllProducts())
                .ReturnsAsync(products);

            _productServiceMock.Setup(ps => ps.SetupPagination(It.IsAny<IEnumerable<Product>>(), 1, It.IsAny<ViewDataDictionary>()))
                .Returns((IEnumerable<Product> input, int _, ViewDataDictionary _) => input.ToList());

            //  Mock unauthenticated user
            var user = new ClaimsPrincipal(new ClaimsIdentity());  // No claims, unauthenticated
            _productController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _productController.Index(searchQuery: "Test") as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<List<Product>>(result.Model);
            Assert.Equal(3, model.Count);

            Assert.Equal("Test 1", model[0].Name);
            Assert.Equal("Test", model[1].Name);
            Assert.Equal("Test 4", model[2].Name);
            Assert.Equal(1, model[0].ProductId);
            Assert.Equal(2, model[1].ProductId);
            Assert.Equal(4, model[2].ProductId);

            _productServiceMock.Verify(p => p.SetupPagination(It.IsAny<IEnumerable<Product>>(), 1, It.IsAny<ViewDataDictionary>()), Times.Once);
            _productServiceMock.Verify(p => p.GetAllProducts(), Times.Once);
        }

        [Fact]
        public async Task Index_WithSearchQuery_ShouldReturnViewWithAllPaginatedProductsContainingDescriptionInSearchQuery()
        {
            // Arrange
            var products = new List<Product>{
                new Product{ProductId = 1, CategoryId = 2, Description = "Test 1", Name = "NOTHINGINCOMMON"},
                new Product{ProductId = 2, CategoryId = 3, Description = "Test", Name = "NOTHINGINCOMMON"},
                new Product{ProductId = 3, CategoryId = 1, Description = "Cest", Name = "NOTHINGINCOMMON"},
                new Product{ProductId = 4, CategoryId = 4, Description = "Test 4", Name = "NOTHINGINCOMMON"},
                new Product{ProductId = 5, CategoryId = 2, Description = "Sest", Name = "NOTHINGINCOMMON"}
            };

            _productServiceMock.Setup(ps => ps.GetAllProducts())
                .ReturnsAsync(products);

            _productServiceMock.Setup(ps => ps.SetupPagination(It.IsAny<IEnumerable<Product>>(), 1, It.IsAny<ViewDataDictionary>()))
                .Returns((IEnumerable<Product> input, int _, ViewDataDictionary _) => input.ToList());

            //  Mock unauthenticated user
            var user = new ClaimsPrincipal(new ClaimsIdentity());  // No claims, unauthenticated
            _productController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _productController.Index(searchQuery: "Test") as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<List<Product>>(result.Model);
            Assert.Equal(3, model.Count);

            Assert.Equal("Test 1", model[0].Description);
            Assert.Equal("Test", model[1].Description);
            Assert.Equal("Test 4", model[2].Description);
            Assert.Equal(1, model[0].ProductId);
            Assert.Equal(2, model[1].ProductId);
            Assert.Equal(4, model[2].ProductId);

            _productServiceMock.Verify(p => p.SetupPagination(It.IsAny<IEnumerable<Product>>(), 1, It.IsAny<ViewDataDictionary>()), Times.Once);
            _productServiceMock.Verify(p => p.GetAllProducts(), Times.Once);
        }

        [Fact]
        public async Task Index_WithSearchQuery_ShouldReturnViewWithAllPaginatedProductsContainingNameOrDescriptionInSearchQuery()
        {
            // Arrange
            var products = new List<Product>{
                new Product{ProductId = 1, CategoryId = 2, Name = "Test 1", Description = "NOTHINGINCOMMON"},
                new Product{ProductId = 2, CategoryId = 3, Name = "NOTHINGINCOMMON", Description = "Test"},
                new Product{ProductId = 3, CategoryId = 1, Name = "NOTHINGINCOMMON", Description = "NOTHINGINCOMMON"},
                new Product{ProductId = 4, CategoryId = 4, Name = "Test 4", Description = "NOTHINGINCOMMON"},
                new Product{ProductId = 5, CategoryId = 2, Name = "NOTHINGINCOMMON", Description = "NOTHINGINCOMMON"}
            };

            _productServiceMock.Setup(ps => ps.GetAllProducts())
                .ReturnsAsync(products);

            _productServiceMock.Setup(ps => ps.SetupPagination(It.IsAny<IEnumerable<Product>>(), 1, It.IsAny<ViewDataDictionary>()))
                .Returns((IEnumerable<Product> input, int _, ViewDataDictionary _) => input.ToList());

            //  Mock unauthenticated user
            var user = new ClaimsPrincipal(new ClaimsIdentity());  // No claims, unauthenticated
            _productController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _productController.Index(searchQuery: "Test") as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<List<Product>>(result.Model);
            Assert.Equal(3, model.Count);

            Assert.Equal("Test 1", model[0].Name);
            Assert.Equal("Test", model[1].Description);
            Assert.Equal("Test 4", model[2].Name);
            Assert.Equal(1, model[0].ProductId);
            Assert.Equal(2, model[1].ProductId);
            Assert.Equal(4, model[2].ProductId);

            _productServiceMock.Verify(p => p.SetupPagination(It.IsAny<IEnumerable<Product>>(), 1, It.IsAny<ViewDataDictionary>()), Times.Once);
            _productServiceMock.Verify(p => p.GetAllProducts(), Times.Once);
        }

        [Fact]
        public async Task Index_WithAuthenticatedUserWithWishlist_ShouldReturnViewWithWishlistItemsInViewBag()
        {
            // Arrange
            var products = new List<Product>{
                new Product{ProductId = 1, CategoryId = 2, Name = "Test", Description = "Test"},
                new Product{ProductId = 2, CategoryId = 3, Name = "Test", Description = "Test"},
                new Product{ProductId = 3, CategoryId = 1, Name = "Test", Description = "Test"},
                new Product{ProductId = 4, CategoryId = 4, Name = "Test", Description = "Test"},
                new Product{ProductId = 5, CategoryId = 2, Name = "Test", Description = "Test"}
            };

            var userId = "Test User";

            _productServiceMock.Setup(ps => ps.GetAllProducts())
                .ReturnsAsync(products);

            _productServiceMock.Setup(ps => ps.SetupPagination(It.IsAny<IEnumerable<Product>>(), 1, It.IsAny<ViewDataDictionary>()))
                .Returns(products);

            _userServiceMock.Setup(us => us.GetUserWishListItemById(userId, 2)) // Product 2 and 5 simulated as on wishlisted
                .ReturnsAsync(new UserWishList { UserId = userId, ProductId = 2 });

            _userServiceMock.Setup(us => us.GetUserWishListItemById(userId, 5))
                .ReturnsAsync(new UserWishList { UserId = userId, ProductId = 5 });

            // Mock UserManager and authentication for authenticatedUser
            var user = new ApplicationUser { Id = userId, UserName = "testuser" };

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "testuser"),
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Role, "User")
                };

            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _userManagerMock.Setup(um => um.IsInRoleAsync(user, "User"))
                .ReturnsAsync(true);

            _productController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await _productController.Index() as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<List<Product>>(result.Model);
            Assert.Equal(5, model.Count);

            for (int i = 0; i < products.Count; i++)
            { // Whatever is in/not in wishlist
                if (i == 1 || i == 4)
                {
                    Assert.True(((List<bool>)result.ViewData["OnWishlist"])[i]);
                }
                else
                {
                    Assert.False(((List<bool>)result.ViewData["OnWishlist"])[i]);
                }
            }

            _productServiceMock.Verify(p => p.SetupPagination(It.IsAny<IEnumerable<Product>>(), 1, It.IsAny<ViewDataDictionary>()), Times.Once);
            _productServiceMock.Verify(p => p.GetAllProducts(), Times.Once);
            _userManagerMock.Verify(um => um.GetUserAsync(claimsPrincipal), Times.Once);
            _userManagerMock.Verify(um => um.IsInRoleAsync(It.IsAny<ApplicationUser>(), "User"), Times.Once);
            _userServiceMock.Verify(us => us.GetUserWishListItemById(userId, 5), Times.Once);
            _userServiceMock.Verify(us => us.GetUserWishListItemById(userId, 2), Times.Once);
            _userServiceMock.Verify(us => us.GetUserWishListItemById(userId, It.IsAny<int>()), Times.Exactly(5));
        }

        [Fact]
        public async Task Index_ShouldUpdateViewDataWithCategoryIdAndSearchQuery()
        {
            // Arrange
            var products = new List<Product>{
                new Product{ProductId = 1, CategoryId = 2, Name = "Test 1", Description = "NOTHINGINCOMMON"},
                new Product{ProductId = 2, CategoryId = 3, Name = "Test", Description = "NOTHINGINCOMMON"},
                new Product{ProductId = 3, CategoryId = 1, Name = "Cest", Description = "NOTHINGINCOMMON"},
                new Product{ProductId = 4, CategoryId = 4, Name = "Test 4", Description = "NOTHINGINCOMMON"},
                new Product{ProductId = 5, CategoryId = 2, Name = "Sest", Description = "NOTHINGINCOMMON"}
            };

            int categoryId = 2;
            string search = "Test"; // cat id of 2 and searchQuery of "Test" expect only product (id = 1) at the end

            _productServiceMock.Setup(ps => ps.GetProductsByCategoryId(categoryId, It.IsAny<QueryOptions<Product>>()))
                .ReturnsAsync(new List<Product> { products[0], products[4]});

            _productServiceMock.Setup(ps => ps.SetupPagination(It.IsAny<IEnumerable<Product>>(), 1, It.IsAny<ViewDataDictionary>()))
                .Returns((IEnumerable<Product> input, int _, ViewDataDictionary _) => input.ToList());

            //  Mock unauthenticated user
            var user = new ClaimsPrincipal(new ClaimsIdentity());  // No claims, unauthenticated
            _productController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _productController.Index(searchQuery: search, catId: categoryId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<List<Product>>(result.Model);
            Assert.Equal(1, model.Count);
            Assert.Equal("Test 1", model[0].Name);
            Assert.Equal(1, model[0].ProductId);

            Assert.Equal(categoryId, result.ViewData["CategoryId"]);
            Assert.Equal(search, result.ViewData["SearchQuery"]);

            _productServiceMock.Verify(p => p.SetupPagination(It.IsAny<IEnumerable<Product>>(), 1, It.IsAny<ViewDataDictionary>()), Times.Once);
            _productServiceMock.Verify(p => p.GetProductsByCategoryId(categoryId, It.IsAny<QueryOptions<Product>>()), Times.Once);
        }

        [Fact]
        public async Task Index_WhenInvalidOperationExceptionOccurs_ShouldReturnBadRequest()
        {
            // Arrange
            _productServiceMock.Setup(p => p.GetAllProducts())
                .ThrowsAsync(new InvalidOperationException("Test Exception"));

            // Act
            var result = await _productController.Index();

            // Assert
            Assert.NotNull(result);
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            
            _categoryServiceMock.Verify(cs => cs.GetAllCategories(), Times.Once);
            _productServiceMock.Verify(p => p.GetAllProducts(), Times.Once);
            _productServiceMock.Verify(p => p.SetupPagination(It.IsAny<IEnumerable<Product>>(), It.IsAny<int>(), It.IsAny<ViewDataDictionary>()), Times.Never);
        }

        [Fact]
        public async Task Index_WhenOtherExceptionOccurs_ShouldReturnErrorCode500()
        {
            // Arrange
            _categoryServiceMock.Setup(cs => cs.GetAllCategories())
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _productController.Index() as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);

            _categoryServiceMock.Verify(cs => cs.GetAllCategories(), Times.Once);
            _productServiceMock.Verify(p => p.GetAllProducts(), Times.Never);
            _productServiceMock.Verify(p => p.SetupPagination(It.IsAny<IEnumerable<Product>>(), It.IsAny<int>(), It.IsAny<ViewDataDictionary>()), Times.Never);

        }

        // --------------------------------------------------- Add Edit [HTTP GET] ------------------------------------------


        // -------------------------------------------------- Add Edit [HTTP POST] ------------------------------------------



        // -------------------------------------------------- Delete --------------------------------------------------------



        // -------------------------------------------------- ViewProduct ----------------------------------------------------


        // -------------------------------------------------- Update Wishlist ------------------------------------------------
    }
}