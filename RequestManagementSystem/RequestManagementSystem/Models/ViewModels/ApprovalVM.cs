using System.ComponentModel.DataAnnotations;
using RequestManagementSystem.Models.Enums;

namespace RequestManagementSystem.Models.ViewModels
{
    public class ApprovalItemVM
    {
        public int Id { get; set; }
        public string RequestNo { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public RequestType Type { get; set; }
        public RequestPriority Priority { get; set; }
        public string CreatedByFullName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class RejectVM
    {
        public int RequestId { get; set; }
        public string RequestNo { get; set; } = string.Empty;
        [Required(ErrorMessage = "Red sebebi zorunludur")]
        [Display(Name = "Red Sebebi")]
        public string Reason { get; set; } = string.Empty;
    }
}
