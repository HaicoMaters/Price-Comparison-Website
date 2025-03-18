using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using NuGet.Protocol.Core.Types;
using Price_Comparison_Website.Data;
using Price_Comparison_Website.Models;

namespace Price_Comparison_Website.Services
{
    public class LoginActivityService : ILoginActivityService
    {
        private IRepository<LoginActivity> _loginActivities;
        private readonly ILogger<LoginActivityService> _logger;

        public LoginActivityService(IRepository<LoginActivity> loginActivites, ILogger<LoginActivityService> logger)
        {
            _loginActivities = loginActivites;
            _logger = logger;
        }

        public async Task<IEnumerable<LoginActivity>> GetAllActivitiesAsync()
        {
            try
            {
                return await _loginActivities.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get all login activities");
                throw;
            }
        }

        public async Task AddActivityAsync(LoginActivity activity)
        {
            try
            {
                await _loginActivities.AddAsync(activity);
                _logger.LogInformation("Login activity recorded for user {UserId}. Success: {IsSuccessful}", 
                    activity.UserId, activity.IsSuccessful);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add login activity for user {UserId}", activity.UserId);
                throw;
            }
        }

        public async Task<IEnumerable<LoginActivity>> GetActivitiesByUserId(string userId, QueryOptions<LoginActivity> queryOptions)
        {
            try
            {
                return await _loginActivities.GetAllByIdAsync(userId, "UserId", queryOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get login activities for user {UserId}", userId);
                throw;
            }
        }

        public async Task<IEnumerable<LoginActivity>> GetNMostRecentActivities(int n)
        {
            try
            {
                var activities = await GetAllActivitiesAsync();
                return activities.OrderByDescending(a => a.LoginTime).Take(n);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get {N} most recent login activities", n);
                throw;
            }
        }

    }
}