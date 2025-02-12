using Microsoft.AspNetCore.Mvc;
using SankoHospital.Business.Abstract;
using SankoHospital.Core.Helpers;
using SankoHospital.MvcWebUI.Controllers.Base;
using SankoHospital.MvcWebUI.Models.NurseModel;
using SankoHospital.MvcWebUI.Models.UserModels;

namespace SankoHospital.MvcWebUI.Controllers;

[Route("[controller]/[action]")]
public class NurseController : BaseController
{
    private readonly IPatientService _patientService;
    private readonly IUserService _userManager;
    private readonly IPasswordHasher _passwordHasher;

    public NurseController(IPatientService patientService, IUserService userManager, IPasswordHasher passwordHasher)
    {
        _patientService = patientService;
        _userManager = userManager;
        _passwordHasher = passwordHasher;
    }

    [HttpGet("")]
    public IActionResult Dashboard()
    {
        var model = new NurseDashboardViewModel
        {
            TodaysPatients = 10,            // Örnek değer
            PendingTasks = 5,               // Örnek değer
            AssignedWards = 2,              // Örnek değer
            MedicationsToAdminister = 3     // Örnek değer
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Patients()
    {
        var patients = _patientService.GetAll().Select(p => new PatientViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Surname = p.Surname,
            BloodType = p.BloodType,
            AdmissionDate = p.AdmissionDate,
            CheckoutDate = p.CheckoutDate,
            Checked = p.Checked
        }).ToList();

        return View("Patients", patients);
    }

    [HttpPost]
    public IActionResult MarkChecked(int id)
    {
        var patient = _patientService.GetById(id);
        if (patient == null)
        {
            return NotFound();
        }

        patient.Checked = true;
        _patientService.Update(patient);
        return Ok();
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
    
    // POST: /nurse/change-username
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
        
        // POST: /nurse/change-password
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
        
        // DELETE: /nurse/delete-account
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