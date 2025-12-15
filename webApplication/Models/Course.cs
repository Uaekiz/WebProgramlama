using System.ComponentModel.DataAnnotations.Schema;
namespace webApplication.Models
{

  public class Course
  {
    public int Id { get; set; }//PK
    public string? Name { get; set; }
    [Column("Capasity")] //This is for, one of our friends made a mistake and migration asve it like Capasity, :( so whe should add this 
    public int Capacity { get; set; }
    public DateTime Date { get; set; }
    public string? Location { get; set; }
    public string? Trainer { get; set; }
    public int Age_Limit { get; set; }
    public string DaysOffered { get; set; }
    public string? info { get; set; }
    public string? ImageUrl { get; set; }

  }
}