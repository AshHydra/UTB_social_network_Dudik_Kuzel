using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UTB_social_network_Dudik.Models;
using Utb_sc_Infrastructure.Identity;


namespace UTB_social_network_Dudik.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;

        public AccountController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    UserName = model.Nickname,
                    Email = model.Email
                };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Můžeš přidat informaci o úspěchu
                    ViewBag.Message = "Registration successful!";
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            // Pokud je něco špatně, nebo úspěšná registrace, vrátí stejnou stránku
            return View(model);
        }
    }
}
