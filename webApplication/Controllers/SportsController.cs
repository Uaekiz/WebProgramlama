using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webApplication.Data;
using webApplication.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace webApplication.Controllers
{
    public class SportsController : Controller
    {
        
        private readonly ApplicationDbContext _context;

        public SportsController (ApplicationDbContext context)
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
            return View(sport);
        }

        // ----------------------------------------------------
        // KAYIT FORMU (GET) - SPORT ID'SİNİ ALACAK ŞEKİLDE GÜNCELLENDİ
        // ----------------------------------------------------
        
        // GET: /Sports/Form/{id} -> Detay sayfasından gelen Sport ID'sini yakalar
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
        
        // POST: /Sports/Create
        [HttpPost]
        [ValidateAntiForgeryToken]

        // Artık Sport modeli değil, SportRegistration modeli bekleniyor
        public async Task<IActionResult> Create([Bind("SportId,ApplicantName,SelectedDay")] SportRegistration registration)
        {
            // Kontenjan kontrolü ve kaydetme işlemi, ModelState geçersiz olsa bile
            // SportId mevcut olduğu sürece yapılmalıdır.
            
            if (ModelState.IsValid)
            {
                // 1. KONTENJAN KONTROLÜ
                
                var sportDetails = await _context.Sports
                    .AsNoTracking() // Daha hızlı okuma
                    .FirstOrDefaultAsync(s => s.Id == registration.SportId);

                if (sportDetails == null)
                {
                    ModelState.AddModelError("", "Hata: Kayıt yapılmaya çalışılan spor bulunamadı.");
                    return View("Form", registration); 
                }

                // Mevcut kayıt sayısını say
                var currentRegistrations = await _context.SportRegistrations
                    .CountAsync(r => r.SportId == registration.SportId);

                // Kontenjanı Karşılaştır
                if (currentRegistrations >= sportDetails.Capacity)
                {
                    ModelState.AddModelError("", $"Üzgünüz, '{sportDetails.Name}' kontenjanı ({sportDetails.Capacity}) dolmuştur.");
                    // Formu doldurulan bilgilerle (hata mesajı ile) geri yolla.
                    return View("Form", registration); 
                }

                // 2. KAYIT İŞLEMİ
                
                registration.RegistrationDate = DateTime.Now;
                registration.Status = "Onay Bekliyor"; 
                
                _context.Add(registration);
                await _context.SaveChangesAsync();
                
                // Başarılı kayıttan sonra onay sayfasına yönlendir.
                return RedirectToAction(nameof(Sent_Signing)); 
            }
            
            // Model doğrulaması (İsim veya Gün seçimi) başarısız olursa formu geri yolla.
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