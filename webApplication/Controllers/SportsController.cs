using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webApplication.Data;
using webApplication.Models;
using System.Security.Claims;
using webApplication.Filters;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace webApplication.Controllers
{
     [NoCache]
    public class SportsController : Controller
    {

        private readonly ApplicationDbContext _context;

        public SportsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ----------------------------------------------------
        // OKUMA (READ) AKSİYONLARI - DEĞİŞMEDİ
        // ----------------------------------------------------

        public async Task<IActionResult> Listing()
        {
            var sports = await _context.Sports.ToListAsync();
            return View(sports);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // DaysOffered bilgisini görmek için de kullanışlıdır
            var sport = await _context.Sports.FirstOrDefaultAsync(m => m.Id == id);

            if (sport == null)
            {
                return NotFound();
            }
            bool isUserRegistered = false;
    
    // Kullanıcının giriş yapıp yapmadığını kontrol ediyoruz
            if (User.Identity.IsAuthenticated)
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                isUserRegistered = await _context.SportRegistrations
                .AnyAsync(r => r.SportId == sport.Id && r.UserId == currentUserId);
            }

            // Sonucu View'a iletiyoruz
            ViewBag.IsUserRegistered = isUserRegistered;
            return View(sport);
        }

        // ----------------------------------------------------
        // KAYIT FORMU (GET) - SPORT ID'SİNİ ALACAK ŞEKİLDE GÜNCELLENDİ
        // ----------------------------------------------------

        // GET: /Sports/Form/{id} -> Detay sayfasından gelen Sport ID'sini yakalar
        [Authorize]
        [NoCache]
        public IActionResult Form(int? id)
        {
            if (id == null || id == 0)
            {
                // ID gelmezse listeye geri yönlendir.
                return RedirectToAction(nameof(Listing));
            }

            // View'a, SportRegistration modelini gönderirken SportId'yi içine yerleştiriyoruz.
            // Bu, formda gizli alandan geri gönderilecek olan kritik bilgidir.
            var registration = new SportRegistration { SportId = id.Value };

            // View, artık bu SportRegistration nesnesini model olarak kullanacak.
            return View(registration);
        }

        // ----------------------------------------------------
        // KAYIT OLUŞTURMA (CREATE) - KONTENJAN KONTROLÜ EKLENDİ
        // ----------------------------------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Bind kısmından "ApplicantName"i çıkardım
        public async Task<IActionResult> Create([Bind("SportId,SelectedDay")] SportRegistration registration)
        {
            registration.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(registration.UserId))
            {
                return Redirect("/Identity/Account/Login");
            }

            ModelState.Remove("UserId");
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                // 1. KONTENJAN KONTROLÜ
                var sportDetails = await _context.Sports
                    .AsNoTracking()
                    .FirstOrDefaultAsync(s => s.Id == registration.SportId);

                if (sportDetails == null)
                {
                    ModelState.AddModelError("", "Hata: Kayıt yapılmaya çalışılan spor bulunamadı.");
                    return View("Form", registration);
                }

                var currentRegistrations = await _context.SportRegistrations
                    .CountAsync(r => r.SportId == registration.SportId);

                if (currentRegistrations >= sportDetails.Capacity)
                {
                    ModelState.AddModelError("", $"Üzgünüz, '{sportDetails.Name}' kontenjanı ({sportDetails.Capacity}) dolmuştur.");
                    return View("Form", registration);
                }

                // 2. KAYIT İŞLEMİ
                registration.RegistrationDate = DateTime.Now;
                registration.Status = "Onay Bekliyor";

                _context.Add(registration);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Sent_Signing));
            }

            return View("Form", registration);
        }

        // ----------------------------------------------------
        // ONAY SAYFASI
        // ----------------------------------------------------

        public IActionResult Sent_Signing()
        {
            return View();
        }
    }
}