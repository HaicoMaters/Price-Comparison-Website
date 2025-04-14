using PriceComparisonWebsite.Models;

namespace PriceComparisonWebsite.Services
{
    public interface ILoginActivityService
    {
        Task<IEnumerable<LoginActivity>> GetAllActivitiesAsync();
        Task AddActivityAsync(LoginActivity activity);
        Task<IEnumerable<LoginActivity>> GetActivitiesByUserId(string userId, QueryOptions<LoginActivity> queryOptions);
        Task<IEnumerable<LoginActivity>> GetNMostRecentActivities(int n);
    }
}
