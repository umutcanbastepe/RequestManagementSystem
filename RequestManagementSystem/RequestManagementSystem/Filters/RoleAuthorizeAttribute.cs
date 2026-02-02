using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using RequestManagementSystem.Helpers;
using RequestManagementSystem.Models.Enums;

namespace RequestManagementSystem.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly Role[] _roles;

        public RoleAuthorizeAttribute(params Role[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var session = context.HttpContext.Session;

            if (!SessionHelper.IsLoggedIn(session))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            if (_roles.Length > 0 && !SessionHelper.IsInRole(session, _roles))
            {
                context.Result = new RedirectToActionResult("UnauthorizedAccess", "Account", null);
            }
        }
    }
}
