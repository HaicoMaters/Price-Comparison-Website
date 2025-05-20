namespace PriceComparisonWebsite.Models
{
    /// <summary>
    /// Generic repository interface for database operations
    /// </summary>
    /// <typeparam name="T">The type of entity being managed</typeparam>
    public interface IRepository<T> where T: class
    {
        /// <summary>
        /// Gets all entities of type T
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Gets all entities of type T with specified options
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync(QueryOptions<T> options);

        /// <summary>
        /// Gets all entities by a specific ID property
        /// </summary>
        Task<IEnumerable<T>> GetAllByIdAsync<TKey>(TKey id, string propertyName, QueryOptions<T> options);

        /// <summary>
        /// Gets an entity by its ID
        /// </summary>
        Task<T> GetByIdAsync(int id, QueryOptions<T> options);

        /// <summary>
        /// Adds a new entity of type T
        /// </summary>
        Task AddAsync(T entity);

        /// <summary>
        /// Updates an existing entity of type T
        /// </summary>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Deletes an entity by its ID
        /// </summary>
        Task DeleteAsync(int id);

        /// <summary>
        /// Deletes a specific entity of type T
        /// </summary>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Gets a user viewing history entry by user and product IDs
        /// </summary>
        Task<UserViewingHistory> GetByIdAsync(string userId, int productId, QueryOptions<UserViewingHistory> options);

        /// <summary>
        /// Gets a user wishlist entry by user and product IDs
        /// </summary>
        Task<UserWishList> GetByIdAsync(string userId, int productId, QueryOptions<UserWishList> options);
    }
}

