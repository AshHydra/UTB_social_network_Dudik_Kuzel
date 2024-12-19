using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public HomeController(
            ILogger<HomeController> logger,
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Admin page with list of all users
        public async Task<IActionResult> Admin()
        {
            // Fetch all users
            var users = _userManager.Users.ToList();

            // Map users to EditUserViewModel
            var model = new List<EditUserViewModel>();
            foreach (var user in users)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var roles = userRoles.Select(roleName =>
                {
                    var role = _roleManager.Roles.FirstOrDefault(r => r.Name == roleName);
                    return role != null ? role.Id.ToString() : null;
                }).Where(r => r != null);

                model.Add(new EditUserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    RoleIds = string.Join(",", roles)
                });
            }

            return View("~/Views/Admin/Adminpage.cshtml", model); // Pass mapped users to view
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
