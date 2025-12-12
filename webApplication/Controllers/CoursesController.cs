using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webApplication.Data;
using webApplication.Models;
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

        // Artık Course modeli değil, CourseRegistration modeli bekleniyor
        public async Task<IActionResult> Create([Bind("CourseId,ApplicantName,SelectedDay")] CourseRegistration registration)
        {
            // ModelState.IsValid kontrolü, sadece C# modelindeki zorunlu alanların (Required)
            // doldurulup doldurulmadığını kontrol eder.
            if (ModelState.IsValid)
            {
                // 1. KONTENJAN KONTROLÜ (KRİTİK)
                
                // Bu Kursun (CourseId) toplam kontenjanını alıyoruz.
                var courseDetails = await _context.Courses
                    .AsNoTracking() // Veritabanından hızlı okuma
                    .FirstOrDefaultAsync(c => c.Id == registration.CourseId);

                if (courseDetails == null)
                {
                    ModelState.AddModelError("", "Hata: Kayıt yapılmaya çalışılan kurs bulunamadı.");
                    return View("Form", registration); 
                }

                // Bu Kursa daha önce yapılan mevcut kayıt sayısını say.
                var currentRegistrations = await _context.CourseRegistrations
                    .CountAsync(r => r.CourseId == registration.CourseId);

                // Kontenjanı Karşılaştır
                if (currentRegistrations >= courseDetails.Capacity)
                {
                    // Kontenjan doluysa, kullanıcıya hata mesajı gösterilir.
                    ModelState.AddModelError("", $"Üzgünüz, '{courseDetails.Name}' kontenjanı ({courseDetails.Capacity}) dolmuştur.");
                    // Formu doldurulan bilgilerle (hata mesajı ile) geri yolla.
                    return View("Form", registration); 
                }

                // 2. KAYIT İŞLEMİ
                
                registration.RegistrationDate = DateTime.Now;
                registration.Status = "Onay Bekliyor"; 
                
                _context.Add(registration); // Yeni kaydı bağlama ekle
                await _context.SaveChangesAsync(); // Veritabanına kaydet
                
                // Başarılı kayıttan sonra onay sayfasına yönlendir.
                return RedirectToAction(nameof(Sent_Signing)); 
            }
            
            // Model doğrulaması (ApplicantName veya SelectedDay) başarısız olursa formu geri yolla.
            return View("Form", registration);
        }

        public IActionResult Sent_Signing()
        {
            return View();
        }

    }
}