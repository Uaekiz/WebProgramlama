using System.ComponentModel.DataAnnotations;

namespace webApplication.Models
{
    public class ProfileEditViewModel
    {
        [Display(Name = "Ad")]
        [Required(ErrorMessage = "Ad alanı zorunludur.")]
        public string Name { get; set; }

        [Display(Name = "Soyad")]
        [Required(ErrorMessage = "Soyad alanı zorunludur.")]
        public string Surname { get; set; }

        [EmailAddress]
        [Display(Name = "E-posta")]
        [Required(ErrorMessage = "E-posta alanı zorunludur.")]
        public string Email { get; set; }

        [Display(Name = "Telefon")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Yaş")]
        public int Age { get; set; }

        // --- ŞİFRE DEĞİŞTİRME ALANLARI (Zorunlu Değil) ---

        [DataType(DataType.Password)]
        [Display(Name = "Mevcut Şifre")]
        public string? CurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Yeni Şifre (Tekrar)")]
        [Compare("NewPassword", ErrorMessage = "Şifreler uyuşmuyor.")]
        public string? ConfirmPassword { get; set; }
    }
}