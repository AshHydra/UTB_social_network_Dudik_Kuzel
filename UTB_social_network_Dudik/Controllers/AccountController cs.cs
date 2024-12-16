using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UTB_social_network_Dudik.Models;
using Utb_sc_Infrastructure.Identity;

namespace UTB_social_network_Dudik.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Register/Register.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (await _userManager.FindByEmailAsync(model.Email) != null)
                {
                    ModelState.AddModelError("Email", "Email is already taken.");
                    return View(model);
                }

                if (await _userManager.FindByNameAsync(model.UserName) != null)
                {
                    ModelState.AddModelError("UserName", "Username is already taken.");
                    return View(model);
                }

                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PasswordHash = model.PasswordHash
                };

                var result = await _userManager.CreateAsync(user, model.PasswordHash);

                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "User");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
    }
}
