using RequestManagementSystem.Models.Entities;
using RequestManagementSystem.Models.Enums;

namespace RequestManagementSystem.Data
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            if (context.Users.Any())
                return;

            context.Users.Add(new User
            {
                Username = "admin",
                Password = "123",
                FullName = "System Admin",
                Role = Role.Admin
            });

            context.Users.Add(new User
            {
                Username = "manager",
                Password = "123",
                FullName = "Manager User",
                Role = Role.Manager
            });

            context.Users.Add(new User
            {
                Username = "user",
                Password = "123",
                FullName = "Normal User",
                Role = Role.User
            });

            context.SaveChanges();
        }
    }
}
