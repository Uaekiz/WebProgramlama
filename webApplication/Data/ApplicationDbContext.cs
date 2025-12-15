using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using webApplication.Models;

namespace webApplication.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Course> Courses { get; set; } //These are our tables which are shown in database and we process with the data 
    public DbSet<Sport> Sports { get; set; }
    
    public DbSet<Hall> Halls { get; set; }

    public DbSet<Seat> Seats { get; set; }

    public DbSet<Reservation> Reservations { get; set; }
   
    public DbSet<CourseRegistration> CourseRegistrations { get; set; }
    public DbSet<SportRegistration> SportRegistrations { get; set; }
}