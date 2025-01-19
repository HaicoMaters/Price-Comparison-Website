
using Microsoft.EntityFrameworkCore;
using Price_Comparison_Website.Data;

namespace Price_Comparison_Website.Models
{
	public class Repository<T> : IRepository<T> where T : class
	{
		protected ApplicationDbContext _context { get; set; }
		private DbSet<T> _dbSet { get; set; }

		public Repository(ApplicationDbContext context)
		{
			_context = context;
			_dbSet = context.Set<T>();
		}

		public async Task AddAsync(T entity)
		{
			await _dbSet.AddAsync(entity);
			await _context.SaveChangesAsync();
		}

        public async Task DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null)
            {
                throw new InvalidOperationException($"Entity with ID {id} not found.");
            }

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }


        public async Task<IEnumerable<T>> GetAllAsync()
		{
			return await _dbSet.ToListAsync();
		}

		public async Task<IEnumerable<T>> GetAllByIdAsync<TKey>(TKey id, string propertyName, QueryOptions<T> options)
		{
			IQueryable<T> query = _dbSet;
			if (options.HasWhere)
			{
				query = query.Where(options.Where);
			}
			if (options.HasOrderBy)
			{
                query = query.OrderBy(options.OrderBy);
			}
			foreach (string includes in options.GetIncludes())
			{
				query.Include(includes);
			}

			query = query.Where(e => EF.Property<TKey>(e, propertyName).Equals(id));

			return await query.ToListAsync();
		}

		public async Task<T> GetByIdAsync(int id, QueryOptions<T> options)
		{
			IQueryable<T> query = _dbSet;
			if (options.HasWhere)
			{
				query = query.Where(options.Where);
			}
			if (options.HasOrderBy)
			{
				query = query.OrderBy(options.OrderBy);
			}
			foreach (string includes in options.GetIncludes())
			{
				query.Include(includes);
			}

			var key = _context.Model.FindEntityType(typeof(T)).FindPrimaryKey().Properties.FirstOrDefault();
			string primaryKeyName = key?.Name;
			return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, primaryKeyName) == id);
		}

        public async Task<UserViewingHistory> GetByIdAsync(string userId, int productId, QueryOptions<UserViewingHistory> options) // For the user viewing history
        {
            IQueryable<UserViewingHistory> query = (IQueryable<UserViewingHistory>)_dbSet;

            if (options.HasWhere)
            {
                query = query.Where(options.Where);
            }

            if (options.HasOrderBy)
            {
                query = query.OrderBy(options.OrderBy);
            }

            foreach (string include in options.GetIncludes())
            {
                query = query.Include(include);
            }

            return await query
                .FirstOrDefaultAsync(uvh => uvh.UserId == userId && uvh.ProductId == productId);
        }


        public async Task UpdateAsync(T entity)
		{
			_context.Update(entity);
			await _context.SaveChangesAsync();
		}
	}
}
