using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using UTB_social_network_Dudik.Models;
using Utb_sc_Infrastructure.Identity;
using System.Diagnostics;

namespace UTB_social_network_Dudik.Controllers
{
    [Authorize] // All actions in this controller require authentication
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager;

        public HomeController(ILogger<HomeController> logger, UserManager<User> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        // Admin page with list of all users
        public IActionResult Admin()
        {
            var users = _userManager.Users.ToList(); // Fetch all users
            return View("~/Views/Admin/Adminpage.cshtml", users); // Pass users to view
        }

        public IActionResult Contacts()
        {
            return View("~/Views/Contacts/Contactspage.cshtml");
        }

        public IActionResult Profile()
        {
            return View("~/Views/Profile/Profile.cshtml");
        }

        // Main page after login
        public IActionResult MainPage()
        {
            return View("~/Views/Mainpage/Mainpage.cshtml");
        }

        // Default index page
        [AllowAnonymous] // Anyone can access this action
        public IActionResult Index()
        {
            return View();
        }

        // Privacy policy page
        [AllowAnonymous] // Anyone can access this action
        public IActionResult Privacy()
        {
            return View();
        }

        // Display error page
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous] // Anyone can access this action
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
