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
            if (ModelState.IsValid)
            {
                if (await _userManager.FindByEmailAsync(model.Email) != null)
                {
                    ModelState.AddModelError("Email", "Email is already taken.");
                    return View("~/Views/Register/register.cshtml", model);
                }

                if (await _userManager.FindByNameAsync(model.UserName) != null)
                {
                    ModelState.AddModelError("UserName", "Username is already taken.");
                    return View("~/Views/Register/register.cshtml", model);
                }

                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };

                var result = await _userManager.CreateAsync(user, model.PasswordHash);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("MainPage", "Home"); // Redirect to MainPage
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View("~/Views/Register/register.cshtml", model);
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
                        Console.WriteLine("ID aktuálního uživatele: " + user.Id); // Použij 'user' místo 'currentUser'
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
