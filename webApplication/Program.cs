using Microsoft.EntityFrameworkCore;
using webApplication.Data;
using webApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services; 
using webApplication.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection"); // we connecting database

builder.Services.AddControllersWithViews();  //VC of MVC 

builder.Services.AddTransient<IEmailSender, EmailSender>(); //Adding our fake email server

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));// we are saying we are gonna use SQLite

builder.Services.AddIdentity<User, IdentityRole>(options => //here are the rules of password, we kept them simple because of this is a school project
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;
})
.AddEntityFrameworkStores<ApplicationDbContext>() //we should add them because of identify package
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options => //If the user hasnt logged in, it directs the user 
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
});

builder.Services.AddRazorPages();//Identify's views use razor so we added its service

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();   //these are security that when you connect it to a server


app.UseAuthentication();  //it is who are you
app.UseAuthorization();  //and this can you have the permission to access

app.MapStaticAssets();  //the speed of dowlanding css and js file 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages(); //reminding

app.Run();