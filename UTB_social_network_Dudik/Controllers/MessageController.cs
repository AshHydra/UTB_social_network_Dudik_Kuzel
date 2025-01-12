using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using Utb_sc_Infrastructure.Database;
using Utb_sc_Domain.Entities;

namespace UTB_social_network_Dudik.Controllers
{
    [Authorize]
    public class MessageController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SocialNetworkDbContext _dbContext;

        public MessageController(UserManager<User> userManager, SocialNetworkDbContext dbContext)
        {
            _userManager = userManager;
            _dbContext = dbContext;
        }

        // GET: Zobrazení všech zpráv aktuálního uživatele
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var messages = _dbContext.Messages
                .Where(m => m.SenderId == currentUser.Id || m.Chat.Participants.Any(u => u.Id == currentUser.Id))
                .OrderByDescending(m => m.SentAt)
                .ToList();

            return View(messages);
        }

        // GET: Zobrazení formuláře pro odeslání zprávy
        [HttpGet]
        public IActionResult SendMessage()
        {
            ViewBag.Users = _dbContext.Users.ToList();
            return View();
        }

        // POST: Odeslání zprávy
        [HttpPost]
        public async Task<IActionResult> SendMessage(int recipientId, string content)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null || string.IsNullOrEmpty(content))
            {
                ModelState.AddModelError("", "Zpráva musí mít příjemce a obsah.");
                return View();
            }

            // Najdi existující chat nebo vytvoř nový
            var chat = _dbContext.Chats.FirstOrDefault(c =>
                c.Participants.Any(u => u.Id == currentUser.Id) &&
                c.Participants.Any(u => u.Id == recipientId));

            if (chat == null)
            {
                chat = new Chat
                {
                    IsGroupChat = false
                };
                chat.Participants.Add(new Utb_sc_Domain.Entities.User
                {
                    Id = currentUser.Id,
                    UserName = currentUser.UserName,
                    Email = currentUser.Email
                });

                // Najdi příjemce
                var recipient = _dbContext.Users.Find(recipientId);

                if (recipient == null)
                {
                    ModelState.AddModelError("", "Příjemce zprávy nebyl nalezen.");
                    return View();
                }

                chat.Participants.Add(new Utb_sc_Domain.Entities.User
                {
                    Id = recipient.Id,
                    UserName = recipient.UserName,
                    Email = recipient.Email
                });

                _dbContext.Chats.Add(chat);
            }

            // Přidání zprávy
            var message = new Message
            {
                Content = content,
                SentAt = DateTime.Now,
                SenderId = currentUser.Id,
                Chat = chat
            };

            _dbContext.Messages.Add(message);
            await _dbContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
