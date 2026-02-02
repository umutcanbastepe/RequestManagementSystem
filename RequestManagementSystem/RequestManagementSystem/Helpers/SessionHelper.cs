using RequestManagementSystem.Models.Enums;

namespace RequestManagementSystem.Helpers
{
    public static class SessionHelper
    {
        public static int? GetUserId(ISession session)
        {
            return session.GetInt32("UserId");
        }

        public static Role? GetRole(ISession session)
        {
            var role = session.GetInt32("Role");
            return role.HasValue ? (Role)role.Value : null;
        }

        public static string? GetFullName(ISession session)
        {
            return session.GetString("FullName");
        }

        public static bool IsLoggedIn(ISession session)
        {
            return GetUserId(session).HasValue;
        }

        public static bool IsInRole(ISession session, params Role[] roles)
        {
            var role = GetRole(session);
            return role.HasValue && roles.Contains(role.Value);
        }
    }
}
