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
using Utb_sc_Domain.Entities;

namespace UTB_social_network_Dudik.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<Utb_sc_Infrastructure.Identity.User> _userManager; // Opravený UserManager
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly SocialNetworkDbContext _dbContext;

        public UserController(UserManager<Utb_sc_Infrastructure.Identity.User> userManager, RoleManager<IdentityRole<int>> roleManager, SocialNetworkDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
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
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id.ToString());
                if (user == null)
                {
                    return NotFound();
                }

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

                if (!string.IsNullOrEmpty(model.RoleIds))
                {
                    var roleIds = model.RoleIds.Split(',')
                                               .Select(id => int.TryParse(id.Trim(), out var result) ? result : (int?)null)
                                               .Where(id => id.HasValue)
                                               .Select(id => id.Value)
                                               .ToList();

                    var currentRoles = _dbContext.UserRoles.Where(ur => ur.UserId == model.Id).ToList();
                    _dbContext.UserRoles.RemoveRange(currentRoles);

                    foreach (var roleId in roleIds)
                    {
                        _dbContext.UserRoles.Add(new IdentityUserRole<int>
                        {
                            UserId = model.Id,
                            RoleId = roleId
                        });
                    }

                    await _dbContext.SaveChangesAsync();
                }

                return RedirectToAction("Admin", "Home");
            }

            return View("~/Views/Admin/EditUser.cshtml", model);
        }


        // GET: Profile
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                Console.WriteLine("User not found");
                return NotFound();
            }

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

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfilePictureFile.CopyToAsync(stream);
                }

                user.ProfilePicturePath = $"/uploads/{fileName}";
                await _userManager.UpdateAsync(user);
                HttpContext.Session.SetString("ProfilePicture", user.ProfilePicturePath);

                TempData["SuccessMessage"] = "Profile picture updated successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = "No file selected.";
            }

            return RedirectToAction("Profile");
        }

        [HttpPost]
        public async Task<IActionResult> AddContacts(string email)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                TempData["ErrorMessage"] = "Přihlášený uživatel nebyl nalezen.";
                return RedirectToAction("Index");
            }

            var friend = await _userManager.FindByEmailAsync(email);
            if (friend == null)
            {
                TempData["ErrorMessage"] = "Uživatel s tímto emailem nebyl nalezen.";
                return RedirectToAction("Contacts", "Home"); // Přesměrování na HomeController
            }

            // Kontrola, zda již přátelství existuje
            var existingFriendship = await _dbContext.FriendLists
                .FirstOrDefaultAsync(f =>
                    (f.UserId == currentUser.Id && f.FriendId == friend.Id) ||
                    (f.UserId == friend.Id && f.FriendId == currentUser.Id));

            if (existingFriendship != null)
            {
                TempData["ErrorMessage"] = "Tento uživatel je již ve vašem seznamu přátel.";
                return RedirectToAction("Contacts", "Home"); // Přesměrování na HomeController
            }

            // Přidání nového přátelství
            var newFriendship = new FriendList
            {
                UserId = currentUser.Id,
                FriendId = friend.Id,
                FriendsSince = DateTime.UtcNow // Nastavení data přátelství
            };

            _dbContext.FriendLists.Add(newFriendship);
            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Kontakt byl úspěšně přidán.";
            return RedirectToAction("Contacts", "Home"); // Přesměrování na HomeController
        }




        // DELETE: User
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = _dbContext.UserRoles.Where(ur => ur.UserId == id).ToList();
            _dbContext.UserRoles.RemoveRange(userRoles);

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "Failed to delete user.";
                return RedirectToAction("Admin", "Home");
            }

            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "User deleted successfully.";
            return RedirectToAction("Admin", "Home");
        }
    }
}
