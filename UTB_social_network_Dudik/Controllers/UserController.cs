using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using UTB_social_network_Dudik.Models;
using Utb_sc_Infrastructure.Identity;

namespace UTB_social_network_Dudik.Controllers
{
    [Authorize] // Ensure only authenticated users can access
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;

        public UserController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // GET: Edit User
        [HttpGet]
        public async Task<IActionResult> Edit(int id) // Change parameter type to int
        {
            var user = await _userManager.FindByIdAsync(id.ToString()); // Use ToString() if FindByIdAsync requires string
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                Id = id, // No conversion needed since both are int
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber
            };

            return View("~/Views/Admin/EditUser.cshtml", model);
        }

        // POST: Edit User
        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id.ToString()); // Convert int Id to string for FindByIdAsync
                if (user == null)
                {
                    return NotFound();
                }

                // Update user properties
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Admin", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View("~/Views/Admin/EditUser.cshtml", model);
        }
    }
}
