using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.DependencyInjection;
using Price_Comparison_Website.Models;
using Price_Comparison_Website.Services;
using Price_Comparison_Website.Services.Implementations;
using Price_Comparison_Website.Services.Interfaces;

namespace Price_Comparison_Website.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Update registrations with new namespaces
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ILoginActivityService, LoginActivityService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPriceListingService, PriceListingService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IVendorService, VendorService>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddHttpContextAccessor();
            
            return services;
        }
    }
}
