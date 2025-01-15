using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UTB_social_network_Dudik.Models;
using Utb_sc_Infrastructure.Identity;
using Utb_sc_Infrastructure.Database;
using System.IO;
using System;

namespace UTB_social_network_Dudik.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;

        public UserController(UserManager<User> userManager, RoleManager<IdentityRole<int>> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Edit User
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = await _roleManager.Roles.ToListAsync();

            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                RoleIds = string.Join(",", userRoles.Select(r => allRoles.FirstOrDefault(role => role.Name == r)?.Id.ToString()))
            };

            return View("~/Views/Admin/EditUser.cshtml", model);
        }

        // POST: Edit User
        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model, [FromServices] SocialNetworkDbContext dbContext)
        {
            if (ModelState.IsValid)
            {
                // Fetch the user
                var user = await _userManager.FindByIdAsync(model.Id.ToString());
                if (user == null)
                {
                    return NotFound();
                }

                // Update user details
                user.UserName = model.UserName;
                user.Email = model.Email;
                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View("~/Views/Admin/EditUser.cshtml", model);
                }

                // Update roles
                if (!string.IsNullOrEmpty(model.RoleIds))
                {
                    var roleIds = model.RoleIds.Split(',')
                                               .Select(id => int.TryParse(id.Trim(), out var result) ? result : (int?)null)
                                               .Where(id => id.HasValue)
                                               .Select(id => id.Value)
                                               .ToList();

                    var currentRoles = dbContext.UserRoles.Where(ur => ur.UserId == model.Id).ToList();
                    dbContext.UserRoles.RemoveRange(currentRoles);

                    foreach (var roleId in roleIds)
                    {
                        dbContext.UserRoles.Add(new IdentityUserRole<int>
                        {
                            UserId = model.Id,
                            RoleId = roleId
                        });
                    }

                    await dbContext.SaveChangesAsync();
                }

                return RedirectToAction("Admin", "Home");
            }

            return View("~/Views/Admin/EditUser.cshtml", model);
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                Console.WriteLine("User not found");
                return NotFound();
            }

            // Fetch profile picture from session
            var profilePicturePath = HttpContext.Session.GetString("ProfilePicture") ?? "/images/default.png";

            var model = new ProfileViewModel
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                ProfilePicturePath = profilePicturePath
            };

            Console.WriteLine($"User profile loaded: {user.UserName}, Profile Picture Path: {profilePicturePath}");

            return View("~/Views/Profile/Profile.cshtml", model);
        }






        [HttpPost]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);

                if (user == null)
                {
                    Console.WriteLine("User not found during profile update");
                    return NotFound();
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;

                Console.WriteLine($"Updating profile for user: {user.UserName}. First Name: {model.FirstName}, Last Name: {model.LastName}");

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    Console.WriteLine($"Profile for {user.UserName} updated successfully.");
                    return RedirectToAction(nameof(Profile));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    Console.WriteLine($"Error updating profile: {error.Description}");
                }
            }

            return View("~/Views/Profile/Profile.cshtml", model);
        }




        [HttpPost]
        public async Task<IActionResult> UploadProfilePicture(IFormFile ProfilePictureFile)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            if (ProfilePictureFile != null && ProfilePictureFile.Length > 0)
            {
                var fileName = Path.GetFileName(ProfilePictureFile.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

                // Zkontroluj, jestli soubor už existuje, a pokud ano, přejmenuj ho
                if (System.IO.File.Exists(filePath))
                {
                    var fileExtension = Path.GetExtension(fileName);
                    var fileWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                    var counter = 1;
                    while (System.IO.File.Exists(filePath))
                    {
                        fileName = $"{fileWithoutExtension}_{counter++}{fileExtension}";
                        filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);
                    }
                }

                // Uložení souboru na server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfilePictureFile.CopyToAsync(stream);
                }

                // Uložení cesty k souboru do uživatelského profilu
                user.ProfilePicturePath = $"/uploads/{fileName}";
                await _userManager.UpdateAsync(user);

                // Uložení cesty k obrázku do session pro okamžitou změnu zobrazení
                HttpContext.Session.SetString("ProfilePicture", user.ProfilePicturePath);

                TempData["SuccessMessage"] = "Profile picture updated successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "No file selected.";
            }

            return RedirectToAction("Profile");
        }






        // DELETE: User
        [HttpPost]
        public async Task<IActionResult> Delete(int id, [FromServices] SocialNetworkDbContext dbContext)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            // Remove user roles
            var userRoles = dbContext.UserRoles.Where(ur => ur.UserId == id).ToList();
            dbContext.UserRoles.RemoveRange(userRoles);

            // Remove user
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "Failed to delete user.";
                return RedirectToAction("Admin", "Home");
            }

            await dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "User deleted successfully.";
            return RedirectToAction("Admin", "Home");
        }
    }
}
