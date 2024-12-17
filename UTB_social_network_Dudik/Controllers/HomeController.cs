using Microsoft.AspNetCore.Authorization; // Pro atribut Authorize
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using UTB_social_network_Dudik.Models;

namespace UTB_social_network_Dudik.Controllers
{
    [Authorize] // Zaji��uje, �e v�echny akce v tomto controlleru jsou p��stupn� pouze p�ihl�en�m u�ivatel�m
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        // �vodn� str�nka po p�ihl�en�
        public IActionResult MainPage()
        {
            return View();
        }

        // V�choz� index str�nka
        [AllowAnonymous] // Tuto akci m��e zobrazit kdokoliv
        public IActionResult Index()
        {
            return View();
        }

        // Str�nka s informacemi o ochran� osobn�ch �daj�
        [AllowAnonymous] // Tuto akci m��e zobrazit kdokoliv
        public IActionResult Privacy()
        {
            return View();
        }

        // Zobraz� chybu
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous] // Tuto akci m��e zobrazit kdokoliv
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
