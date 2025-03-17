using Microsoft.Extensions.DependencyInjection;
using Price_Comparison_Website.Services;

namespace Price_Comparison_Website.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ILoginActivityService, LoginActivityService>();
            services.AddHttpContextAccessor();
            
            return services;
        }
    }
}
