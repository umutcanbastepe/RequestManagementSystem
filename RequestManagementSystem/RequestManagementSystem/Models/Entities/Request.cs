using RequestManagementSystem.Models.Enums;

namespace RequestManagementSystem.Models.Entities
{
    public class Request
    {
        public int Id { get; set; }

        public string RequestNo { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public RequestType Type { get; set; }
        public RequestPriority Priority { get; set; }
        public RequestStatus Status { get; set; }

        public int CreatedByUserId { get; set; }
        public User CreatedByUser { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
