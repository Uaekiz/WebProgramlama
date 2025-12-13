using Microsoft.EntityFrameworkCore;
using webApplication.Data;
using webApplication.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services; 
using webApplication.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Servisleri ekle
builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IEmailSender, EmailSender>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    // İsterseniz şifre kurallarını buradan gevşetebilirsiniz (Geliştirme aşaması için)
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
});

// Identity UI sayfaları (Login/Register) için bunu eklemelisiniz:
builder.Services.AddRazorPages();

var app = builder.Build();

// ... (Data seeding kısmını sildiğinizi varsayıyorum) ...

// HTTP request pipeline yapılandırması
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Kimlik doğrulama sıralaması önemlidir:
app.UseAuthentication(); // 1. Kimsin?
app.UseAuthorization();  // 2. Yetkin var mı?

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Login/Register sayfalarının rotalarını ekle:
app.MapRazorPages();

app.Run();