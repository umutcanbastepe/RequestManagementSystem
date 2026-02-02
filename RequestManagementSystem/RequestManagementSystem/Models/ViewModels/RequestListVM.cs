using RequestManagementSystem.Models.Enums;

namespace RequestManagementSystem.Models.ViewModels
{
    public class RequestListVM
    {
        public int Id { get; set; }
        public string RequestNo { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public RequestType Type { get; set; }
        public RequestPriority Priority { get; set; }
        public RequestStatus Status { get; set; }
        public string CreatedByFullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
