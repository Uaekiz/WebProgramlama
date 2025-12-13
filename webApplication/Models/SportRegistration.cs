using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webApplication.Models
{
    public class SportRegistration
    {
        public int Id { get; set; } 
        public int SportId { get; set; } 
        public Sport? Sport { get; set; } 

        public string UserId { get; set; } 

        [ForeignKey("UserId")]
        public User User { get; set; } 

        [Required(ErrorMessage = "Lütfen ders gününü seçiniz.")]
        public string SelectedDay { get; set; } 
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Onay Bekliyor";
    }
}