using RequestManagementSystem.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace RequestManagementSystem.Models.ViewModels
{
    public class RequestCreateVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur")]
        [StringLength(200)]
        [Display(Name = "Başlık")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Talep Türü")]
        public RequestType Type { get; set; }

        [Required]
        [Display(Name = "Öncelik")]
        public RequestPriority Priority { get; set; }

        [Display(Name = "Durum")]
        public RequestStatus Status { get; set; }
    }
}
