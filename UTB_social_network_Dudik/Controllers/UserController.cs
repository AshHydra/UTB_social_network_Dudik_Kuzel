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
            if (!ModelState.IsValid)
            {
                return View("~/Views/Admin/EditUser.cshtml", model);
            }

            var user = await _userManager.FindByIdAsync(model.Id.ToString());
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

            var updateResult = await _userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("~/Views/Admin/EditUser.cshtml", model);
            }

            // Update roles if provided
            if (!string.IsNullOrEmpty(model.RoleIds))
            {
                // Parse role IDs from the input
                var roleIds = model.RoleIds
                                   .Split(',')
                                   .Select(id => int.TryParse(id.Trim(), out var result) ? result : (int?)null)
                                   .Where(id => id.HasValue)
                                   .Select(id => id.Value)
                                   .ToList();

                // Get all current roles for the user
                var currentRoleNames = await _userManager.GetRolesAsync(user);

                // Find roles to remove
                var currentRoles = _roleManager.Roles.Where(r => currentRoleNames.Contains(r.Name)).ToList();
                var rolesToRemove = currentRoles.Where(r => !roleIds.Contains(r.Id)).Select(r => r.Name).ToList();

                // Find roles to add
                var rolesToAdd = _roleManager.Roles.Where(r => roleIds.Contains(r.Id) && !currentRoleNames.Contains(r.Name))
                                                   .Select(r => r.Name)
                                                   .ToList();

                // Remove roles
                if (rolesToRemove.Any())
                {
                    var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
                    if (!removeResult.Succeeded)
                    {
                        foreach (var error in removeResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View("~/Views/Admin/EditUser.cshtml", model);
                    }
                }

                // Add roles
                if (rolesToAdd.Any())
                {
                    var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
                    if (!addResult.Succeeded)
                    {
                        foreach (var error in addResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View("~/Views/Admin/EditUser.cshtml", model);
                    }
                }
            }

            TempData["SuccessMessage"] = "User updated successfully!";
            return RedirectToAction("Admin", "Home");
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
                    return NotFound();
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;

                if (model.ProfilePictureFile != null && model.ProfilePictureFile.Length > 0)
                {
                    var fileName = Path.GetFileName(model.ProfilePictureFile.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);

                    // Handle duplicate file names
                    if (System.IO.File.Exists(filePath))
                    {
                        var fileExtension = Path.GetExtension(fileName);
                        var baseName = Path.GetFileNameWithoutExtension(fileName);
                        var counter = 1;
                        while (System.IO.File.Exists(filePath))
                        {
                            fileName = $"{baseName}_{counter++}{fileExtension}";
                            filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", fileName);
                        }
                    }

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ProfilePictureFile.CopyToAsync(stream);
                    }

                    user.ProfilePicturePath = $"/uploads/{fileName}";
                }

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction(nameof(Profile));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
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
                TempData["ErrorMessage"] = "Logged-in user not found.";
                return RedirectToAction("Index");
            }

            var friend = await _userManager.FindByEmailAsync(email);
            if (friend == null)
            {
                TempData["ErrorMessage"] = "User with this email not found.";
                return RedirectToAction("Contacts", "Home");
            }

            // Check if friendship already exists
            var existingFriendship = await _dbContext.FriendLists
                .FirstOrDefaultAsync(f =>
                    (f.UserId == currentUser.Id && f.FriendId == friend.Id) ||
                    (f.UserId == friend.Id && f.FriendId == currentUser.Id));

            if (existingFriendship != null)
            {
                TempData["ErrorMessage"] = "This user is already in your friend list.";
                return RedirectToAction("Contacts", "Home");
            }

            // Add friendship
            var newFriendship = new FriendList
            {
                UserId = currentUser.Id,
                FriendId = friend.Id,
                FriendsSince = DateTime.UtcNow
            };

            _dbContext.FriendLists.Add(newFriendship);

            // Automatically create a chat for the two users
            var newChat = new Chat
            {
                Name = $"Chat between {currentUser.UserName} and {friend.UserName}",
                IsGroupChat = false
            };
            _dbContext.Chats.Add(newChat);
            await _dbContext.SaveChangesAsync();

            // Add users to the chat using ChatUsers
            var chatUsers = new List<ChatUsers>
    {
        new ChatUsers
        {
            ChatId = newChat.Id,
            UserId = currentUser.Id,
            JoinedAt = DateTime.UtcNow
        },
        new ChatUsers
        {
            ChatId = newChat.Id,
            UserId = friend.Id,
            JoinedAt = DateTime.UtcNow
        }
    };

            _dbContext.ChatUsers.AddRange(chatUsers);
            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Contact added and chat created.";
            return RedirectToAction("Contacts", "Home");
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
