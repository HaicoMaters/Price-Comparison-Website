using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PriceComparisonWebsite.Models;

namespace PriceComparisonWebsite.Services.Interfaces
{
    /// <summary>
    /// Service for managing user-specific operations including wishlists and viewing history
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Adds an item to a user's wishlist
        /// </summary>
        Task AddWishlistItem(UserWishList item);

        /// <summary>
        /// Gets all items in a user's wishlist
        /// </summary>
        Task<IEnumerable<UserWishList>> GetUserWishListItems(string userId);

        /// <summary>
        /// Gets a specific wishlist item for a user
        /// </summary>
        Task<UserWishList> GetUserWishListItemById(string userId, int prodId);

        /// <summary>
        /// Retrieves the products from a collection of wishlist items
        /// </summary>
        Task<IEnumerable<Product>> GetProductsFromWishList(IEnumerable<UserWishList> wishListItems);

        /// <summary>
        /// Gets a user's viewing history
        /// </summary>
        Task<IEnumerable<UserViewingHistory>> GetUserViewingHistory(string userId);

        /// <summary>
        /// Retrieves the products from a collection of viewing history items
        /// </summary>
        Task<IEnumerable<Product>> GetProductsFromViewingHistory(IEnumerable<UserViewingHistory> viewingHistoryItems);

        /// <summary>
        /// Removes a product from a user's wishlist
        /// </summary>
        Task RemoveFromWishlist(int prodId, string userId);

        /// <summary>
        /// Deletes a user's entire viewing history
        /// </summary>
        Task DeleteViewingHistory(string userId);

        /// <summary>
        /// Updates a user's viewing history for a product
        /// </summary>
        Task UpdateViewingHistory(string userId, int prodId);

        /// <summary>
        /// Removes excess entries from a user's viewing history
        /// </summary>
        Task CleanupViewingHistory(string userId);

        /// <summary>
        /// Toggles a product's presence in a user's wishlist
        /// </summary>
        /// <returns>True if successful, False if failed, null if product not found</returns>
        Task<bool?> UpdateUserWishlist(string userId, int prodId);
    }
}