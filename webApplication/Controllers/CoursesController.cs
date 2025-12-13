using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webApplication.Data;
using webApplication.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace webApplication.Controllers
{
    public class CoursesController : Controller
    {

        private readonly ApplicationDbContext _context;

        public CoursesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ----------------------------------------------------
        // OKUMA (READ) AKSİYONLARI - KURS LİSTELEME
        // ----------------------------------------------------

        public async Task<IActionResult> Listing()
        {
            var courses = await _context.Courses.ToListAsync();
            return View(courses);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FirstOrDefaultAsync(m => m.Id == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // ----------------------------------------------------
        // KAYIT FORMU (GET) - KURS ID'SİNİ ALACAK ŞEKİLDE GÜNCELLENDİ
        // ----------------------------------------------------
        // CoursesController.cs
        // ...
        // GET: /Courses/Form/{id} -> Detay sayfasından gelen Course ID'sini yakalar
        [Authorize]
        public IActionResult Form(int? id)
        {
            if (id == null || id == 0)
            {
                return RedirectToAction(nameof(Listing));
            }

            // View'a, CourseRegistration modelini gönderirken CourseId'yi içine yerleştiriyoruz.
            var registration = new CourseRegistration { CourseId = id.Value };

            return View(registration);
        }
        // ...
        // ----------------------------------------------------
        // KAYIT OLUŞTURMA (CREATE) - KONTENJAN KONTROLÜ EKLENDİ
        // ----------------------------------------------------

        [HttpPost]
        [ValidateAntiForgeryToken]
        // Bind kısmından "ApplicantName"i çıkardım
        public async Task<IActionResult> Create([Bind("CourseId,SelectedDay")] CourseRegistration registration)
        {
            // Giriş yapan kullanıcının ID'sini ata
            registration.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // UserId boş ise (Kullanıcı giriş yapmamışsa) Login'e gönder
            if (string.IsNullOrEmpty(registration.UserId))
            {
                return Redirect("/Identity/Account/Login");
            }

            // ModelState kontrolünü yaparken "ApplicantName" hatasını temizle (Çünkü artık modelde o yok ama eski cache kalmış olabilir)
            ModelState.Remove("UserId");
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
                // 1. KONTENJAN KONTROLÜ
                var courseDetails = await _context.Courses
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == registration.CourseId);

                if (courseDetails == null)
                {
                    ModelState.AddModelError("", "Hata: Kayıt yapılmaya çalışılan kurs bulunamadı.");
                    return View("Form", registration);
                }

                var currentRegistrations = await _context.CourseRegistrations
                    .CountAsync(r => r.CourseId == registration.CourseId);

                if (currentRegistrations >= courseDetails.Capacity)
                {
                    ModelState.AddModelError("", $"Üzgünüz, '{courseDetails.Name}' kontenjanı ({courseDetails.Capacity}) dolmuştur.");
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

        public IActionResult Sent_Signing()
        {
            return View();
        }

    }
}