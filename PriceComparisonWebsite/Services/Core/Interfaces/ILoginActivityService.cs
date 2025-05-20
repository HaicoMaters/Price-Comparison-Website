using PriceComparisonWebsite.Models;

namespace PriceComparisonWebsite.Services
{
    /// <summary>
    /// Service for tracking and managing user login activities
    /// </summary>
    public interface ILoginActivityService
    {
        /// <summary>
        /// Gets all login activities
        /// </summary>
        Task<IEnumerable<LoginActivity>> GetAllActivitiesAsync();

        /// <summary>
        /// Records a new login activity
        /// </summary>
        Task AddActivityAsync(LoginActivity activity);

        /// <summary>
        /// Gets all login activities for a specific user
        /// </summary>
        Task<IEnumerable<LoginActivity>> GetActivitiesByUserId(string userId, QueryOptions<LoginActivity> queryOptions);

        /// <summary>
        /// Gets the N most recent login activities across all users
        /// </summary>
        /// <param name="n">Number of activities to retrieve</param>
        Task<IEnumerable<LoginActivity>> GetNMostRecentActivities(int n);
    }
}
