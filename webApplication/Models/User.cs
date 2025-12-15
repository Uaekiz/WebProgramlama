namespace webApplication.Models;

using Microsoft.AspNetCore.Identity;

public class User : IdentityUser
{
    public string? Name { get; set; }  //This 4 line are used for Identify,I know we have Identify package 
    // but it just involves some phone email and password, If you check the Register cs and cshtml, you can see them in input area
    public string? Surname { get; set; }
    public bool Gender { get; set; }
    public int Age { get; set; }

    public ICollection<SportRegistration> SportRegistrations { get; set; } //In profile page we are showing the reservations and courses, sports
    //joined information, so we need list of them 
    public ICollection<CourseRegistration> CourseRegistrations { get; set; }
    public ICollection<Reservation> Reservations { get; set; }
}