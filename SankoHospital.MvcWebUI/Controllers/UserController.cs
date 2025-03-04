using Microsoft.AspNetCore.Mvc;
using SankoHospital.Business.Abstract;
using SankoHospital.Core.Security;
using SankoHospital.MvcWebUI.Controllers.Base;
using SankoHospital.MvcWebUI.Models;
using SankoHospital.MvcWebUI.Models.UserModels;

namespace SankoHospital.MvcWebUI.Controllers
{
    [Route("[controller]/[action]")]
    public class UserController : BaseController
    {
        private readonly IUserService _userManager;
        // GET: /user/dashboard
        public UserController(IUserService userManager, IPasswordHasher passwordHasher) : base(userManager, passwordHasher)
        {
            _userManager = userManager;
        }

        [HttpGet("")]
        public IActionResult Dashboard()
        {
            return View("Dashboard");
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var username = HttpContext.Session.GetString("Username") ?? "DefaultUser";

            var user = _userManager.GetByUsername(username);
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }
            
            var model = new UserProfileViewModel
            {
                Username = user.Username,
                Role = user.Role,
                CreatedDate = user.CreatedAt
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