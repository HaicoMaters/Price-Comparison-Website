using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Price_Comparison_Website.Models;

namespace Price_Comparison_Website.Services.Interfaces
{
    public interface IUserService
    {
        Task AddWishlistItem(UserWishList item);
        Task<IEnumerable<UserWishList>> GetUserWishListItems(string userId);
        Task<UserWishList> GetUserWishListItemById(string userId, int prodId);
        Task<IEnumerable<Product>> GetProductsFromWishList(IEnumerable<UserWishList> wishListItems);
        Task <IEnumerable<UserViewingHistory>> GetUserViewingHistory(string userId);
        Task<IEnumerable<Product>> GetProductsFromViewingHistory(IEnumerable<UserViewingHistory> viewingHistoryItems);
        Task RemoveFromWishlist(int prodId, string userId);
        Task DeleteViewingHistory(string userId);
        Task UpdateViewingHistory(string userId, int prodId);
        Task CleanupViewingHistory(string userId);
        Task<bool?> UpdateUserWishlist(string userId, int prodId);
    }
}