using Microsoft.AspNetCore.Mvc;
using webApplication.Data;
using Microsoft.EntityFrameworkCore;
using webApplication.Models;
using webApplication.Filters;


namespace webApplication.Controllers
{
    [NoCache] //the filter
    public class HallController : Controller
    {
        private readonly ApplicationDbContext _context; //again our database

        public HallController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index() //we need a list to show
        {
            var halls = await _context.Halls.ToListAsync();
            return View(halls);
        }


        public IActionResult Details(int id, string? date, string? startTime, string? endTime) //here we offer the user to see times and seats
        {
            var hall = _context.Halls
                .Include(h => h.Seats)
                .FirstOrDefault(h => h.Id == id); //seats are pulled

            if (hall == null) return NotFound();

            string dateStr = date ?? DateTime.Today.ToString("yyyy-MM-dd");

            ViewBag.SelectedDateString = dateStr;

            if (string.IsNullOrEmpty(startTime) || string.IsNullOrEmpty(endTime)) //if the user don enter the time and
            //  press filter button we should stand there
            {
                ViewBag.FilterApplied = false;
                ViewBag.Reservations = new List<Reservation>();

                return View(hall);
            }

            ViewBag.FilterApplied = true; //if they fill, to show the seats, we need to have a boolean variable

            DateTime selectedStart = DateTime.Parse($"{date} {startTime}");
            DateTime selectedEnd = DateTime.Parse($"{date} {endTime}");

            ViewBag.SelectedStart = selectedStart;
            ViewBag.SelectedEnd = selectedEnd;

            ViewBag.SelectedStartString = startTime;
            ViewBag.SelectedEndString = endTime;
            //these are necessary because a seat must be empty for all hours, if the user just select for an hour,
            //  it must be empty for other hours

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
