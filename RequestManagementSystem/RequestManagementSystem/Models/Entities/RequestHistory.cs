using RequestManagementSystem.Models.Enums;

namespace RequestManagementSystem.Models.Entities
{
    public class RequestHistory
    {
        public int Id { get; set; }

        public int RequestId { get; set; }
        public Request Request { get; set; }

        public RequestStatus Status { get; set; }
        public string Description { get; set; }

        public int ActionByUserId { get; set; }
        public User ActionByUser { get; set; }

        public DateTime ActionDate { get; set; }
    }
}
