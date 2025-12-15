using Microsoft.AspNetCore.Mvc;
using webApplication.Data;
using webApplication.Models;
using System.Security.Claims;
using webApplication.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace webApplication.Controllers
{
    [NoCache]//the filter
    public class ReservationController : Controller
    {
        private readonly ApplicationDbContext _context;//database

        public ReservationController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize]//of course if they are in reservation page they must log in
        [NoCache]
        public IActionResult Create(int seatId, string date, string startTime, string endTime)
        {
            var seat = _context.Seats.FirstOrDefault(s => s.Id == seatId); //it pull the clicked seat
            if (seat == null) return NotFound();

            ViewBag.SeatNumber = seat.SeatNumber;
            ViewBag.SelectedDate = date;
            ViewBag.SelectedStart = startTime;
            ViewBag.SelectedEnd = endTime;

            var reservation = new Reservation
            {
                SeatId = seatId
            };

            return View(reservation); //we prepare all of thing for user to confirm
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Create")]// two create function and same parameter but for different proccess, we changed its name
        public IActionResult CreatePost(int seatId, string selectedDate, string startTime, string endTime) 
        {
        
            DateTime date = DateTime.Parse(selectedDate);
            DateTime start = DateTime.Parse(selectedDate + " " + startTime);
            DateTime end = DateTime.Parse(selectedDate + " " + endTime);  

            if (start < DateTime.Now)//it is impossible thans to our view but, if the user select a time past we should error it
            {
                ModelState.AddModelError("", "Geçmiş bir zamana rezervasyon yapamazsınız.");

                var tempSeat = _context.Seats.FirstOrDefault(s => s.Id == seatId);
                ViewBag.SeatNumber = tempSeat?.SeatNumber;
                ViewBag.SelectedDate = selectedDate;
                ViewBag.SelectedStart = startTime;
                ViewBag.SelectedEnd = endTime;

                return View(new Reservation { SeatId = seatId });
            }

            string currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier); //for reservation, we pull users id

            bool userHasConflict = _context.Reservations.Any(r => //this boolean variable is neccesery for
            //  the user have a reservation at the same time
                r.UserId == currentUserId &&
                r.StartTime.Date == start.Date &&
                start < r.EndTime &&
                end > r.StartTime
            );

            if (userHasConflict)//if they have we should warn them
            {
                ModelState.AddModelError("", "Bu saat aralığında zaten başka bir rezervasyonunuz bulunuyor.");

                var tempSeat = _context.Seats.FirstOrDefault(s => s.Id == seatId);
                ViewBag.SeatNumber = tempSeat?.SeatNumber;
                ViewBag.SelectedDate = selectedDate;
                ViewBag.SelectedStart = startTime;
                ViewBag.SelectedEnd = endTime;

                return View(new Reservation { SeatId = seatId });
            }

            var reservation = new Reservation
            {
                SeatId = seatId,
                StartTime = start,
                EndTime = end,
                UserId = currentUserId 
            };

            //Under here as I mentioned before up, there are some control that already under control by view
            ViewBag.Today = DateTime.Today.ToString("yyyy-MM-dd");
            ViewBag.Tomorrow = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");

            var minDate = DateTime.Today;
            var maxDate = DateTime.Today.AddDays(1);

            if (reservation.StartTime.Date < minDate || reservation.StartTime.Date > maxDate ||
                reservation.EndTime.Date < minDate || reservation.EndTime.Date > maxDate)
            {
                ModelState.AddModelError("", "Rezervasyon yalnızca bugün veya yarın için olmalıdır.");
                return View(reservation);
            }

            if (reservation.StartTime >= reservation.EndTime)
            {
                ModelState.AddModelError("", "Bitiş saati başlangıç saatinden büyük olmalıdır.");
                return View(reservation);
            }

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

            _context.Reservations.Add(reservation);
            _context.SaveChanges();

            TempData["SuccessMessage"] = $"Rezervasyonunuz başarıyla oluşturuldu! Tarih: {reservation.StartTime:dd.MM.yyyy}.";
            //after reservtion we show it is okey

            var seat = _context.Seats.First(s => s.Id == seatId);
            return RedirectToAction("Details", "Hall", new { id = seat.HallId });
        }
    }
}
