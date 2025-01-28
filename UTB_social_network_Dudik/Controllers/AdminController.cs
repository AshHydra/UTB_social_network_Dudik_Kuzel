using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Utb_sc_Infrastructure.Database;
using Utb_sc_Infrastructure.Identity;
using Utb_sc_Domain.Entities;

namespace UTB_social_network_Dudik.Controllers
{
    [Authorize(Roles = "Admin")] // Only allow admins
    [Route("Admin")] // Base route for all actions in this controller
    public class AdminController : Controller
    {
        private readonly SocialNetworkDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public AdminController(SocialNetworkDbContext dbContext, UserManager<User> userManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        // **Manage Chats**
        [HttpGet("ManageChats")]
        public async Task<IActionResult> ManageChats()
        {
            var chats = await _dbContext.Chats
                .Include(c => c.Participants)
                .ThenInclude(p => p.User) // Ensure User details are loaded
                .ToListAsync();

            return View("~/Views/Admin/ManageChats.cshtml", chats);
        }

        [HttpPost("EditChat")]
        public async Task<IActionResult> EditChat(int chatId, string newName)
        {
            var chat = await _dbContext.Chats.FindAsync(chatId);
            if (chat == null) return NotFound();

            chat.Name = newName;
            await _dbContext.SaveChangesAsync();
            TempData["SuccessMessage"] = "Chat updated successfully!";
            return RedirectToAction("ManageChats");
        }

        [HttpPost("DeleteChat")]
        public async Task<IActionResult> DeleteChat(int chatId)
        {
            var chat = await _dbContext.Chats.FindAsync(chatId);
            if (chat == null) return NotFound();

            _dbContext.Chats.Remove(chat);
            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Chat deleted successfully!";
            return RedirectToAction("ManageChats");
        }

        // **Manage Messages**
        [HttpGet("ManageMessages")]
        public async Task<IActionResult> ManageMessages()
        {
            var messages = await _dbContext.Messages
                .Include(m => m.Sender)
                .Include(m => m.Chat)
                .ToListAsync();

            return View("~/Views/Admin/ManageMessages.cshtml", messages);
        }

        [HttpPost("EditMessage")]
        public async Task<IActionResult> EditMessage(int messageId, string newContent)
        {
            var message = await _dbContext.Messages.FindAsync(messageId);
            if (message == null) return NotFound();

            message.Content = newContent;
            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Message updated successfully!";
            return RedirectToAction("ManageMessages");
        }

        [HttpPost("DeleteMessage")]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            var message = await _dbContext.Messages.FindAsync(messageId);
            if (message == null) return NotFound();

            _dbContext.Messages.Remove(message);
            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Message deleted successfully!";
            return RedirectToAction("ManageMessages");
        }

        // **Manage Friend Lists**
        [HttpGet("ManageFriendLists")]
        public async Task<IActionResult> ManageFriendLists()
        {
            var friends = await _dbContext.FriendLists
                .Include(f => f.User)
                .Include(f => f.Friend)
                .ToListAsync();

            return View("~/Views/Admin/ManageFriendLists.cshtml", friends);
        }

        [HttpPost("DeleteFriend")]
        public async Task<IActionResult> DeleteFriend(int friendId)
        {
            var friendship = await _dbContext.FriendLists
                .FirstOrDefaultAsync(f => f.FriendId == friendId || f.UserId == friendId);

            if (friendship == null) return NotFound();

            _dbContext.FriendLists.Remove(friendship);
            await _dbContext.SaveChangesAsync();

            TempData["SuccessMessage"] = "Friend removed successfully!";
            return RedirectToAction("ManageFriendLists");
        }
    }
}
