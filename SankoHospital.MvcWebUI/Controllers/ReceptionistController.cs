using Microsoft.AspNetCore.Mvc;

namespace SankoHospital.MvcWebUI.Controllers;

public class ReceptionistController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}