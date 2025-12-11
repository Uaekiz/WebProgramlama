using Microsoft.AspNetCore.Mvc;
using webApplication.Data;
using Microsoft.EntityFrameworkCore;
using webApplication.Models;

namespace webApplication.Controllers
{
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

            // İlk giriş mi? (Filtre yok)
            if (string.IsNullOrEmpty(date) || string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime))
            {
                ViewBag.FilterApplied = false;
                ViewBag.Reservations = new List<Reservation>();

                return View(hall);
            }

            // Filtre VAR → değerleri ViewBag'e koy
            ViewBag.FilterApplied = true;

            DateTime selectedStart = DateTime.Parse($"{date} {startTime}");
            DateTime selectedEnd = DateTime.Parse($"{date} {endTime}");

            ViewBag.SelectedStart = selectedStart;
            ViewBag.SelectedEnd = selectedEnd;

            // Bu 3 değer Create sayfasına taşınacak!
            ViewBag.SelectedDateString = date;
            ViewBag.SelectedStartString = startTime;
            ViewBag.SelectedEndString = endTime;

            // Çakışan rezervasyonlar
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
