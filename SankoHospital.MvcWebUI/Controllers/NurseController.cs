using Microsoft.AspNetCore.Mvc;
using SankoHospital.MvcWebUI.Models;

namespace SankoHospital.MvcWebUI.Controllers;

[Route("[controller]/[action]")] //DURUMA BAK//
public class NurseController : Controller
{
    [HttpGet("")]
    public Task<IActionResult> Dashboard()
    {
        return Task.FromResult<IActionResult>(View());
    }
    
    [HttpGet("profile")]
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
    
    [HttpGet("settings")]
    public IActionResult Settings()
    {
        return View();
    }
}