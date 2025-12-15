using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using webApplication.Data;
using webApplication.Models;
using webApplication.Filters;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;

namespace webApplication.Controllers
{
    [Authorize]//of course authorize because its completly about the user
    [NoCache]//it is important because after log out, we mustnt be able to go back directly the profile with the back button
    public class ProfileController : Controller
    {
        private readonly UserManager<User> _userManager; //to find the user
        private readonly ApplicationDbContext _context; //to list the reservations, courses and sports
        private readonly SignInManager<User> _signInManager; // to refresh the user data

        public ProfileController(UserManager<User> userManager, SignInManager<User> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager; //we start them
            _signInManager = signInManager; 
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User); //find the user

            var user = await _context.Users //its like inner join in sql, we pull the information with their FK
                .Include(u => u.SportRegistrations)
                    .ThenInclude(sr => sr.Sport)
                .Include(u => u.CourseRegistrations)
                    .ThenInclude(cr => cr.Course)
                .Include(u => u.Reservations)
                    .ThenInclude(r => r.Seat)
                    .ThenInclude(s => s.Hall)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null) return NotFound();

            user.Reservations = user.Reservations.OrderByDescending(r => r.StartTime).ToList(); //we list them
            user.SportRegistrations = user.SportRegistrations.OrderByDescending(s => s.RegistrationDate).ToList();
            user.CourseRegistrations = user.CourseRegistrations.OrderByDescending(c => c.RegistrationDate).ToList();

            return View(user);
        }

        [HttpPost]//canceling a reservation is a post request
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var userId = _userManager.GetUserId(User);
            var reservation = await _context.Reservations //we double check, it is the correct reservataion and
            //  it is really user's reservation
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (reservation != null)
            {
                if (reservation.StartTime > DateTime.Now)//if it is from future we can cancel 
                // if not of course you cant cancel, bitti borun pazarı sür eşşeği niğdeye
                {
                    _context.Reservations.Remove(reservation);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Rezervasyon iptal edildi."; // if it is deleted successfully, we show it
                }
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelSport(int id)
        {
            var userId = _userManager.GetUserId(User);
            var registration = await _context.SportRegistrations
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId); //the same control for sport

            if (registration != null && registration.Status == "Onay Bekliyor")//if it still waiting for "approval" it can be canceled
            {
                _context.SportRegistrations.Remove(registration);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Spor başvurusu silindi."; 
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelCourse(int id) //again same thing with sports and reservaations
        {
            var userId = _userManager.GetUserId(User);
            var registration = await _context.CourseRegistrations
                .FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);

            if (registration != null && registration.Status == "Onay Bekliyor")
            {
                _context.CourseRegistrations.Remove(registration);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Kurs başvurusu silindi.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new ProfileEditViewModel
            {
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Age = user.Age
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileEditViewModel model) //thats the edit part that
        //  user can edit their information
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            user.Name = model.Name;
            user.Surname = model.Surname;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.Age = model.Age;
            user.UserName = model.Email; //we pull their informations

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                if (string.IsNullOrEmpty(model.CurrentPassword))
                {
                    ModelState.AddModelError("CurrentPassword", "Şifre değiştirmek için mevcut şifrenizi girmelisiniz.");
                    return View(model);
                }

                var passwordChangeResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                //we try to change the password and check it is succeded or not

                if (!passwordChangeResult.Succeeded)//if not we show the mistake
                {
                    foreach (var error in passwordChangeResult.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
            }

            var updateResult = await _userManager.UpdateAsync(user);

            if (updateResult.Succeeded) //if it is okey
            {
                await _signInManager.RefreshSignInAsync(user);//we should use await bcs you changed the information,
                //  but the cookies may not know you changed and remove the user from the system

                TempData["SuccessMessage"] = "Profiliniz başarıyla güncellendi.";
                return RedirectToAction("Index");
            }

            foreach (var error in updateResult.Errors)//if there is a mistake, again we show 
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }
    }
}