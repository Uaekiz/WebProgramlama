namespace webApplication.Models;

using Microsoft.AspNetCore.Identity;

public class User : IdentityUser
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public bool Gender { get; set; }
    public int Age { get; set; }

    public ICollection<SportRegistration> SportRegistrations { get; set; }
    public ICollection<CourseRegistration> CourseRegistrations { get; set; }
    public ICollection<Reservation> Reservations { get; set; }
}