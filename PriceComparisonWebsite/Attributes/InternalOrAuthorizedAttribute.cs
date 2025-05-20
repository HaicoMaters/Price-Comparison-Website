using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace PriceComparisonWebsite.Attributes
{
    /// <summary>
    /// Authorization attribute that allows access either through internal API authentication or role-based authorization
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class InternalOrAuthorizedAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;
        private const string INTERNAL_CLIENT_HEADER = "X-Internal-Client";
        private const string INTERNAL_AUTH_HEADER = "X-Internal-Auth";

        /// <summary>
        /// Creates a new instance of InternalOrAuthorizedAttribute
        /// </summary>
        /// <param name="roles">Optional roles that are allowed to access the resource</param>
        public InternalOrAuthorizedAttribute(params string[] roles)
        {
            _roles = roles;
        }

        /// <summary>
        /// Performs authorization for the current request
        /// </summary>
        /// <param name="context">The authorization filter context</param>
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var internalApiKey = configuration["InternalApi:Key"];

            // Check for internal client headers
            if (context.HttpContext.Request.Headers.TryGetValue(INTERNAL_CLIENT_HEADER, out var clientHeader) 
                && context.HttpContext.Request.Headers.TryGetValue(INTERNAL_AUTH_HEADER, out var authHeader))
            {
                if (clientHeader == "true" && authHeader == internalApiKey)
                {
                    return; // Allow internal client with correct auth key
                }
            }

            // Check if user is authenticated and in the required role
            if (context.HttpContext.User.Identity?.IsAuthenticated == true)
            {
                if (_roles == null || _roles.Length == 0)
                    return;

                if (_roles.Any(role => context.HttpContext.User.IsInRole(role)))
                    return;
            }

            context.Result = new UnauthorizedResult();
        }
    }
}