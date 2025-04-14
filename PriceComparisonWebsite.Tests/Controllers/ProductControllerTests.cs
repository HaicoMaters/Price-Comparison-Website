
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client;
using Moq.Protected;

namespace PriceComparisonWebsite.Tests.Controllers
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
        public async Task Index_WithAuthenticatedUserWithWishlist_ShouldReturnViewWithWishlistItemsInViewData()
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
                .ReturnsAsync(new List<Product> { products[0], products[4] });

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

        [Fact]
        public async Task AddEditGet_WhenIdIsZero_ShouldReturnAddView()
        {
            // Arrange
            var id = 0;

            // Act
            var result = await _productController.AddEdit(id) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Product>(result.Model);
            Assert.Equal("Add", result.ViewData["Operation"]);
        }

        [Fact]
        public async Task AddEditGet_WhenIdIsNotZero_ShouldReturnEditView()
        {
            // Arrange
            var id = 1;

            var product = new Product { ProductId = 1 };

            _productServiceMock.Setup(p => p.GetProductById(id, It.IsAny<QueryOptions<Product>>()))
                .ReturnsAsync(product);

            // Act
            var result = await _productController.AddEdit(id) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Product>(result.Model);
            Assert.Equal(product, result.Model);
            Assert.Equal("Edit", result.ViewData["Operation"]);
            _productServiceMock.Verify(p => p.GetProductById(id, It.IsAny<QueryOptions<Product>>()), Times.Once);
        }

        [Fact]
        public async Task AddEditGet_ShouldAddCategoriesToViewData()
        {
            // Arrange
            var id = 0;

            var categories = new List<Category>{
                new Category {CategoryId = 1},
                new Category {CategoryId = 2},
            };

            _categoryServiceMock.Setup(c => c.GetAllCategories())
                .ReturnsAsync(categories);

            // Act
            var result = await _productController.AddEdit(id) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Product>(result.Model);
            Assert.Equal("Add", result.ViewData["Operation"]);

            Assert.NotNull(result.ViewData["Categories"]);
            Assert.Equal(categories, (List<Category>)result.ViewData["Categories"]);
        }

        [Fact]
        public async Task AddEditGet_WhenEditProductDoesNotExist_ShouldReturnNotFound()
        {
            // Arrange
            var id = 1;

            var product = new Product { ProductId = 1 };

            _productServiceMock.Setup(p => p.GetProductById(id, It.IsAny<QueryOptions<Product>>()))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _productController.AddEdit(id) as NotFoundResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);

            _productServiceMock.Verify(p => p.GetProductById(id, It.IsAny<QueryOptions<Product>>()), Times.Once);
        }

        [Fact]
        public async Task AddEditGet_WhenExceptionOccurs_ShouldReturnErrorCode500()
        {
            // Arrange
            var id = 1;

            var product = new Product { ProductId = 1 };

            _productServiceMock.Setup(p => p.GetProductById(id, It.IsAny<QueryOptions<Product>>()))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _productController.AddEdit(id) as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);

            _productServiceMock.Verify(p => p.GetProductById(id, It.IsAny<QueryOptions<Product>>()), Times.Once);

        }

        // -------------------------------------------------- Add Edit [HTTP POST] ------------------------------------------
        [Fact]
        public async Task AddEditPost_WhenIdIs0_ShouldAddNewProduct()
        {
            // Arrange
            var product = new Product { ProductId = 0, CategoryId = 1 };

            // Act
            var result = await _productController.AddEdit(product);

            // Assert
            Assert.NotNull(result);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewProduct", redirectResult.ActionName);
            Assert.Equal(product.ProductId, redirectResult.RouteValues["id"]);

            _productServiceMock.Verify(p => p.AddProduct(product), Times.Once);
            _productServiceMock.Verify(p => p.UpdateProduct(product), Times.Never);
        }

        [Fact]
        public async Task AddEditPost_WhenIdIsNot0_ShouldEditExistingProduct()
        {
            // Arrange
            var product = new Product { ProductId = 2, CategoryId = 1 };

            _productServiceMock.Setup(p => p.UpdateProduct(product))
                .ReturnsAsync(product);

            // Act
            var result = await _productController.AddEdit(product);

            // Assert
            Assert.NotNull(result);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewProduct", redirectResult.ActionName);
            Assert.Equal(product.ProductId, redirectResult.RouteValues["id"]);

            _productServiceMock.Verify(p => p.AddProduct(product), Times.Never);
            _productServiceMock.Verify(p => p.UpdateProduct(product), Times.Once);
        }


        [Fact]
        public async Task AddEditPost_WhenModelStateIsInValid_ShouldReturnAddEditView()
        {
            // Arrange
            var product = new Product { ProductId = 0, CategoryId = 1 };

            var categories = new List<Category>{
                new Category {CategoryId = 1},
                new Category {CategoryId = 2},
            };

            _categoryServiceMock.Setup(c => c.GetAllCategories())
                .ReturnsAsync(categories);


            _productController.ModelState.AddModelError("Product", "Invalid Product");

            // Act
            var result = await _productController.AddEdit(product) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<Product>(result.Model);
            Assert.Equal("Add", result.ViewData["Operation"]);

            Assert.NotNull(result.ViewData["Categories"]);
            Assert.Equal(categories, (List<Category>)result.ViewData["Categories"]);

            _productServiceMock.Verify(p => p.AddProduct(product), Times.Never);
            _productServiceMock.Verify(p => p.UpdateProduct(product), Times.Never);

        }

        [Fact]
        public async Task AddEditPost_WhenInvalidOperationExceptionOccurs_ShouldReturnBadRequest()
        {
            // Arrange
            var product = new Product { ProductId = 0, CategoryId = 1 };

            _productServiceMock.Setup(p => p.AddProduct(product))
                .ThrowsAsync(new InvalidOperationException("Test Exception"));

            // Act
            var result = await _productController.AddEdit(product);

            // Assert
            Assert.NotNull(result);
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);

            _productServiceMock.Verify(p => p.AddProduct(product), Times.Once);
            _productServiceMock.Verify(p => p.UpdateProduct(product), Times.Never);
        }

        [Fact]
        public async Task AddEditPost_WhenOtherExceptionOccurs_ShouldReturnErrorCode500()
        {
            // Arrange
            var product = new Product { ProductId = 0, CategoryId = 1 };

            _productServiceMock.Setup(p => p.AddProduct(product))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _productController.AddEdit(product);

            // Assert
            Assert.NotNull(result);
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            _productServiceMock.Verify(p => p.AddProduct(product), Times.Once);
            _productServiceMock.Verify(p => p.UpdateProduct(product), Times.Never);
        }

        // -------------------------------------------------- Delete --------------------------------------------------------

        [Fact]
        public async Task Delete_WhenValidId_ShouldDeleteProductAndRedirectToIndexPage()
        {
            // Arrange
            int id = 1;

            // Act
            var result = await _productController.Delete(id);

            // Assert
            Assert.NotNull(result);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Product", redirectResult.ControllerName);
            _productServiceMock.Verify(p => p.DeleteProduct(id), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenInvalidOperationExceptionOccurs_ShouldReturnBadRequest()
        {
            // Arrange
            int id = 1;

            _productServiceMock.Setup(p => p.DeleteProduct(id))
                .ThrowsAsync(new InvalidOperationException("Test Exception"));

            // Act
            var result = await _productController.Delete(id);

            // Assert
            Assert.NotNull(result);
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _productServiceMock.Verify(p => p.DeleteProduct(id), Times.Once);
        }

        [Fact]
        public async Task Delete_OtherExceptionOccurs_ShouldReturnErrorCode500()
        {
            // Arrange
            int id = 1;

            _productServiceMock.Setup(p => p.DeleteProduct(id))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _productController.Delete(id);

            // Assert
            Assert.NotNull(result);
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            _productServiceMock.Verify(p => p.DeleteProduct(id), Times.Once);
        }


        // -------------------------------------------------- ViewProduct ----------------------------------------------------
        [Fact]
        public async Task ViewProduct_WhenIdIsNoneZero_ShouldReturnViewOfProductPage()
        {
            // Arrange
            var prodId = 1;
            var product = new Product { ProductId = prodId, Name = "Test" };

            _productServiceMock.Setup(p => p.GetProductById(prodId, It.IsAny<QueryOptions<Product>>()))
                .ReturnsAsync(product);

            //  Mock unauthenticated user
            var user = new ClaimsPrincipal(new ClaimsIdentity());  // No claims, unauthenticated
            _productController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _productController.ViewProduct(prodId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<Product>(result.Model);
            Assert.Equal(product, model);
            _productServiceMock.Verify(p => p.GetProductById(prodId, It.IsAny<QueryOptions<Product>>()), Times.Once);
        }

        [Fact]
        public async Task ViewProduct_WhenIdIsZero_ShouldRedirectToProductIndex()
        {
            // Arrange
            var prodId = 0;

            // Act
            var result = await _productController.ViewProduct(prodId);

            // Assert
            Assert.NotNull(result);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirectResult.ActionName);
            Assert.Equal("Product", redirectResult.ControllerName);

            _productServiceMock.Verify(p => p.GetProductById(It.IsAny<int>(), It.IsAny<QueryOptions<Product>>()), Times.Never);
        }

        [Fact]
        public async Task ViewProduct_WhenListingsExist_ShouldAddListingsToViewData()
        {
            // Arrange
            var prodId = 1;
            var product = new Product { ProductId = prodId, Name = "Test" };

            var listings = new List<PriceListing>{
                new PriceListing{PriceListingId = 4, ProductId = prodId, DiscountedPrice = 2.00m, Price = 2.00m},
                new PriceListing{PriceListingId = 1, ProductId = prodId, DiscountedPrice = 2.20m, Price = 2.50m},
                new PriceListing{PriceListingId = 2, ProductId = prodId, DiscountedPrice = 2.60m, Price = 2.80m},
            };

            _productServiceMock.Setup(p => p.GetProductById(prodId, It.IsAny<QueryOptions<Product>>()))
                .ReturnsAsync(product);

            _priceListingServiceMock.Setup(l => l.GetPriceListingsByProductId(prodId, It.IsAny<QueryOptions<PriceListing>>()))
                .ReturnsAsync(listings);
            
            //  Mock unauthenticated user
            var user = new ClaimsPrincipal(new ClaimsIdentity());  // No claims, unauthenticated
            _productController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            // Act
            var result = await _productController.ViewProduct(prodId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<Product>(result.Model);
            Assert.Equal(product, model);

            Assert.Equal(listings, result.ViewData["Listings"]);

            _productServiceMock.Verify(p => p.GetProductById(prodId, It.IsAny<QueryOptions<Product>>()), Times.Once);
            _priceListingServiceMock.Verify(l => l.GetPriceListingsByProductId(prodId, It.IsAny<QueryOptions<PriceListing>>()), Times.Once);
        }


        [Fact]
        public async Task ViewProduct_WhenInUserRole_ShouldUpdateAndCleanupViewingHistory()
        {
            // Arrange
            var userId = "Test User";
            var prodId = 1;
            var product = new Product { ProductId = prodId, Name = "Test" };
            
            _productServiceMock.Setup(p => p.GetProductById(prodId, It.IsAny<QueryOptions<Product>>()))
                .ReturnsAsync(product);

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
            var result = await _productController.ViewProduct(prodId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<Product>(result.Model);
            Assert.Equal(product, model);

            _productServiceMock.Verify(p => p.GetProductById(prodId, It.IsAny<QueryOptions<Product>>()), Times.Once);

            _userManagerMock.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
            _userManagerMock.Verify(um => um.IsInRoleAsync(user, "User"), Times.Once);
            _userServiceMock.Verify(u => u.UpdateViewingHistory(userId, prodId), Times.Once);
            _userServiceMock.Verify(u => u.CleanupViewingHistory(userId), Times.Once);
        }

        [Fact]
        public async Task ViewProduct_WhenInUserRoleAndProductOnWishlist_ShouldAddOnWishlistToViewDataAsTrue()
        {
            // Arrange
            var userId = "Test User";
            var prodId = 1;
            var product = new Product { ProductId = prodId, Name = "Test" };
            var wishlistItem = new UserWishList { UserId = userId, ProductId = prodId, LastCheapestPrice = 2.45m};
            
            _productServiceMock.Setup(p => p.GetProductById(prodId, It.IsAny<QueryOptions<Product>>()))
                .ReturnsAsync(product);

            _userServiceMock.Setup(u => u.GetUserWishListItemById(userId, prodId))
                .ReturnsAsync(wishlistItem);

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
            var result = await _productController.ViewProduct(prodId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<Product>(result.Model);
            Assert.Equal(product, model);

            Assert.NotNull(result.ViewData["OnWishlist"]);
            Assert.True((bool)result.ViewData["OnWishlist"]);

            _productServiceMock.Verify(p => p.GetProductById(prodId, It.IsAny<QueryOptions<Product>>()), Times.Once);
            _userManagerMock.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
            _userManagerMock.Verify(um => um.IsInRoleAsync(user, "User"), Times.Once);
            _userServiceMock.Verify(u => u.GetUserWishListItemById(userId, prodId), Times.Once);
        }

        [Fact]
        public async Task ViewProduct_WhenInUserRoleAndProductNotOnWishlist_ShouldAddOnWishlistToViewDataAsFalse()
        {
            // Arrange
            var userId = "Test User";
            var prodId = 1;
            var product = new Product { ProductId = prodId, Name = "Test" };
            var wishlistItem = new UserWishList { UserId = userId, ProductId = prodId, LastCheapestPrice = 2.45m};
            
            _productServiceMock.Setup(p => p.GetProductById(prodId, It.IsAny<QueryOptions<Product>>()))
                .ReturnsAsync(product);

            _userServiceMock.Setup(u => u.GetUserWishListItemById(userId, prodId))
                .ReturnsAsync((UserWishList)null);

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
            var result = await _productController.ViewProduct(prodId) as ViewResult;

            // Assert
            Assert.NotNull(result);
            var model = Assert.IsType<Product>(result.Model);
            Assert.Equal(product, model);

            Assert.NotNull(result.ViewData["OnWishlist"]);
            Assert.False((bool)result.ViewData["OnWishlist"]);

            _productServiceMock.Verify(p => p.GetProductById(prodId, It.IsAny<QueryOptions<Product>>()), Times.Once);
            _userManagerMock.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
            _userManagerMock.Verify(um => um.IsInRoleAsync(user, "User"), Times.Once);
            _userServiceMock.Verify(u => u.GetUserWishListItemById(userId, prodId), Times.Once);
        }

        [Fact]
        public async Task ViewProduct_WhenProductIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var prodId = 1;

            _productServiceMock.Setup(p => p.GetProductById(prodId, It.IsAny<QueryOptions<Product>>()))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _productController.ViewProduct(prodId);

            // Assert
            Assert.NotNull(result);
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);

            _productServiceMock.Verify(p => p.GetProductById(prodId, It.IsAny<QueryOptions<Product>>()), Times.Once);
        }

        [Fact]
        public async Task ViewProduct_WhenInvalidOperationExceptionOccurs_ShouldReturnBadRequest()
        {
            // Arrange
            var prodId = 1;

            _productServiceMock.Setup(p => p.GetProductById(prodId, It.IsAny<QueryOptions<Product>>()))
                .ThrowsAsync(new InvalidOperationException("Test Exception"));

            // Act
            var result = await _productController.ViewProduct(prodId);

            // Assert
            Assert.NotNull(result);
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);

            _productServiceMock.Verify(p => p.GetProductById(prodId, It.IsAny<QueryOptions<Product>>()), Times.Once);
        }

        [Fact]
        public async Task ViewProduct_WhenOtherxceptionOccurs_ShouldReturnErrorCode500()
        {
            // Arrange
            var prodId = 1;

            _productServiceMock.Setup(p => p.GetProductById(prodId, It.IsAny<QueryOptions<Product>>()))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _productController.ViewProduct(prodId);

            // Assert
            Assert.NotNull(result);
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);

            _productServiceMock.Verify(p => p.GetProductById(prodId, It.IsAny<QueryOptions<Product>>()), Times.Once);
        }


        // -------------------------------------------------- Update Wishlist ------------------------------------------------
        [Fact]
        public async Task UpdateWishlist_WhenSucceeds_ShouldReturnViewProductPage()
        {
            // Arrange
            var user = new ApplicationUser { Id = "Test" };
            int prodId = 1;

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _userServiceMock.Setup(u => u.UpdateUserWishlist(user.Id, prodId))
                .ReturnsAsync(true);

            // Act
            var result = await _productController.UpdateWishList(prodId);

            // Assert
            Assert.NotNull(result);
            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("ViewProduct", redirectResult.ActionName);
            Assert.Equal("Product", redirectResult.ControllerName);
            Assert.Equal(prodId, redirectResult.RouteValues["id"]);

            _userManagerMock.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
            _userServiceMock.Verify(u => u.UpdateUserWishlist(user.Id, prodId), Times.Once);

        }

        [Fact]
        public async Task UpdateWishlist_WhenUserIsNull_ShouldReturnUnauthorized()
        {
            // Arrange
            int prodId = 1;

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _productController.UpdateWishList(prodId);

            // Assert
            Assert.NotNull(result);
            var unauthorizedResult = Assert.IsType<UnauthorizedResult>(result);
            Assert.Equal(401, unauthorizedResult.StatusCode);
            _userManagerMock.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
            _userServiceMock.Verify(u => u.UpdateUserWishlist(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task UpdateWishlist_WhenResultIsNull_ShouldReturnNotFound()
        {
            // Arrange
            var user = new ApplicationUser { Id = "Test" };
            int prodId = 1;

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _userServiceMock.Setup(u => u.UpdateUserWishlist(user.Id, prodId))
                .ReturnsAsync((bool?)null);

            // Act
            var result = await _productController.UpdateWishList(prodId);

            // Assert
            Assert.NotNull(result);
            var notFoundResult = Assert.IsType<NotFoundResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
            _userManagerMock.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
            _userServiceMock.Verify(u => u.UpdateUserWishlist(user.Id, prodId), Times.Once);
        }

        [Fact]
        public async Task UpdateWishlist_WhenFails_ShouldReturnBadRequest()
        {
            // Arrange
            var user = new ApplicationUser { Id = "Test" };
            int prodId = 1;

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _userServiceMock.Setup(u => u.UpdateUserWishlist(user.Id, prodId))
                .ReturnsAsync(false);

            // Act
            var result = await _productController.UpdateWishList(prodId);

            // Assert
            Assert.NotNull(result);
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            _userManagerMock.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
            _userServiceMock.Verify(u => u.UpdateUserWishlist(user.Id, prodId), Times.Once);
        }

        [Fact]
        public async Task UpdateWishlist_WhenExceptionOccurs_ShouldReturnStatusCode500()
        {
            // Arrange
            var user = new ApplicationUser { Id = "Test" };
            int prodId = 1;

            _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            _userServiceMock.Setup(u => u.UpdateUserWishlist(user.Id, prodId))
                .ThrowsAsync(new Exception("Test Exception"));

            // Act
            var result = await _productController.UpdateWishList(prodId);

            // Assert
            Assert.NotNull(result);
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            _userManagerMock.Verify(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()), Times.Once);
            _userServiceMock.Verify(u => u.UpdateUserWishlist(user.Id, prodId), Times.Once);
        }
    }
}