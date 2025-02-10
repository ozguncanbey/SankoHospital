using Microsoft.AspNetCore.Mvc;
using SankoHospital.MvcWebUI.Models;

namespace SankoHospital.MvcWebUI.Controllers
{
    [Route("user")]
    public class UserController : BaseController
    {
        // GET: /user/dashboard
        [HttpGet("dashboard")]
        public IActionResult Dashboard()
        {
            // Oturumdan kullanıcı adını alıyoruz. (Login sırasında session'a kaydedilmiş olmalı.)
            var username = HttpContext.Session.GetString("Username") ?? "DefaultUser";
            
            var model = new UserDashboardViewModel
            {
                Username = username
            };
            
            return View("UserDashboard", model);
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

    }
}