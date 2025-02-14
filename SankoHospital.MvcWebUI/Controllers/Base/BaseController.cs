using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SankoHospital.Business.Abstract;
using SankoHospital.Core.Security;

namespace SankoHospital.MvcWebUI.Controllers.Base
{
    public class BaseController : Controller
    {
        private readonly IUserService _userManager;
        private readonly IPasswordHasher _passwordHasher;

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
        
        public IActionResult ChangeUsername([FromForm] string newUsername)
    {
        if (_userManager == null)
            return BadRequest(new { success = false, message = "Service not initialized." });

        if (string.IsNullOrEmpty(newUsername))
            return BadRequest(new { success = false, message = "New username cannot be empty." });

        // Kullanıcı adını Session'dan alıyoruz
        var currentUsername = HttpContext.Session.GetString("Username");
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

    public IActionResult ChangePassword([FromForm] string currentPassword, [FromForm] string newPassword)
    {
        if (_userManager == null || _passwordHasher == null)
            return BadRequest(new { success = false, message = "Services not initialized." });

        if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
            return BadRequest(new { success = false, message = "Password fields cannot be empty." });

        // Oturumdan kullanıcı adını alıyoruz
        var currentUsername = HttpContext.Session.GetString("Username");
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

    public IActionResult DeleteAccount()
    {
        if (_userManager == null)
            return BadRequest(new { success = false, message = "Service not initialized." });

        var currentUsername = HttpContext.Session.GetString("Username");
        if (string.IsNullOrEmpty(currentUsername))
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