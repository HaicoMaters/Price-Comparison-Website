using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using NuGet.Packaging.Signing;
using Price_Comparison_Website.Services.Implementations;
using Price_Comparison_Website.Services.Interfaces;

namespace Price_Comparison_Website.Tests.Services
{
    public class UserServiceTests
    {
        private readonly IUserService _userService;
        private readonly Mock<IRepository<Product>> _productRepoMock;
        private readonly Mock<IRepository<UserWishList>> _userWishlistRepoMock;
        private readonly Mock<IRepository<UserViewingHistory>> _userViewingHistoryRepoMock;
        private readonly Mock<ILogger<UserService>> _loggerMock;

        public UserServiceTests(){
            _productRepoMock = new Mock<IRepository<Product>>();
            _userWishlistRepoMock = new Mock<IRepository<UserWishList>>();
            _userViewingHistoryRepoMock = new Mock<IRepository<UserViewingHistory>>();
            _loggerMock = new Mock<ILogger<UserService>>();

            _userService = new UserService(_productRepoMock.Object, _userViewingHistoryRepoMock.Object, _userWishlistRepoMock.Object, _loggerMock.Object);
        }

        // ----------------------------------------------------- Get User Viewing History ------------------------------------------------------

        [Fact]
        public async Task GetUserViewingHistory_WithValidHistory_ShouldBeInDescendingOrderOfLastViewed(){
            // Arrange
            var viewingHistories = new UserViewingHistory[]{
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now, ProductId = 1}, // Expected order 1432
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-45), ProductId = 2},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-30), ProductId = 3},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-5), ProductId = 4}
            };

            _userViewingHistoryRepoMock.Setup(r => r.GetAllByIdAsync("Test", "UserId", It.IsAny<QueryOptions<UserViewingHistory>>()))
            .ReturnsAsync(viewingHistories);

            // Act
            var historyItems = await _userService.GetUserViewingHistory("Test");
            List<UserViewingHistory> historyList = historyItems.ToList();

            // Assert
            Assert.Equal(historyList.Count, viewingHistories.Length);
            Assert.Equal(viewingHistories[0].ProductId, historyList[0].ProductId);
            Assert.Equal(viewingHistories[3].ProductId, historyList[1].ProductId);
            Assert.Equal(viewingHistories[2].ProductId, historyList[2].ProductId);
            Assert.Equal(viewingHistories[1].ProductId, historyList[3].ProductId);

        }

        // ----------------------------------------------------- Cleanup Viewing History ------------------------------------------------------
        [Fact]
        public async Task CleanupViewingHistory_WithMoreThan20_ShouldRemoveUntil20MostRecentLeft(){
            // Arrange
            var viewingHistories = new UserViewingHistory[]{ // Pre-Ordered by Decending here
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now, ProductId = 1},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-1), ProductId = 2}, // Expected for delete async to be called 5 times
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-2), ProductId = 3}, // With prod ids, in Order: 445, 465, 432, 554, 26
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-3), ProductId = 4},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-4), ProductId = 8},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-5), ProductId = 5},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-6), ProductId = 6},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-7), ProductId = 23},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-8), ProductId = 54},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-9), ProductId = 21},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-10), ProductId = 42},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-11), ProductId = 67},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-12), ProductId = 88},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-13), ProductId = 77},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-14), ProductId = 9},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-15), ProductId = 332},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-16), ProductId = 78},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-17), ProductId = 12},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-18), ProductId = 55},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-19), ProductId = 89},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-20), ProductId = 26},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-21), ProductId = 554},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-22), ProductId = 432},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-23), ProductId = 465},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-24), ProductId = 445}
            };

            _userViewingHistoryRepoMock.Setup(r => r.GetAllByIdAsync("Test", "UserId", It.IsAny<QueryOptions<UserViewingHistory>>()))
            .ReturnsAsync(viewingHistories);

            var deletedHistories = new List<UserViewingHistory>();
            _userViewingHistoryRepoMock.Setup(r => r.DeleteAsync(It.IsAny<UserViewingHistory>())).Callback<UserViewingHistory>(vh => deletedHistories.Add(vh));

            // Act
            await _userService.CleanupViewingHistory("Test");

            // Assert
            _userViewingHistoryRepoMock.Verify(r => r.DeleteAsync(It.IsAny<UserViewingHistory>()), Times.Exactly(5));
            Assert.Equal(5, deletedHistories.Count);
            Assert.Equal(25, viewingHistories.Length); // Make sure that changes to test are obious as what else needs to change

            for (int i = 0; i < 5; i++){
                Assert.Equal(deletedHistories[i].ProductId, viewingHistories[24 - i].ProductId);
            }
        }

        [Fact]
        public async Task CleanupViewingHistory_WithLessThan20_ShouldNotRemoveAny(){
            // Arrange
            var viewingHistories = new UserViewingHistory[]{
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now, ProductId = 1},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-45), ProductId = 2},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-30), ProductId = 3},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-5), ProductId = 4}
            }.OrderByDescending(vh => vh.LastViewed);

            _userViewingHistoryRepoMock.Setup(r => r.GetAllByIdAsync("Test", "UserId", It.IsAny<QueryOptions<UserViewingHistory>>()))
            .ReturnsAsync(viewingHistories);

            // Act
            await _userService.CleanupViewingHistory("Test");

            // Assert
            Assert.Equal(4, viewingHistories.Count());
            Assert.True(viewingHistories.Count() < 20);
            _userViewingHistoryRepoMock.Verify(r => r.DeleteAsync(It.IsAny<UserViewingHistory>()), Times.Never);
        }

       // ------------------------------------------------- Delete Viewing History -------------------------------------------------
        [Fact]
        public async Task DeleteViewingHistory_WithValidHistory_ShouldDeleteEntireHistory(){
            // Arrange
            var viewingHistories = new UserViewingHistory[]{
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now, ProductId = 1},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-45), ProductId = 2},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-30), ProductId = 3},
                new UserViewingHistory{UserId = "Test", LastViewed = DateTime.Now.AddMinutes(-5), ProductId = 4}
            };

            _userViewingHistoryRepoMock.Setup(r => r.GetAllByIdAsync("Test", "UserId", It.IsAny<QueryOptions<UserViewingHistory>>()))
            .ReturnsAsync(viewingHistories);

            // Act
            await _userService.DeleteViewingHistory("Test");

            // Assert
            Assert.Equal(4, viewingHistories.Count());
            _userViewingHistoryRepoMock.Verify(r => r.DeleteAsync(It.IsAny<UserViewingHistory>()), Times.Exactly(4));
        }

        // -------------------------------------------------- Get Products From Viewing History ----------------------------------------------------
        [Fact]
        public async Task GetProductsFromViewingHistory_ShouldReturnProducts()
        {
            // Arrange
            var viewingHistory = new UserViewingHistory[]{
                new UserViewingHistory { ProductId = 1 },
                new UserViewingHistory { ProductId = 2 }
            };

            var products = new List<Product>{
                new Product { ProductId = 1, Name = "Product 1" },
                new Product { ProductId = 2, Name = "Product 2" }
            };

            _productRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<QueryOptions<Product>>()))
                        .ReturnsAsync(products[0]);
            
            _productRepoMock.Setup(r => r.GetByIdAsync(2, It.IsAny<QueryOptions<Product>>()))
                        .ReturnsAsync(products[1]);

            // Act
            var result = await _userService.GetProductsFromViewingHistory(viewingHistory);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.ProductId == 1);
            Assert.Contains(result, p => p.ProductId == 2);

            // Verify GetByIdAsync called for each history item
            _productRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<QueryOptions<Product>>()), Times.Exactly(2));
        }

        [Fact]
        public async Task GetProductsFromViewingHistory_WithNonExistentProduct_ShouldSkipOverNullProduct()
        {
            // Arrange
            var viewingHistory = new UserViewingHistory[]{
                new UserViewingHistory { ProductId = 1 },
                new UserViewingHistory { ProductId = 2 }
            };

            var products = new List<Product>{
                new Product { ProductId = 1, Name = "Product 1" }
            };

            _productRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<QueryOptions<Product>>()))
                        .ReturnsAsync(products[0]);
            
            _productRepoMock.Setup(r => r.GetByIdAsync(2, It.IsAny<QueryOptions<Product>>()))
                        .ReturnsAsync((Product)null); // Product does not exist

            // Act
            var result = await _userService.GetProductsFromViewingHistory(viewingHistory);

            // Assert
            Assert.Single(result);
            Assert.Contains(result, p => p.ProductId == 1);
            Assert.DoesNotContain(result, p => p.ProductId == 2);

            _productRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<QueryOptions<Product>>()), Times.Exactly(2));
        }


        // ------------------------------------------------------------------ Get Product From Wishlist ------------------------------------------------
        [Fact]
        public async Task GetProductsFromWishlist_ShouldReturnProducts()
        {
            // Arrange
            var wishlist = new UserWishList[]{
                new UserWishList { ProductId = 1 },
                new UserWishList { ProductId = 2 }
            };

            var products = new List<Product>{
                new Product { ProductId = 1, Name = "Product 1" },
                new Product { ProductId = 2, Name = "Product 2" }
            };

            _productRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<QueryOptions<Product>>()))
                        .ReturnsAsync(products[0]);
            
            _productRepoMock.Setup(r => r.GetByIdAsync(2, It.IsAny<QueryOptions<Product>>()))
                        .ReturnsAsync(products[1]);

            // Act
            var result = await _userService.GetProductsFromWishList(wishlist);

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(result, p => p.ProductId == 1);
            Assert.Contains(result, p => p.ProductId == 2);

            // Verify GetByIdAsync called for each history item
            _productRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<QueryOptions<Product>>()), Times.Exactly(2));
        }

        [Fact]
        public async Task GetProductsFromWishlist_WithNonExistentProduct_ShouldSkipOverNullProduct()
        {
            // Arrange
            var wishlist = new UserWishList[]{
                new UserWishList { ProductId = 1 },
                new UserWishList { ProductId = 2 }
            };

            var products = new List<Product>{
                new Product { ProductId = 1, Name = "Product 1" }
            };

            _productRepoMock.Setup(r => r.GetByIdAsync(1, It.IsAny<QueryOptions<Product>>()))
                        .ReturnsAsync(products[0]);
            
            _productRepoMock.Setup(r => r.GetByIdAsync(2, It.IsAny<QueryOptions<Product>>()))
                        .ReturnsAsync((Product)null); // Product does not exist

            // Act
            var result = await _userService.GetProductsFromWishList(wishlist);

            // Assert
            Assert.Single(result);
            Assert.Contains(result, p => p.ProductId == 1);
            Assert.DoesNotContain(result, p => p.ProductId == 2);

            _productRepoMock.Verify(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<QueryOptions<Product>>()), Times.Exactly(2));
        }

        // -------------------------------------------------- Update Viewing History --------------------------------------------

        [Fact]
        public async Task UpdateViewingHistory_WhenNotInViewingHistory_ShouldAddNewUserViewingHistoryToDatabase(){
            // Arrange
            _userViewingHistoryRepoMock.Setup(r => r.GetByIdAsync("Test", 1, It.IsAny<QueryOptions<UserViewingHistory>>()))
            .ReturnsAsync((UserViewingHistory)null);

            UserViewingHistory capturedHistory = null;
            _userViewingHistoryRepoMock.Setup(r => r.AddAsync(It.IsAny<UserViewingHistory>())).
            Callback<UserViewingHistory>(vh => capturedHistory = vh);

            var before = DateTime.Now; // Using a tolerance window to determine if time has been updated correctly

            // Act
            await _userService.UpdateViewingHistory("Test", 1);
            
            // Assert

            var after = DateTime.Now;
            
            Assert.NotNull(capturedHistory);
            Assert.Equal("Test", capturedHistory.UserId);
            Assert.Equal(1, capturedHistory.ProductId);
            Assert.True(capturedHistory.LastViewed >= before && capturedHistory.LastViewed <= after); // Use a tolerance window to determine datetime was updated

            _userViewingHistoryRepoMock.Verify(r => r.AddAsync(It.IsAny<UserViewingHistory>()), Times.Once);
        }

        [Fact]
        public async Task UpdateViewingHistory_WhenInViewingHistory_ShouldUpdateUserViewingHistoryInDatabase(){
            // Arrange
            var oldHistory = new UserViewingHistory{ UserId = "Test", ProductId = 1, LastViewed = DateTime.Now.AddDays(-3)};

            _userViewingHistoryRepoMock.Setup(r => r.GetByIdAsync("Test", 1, It.IsAny<QueryOptions<UserViewingHistory>>()))
            .ReturnsAsync(oldHistory);

            UserViewingHistory capturedHistory = null;
            _userViewingHistoryRepoMock.Setup(r => r.UpdateAsync(It.IsAny<UserViewingHistory>())).
            Callback<UserViewingHistory>(vh => capturedHistory = vh);

            var before = DateTime.Now; // Using a tolerance window to determine if time has been updated correctly

            // Act
            await _userService.UpdateViewingHistory("Test", 1);
            
            // Assert

            var after = DateTime.Now;
            
            Assert.NotNull(capturedHistory);
            Assert.Equal("Test", capturedHistory.UserId);
            Assert.Equal(1, capturedHistory.ProductId);
            Assert.True(capturedHistory.LastViewed >= before && capturedHistory.LastViewed <= after); // Use a tolerance window to determine datetime was updated

            _userViewingHistoryRepoMock.Verify(r => r.UpdateAsync(oldHistory), Times.Once);
        }

        // ---------------------------------------------------- Update Wishlist ---------------------------------------------
        [Fact]
        public async Task UpdateWishlist_WhenIsOnWishlist_ShouldRemoveFromWishlist(){

        }
         
        [Fact]
        public async Task UpdateWishlist_WhenIsOnNotWishlist_ShouldAddToWishlist(){

        }

        // Error checking here ,, all other checks as well
    }
}