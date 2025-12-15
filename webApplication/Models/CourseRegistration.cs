using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webApplication.Models
{
    public class CourseRegistration
    {
        public int Id { get; set; } //PK
        public int CourseId { get; set; } //Fk
        public Course? Course { get; set; }
        public string UserId { get; set; } //FK

        [ForeignKey("UserId")]
        public User User { get; set; } //ef

        [Required(ErrorMessage = "Lütfen ders gününü seçiniz.")]
        public string SelectedDay { get; set; }   
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Onay Bekliyor";
    }
}