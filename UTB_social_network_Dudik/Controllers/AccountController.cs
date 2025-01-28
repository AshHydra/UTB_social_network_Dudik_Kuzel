using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UTB_social_network_Dudik.Models;
using Utb_sc_Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Utb_sc_Infrastructure.Database;

namespace UTB_social_network_Dudik.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly SocialNetworkDbContext _context;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, SocialNetworkDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: Register
        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Register/register.cshtml");
        }

        // POST: Register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Check if password meets length requirement
            if (model.Password.Length < 4)
            {
                TempData["Error"] = "The password must be at least 4 characters long.";
                return RedirectToAction("Register");
            }

            // Check if username already exists
            var existingUser = await _userManager.FindByNameAsync(model.UserName);
            if (existingUser != null)
            {
                ModelState.AddModelError("UserName", "The username is already taken.");
                return View(model);
            }

            // Check if email already exists
            var existingEmail = await _userManager.FindByEmailAsync(model.Email);
            if (existingEmail != null)
            {
                ModelState.AddModelError("Email", "The email is already registered.");
                return View(model);
            }

            // Create user
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Assign the "User" role by default
                await _userManager.AddToRoleAsync(user, "User");

                return RedirectToAction("Login", "Account");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        // GET: Login
        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Home/Index.cshtml");
        }

        // POST: Login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);
                    if (result.Succeeded)
                    {
                        // Uložení emailu a cesty k profilovému obrázku do session
                        HttpContext.Session.SetString("UserEmail", user.Email);
                        HttpContext.Session.SetString("ProfilePicture", user.ProfilePicturePath ?? "/images/default.png");

                        // Po přihlášení přesměrujeme na stránku s kontakty
                        return RedirectToAction("MainPage", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View("~/Views/Home/Index.cshtml", model);
        }

        // Log out
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }
    }
}