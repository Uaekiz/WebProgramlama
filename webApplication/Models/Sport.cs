
using System.ComponentModel.DataAnnotations.Schema;
namespace webApplication.Models

{
    public class Sport
    {
        public int Id {get; set;}
        public string ?Name {get; set;}
        [Column("Capasity")] //same thing 
        public int Capacity {get; set;}
        public DateTime Date {get; set;}
        public string ?Location {get; set;}
        public string ?Trainer {get; set;}
        public int Age_Limit {get; set;}
        public string DaysOffered { get; set; }
        public string? info { get; set;}
        public string? ImageUrl { get; set; }

    }
}