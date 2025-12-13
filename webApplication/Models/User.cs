namespace webApplication.Models;
using Microsoft.AspNetCore.Identity;

public class User : IdentityUser
{
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public bool Gender { get; set; }
    public int Age {get; set;}
}