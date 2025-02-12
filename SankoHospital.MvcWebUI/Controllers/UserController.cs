using Microsoft.AspNetCore.Mvc;
using SankoHospital.Business.Abstract;
using SankoHospital.Core.Helpers;
using SankoHospital.MvcWebUI.Controllers.Base;
using SankoHospital.MvcWebUI.Models;
using SankoHospital.MvcWebUI.Models.UserModels;

namespace SankoHospital.MvcWebUI.Controllers
{
    [Route("[controller]/[action]")]
    public class UserController : BaseController
    {
        // GET: /user/dashboard
        public UserController(IUserService userManager, IPasswordHasher passwordHasher)
        {
        }

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
            var model = new UserSettingsViewModel
            {
                Username = HttpContext.Session.GetString("Username") ?? "DefaultUser",
                Role = HttpContext.Session.GetString("UserRole") ?? "Account" // Varsayılan bir rol değeri
            };

            return View(model);
        }
    }
}