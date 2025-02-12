using Microsoft.AspNetCore.Mvc;
using SankoHospital.Business.Abstract;
using SankoHospital.Core.Helpers;
using SankoHospital.Entities.Concrete;
using SankoHospital.MvcWebUI.Controllers.Base;
using SankoHospital.MvcWebUI.Models.CleanerModel;
using SankoHospital.MvcWebUI.Models.NurseModel;
using SankoHospital.MvcWebUI.Models.ReceptionistModel;
using SankoHospital.MvcWebUI.Models.UserModels;

namespace SankoHospital.MvcWebUI.Controllers;

[Route("[controller]/[action]")]
public class ReceptionistController : BaseController
{
    private readonly IPatientService _patientManager;
    private readonly IRoomService _roomManager;
    private readonly IUserService _userManager;
    private readonly IPasswordHasher _passwordHasher;

    public ReceptionistController(IPatientService patientManager, IRoomService roomManager, IUserService userManager, IPasswordHasher passwordHasher)
    {
        _patientManager = patientManager;
        _roomManager = roomManager;
        _userManager = userManager;
        _passwordHasher = passwordHasher;
    }

    [HttpGet("")]
    public IActionResult Dashboard()
    {
        var model = new ReceptionistDashboardViewModel
        {
            TodaysAppointments = 12,  // Örnek değer
            CheckIns = 9,             // Örnek değer
            WaitingPatients = 3,      // Örnek değer
            TotalPatients = 200       // Örnek değer
        };

        return View(model);
    }


    // Receptionist için Patients sayfası
    [HttpGet]
    public IActionResult Patients()
    {
        var patients = _patientManager.GetAll()
            .Select(p => new PatientViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Surname = p.Surname,
                BloodType = p.BloodType,
                AdmissionDate = p.AdmissionDate,
                CheckoutDate = p.CheckoutDate,
                Checked = p.Checked,
                RoomId = p.RoomId, 
                RoomNumber = _roomManager.GetById(p.RoomId)?.RoomNumber.ToString() ?? "Not Assigned"
            }).ToList();

        var availableRooms = _roomManager.GetAll()
            .Where(r => r.CurrentPatientCount < r.Capacity) // 🔥 SADECE UYGUN ODALAR
            .Select(r => new RoomViewModel
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                Capacity = r.Capacity,
                CurrentPatientCount = r.CurrentPatientCount
            }).ToList();

        var model = new PatientsViewModel
        {
            Patients = patients,
            AvailableRooms = availableRooms
        };

        return View(model);
    }

    // Receptionist için Rooms sayfası (sadece görüntüleme)
    [HttpGet]
    public IActionResult Rooms()
    {
        var rooms = _roomManager.GetAll()
            .Select(r => new RoomViewModel
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                Capacity = r.Capacity,
                CurrentPatientCount = r.CurrentPatientCount,
                LastCleanedDate = r.LastCleanedDate,
                Status = r.Status
            }).ToList();

        return View(rooms);
    }

    // Receptionist hasta ekleyebilir
    [HttpPost]
    public IActionResult AddPatient([FromBody] PatientViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest("Invalid data.");

        var room = _roomManager.GetById(model.RoomId);
        if (room == null) return NotFound("Room not found.");
        if (room.CurrentPatientCount >= room.Capacity)
            return BadRequest("Room is full.");

        var newPatient = new Patient
        {
            Name = model.Name,
            Surname = model.Surname,
            BloodType = model.BloodType,
            AdmissionDate = model.AdmissionDate,
            RoomId = model.RoomId, // ✅ Odayı hastaya atıyoruz
            Checked = false
        };

        try
        {
            _patientManager.Add(newPatient);
            
            room.CurrentPatientCount++;
            _roomManager.Update(room);

            return Ok(new { success = true, message = "Patient added successfully.", patient = newPatient });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    // Receptionist hastayı güncelleyebilir
    [HttpPost]
public IActionResult UpdatePatient([FromBody] PatientViewModel model)
{
    if (!ModelState.IsValid)
    {
        return BadRequest(new { success = false, message = "Invalid data." });
    }
    
    try 
    {
        var existingPatient = _patientManager.GetById(model.Id);
        if (existingPatient == null)
        {
            return NotFound(new { success = false, message = "Patient not found." });
        }

        if (existingPatient.RoomId != model.RoomId)
        {
            // Eski oda: sayıyı azalt
            var oldRoom = _roomManager.GetById(existingPatient.RoomId);
            if (oldRoom != null)
            {
                oldRoom.CurrentPatientCount--;
                _roomManager.Update(oldRoom);
            }
            
            // Yeni oda: kapasite kontrolü ve sayıyı artır
            var newRoom = _roomManager.GetById(model.RoomId);
            if (newRoom != null)
            {
                if (newRoom.CurrentPatientCount >= newRoom.Capacity)
                {
                    return BadRequest(new { success = false, message = "New room is full." });
                }
                newRoom.CurrentPatientCount++;
                _roomManager.Update(newRoom);
            }
        }

        existingPatient.Name = model.Name;
        existingPatient.Surname = model.Surname;
        existingPatient.BloodType = model.BloodType;
        existingPatient.AdmissionDate = model.AdmissionDate;
        existingPatient.RoomId = model.RoomId;

        _patientManager.Update(existingPatient);

        var updatedRoom = _roomManager.GetById(existingPatient.RoomId);
        string roomNumber = updatedRoom?.RoomNumber.ToString() ?? "Not Assigned";

        var updatedPatient = new
        {
            existingPatient.Id,
            existingPatient.Name,
            existingPatient.Surname,
            existingPatient.BloodType,
            AdmissionDate = existingPatient.AdmissionDate,
            RoomId = existingPatient.RoomId,
            RoomNumber = roomNumber
        };

        return Ok(new { success = true, message = "Patient updated successfully.", patient = updatedPatient });
    }
    catch (Exception ex)
    {
        return BadRequest(new { success = false, message = ex.Message });
    }
}

    // Receptionist hasta silebilir
    [HttpDelete("{id:int}")]
    public IActionResult DeletePatient(int id)
    {
        var patient = _patientManager.GetById(id);
        if (patient == null) return NotFound("Patient not found.");

        try
        {
            // Eğer hasta bir odaya atanmışsa, oda doluluk bilgisini güncelle
            if (patient.RoomId != null)
            {
                var room = _roomManager.GetById(patient.RoomId);
                if (room != null)
                {
                    room.CurrentPatientCount--;
                    _roomManager.Update(room);
                }
            }

            _patientManager.Delete(patient);
            return Ok(new { success = true, message = "Patient deleted successfully." });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
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
    
    // POST: /receptionist/change-username
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
        
        // POST: /receptionist/change-password
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
        
        // DELETE: /receptionist/delete-account
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