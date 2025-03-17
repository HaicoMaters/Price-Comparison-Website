using Price_Comparison_Website.Models;

namespace Price_Comparison_Website.Services
{
    public interface ILoginActivityService
    {
        Task<IEnumerable<LoginActivity>> GetAllActivitiesAsync();
        Task AddActivityAsync(LoginActivity activity);
        Task<IEnumerable<LoginActivity>> GetActivitiesByUserId(string userId);
        Task<IEnumerable<LoginActivity>> GetNMostRecentActivities(int n);
    }
}
