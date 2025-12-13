using Microsoft.AspNetCore.Mvc;
using webApplication.Data;
using webApplication.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace webApplication.Controllers
{
    public class ReservationController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReservationController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Reservation/Create?seatId=5
        [Authorize]
        public IActionResult Create(int seatId, string date, string startTime, string endTime)
        {
            var seat = _context.Seats.FirstOrDefault(s => s.Id == seatId);
            if (seat == null) return NotFound();

            ViewBag.SeatNumber = seat.SeatNumber;

            // Form ekranında sadece görüntülemek için
            ViewBag.SelectedDate = date;
            ViewBag.SelectedStart = startTime;
            ViewBag.SelectedEnd = endTime;

            var reservation = new Reservation
            {
                SeatId = seatId
            };

            return View(reservation);
        }



        // POST: Reservation/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]
        public IActionResult CreatePost(int seatId, string selectedDate, string startTime, string endTime) // "userName" parametresini sildim
        {
            // Tarih ve saatleri birleştir
            DateTime date = DateTime.Parse(selectedDate);
            DateTime start = DateTime.Parse(selectedDate + " " + startTime);
            DateTime end = DateTime.Parse(selectedDate + " " + endTime);

            // Kullanıcının ID'sini alıyoruz
            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Model nesnesini oluştur
            var reservation = new Reservation
            {
                SeatId = seatId,
                StartTime = start,
                EndTime = end,
                UserId = currentUserId // ARTIK BURAYA USERID YAZIYORUZ
            };

            // ViewBag değerlerini HER durumda doldur
            ViewBag.Today = DateTime.Today.ToString("yyyy-MM-dd");
            ViewBag.Tomorrow = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");

            // Sadece bugün ve yarın için izin ver
            var minDate = DateTime.Today;
            var maxDate = DateTime.Today.AddDays(1);

            if (reservation.StartTime.Date < minDate || reservation.StartTime.Date > maxDate ||
                reservation.EndTime.Date < minDate || reservation.EndTime.Date > maxDate)
            {
                ModelState.AddModelError("", "Rezervasyon yalnızca bugün veya yarın için olmalıdır.");
                return View(reservation);
            }

            // Bitiş başlangıçtan büyük olmalı
            if (reservation.StartTime >= reservation.EndTime)
            {
                ModelState.AddModelError("", "Bitiş saati başlangıç saatinden büyük olmalıdır.");
                return View(reservation);
            }

            // ÇAKIŞMA KONTROLÜ
            bool conflict = _context.Reservations.Any(r =>
                r.SeatId == reservation.SeatId &&
                r.StartTime.Date == reservation.StartTime.Date &&
                reservation.StartTime < r.EndTime &&
                reservation.EndTime > r.StartTime
            );

            if (conflict)
            {
                ModelState.AddModelError("", "Bu saat aralığı bu koltuk için zaten rezerve edilmiştir.");
                return View(reservation);
            }

            // Her şey tamamsa kaydet
            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Rezervasyonunuz başarıyla oluşturuldu! Tarih: {reservation.StartTime:dd.MM.yyyy}.";

            var seat = _context.Seats.First(s => s.Id == seatId);
            return RedirectToAction("Details", "Hall", new { id = seat.HallId });
        }



    }
}
