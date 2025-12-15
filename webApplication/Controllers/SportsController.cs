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

namespace webApplication.Controllers //It is exactly the same thing with course controller,
//  thats why I didnt feel the need to mention it again
{
    [NoCache]
    public class SportsController : Controller
    {

        private readonly ApplicationDbContext _context;

        public SportsController(ApplicationDbContext context)
        {
            _context = context;
        }
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

            var sport = await _context.Sports.FirstOrDefaultAsync(m => m.Id == id);

            if (sport == null)
            {
                return NotFound();
            }
            bool isUserRegistered = false;
    
            if (User.Identity.IsAuthenticated)
            {
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                isUserRegistered = await _context.SportRegistrations
                .AnyAsync(r => r.SportId == sport.Id && r.UserId == currentUserId);
            }

            ViewBag.IsUserRegistered = isUserRegistered;
            return View(sport);
        }

        [Authorize]
        [NoCache]
        public IActionResult Form(int? id)
        {
            if (id == null || id == 0)
            {
                return RedirectToAction(nameof(Listing));
            }

            var registration = new SportRegistration { SportId = id.Value };

            return View(registration);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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