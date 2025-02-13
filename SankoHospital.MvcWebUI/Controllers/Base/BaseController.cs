using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SankoHospital.Business.Abstract;
using SankoHospital.Core.Security;

namespace SankoHospital.MvcWebUI.Controllers.Base
{
    public class BaseController : Controller
    {
        protected readonly IUserService _userManager;
        protected readonly IPasswordHasher _passwordHasher;

        public BaseController(IUserService userManager = null, IPasswordHasher passwordHasher = null)
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            ViewData["Username"] = HttpContext.Session.GetString("Username") ?? "Guest";
            ViewData["UserRole"] = HttpContext.Session.GetString("UserRole") ?? "User";
            
            base.OnActionExecuting(context);
        }

        [HttpPost]
        public virtual IActionResult ChangeUsername([FromForm] string newUsername)
        {
            if (_userManager == null)
                return BadRequest(new { success = false, message = "Service not initialized." });

            if (string.IsNullOrEmpty(newUsername))
                return BadRequest(new { success = false, message = "New username cannot be empty." });

            var currentUsername = User.Identity?.Name;
            if (string.IsNullOrEmpty(currentUsername))
                return Unauthorized(new { success = false, message = "User not authenticated." });

            var user = _userManager.GetAll().FirstOrDefault(u => u.Username == currentUsername);
            if (user == null)
                return NotFound(new { success = false, message = "User not found." });

            user.Username = newUsername;
            _userManager.Update(user);

            // Session'ı güncelle
            HttpContext.Session.SetString("Username", newUsername);

            return Ok(new { success = true, message = "Username updated successfully!" });
        }

        [HttpPost]
        public virtual IActionResult ChangePassword([FromForm] string currentPassword, [FromForm] string newPassword)
        {
            if (_userManager == null || _passwordHasher == null)
                return BadRequest(new { success = false, message = "Services not initialized." });

            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
                return BadRequest(new { success = false, message = "Password fields cannot be empty." });

            var currentUsername = User.Identity?.Name;
            if (string.IsNullOrEmpty(currentUsername))
                return Unauthorized(new { success = false, message = "User not authenticated." });

            var user = _userManager.GetAll().FirstOrDefault(u => u.Username == currentUsername);
            if (user == null)
                return NotFound(new { success = false, message = "User not found." });

            var token = _userManager.Authenticate(user.Username, currentPassword);
            if (token == null)
                return BadRequest(new { success = false, message = "Current password is incorrect." });

            user.PasswordHash = _passwordHasher.HashPassword(newPassword);
            _userManager.Update(user);

            return Ok(new { success = true, message = "Password updated successfully!" });
        }

        [HttpDelete]
        public virtual IActionResult DeleteAccount()
        {
            if (_userManager == null)
                return BadRequest(new { success = false, message = "Service not initialized." });

            var currentUsername = User.Identity?.Name;
            if (string.IsNullOrEmpty(currentUsername))
                return Unauthorized(new { success = false, message = "User not authenticated." });

            var user = _userManager.GetAll().FirstOrDefault(u => u.Username == currentUsername);
            if (user == null)
                return NotFound(new { success = false, message = "User not found." });

            _userManager.Delete(user);

            // Session'ı temizle
            HttpContext.Session.Clear();

            return Ok(new { success = true, message = "Account deleted successfully!" });
        }
    }
}