using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webApplication.Data;
using webApplication.Models;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace webApplication.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly SignInManager<User> _signInManager;

        public ProfileController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager; 
            _context = context;
        }

        // GET: /Profile/Index
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            var user = await _context.Users
                // Spor Başvurularını Çek (Spor detayıyla beraber)
                .Include(u => u.SportRegistrations)
                    .ThenInclude(sr => sr.Sport)
                // Kurs Başvurularını Çek (Kurs detayıyla beraber)
                .Include(u => u.CourseRegistrations)
                    .ThenInclude(cr => cr.Course)
                // Rezervasyonları Çek (Koltuk ve Salon detayıyla beraber)
                .Include(u => u.Reservations)
                    .ThenInclude(r => r.Seat)
                    .ThenInclude(s => s.Hall)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            user.Reservations = user.Reservations.OrderByDescending(r => r.StartTime).ToList();
            user.SportRegistrations = user.SportRegistrations.OrderByDescending(s => s.RegistrationDate).ToList();
            user.CourseRegistrations = user.CourseRegistrations.OrderByDescending(c => c.RegistrationDate).ToList();

            return View(user);
        }

        // POST: Rezervasyon İptal Et (Sil)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var userId = _userManager.GetUserId(User);
            // Sadece bu kullanıcıya ait ve ID'si tutan rezervasyonu bul
            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (reservation != null)
            {
                // Tarihi geçmiş mi kontrolü (Controller tarafında da güvenlik için)
                if (reservation.StartTime > DateTime.Now)
                {
                    _context.Reservations.Remove(reservation);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Rezervasyon iptal edildi.";
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Spor Başvurusu İptal Et
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelSport(int id)
        {
            var userId = _userManager.GetUserId(User);
            var registration = await _context.SportRegistrations
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (registration != null && registration.Status == "Onay Bekliyor")
            {
                _context.SportRegistrations.Remove(registration);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Spor başvurusu silindi.";
            }
            return RedirectToAction(nameof(Index));
        }

        // POST: Kurs Başvurusu İptal Et
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelCourse(int id)
        {
            var userId = _userManager.GetUserId(User);
            var registration = await _context.CourseRegistrations
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (registration != null && registration.Status == "Onay Bekliyor")
            {
                _context.CourseRegistrations.Remove(registration);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Kurs başvurusu silindi.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // Mevcut bilgileri kutucuklara dolduruyoruz
            var model = new ProfileEditViewModel
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Age = user.Age
            };

            return View(model);
        }

        // -----------------------------------------------------------
        // 2. GÜNCELLEMEYİ KAYDET (POST)
        // -----------------------------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // --- Temel Bilgileri Güncelle ---
            user.Name = model.Name;
            user.Surname = model.Surname;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Age = model.Age;
            user.UserName = model.Email; // Genelde kullanıcı adı email ile aynı tutulur

            // --- Şifre Değişikliği İstenmiş mi? ---
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                if (string.IsNullOrEmpty(model.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Şifre değiştirmek için mevcut şifrenizi girmelisiniz.");
                    return View(model);
                }

                // Şifreyi değiştirmeyi dene
                var passwordChangeResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

                if (!passwordChangeResult.Succeeded)
                {
                    foreach (var error in passwordChangeResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
            }

            // --- Kullanıcıyı Kaydet ---
            var updateResult = await _userManager.UpdateAsync(user);

            if (updateResult.Succeeded)
            {
                // ÖNEMLİ: Şifre veya kritik bilgi değişince oturum tazelemek gerekir
                await _signInManager.RefreshSignInAsync(user);

                TempData["SuccessMessage"] = "Profiliniz başarıyla güncellendi.";
                return RedirectToAction("Index");
            }

            // Hata varsa listele
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
    }
}