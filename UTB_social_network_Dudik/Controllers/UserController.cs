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

        // Method to load user's contacts
        [HttpGet]
        public async Task<IActionResult> LoadContacts([FromServices] SocialNetworkDbContext dbContext)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Načtení všech přátel aktuálního uživatele
            var friends = await dbContext.FriendLists
                .Where(fl => fl.UserId == user.Id && fl.Status == "Active")
                .Select(fl => fl.Friend)
                .ToListAsync();

            // Zobrazení kontaktů ve ViewModelu
            var model = new ContactsViewModel
            {
                Contacts = friends.Select(f => new Utb_sc_Infrastructure.Identity.User
                {
                    UserName = f.UserName,
                    Email = f.Email
                 
                }).ToList()
            };


            return View("~/Views/Contacts/Contactspage.cshtml", model);
        }


        // Method to add a contact for the user
        [HttpPost]
        public async Task<IActionResult> AddContacts(string email, [FromServices] SocialNetworkDbContext dbContext)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Vyhledání přítele podle emailu
            var friend = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email == email); // Hledáme uživatele podle emailu

            if (friend == null)
            {
                TempData["ErrorMessage"] = "Friend not found.";
                return RedirectToAction("LoadContacts");
            }

            var existingFriendship = await dbContext.FriendLists
                .FirstOrDefaultAsync(fl => (fl.UserId == user.Id && fl.FriendId == friend.Id) ||
                                           (fl.UserId == friend.Id && fl.FriendId == user.Id));

            if (existingFriendship != null)
            {
                TempData["ErrorMessage"] = "This user is already your friend.";
                return RedirectToAction("LoadContacts");
            }

            var friendList = new FriendList
            {
                UserId = user.Id,
                FriendId = friend.Id,
                FriendsSince = DateTime.Now,
                Status = "Active"
            };

            dbContext.FriendLists.Add(friendList);
            await dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Friend added successfully!";
            return RedirectToAction("LoadContacts");
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
