using Microsoft.AspNetCore.Mvc;
using SankoHospital.MvcWebUI.Controllers.Base;
using SankoHospital.MvcWebUI.Models;

namespace SankoHospital.MvcWebUI.Controllers;

[Route("[controller]/[action]")]
public class NurseController : BaseController
{
    [HttpGet("")]
    public IActionResult Dashboard()
    {
        return View("Dashboard");
    }
    
    [HttpGet]
    public IActionResult Profile()
    {
        // Kullanıcı adını ve rolü session'dan alıyoruz.
        var username = HttpContext.Session.GetString("Username") ?? "DefaultUser";
        var role = HttpContext.Session.GetString("UserRole") ?? "User";
    
        var model = new UserProfileViewModel
        {
            Username = username,
            Role = role
        };

        return View(model);
    }
    
    [HttpGet]
    public IActionResult Settings()
    {
        return View();
    }
}