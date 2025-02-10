using Microsoft.AspNetCore.Mvc;

namespace SankoHospital.MvcWebUI.Controllers;

public class NurseController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}