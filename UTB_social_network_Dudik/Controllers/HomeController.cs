using Microsoft.AspNetCore.Authorization; // Pro atribut Authorize
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using UTB_social_network_Dudik.Models;

namespace UTB_social_network_Dudik.Controllers
{
    [Authorize] // Zajišuje, e všechny akce v tomto controlleru jsou pøístupné pouze pøihlášenım uivatelùm
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public IActionResult Contacts()
        {
            return View("~/Views/Contacts/Contactspage.cshtml");
        }

        public IActionResult Profile()
        {
            return View("~/Views/Profile/Profile.cshtml");
        }

        public IActionResult Admin()
        {
            return View("~/Views/Admin/Adminpage.cshtml");
        }

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // Úvodní stránka po pøihlášení
        public IActionResult MainPage()
        {
            return View("~/Views/Mainpage/Mainpage.cshtml");
        }

        // Vıchozí index stránka
        [AllowAnonymous] // Tuto akci mùe zobrazit kdokoliv
        public IActionResult Index()
        {
            return View();
        }

        // Stránka s informacemi o ochranì osobních údajù
        [AllowAnonymous] // Tuto akci mùe zobrazit kdokoliv
        public IActionResult Privacy()
        {
            return View();
        }

        // Zobrazí chybu
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous] // Tuto akci mùe zobrazit kdokoliv
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
