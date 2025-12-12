using System.ComponentModel.DataAnnotations.Schema; // [Column] özniteliği için
namespace webApplication.Models
{

    public class Course
    {
      public int Id { get; set;}
      public string? Name {get; set;}    
      [Column("Capasity")]
      public int Capacity { get; set; }
      public DateTime Date {get; set;}  
      public string? Location {get; set;}
      public string? Trainer {get; set;}
      public int Age_Limit {get; set;}
      public string DaysOffered { get; set; }
      public string? info { get; set;}
      public string? ImageUrl { get; set; }

    }
}