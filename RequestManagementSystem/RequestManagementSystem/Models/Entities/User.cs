using RequestManagementSystem.Models.Enums;

namespace RequestManagementSystem.Models.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }

        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}
