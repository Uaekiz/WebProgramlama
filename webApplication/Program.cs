using Microsoft.EntityFrameworkCore;
using webApplication.Data;
using webApplication.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // 1) Add hall
    if (!db.Halls.Any())
    {
        db.Halls.AddRange(
            new Hall { Name = "PC Hall", Type = "Computer", SeatCount = 20 },
            new Hall { Name = "Study Hall", Type = "Study", SeatCount = 25 },
            new Hall { Name = "Reading Hall", Type = "Library", SeatCount = 15 }
        );

        db.SaveChanges();
    }

    // 2) Add Seat
    if (!db.Seats.Any())
    {
        var halls = db.Halls.ToList();

        foreach (var hall in halls)
        {
            for (int i = 1; i <= hall.SeatCount; i++)
            {
                db.Seats.Add(new Seat
                {
                    SeatNumber = i,
                    HallId = hall.Id
                });
            }
        }

        db.SaveChanges();
    }
}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
