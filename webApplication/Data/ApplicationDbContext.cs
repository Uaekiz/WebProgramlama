using Microsoft.EntityFrameworkCore;
using webApplication.Models; // BU SATIR ZATEN MEVCUTTU VE DOĞRUYDU.

namespace webApplication.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Sport> Sports { get; set; }
    
    public DbSet<Hall> Halls { get; set; }

    public DbSet<Seat> Seats { get; set; }

    public DbSet<Reservation> Reservations { get; set; }
   
     public DbSet<CourseRegistration> CourseRegistrations { get; set; }
     public DbSet<SportRegistration> SportRegistrations { get; set; }
}