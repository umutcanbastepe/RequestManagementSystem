using RequestManagementSystem.Models.Enums;

namespace RequestManagementSystem.Models.ViewModels
{
    public class RequestDetailVM
    {
        public int Id { get; set; }
        public string RequestNo { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public RequestType Type { get; set; }
        public RequestPriority Priority { get; set; }
        public RequestStatus Status { get; set; }
        public string CreatedByFullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool CanEdit { get; set; }
        public bool CanApproveReject { get; set; }
        public List<RequestHistoryItemVM> History { get; set; } = new();
    }

    public class RequestHistoryItemVM
    {
        public RequestStatus Status { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ActionByFullName { get; set; } = string.Empty;
        public DateTime ActionDate { get; set; }
    }
}
