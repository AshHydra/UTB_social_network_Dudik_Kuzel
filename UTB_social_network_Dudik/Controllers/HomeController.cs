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

        // Profile actions

        // GET: /Home/Profile
        public async Task<IActionResult> Profile()
        {
            Console.WriteLine("HomeController: Profile GET action called");

            // Fetch the logged-in user
            var userEmail = HttpContext.Session.GetString("UserEmail");

            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account"); // Redirect to login if no session
            }

            var user = await _userManager.FindByEmailAsync(userEmail);

            if (user == null)
            {
                return NotFound(); // Return 404 if user is not found
            }

            var model = new ProfileViewModel
            {
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            return View("~/Views/Profile/Profile.cshtml", model);
        }

        // POST: /Home/Profile
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            Console.WriteLine("HomeController: Profile POST action called");

            if (ModelState.IsValid)
            {
                var userEmail = HttpContext.Session.GetString("UserEmail");

                if (string.IsNullOrEmpty(userEmail))
                {
                    return RedirectToAction("Login", "Account"); // Redirect to login if no session
                }

                var user = await _userManager.FindByEmailAsync(userEmail);

                if (user == null)
                {
                    return NotFound();
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction(nameof(Profile));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View("~/Views/Profile/Profile.cshtml", model);
        }

        // Other actions
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
