using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webApplication.Models
{
    public class SportRegistration //This class is a bridge table like in sql, it has its PK which is Id and has FK which comes from User
    {
        public int Id { get; set; } 
        public int SportId { get; set; } 
        public Sport? Sport { get; set; } 

        public string UserId { get; set; } //Here are your exam question we have UserId for pk and we have User because we help ef to browse

        [ForeignKey("UserId")]
        public User User { get; set; } 

        [Required(ErrorMessage = "Lütfen ders gününü seçiniz.")]
        public string SelectedDay { get; set; } 
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Onay Bekliyor";
    }
}