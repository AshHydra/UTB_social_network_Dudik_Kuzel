using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UTB_social_network_Dudik.Models;
using Utb_sc_Infrastructure.Identity;
using Utb_sc_Infrastructure.Database;
using Utb_sc_Domain.Entities;

namespace UTB_social_network_Dudik.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<Utb_sc_Domain.Entities.User> _userManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly SocialNetworkDbContext _dbContext;

        public UserController(UserManager<Utb_sc_Domain.Entities.User> userManager, RoleManager<IdentityRole<int>> roleManager, SocialNetworkDbContext dbContext)
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

        // GET: User Profile
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            // Získání aktuálního přihlášeného uživatele
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound(); // Pokud uživatel neexistuje, vrať 404
            }

            // Naplnění modelu pro zobrazení
            var model = new ProfileViewModel
            {
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email
            };

            return View("~/Views/Profile/Profile.cshtml", model);
        }

        // POST: User Profile
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

        // GET: User Contacts Page
        [HttpGet]
        public async Task<IActionResult> Contacts()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var contacts = await _dbContext.FriendLists
                .Where(fl => fl.UserId == user.Id)
                .Include(fl => fl.Friend)
                .Select(fl => fl.Friend)
                .ToListAsync();

            var model = new ContactsViewModel
            {
                Contacts = contacts
            };

            return View("~/Views/Contacts/Contactspage.cshtml", model);
        }

        // POST: Add a contact
        [HttpPost]
        public async Task<IActionResult> AddContact(string email)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var friend = await _userManager.FindByEmailAsync(email);

            if (friend == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(Contacts));
            }

            if (friend.Id == user.Id)
            {
                TempData["ErrorMessage"] = "You cannot add yourself as a contact.";
                return RedirectToAction(nameof(Contacts));
            }

            var existingContact = await _dbContext.FriendLists
                .FirstOrDefaultAsync(fl => fl.UserId == user.Id && fl.FriendId == friend.Id);

            if (existingContact != null)
            {
                TempData["ErrorMessage"] = "You are already friends with this user.";
                return RedirectToAction(nameof(Contacts));
            }

            _dbContext.FriendLists.Add(new FriendList
            {
                UserId = user.Id,
                FriendId = friend.Id
            });

            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Contact added successfully.";
            return RedirectToAction(nameof(Contacts));
        }
    }
}
