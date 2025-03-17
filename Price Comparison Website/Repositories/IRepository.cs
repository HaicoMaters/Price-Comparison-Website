namespace Price_Comparison_Website.Models
{
    public interface IRepository<T> where T: class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllByIdAsync<TKey>(TKey id, string propertyName, QueryOptions<T> options);
        Task<T> GetByIdAsync(int id, QueryOptions<T> options);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);
        Task DeleteAsync(T entity);
        Task<UserViewingHistory> GetByIdAsync(string userId, int productId, QueryOptions<UserViewingHistory> options);
        Task<UserWishList> GetByIdAsync(string userId, int productId, QueryOptions<UserWishList> options);
    }
}

