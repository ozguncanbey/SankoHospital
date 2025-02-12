using Microsoft.AspNetCore.Mvc;
using SankoHospital.Business.Abstract;
using SankoHospital.Core.Helpers;
using SankoHospital.MvcWebUI.Controllers.Base;
using SankoHospital.MvcWebUI.Models.CleanerModel;
using SankoHospital.MvcWebUI.Models.UserModels;

namespace SankoHospital.MvcWebUI.Controllers;

[Route("[controller]/[action]")]
public class CleanerController : BaseController
{
    private readonly IRoomService _roomService;
    private readonly IUserService _userManager;
    private readonly IPasswordHasher _passwordHasher;

    public CleanerController(IRoomService roomService, IUserService userManager, IPasswordHasher passwordHasher)
    {
        _roomService = roomService;
        _userManager = userManager;
        _passwordHasher = passwordHasher;
    }

    [HttpGet("")]
    public IActionResult Dashboard()
    {
        var model = new CleanerDashboardViewModel
        {
            TodaysCleaningTasks = 8,   // Örnek değer
            CompletedTasks = 5,        // Örnek değer
            PendingTasks = 3,          // Örnek değer
            AssignedAreas = 4          // Örnek değer
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Rooms()
    {
        var rooms = _roomService.GetAll().Select(r => new RoomViewModel
        {
            Id = r.Id,
            RoomNumber = r.RoomNumber,
            Capacity = r.Capacity,
            CurrentPatientCount = r.CurrentPatientCount,
            Status = r.Status,
            LastCleanedDate = r.LastCleanedDate
        }).ToList();

        return View("Rooms", rooms);
    }

    [HttpPost]
    public IActionResult UpdateRoomStatus(int id, string status)
    {
        try
        {
            var room = _roomService.GetById(id);
            if (room == null)
                return NotFound(new { message = "Room not found" });

            // Eğer oda "Cleaned" durumuna geçiyorsa, LastCleanedDate güncellensin
            if (status == "Cleaned")
            {
                room.LastCleanedDate = DateTime.UtcNow; // UTC kullanarak saat farklarını önlüyoruz
            }

            room.Status = status;
            _roomService.Update(room);

            // Değişikliklerin veritabanına yansıdığını doğrulamak için tekrar çekelim
            var updatedRoom = _roomService.GetById(id);

            return Ok(new
            {
                message = "Room status updated successfully",
                newStatus = updatedRoom.Status,
                lastCleanedDate = updatedRoom.LastCleanedDate?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while updating room status.", error = ex.Message });
        }
    }


    [HttpGet]
    public IActionResult Profile()
    {
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
    
    // POST: /admin/change-username
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
        
        // POST: /admin/change-password
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
        
        // DELETE: /admin/delete-account
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