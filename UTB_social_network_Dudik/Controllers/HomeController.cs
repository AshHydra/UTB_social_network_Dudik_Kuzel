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
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            var model = new ProfileViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                ProfilePicturePath = user.ProfilePicturePath ?? "/images/default.png"
            };

            return View("~/Views/Profile/Profile.cshtml", model);
        }

        // POST: /Home/Profile
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Profile update failed. Please check your input.";
                return View("~/Views/Profile/Profile.cshtml", model);
            }

            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return View("~/Views/Profile/Profile.cshtml", model);
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;

            if (!string.IsNullOrEmpty(model.CurrentPassword) &&
                !string.IsNullOrEmpty(model.NewPassword) &&
                !string.IsNullOrEmpty(model.ConfirmNewPassword))
            {
                var isPasswordValid = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
                if (!isPasswordValid)
                {
                    ModelState.AddModelError("CurrentPassword", "The current password is incorrect.");
                    TempData["ErrorMessage"] = "Failed to update profile. Current password is incorrect.";
                    return View("~/Views/Profile/Profile.cshtml", model);
                }

                var passwordChangeResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!passwordChangeResult.Succeeded)
                {
                    foreach (var error in passwordChangeResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    TempData["ErrorMessage"] = "Failed to change password.";
                    return View("~/Views/Profile/Profile.cshtml", model);
                }
            }

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                TempData["ErrorMessage"] = "Profile update failed.";
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

            var currentUserId = currentUser.Id; // Získání ID aktuálního uživatele

            try
            {
                var friends = await _dbContext.FriendLists
                    .Where(f => f.UserId == currentUserId || f.FriendId == currentUserId)
                    .Select(f => new
                    {
                        FriendId = f.UserId == currentUserId ? f.FriendId : f.UserId,
                        FriendsSince = f.FriendsSince
                    })
                    .ToListAsync();

                // Naètení uživatelských dat pro pøátele
                var friendDetailsRaw = await _dbContext.Users
                    .Where(u => friends.Select(f => f.FriendId).Contains(u.Id))
                    .ToListAsync(); // Bring data into memory

                // Step 2: Process data in memory
                var friendDetails = friendDetailsRaw.Select(u => new UserViewModel
                {
                    Email = u.Email,
                    UserName = u.UserName,
                    ProfilePicturePath = (u as Utb_sc_Infrastructure.Identity.User)?.ProfilePicturePath ?? "/images/default.png" // Cast to custom User and access ProfilePicturePath
                }).ToList();

                var model = new ContactsViewModel
                {
                    Contacts = friendDetails
                };

                // Debugování: Výpis dat pøátel do konzole
                foreach (var friend in friendDetails)
                {
                    Debug.WriteLine($"Friend: {friend.UserName}, Email: {friend.Email}");
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
        [HttpGet]
        public async Task<IActionResult> MainPage()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Index");
            }

            // Fetch only the chats where the user is a participant
            var chats = await _dbContext.ChatUsers
                .Where(cu => cu.UserId == currentUser.Id)
                .Select(cu => new ChatViewModel
                {
                    ChatId = cu.ChatId,
                    ChatName = cu.Chat.Name
                })
                .ToListAsync();

            // Wrap inside MainPageViewModel
            var model = new MainPageViewModel
            {
                Chats = chats
            };

            return View("~/Views/Mainpage/Mainpage.cshtml", model);
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
