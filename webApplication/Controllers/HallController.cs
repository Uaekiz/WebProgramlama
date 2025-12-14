using Microsoft.AspNetCore.Mvc;
using webApplication.Data;
using Microsoft.EntityFrameworkCore;
using webApplication.Models;
using webApplication.Filters;


namespace webApplication.Controllers
{
    [NoCache]
    public class HallController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HallController(ApplicationDbContext context)
        {
            _context = context;
        }

        // -------------------- HALL LİSTELEME --------------------
        public async Task<IActionResult> Index()
        {
            var halls = await _context.Halls.ToListAsync();
            return View(halls);
        }

        // -------------------- HALL DETAY + FİLTRE --------------------

        public IActionResult Details(int id, string? date, string? startTime, string? endTime)
        {
            var hall = _context.Halls
                .Include(h => h.Seats)
                .FirstOrDefault(h => h.Id == id);

            if (hall == null) return NotFound();

            // --- DÜZELTME BAŞLANGICI ---

            // Tarih parametresi boşsa Bugünü, doluysa geleni al.
            string dateStr = date ?? DateTime.Today.ToString("yyyy-MM-dd");

            // Bunu EN BAŞTA ViewBag'e atıyoruz ki View hangi günde olduğunu bilsin.
            ViewBag.SelectedDateString = dateStr;

            // Saat seçilmemişse (Sadece sayfa açıldıysa veya tarih değiştirildiyse)
            if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
            {
                ViewBag.FilterApplied = false;
                ViewBag.Reservations = new List<Reservation>();

                // Artık View'a döndüğümüzde 'dateStr' bilgisini biliyor olacak!
                return View(hall);
            }

            // --- DÜZELTME BİTİŞİ ---

            // Filtre VAR (Aşağısı aynen kalabilir)
            ViewBag.FilterApplied = true;

            DateTime selectedStart = DateTime.Parse($"{date} {startTime}");
            DateTime selectedEnd = DateTime.Parse($"{date} {endTime}");

            ViewBag.SelectedStart = selectedStart;
            ViewBag.SelectedEnd = selectedEnd;

            // ViewBag.SelectedDateString = date; // <-- Bunu yukarı taşıdığımız için buradan silebilirsin
            ViewBag.SelectedStartString = startTime;
            ViewBag.SelectedEndString = endTime;

            var reservations = _context.Reservations
                .Where(r => r.Seat.HallId == id &&
                            r.StartTime < selectedEnd &&
                            r.EndTime > selectedStart)
                .ToList();

            ViewBag.Reservations = reservations;

            return View(hall);
        }

    }
}
