using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using webApplication.Filters;
using webApplication.Models;

namespace webApplication.Controllers;
 [NoCache] //the filter because when the user log out, it directs the user here
public class HomeController : Controller
{
   
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]//the error shouldnt be saved
    public IActionResult Error()//it sends the error with a unique id
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
