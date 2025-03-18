using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Price_Comparison_Website.Data;
using Price_Comparison_Website.Models;
using Xunit;

namespace Price_Comparison_Website.Tests.Repositories
{
    public class RepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IRepository<Category> _categories; // Simplest db item so easiest to test

        // For their specific methods and tests requiring multiple ids
        private readonly IRepository<UserWishList> _userWishlist;
        private readonly IRepository<UserViewingHistory> _userViewingHistories;
        private readonly IRepository<Product> _products; // For testing Query Options (e.g. get sorted by cheapest price)


        public RepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _categories = new Repository<Category>(_context);
            _userWishlist = new Repository<UserWishList>(_context);
            _userViewingHistories = new Repository<UserViewingHistory>(_context);
            _products = new Repository<Product>(_context);
        }

        // ------------------------------------------------ Basic Method Tests ----------------------------------------------------------------------

        [Fact]
        public async Task AddAsync_ValidCategory_ShouldAddToDatabase()
        {
            // Arrange
            Category category = new Category { Name = "Test Category" };

            // Act
            await _categories.AddAsync(category);

            // Assert
            var result = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Test Category");
            Assert.NotNull(result);
            Assert.Equal("Test Category", result.Name);
        }

        [Fact]
        public async Task GetAllAsync_WithMultipleCategories_ShouldReturnAllCategories()
        {
            // Arrange
            var categories = new Category[] {new Category{Name = "Test1", CategoryId = 1}, 
            new Category{Name = "Test2", CategoryId = 2}, 
            new Category{Name = "Test3", CategoryId = 3}
            };

            foreach (var cat in categories){
                await _context.Categories.AddAsync(cat);
                await _context.SaveChangesAsync();
            }

            // Act
            IEnumerable<Category> dbCats = await _categories.GetAllAsync();
            List<Category> catList = [.. dbCats];

            // Assert
            Assert.Equal(3, catList.Count);

            for (int i = 0; i < 3; i ++){
                Assert.Equal(categories[i].Name, catList[i].Name);
                Assert.Equal(categories[i].CategoryId, catList[i].CategoryId);
            }
        }

        [Fact]
        public async Task GetByIdAsync_ExistingCategoryId_ShouldReturnCorrectCategory()
        {
            // Arrange
            var categories = new Category[] {new Category{Name = "Test1", CategoryId = 1}, 
            new Category{Name = "Test2", CategoryId = 2}, 
            new Category{Name = "Test3", CategoryId = 3}
            };

            foreach (var cat in categories){
                await _context.Categories.AddAsync(cat);
                await _context.SaveChangesAsync();
            }

            // Act
            Category dbCat = await _categories.GetByIdAsync(2, new QueryOptions<Category>());

            // Assert
            Assert.Equal("Test2", dbCat.Name);
            Assert.Equal(2, dbCat.CategoryId);
        }

        [Fact]
        public async Task DeleteAsync_UsingId_ShouldRemoveCategory()
        {
            // Arrange
            Category category = new Category{Name = "Test Category", CategoryId = 1};

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var result = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Test Category");
            Assert.NotNull(result);
            Assert.Equal("Test Category", result.Name);

            // Act
            await _categories.DeleteAsync(1); // Delete by id for this test

            // Assert
            result = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Test Category");
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_UsingEntity_ShouldRemoveCategory()
        {
            // Arrange
            Category category = new Category{Name = "Test Category", CategoryId = 1};

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var result = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Test Category");
            Assert.NotNull(result);
            Assert.Equal("Test Category", result.Name);

            // Act
            await _categories.DeleteAsync(result); // Delete by entity for this test

            // Assert
            result = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Test Category");
            Assert.Null(result);
        }
        [Fact]
        public async Task UpdateAsync_ExistingCategory_ShouldUpdateName()
        {
            // Arrange
            Category category = new Category{Name = "Unchanged", CategoryId = 1};

            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var result = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Unchanged");
            Assert.NotNull(result);
            Assert.Equal("Unchanged", result.Name);

            // Act
            result.Name = "Changed";
            await _categories.UpdateAsync(result);

            // Assert
            result = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Unchanged");
            Assert.Null(result);

            result = await _context.Categories.FirstOrDefaultAsync(c => c.Name == "Changed");
            Assert.NotNull(result);
            Assert.Equal("Changed", result.Name);
        }

        [Fact]
        public async Task GetAllByIdAsync_ExistingProductId_ShouldReturnMatchingWishlists()
        {
            // Arrange
            var userWishListItems = new UserWishList[]
            {
                new UserWishList { UserId = "User1", ProductId = 1 },
                new UserWishList { UserId = "User2", ProductId = 1 },
                new UserWishList { UserId = "User3", ProductId = 2 }
            };

            foreach (var item in userWishListItems)
            {
                await _context.UserWishLists.AddAsync(item);
                await _context.SaveChangesAsync();
            }

            // Act
            IEnumerable<UserWishList> dbUserWishLists = await _userWishlist.GetAllByIdAsync(1, "ProductId", new QueryOptions<UserWishList>());
            List<UserWishList> userWishList = [.. dbUserWishLists];

            // Assert
            Assert.Equal(2, userWishList.Count);

            foreach (var item in userWishList)
            {
                Assert.Equal(1, item.ProductId);
            }
        }

        [Fact]
        public async Task GetAllByIdAsync_ExistingUserId_ShouldReturnMatchingWishlists()
        {
            // Arrange
            var userWishListItems = new UserWishList[]
            {
                new UserWishList { UserId = "User1", ProductId = 1 },
                new UserWishList { UserId = "User2", ProductId = 1 },
                new UserWishList { UserId = "User3", ProductId = 2 },
                new UserWishList { UserId = "User2", ProductId = 2}
            };

            foreach (var item in userWishListItems)
            {
                await _context.UserWishLists.AddAsync(item);
                await _context.SaveChangesAsync();
            }

            // Act
            IEnumerable<UserWishList> dbUserWishLists = await _userWishlist.GetAllByIdAsync("User2", "UserId", new QueryOptions<UserWishList>());
            List<UserWishList> userWishList = [.. dbUserWishLists];

            // Assert
            Assert.Equal(2, userWishList.Count);

            foreach (var item in userWishList)
            {
                Assert.Equal("User2", item.UserId);
            }
        }

        [Fact]
        public async Task GetByIdAsync_ExistingUserIdAndProductId_ShouldReturnCorrectWishlistItem()
        {
            // Arrange
            var userWishListItems = new UserWishList[]
            {
                new UserWishList { UserId = "User1", ProductId = 1 },
                new UserWishList { UserId = "User2", ProductId = 1 },
                new UserWishList { UserId = "User3", ProductId = 2 },
                new UserWishList { UserId = "User2", ProductId = 2}
            };

            foreach (var item in userWishListItems)
            {
                await _context.UserWishLists.AddAsync(item);
                await _context.SaveChangesAsync();
            }

            // Act
            UserWishList item1 = await _userWishlist.GetByIdAsync("User2", 1 , new QueryOptions<UserWishList>());
            UserWishList item2 = await _userWishlist.GetByIdAsync("User2", 2, new QueryOptions<UserWishList>());
            UserWishList item3 = await _userWishlist.GetByIdAsync("User1", 1, new QueryOptions<UserWishList>());

            // Assert
            Assert.NotNull(item1);
            Assert.NotNull(item2);
            Assert.NotNull(item3);
            Assert.Equal("User2", item1.UserId);
            Assert.Equal("User2", item2.UserId);
            Assert.Equal("User1", item3.UserId);
            Assert.Equal(1, item1.ProductId);
            Assert.Equal(2, item2.ProductId);
            Assert.Equal(1, item3.ProductId);
        }

        [Fact]
        public async Task GetByIdAsync_ExistingUserIdAndProductId_ShouldReturnCorrectViewingHistoryItem()
        {
            // Arrange
            var userViewingHistoryItems = new UserViewingHistory[]
            {
                new UserViewingHistory { UserId = "User1", ProductId = 1 },
                new UserViewingHistory { UserId = "User2", ProductId = 1 },
                new UserViewingHistory { UserId = "User3", ProductId = 2 },
                new UserViewingHistory { UserId = "User2", ProductId = 2}
            };

            foreach (var item in userViewingHistoryItems)
            {
                await _context.UserViewingHistories.AddAsync(item);
                await _context.SaveChangesAsync();
            }

            // Act
            UserViewingHistory item1 = await _userViewingHistories.GetByIdAsync("User2", 1 , new QueryOptions<UserViewingHistory>());
            UserViewingHistory item2 = await _userViewingHistories.GetByIdAsync("User2", 2, new QueryOptions<UserViewingHistory>());
            UserViewingHistory item3 = await _userViewingHistories.GetByIdAsync("User1", 1, new QueryOptions<UserViewingHistory>());

            // Assert
            Assert.NotNull(item1);
            Assert.NotNull(item2);
            Assert.NotNull(item3);
            Assert.Equal("User2", item1.UserId);
            Assert.Equal("User2", item2.UserId);
            Assert.Equal("User1", item3.UserId);
            Assert.Equal(1, item1.ProductId);
            Assert.Equal(2, item2.ProductId);
            Assert.Equal(1, item3.ProductId);
        }

        // -------------------------------------------------- Query Options Tests --------------------------------------------------------
        [Fact]
        public async Task GetAllByIdAsync_WhenOrderedByCheapestPrice_ShouldReturnProductsInCategoryInPriceOrder()
        {
            // Arrange
            var products = new Product[]
            {
                new Product { Name = "Product1", CheapestPrice = 10, CategoryId = 1 },
                new Product { Name = "Product2", CheapestPrice = 5, CategoryId = 1 },
                new Product { Name = "Product3", CheapestPrice = 15, CategoryId = 1 }
            };

            foreach (var product in products)
            {
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
            }

            var queryOptions = new QueryOptions<Product>
            {
                OrderBy = p => p.CheapestPrice
            };

            // Act
            IEnumerable<Product> dbProducts = await _products.GetAllByIdAsync(1, "CategoryId", queryOptions);
            List<Product> productList = dbProducts.ToList();

            // Assert
            Assert.Equal(3, productList.Count);
            Assert.Equal("Product2", productList[0].Name);
            Assert.Equal("Product1", productList[1].Name);
            Assert.Equal("Product3", productList[2].Name);
        }

        [Fact]
        public async Task GetAllByIdAsync_WhenOrderedByCheapestPriceDescending_ShouldReturnProductsInReverseOrder()
        {
            // Arrange
            var products = new Product[]
            {
                new Product { Name = "Product1", CheapestPrice = 10, CategoryId = 1 },
                new Product { Name = "Product2", CheapestPrice = 5, CategoryId = 1 },
                new Product { Name = "Product3", CheapestPrice = 15, CategoryId = 1 }
            };

            foreach (var product in products)
            {
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
            }

            var queryOptions = new QueryOptions<Product>
            {
                OrderBy = p => -p.CheapestPrice // Negative for descending order
            };

            // Act
            IEnumerable<Product> dbProducts = await _products.GetAllByIdAsync(1, "CategoryId", queryOptions);
            List<Product> productList = dbProducts.ToList();

            // Assert
            Assert.Equal(3, productList.Count);
            Assert.Equal("Product3", productList[0].Name);
            Assert.Equal("Product1", productList[1].Name);
            Assert.Equal("Product2", productList[2].Name);
        }

        [Fact]
        public async Task GetAllByIdAsync_WithPriceFilter_ShouldReturnFilteredProducts()
        {
            // Arrange
            var products = new Product[]
            {
                new Product { Name = "Product1", CheapestPrice = 10, CategoryId = 1 },
                new Product { Name = "Product2", CheapestPrice = 5, CategoryId = 1 },
                new Product { Name = "Product3", CheapestPrice = 15, CategoryId = 1 }
            };

            foreach (var product in products)
            {
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
            }

            var queryOptions = new QueryOptions<Product>
            {
                Where = p => p.CheapestPrice <= 10
            };

            // Act
            IEnumerable<Product> dbProducts = await _products.GetAllByIdAsync(1, "CategoryId", queryOptions);
            List<Product> productList = dbProducts.ToList();

            // Assert
            Assert.Equal(2, productList.Count);
            Assert.Contains(productList, p => p.Name == "Product1");
            Assert.Contains(productList, p => p.Name == "Product2");
            Assert.DoesNotContain(productList, p => p.Name == "Product3");
        }

        [Fact]
        public async Task GetAllByIdAsync_WithFilterAndOrder_ShouldReturnFilteredOrderedProducts()
        {
            // Arrange
            var products = new Product[]
            {
                new Product { Name = "Product1", CheapestPrice = 10, CategoryId = 1 },
                new Product { Name = "Product2", CheapestPrice = 5, CategoryId = 1 },
                new Product { Name = "Product3", CheapestPrice = 15, CategoryId = 1 },
                new Product { Name = "Product4", CheapestPrice = 8, CategoryId = 1 }
            };

            foreach (var product in products)
            {
                await _context.Products.AddAsync(product);
                await _context.SaveChangesAsync();
            }

            var queryOptions = new QueryOptions<Product>
            {
                Where = p => p.CheapestPrice <= 10,
                OrderBy = p => p.CheapestPrice
            };

            // Act
            IEnumerable<Product> dbProducts = await _products.GetAllByIdAsync(1, "CategoryId", queryOptions);
            List<Product> productList = dbProducts.ToList();

            // Assert
            Assert.Equal(3, productList.Count);
            Assert.Equal("Product2", productList[0].Name); // 5
            Assert.Equal("Product4", productList[1].Name); // 8
            Assert.Equal("Product1", productList[2].Name); // 10
        }

        [Fact]
        public async Task GetByIdAsync_WithIncludes_ShouldReturnProductWithCategory()
        {
            // Arrange
            var category = new Category { Name = "Electronics", CategoryId = 1 };
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            var product = new Product 
            { 
                Name = "Laptop", 
                CheapestPrice = 1000, 
                CategoryId = 1 
            };
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            var queryOptions = new QueryOptions<Product>
            {
                Includes = "Category"
            };

            // Act
            var result = await _products.GetByIdAsync(product.ProductId, queryOptions);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Category);
            Assert.Equal("Electronics", result.Category.Name);
        }

        // -------------------------------------------- Invalid Test Cases ---------------------------------------------------------------------------------
        [Fact]
        public async Task GetByIdAsync_NonExistentId_ShouldReturnNull()
        {
            // Act
            var result = await _categories.GetByIdAsync(999, new QueryOptions<Category>());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteAsync_NonExistentId_ShouldThrowInvalidOperationException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                async () => await _categories.DeleteAsync(999));
        }

        [Fact]
        public async Task DeleteAsync_NullEntity_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await _categories.DeleteAsync(null));
        }

        [Fact]
        public async Task UpdateAsync_NonExistentEntity_ShouldThrowDbUpdateConcurrencyException()
        {
            // Arrange
            var nonExistentCategory = new Category { CategoryId = 999, Name = "Non-existent" };

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
                async () => await _categories.UpdateAsync(nonExistentCategory));
        }

        [Fact]
        public async Task GetByIdAsync_InvalidUserIdOrProductId_ShouldReturnNull()
        {
            // Arrange
            var wishListItem = new UserWishList { UserId = "User1", ProductId = 1 };
            await _context.UserWishLists.AddAsync(wishListItem);
            await _context.SaveChangesAsync();

            // Act
            var result1 = await _userWishlist.GetByIdAsync("NonExistentUser", 1, new QueryOptions<UserWishList>());
            var result2 = await _userWishlist.GetByIdAsync("User1", 999, new QueryOptions<UserWishList>());

            // Assert
            Assert.Null(result1);
            Assert.Null(result2);
        }

        [Fact]
        public async Task AddAsync_NullEntity_ShouldThrowArgumentNullException()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await _categories.AddAsync(null));
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
