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
        private readonly IUserService _userManager;
        private readonly IPasswordHasher _passwordHasher;
        // GET: /user/dashboard
        public UserController(IUserService userManager, IPasswordHasher passwordHasher)
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
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
                Role = HttpContext.Session.GetString("UserRole") ?? "Account"  // Varsayılan bir rol değeri
            };

            return View(model);
        }
        
        // POST: /user/change-username
        [HttpPost("change-username")]
        public IActionResult ChangeUsername([FromForm] string newUsername)
        {
            if (string.IsNullOrEmpty(newUsername))
                return BadRequest(new { success = false, message = "New username cannot be empty." });

            // Şu anki kullanıcıyı token ya da HttpContext.User üzerinden alıyoruz.
            var currentUsername = User.Identity?.Name;
            if (string.IsNullOrEmpty(currentUsername))
                return Unauthorized(new { success = false, message = "User not authenticated." });

            // Kullanıcıyı bulun
            var user = _userManager.GetAll().FirstOrDefault(u => u.Username == currentUsername);
            if (user == null)
                return NotFound(new { success = false, message = "User not found." });

            user.Username = newUsername;
            _userManager.Update(user);

            return Ok(new { success = true, message = "Username updated successfully!" });
        }
        
        // POST: /user/change-password
        [HttpPost("change-password")]
        public IActionResult ChangePassword([FromForm] string currentPassword, [FromForm] string newPassword)
        {
            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
                return BadRequest(new { success = false, message = "Password fields cannot be empty." });

            var currentUsername = User.Identity?.Name;
            if (string.IsNullOrEmpty(currentUsername))
                return Unauthorized(new { success = false, message = "User not authenticated." });

            var user = _userManager.GetAll().FirstOrDefault(u => u.Username == currentUsername);
            if (user == null)
                return NotFound(new { success = false, message = "User not found." });

            // Doğru mevcut şifre kontrolü: Authenticate metodunu kullanarak
            var token = _userManager.Authenticate(user.Username, currentPassword);
            if (token == null)
                return BadRequest(new { success = false, message = "Current password is incorrect." });

            // Yeni şifreyi hashleyin ve güncelleyin
            user.PasswordHash = _passwordHasher.HashPassword(newPassword);
            _userManager.Update(user);

            return Ok(new { success = true, message = "Password updated successfully!" });
        }
        
        // DELETE: /user/delete-account
        [HttpDelete("delete-account")]
        public IActionResult DeleteAccount()
        {
            var currentUsername = User.Identity?.Name;
            if (string.IsNullOrEmpty(currentUsername))
                return Unauthorized(new { success = false, message = "User not authenticated." });

            var user = _userManager.GetAll().FirstOrDefault(u => u.Username == currentUsername);
            if (user == null)
                return NotFound(new { success = false, message = "User not found." });

            _userManager.Delete(user);

            // Not: Hesap silindikten sonra kullanıcının oturumunu kapatmak gerekebilir.
            return Ok(new { success = true, message = "Account deleted successfully!" });
        }
    }
}