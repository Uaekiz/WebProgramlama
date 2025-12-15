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
    [NoCache]//this is the filters key which I mentioned
    public class CoursesController : Controller
    {

        private readonly ApplicationDbContext _context; //It is our database object that we can do CRUD

        public CoursesController(ApplicationDbContext context) // we start the database object with constructor
        {
            _context = context;
        }

        public async Task<IActionResult> Listing() // for listing the courses, we need a list
        {
            var courses = await _context.Courses.ToListAsync();
            return View(courses); //it return the list
        }

        public async Task<IActionResult> Details(int? id) //this action for detatils of the course
        {
            if (id == null) 
            {
                return NotFound();
            }

            var course = await _context.Courses.FirstOrDefaultAsync(m => m.Id == id);

            if (course == null)
            {
                return NotFound();
            } //until here we ckecked, courses are okey or not

            bool isUserRegistered = false;
    
            if (User.Identity.IsAuthenticated) //if user logs in 
            {
                
                var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                
                isUserRegistered = await _context.CourseRegistrations
                    .AnyAsync(r => r.CourseId == course.Id && r.UserId == currentUserId);
            }

            
            ViewBag.IsUserRegistered = isUserRegistered; //we send this because if they dont log in before sellecting date, 
            // they must log the website
            return View(course);
        }

        [Authorize] //for form section they must be authorize, so we add this key
        [NoCache]
        public IActionResult Form(int? id)//it create the form but it knows which course it is
        {
            if (id == null || id == 0)
            {
                return RedirectToAction(nameof(Listing));
            }

            var registration = new CourseRegistration { CourseId = id.Value };

            return View(registration);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] //security 
        public async Task<IActionResult> Create([Bind("CourseId,SelectedDay")] CourseRegistration registration)
        {
            registration.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier); //we pull id because it is bad that 
            // directly users enter their ids 

            if (string.IsNullOrEmpty(registration.UserId))
            {
                return Redirect("/Identity/Account/Login");
            }

            ModelState.Remove("UserId");
            ModelState.Remove("User");

            if (ModelState.IsValid)
            {
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

                registration.RegistrationDate = DateTime.Now;
                registration.Status = "Onay Bekliyor";

                _context.Add(registration);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Sent_Signing)); //everythins is okey we direct them to signing page
            }

            return View("Form", registration);
        }

        public IActionResult Sent_Signing()
        {
            return View();
        }

    }
}