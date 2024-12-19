using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using UTB_social_network_Dudik.Models;
using Utb_sc_Infrastructure.Identity;
using Utb_sc_Infrastructure.Database;

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
    }
}
