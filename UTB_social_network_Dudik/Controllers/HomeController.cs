using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using UTB_social_network_Dudik.Models;
using Utb_sc_Infrastructure.Identity;
using System.Diagnostics;
using Utb_sc_Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

// Alias pro jmenné prostory
using IdentityUser = Utb_sc_Infrastructure.Identity.User;
using DomainUser = Utb_sc_Domain.Entities.User;


namespace UTB_social_network_Dudik.Controllers
{
    [Authorize] // All actions in this controller require authentication
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly SocialNetworkDbContext _dbContext;

        public HomeController(
            ILogger<HomeController> logger,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            SocialNetworkDbContext dbContext)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        // Profile actions

        // GET: /Home/Profile
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            // Fetch the currently logged-in user
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("User not found."); // Return 404 if the user is not found
            }

            // Populate the ProfileViewModel
            var model = new ProfileViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            };

            return View("~/Views/Profile/Profile.cshtml", model);
        }

        // POST: /Home/Profile
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            Console.WriteLine("Profile update POST method called.");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid.");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"- {error.ErrorMessage}");
                }

                TempData["ErrorMessage"] = "Failed to update profile. Please check the input.";
                return View("~/Views/Profile/Profile.cshtml", model);
            }

            // Fetch the user based on their ID
            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                Console.WriteLine("User not found in the database.");
                TempData["ErrorMessage"] = "User not found.";
                return View("~/Views/Profile/Profile.cshtml", model);
            }

            Console.WriteLine($"User found: {user.UserName}");
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;

            // Handle password change if provided
            if (!string.IsNullOrEmpty(model.CurrentPassword) &&
                !string.IsNullOrEmpty(model.NewPassword) &&
                !string.IsNullOrEmpty(model.ConfirmNewPassword))
            {
                Console.WriteLine("Attempting password change.");

                // Verify the current password
                var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
                if (!isPasswordValid)
                {
                    Console.WriteLine("Current password is invalid.");
                    ModelState.AddModelError("CurrentPassword", "The current password is incorrect.");
                    TempData["ErrorMessage"] = "Failed to update profile. The current password is incorrect.";
                    return View("~/Views/Profile/Profile.cshtml", model);
                }

                // Change the password
                var passwordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
                    {
                        Console.WriteLine($"- {error.Description}");
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    TempData["ErrorMessage"] = "Failed to change password.";
                    return View("~/Views/Profile/Profile.cshtml", model);
                }

                Console.WriteLine("Password updated successfully.");
                TempData["SuccessMessage"] = "Profile updated successfully, including password!";
            }

            // Update other user details
            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    Console.WriteLine($"- {error.Description}");
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                TempData["ErrorMessage"] = "Failed to update profile.";
                return View("~/Views/Profile/Profile.cshtml", model);
            }

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Profile));
        }

        // Admin actions

        // GET: /Home/Admin
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



        // GET: /Home/Contacts
        [HttpGet]
        public async Task<IActionResult> Contacts()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "Pøihlášený uživatel nebyl nalezen.";
                return RedirectToAction("Index");
            }

            try
            {
                var friends = await _dbContext.FriendLists
                    .Where(f => f.UserId == currentUser.Id || f.FriendId == currentUser.Id) // Filtrace pro obì strany vztahu
                    .Include(f => f.Friend)
                    .ToListAsync();

                Debug.WriteLine("Poèet pøátel: " + friends.Count); // Debugging: poèet pøátel

                var model = new ContactsViewModel
                {
                    Contacts = friends.Select(f => new UserViewModel
                    {
                        Email = f.Friend.Email,
                        UserName = f.Friend.UserName,
                        ProfilePicturePath = f.Friend.ProfilePicturePath ?? "/images/default.png"
                    }).ToList()
                };

                foreach (var friend in friends)
                {
                    Debug.WriteLine($"Friend: {friend.Friend.UserName}, Email: {friend.Friend.Email}"); // Debugging: zobrazit všechny pøátele
                }

                return View("~/Views/Contacts/Contactspage.cshtml", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Chyba pøi naèítání kontaktù.");
                TempData["ErrorMessage"] = "Nastala chyba pøi naèítání kontaktù.";
                return RedirectToAction("Index");
            }
        }




        // GET: /Home/MainPage
        public IActionResult MainPage()
        {
            return View("~/Views/Mainpage/Mainpage.cshtml");
        }

        // GET: /Home/Index
        [AllowAnonymous] // Anyone can access this action
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Home/Privacy
        [AllowAnonymous] // Anyone can access this action
        public IActionResult Privacy()
        {
            return View();
        }

        // GET: /Home/Error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous] // Anyone can access this action
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
