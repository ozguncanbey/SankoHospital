using Microsoft.AspNetCore.Mvc;

namespace SankoHospital.MvcWebUI.Controllers;

public class CleanerController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}