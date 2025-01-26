using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using UTB_social_network_Dudik.Models;
using Utb_sc_Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Utb_sc_Infrastructure.Identity;
using System.Diagnostics;

namespace UTB_social_network_Dudik.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<User> _userManager; // Use the custom User class
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly SocialNetworkDbContext _dbContext;

        public HomeController(
            ILogger<HomeController> logger,
            UserManager<User> userManager,
            RoleManager<IdentityRole<int>> roleManager,
            SocialNetworkDbContext dbContext)
        {
            _logger = logger;
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

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
                ProfilePicturePath = user.ProfilePicturePath
            };

            return View("~/Views/Profile/Profile.cshtml", model);
        }

        // POST: /Home/Profile
        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Failed to update profile. Please check the input.";
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
                    TempData["ErrorMessage"] = "Failed to update profile. The current password is incorrect.";
                    return View("~/Views/Profile/Profile.cshtml", model);
                }

                var passwordResult = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                if (!passwordResult.Succeeded)
                {
                    foreach (var error in passwordResult.Errors)
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
                TempData["ErrorMessage"] = "Failed to update profile.";
                return View("~/Views/Profile/Profile.cshtml", model);
            }

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Profile));
        }

        // GET: /Home/MainPage
        public async Task<IActionResult> MainPage()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index");
            }

            var userChats = await _dbContext.ChatUsers
                .Where(cu => cu.ParticipantsId == currentUser.Id)
                .Include(cu => cu.Chat)
                .Select(cu => new
                {
                    ChatId = cu.Chat.Id,
                    ChatName = cu.Chat.Name,
                    IsGroupChat = cu.Chat.IsGroupChat
                })
                .ToListAsync();

            ViewData["Chats"] = userChats;

            return View("~/Views/Mainpage/Mainpage.cshtml");
        }

        // GET: /Home/Contacts
        [HttpGet]
        public async Task<IActionResult> Contacts()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index");
            }

            var friends = await _dbContext.FriendLists
                .Where(f => f.UserId == currentUser.Id || f.FriendId == currentUser.Id)
                .Include(f => f.Friend)
                .ToListAsync();

            var model = new ContactsViewModel
            {
                Contacts = friends.Select(f => new UserViewModel
                {
                    Email = f.Friend.Email,
                    UserName = f.Friend.UserName,
                    ProfilePicturePath = f.Friend.ProfilePicturePath ?? "/images/default.png"
                }).ToList()
            };

            return View("~/Views/Contacts/Contactspage.cshtml", model);
        }

        // GET: /Home/Index
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Home/Privacy
        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        // GET: /Home/Error
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
