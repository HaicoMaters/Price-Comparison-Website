using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PriceComparisonWebsite.Models;
using PriceComparisonWebsite.Services.Interfaces;

namespace PriceComparisonWebsite.Services.Implementations
{
    /// <inheritdoc />
    public class UserService : IUserService
    {
        private readonly IRepository<Product> _products;
        private readonly IRepository<UserViewingHistory> _userViewingHistories;
        private readonly IRepository<UserWishList> _userWishlists;
        private readonly ILogger<UserService> _logger;

        public UserService(IRepository<Product> products,
            IRepository<UserViewingHistory> userViewingHistories,
            IRepository<UserWishList> userWishlists, ILogger<UserService> logger)
        {
            _products = products;
            _userViewingHistories = userViewingHistories;
            _userWishlists = userWishlists;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task AddWishlistItem(UserWishList item)
        {
            try
            {
                await _userWishlists.AddAsync(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add wishlist item");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task CleanupViewingHistory(string userId)
        {
            try
            {
                var viewingHistories = await GetUserViewingHistory(userId);
                var viewingHistoriesList = viewingHistories.ToList();

                while (viewingHistoriesList.Count > 20)
                {
                    int count = viewingHistoriesList.Count;
                    var entityToDelete = viewingHistoriesList[count - 1];
                    await _userViewingHistories.DeleteAsync(entityToDelete);
                    viewingHistoriesList.RemoveAt(count - 1);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove cleanup viewing history");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task DeleteViewingHistory(string userId)
        {
            try
            {
                var viewingHistories = await _userViewingHistories.GetAllByIdAsync(userId, "UserId",
                            new QueryOptions<UserViewingHistory>());

                foreach (var history in viewingHistories)
                {
                    await _userViewingHistories.DeleteAsync(history);
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete viewing history for user {UserId}", userId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Product>> GetProductsFromViewingHistory(IEnumerable<UserViewingHistory> viewingHistoryItems)
        {
            try
            {
                List<Product> productList = new List<Product>();
                foreach (var history in viewingHistoryItems)
                {
                    var product = await _products.GetByIdAsync(history.ProductId, new QueryOptions<Product>()); ;
                    if (product != null)
                    {
                        productList.Add(product);
                    }
                }
                return productList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get products from viewing history");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Product>> GetProductsFromWishList(IEnumerable<UserWishList> wishListItems)
        {
            try
            {
                List<Product> productList = new List<Product>();
                foreach (var wishlistItem in wishListItems)
                {
                    var product = await _products.GetByIdAsync(wishlistItem.ProductId, new QueryOptions<Product>());
                    if (product != null)
                    {
                        productList.Add(product);
                    }
                }
                return productList;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get products from wishlist");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<UserViewingHistory>> GetUserViewingHistory(string userId)
        {
            try
            {
                var viewingHistories = await _userViewingHistories.GetAllByIdAsync(userId, "UserId", new QueryOptions<UserViewingHistory>());
                return viewingHistories.OrderByDescending(v => v.LastViewed);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get viewing history for user {UserId}", userId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<UserWishList> GetUserWishListItemById(string userId, int prodId)
        {
            try
            {
                return await _userWishlists.GetByIdAsync(userId, prodId, new QueryOptions<UserWishList>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get wishlist item for user {UserId}", userId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<UserWishList>> GetUserWishListItems(string userId)
        {
            try
            {
                return await _userWishlists.GetAllByIdAsync(userId, "UserId", new QueryOptions<UserWishList>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get wishlist items for user {UserId}", userId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task RemoveFromWishlist(int prodId, string userId)
        {
            try
            {
                var existingEntity = await _userWishlists.GetByIdAsync(userId, prodId, new QueryOptions<UserWishList>());
                if (existingEntity == null)
                    throw new InvalidOperationException("Wishlist item not found");
                await _userWishlists.DeleteAsync(existingEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove item from wishlist. ProductId: {ProductId}, UserId: {UserId}", prodId, userId);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task UpdateViewingHistory(string userId, int prodId)
        {
            try
            {
                var exsistingEntity = await _userViewingHistories.GetByIdAsync(userId, prodId, new QueryOptions<UserViewingHistory>());
                if (exsistingEntity != null)
                {
                    exsistingEntity.LastViewed = DateTime.Now;
                    await _userViewingHistories.UpdateAsync(exsistingEntity);
                }
                else
                {
                    var newEntity = new UserViewingHistory
                    {
                        ProductId = prodId,
                        LastViewed = DateTime.Now,
                        UserId = userId
                    };
                    await _userViewingHistories.AddAsync(newEntity);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update wishlist");
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<bool?> UpdateUserWishlist(string userId, int prodId)
        {
            try
            {
                var existingEntity = await GetUserWishListItemById(userId, prodId);

                if (existingEntity != null)
                {
                    // Remove from wishlist
                    await RemoveFromWishlist(prodId, userId);
                    return true;
                }
                else
                {
                    // Add to wishlist
                    var existingProd = await _products.GetByIdAsync(prodId, new QueryOptions<Product>());

                    if (existingProd == null)
                    {
                        return null;  // Product not found
                    }

                    var newWishlistItem = new UserWishList
                    {
                        ProductId = prodId,
                        UserId = userId,
                        LastCheapestPrice = existingProd.CheapestPrice
                    };

                    await AddWishlistItem(newWishlistItem);
                    return true; // Worked
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating wishlist. UserId: {UserId}, ProductId: {ProductId}", userId, prodId);
                return false;  // Failed
            }
        }
    }
}