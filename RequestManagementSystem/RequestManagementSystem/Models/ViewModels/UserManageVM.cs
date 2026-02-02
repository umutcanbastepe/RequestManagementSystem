using System.ComponentModel.DataAnnotations;
using RequestManagementSystem.Models.Enums;

namespace RequestManagementSystem.Models.ViewModels
{

    public class UserListVM
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
    }

    public class UserCreateVM
    {
        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        [StringLength(50)]
        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur")]
        [StringLength(50, MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ad soyad zorunludur")]
        [StringLength(100)]
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Rol")]
        public Role Role { get; set; }
    }

    public class UserEditVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı zorunludur")]
        [StringLength(50)]
        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; } = string.Empty;

        [StringLength(50, MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre (boş bırakılırsa değişmez)")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Ad soyad zorunludur")]
        [StringLength(100)]
        [Display(Name = "Ad Soyad")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Rol")]
        public Role Role { get; set; }
    }
}

