using System.Linq.Expressions;

namespace PriceComparisonWebsite.Models
{
    /// <summary>
    /// Provides options for querying entities from the database
    /// </summary>
    /// <typeparam name="T">The type of entity being queried</typeparam>
    public class QueryOptions<T> where T: class
    {
        /// <summary>
        /// Expression defining how to order the query results
        /// </summary>
        public Expression<Func<T, Object>> OrderBy { get; set; } = null;

        /// <summary>
        /// Expression defining the filter conditions for the query
        /// </summary>
        public Expression<Func<T, bool>> Where { get; set; } = null;

        private string[] includes = Array.Empty<string>();

        /// <summary>
        /// Comma-separated list of navigation properties to include in the query
        /// </summary>
        public string Includes
        {
            set => includes = value.Replace(" ", "").Split(',');
        }

        /// <summary>
        /// Gets the array of navigation properties to include
        /// </summary>
        public string[] GetIncludes() => includes;

        /// <summary>
        /// Indicates whether a where clause is specified
        /// </summary>
        public bool HasWhere => Where != null;

        /// <summary>
        /// Indicates whether an order by clause is specified
        /// </summary>
        public bool HasOrderBy => OrderBy != null;
    }
}
