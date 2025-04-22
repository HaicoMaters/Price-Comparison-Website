using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PriceComparisonWebsite.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class InternalOrAuthorizedAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;
        private const string INTERNAL_CLIENT_HEADER = "X-Internal-Client";

        public InternalOrAuthorizedAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Check for internal client header first
            if (context.HttpContext.Request.Headers.TryGetValue(INTERNAL_CLIENT_HEADER, out var clientHeader) 
                && clientHeader == "true")
            {
                return; // Allow internal client
            }

            // Check if user is authenticated and in the required role
            if (context.HttpContext.User.Identity?.IsAuthenticated == true)
            {
                // If no roles specified, any authenticated user is allowed
                if (_roles == null || _roles.Length == 0)
                    return;

                // Check if user is in any of the required roles
                if (_roles.Any(role => context.HttpContext.User.IsInRole(role)))
                    return;
            }

            context.Result = new UnauthorizedResult();
        }
    }
}