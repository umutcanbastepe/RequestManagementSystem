namespace RequestManagementSystem.Models.ViewModels
{
    public class DashboardVM
    {
        public int TotalRequestCount { get; set; }
        public int PendingApprovalCount { get; set; }
        public int MyRequestCount { get; set; }
        public int MyPendingCount { get; set; }
        public List<RequestListVM> LastRequests { get; set; } = new();
        public bool IsManagerOrAdmin { get; set; }
    }
}
