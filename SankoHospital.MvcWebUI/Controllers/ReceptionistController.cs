using Microsoft.AspNetCore.Mvc;
using SankoHospital.Business.Abstract;
using SankoHospital.Core.Security;
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

    public ReceptionistController(IPatientService patientManager, IRoomService roomManager, IUserService userManager,
        IPasswordHasher passwordHasher) : base(userManager, passwordHasher)
    {
        _patientManager = patientManager;
        _roomManager = roomManager;
    }

    [HttpGet("")]
    public IActionResult Dashboard()
    {
        // Tüm hastaları ve odaları çekiyoruz
        var allPatients = _patientManager.GetAll();
        var allRooms = _roomManager.GetAll();

        // Today's Admissions: Bugün kabul edilen hastalar
        int todaysAdmissions = allPatients.Count(p => p.AdmissionDate.Date == DateTime.Today);

        // Today's Checkouts: Bugün çıkış yapan hastalar (CheckoutDate değeri varsa ve bugüne aitse)
        int todaysCheckouts = allPatients.Count(p => p.CheckoutDate.HasValue && p.CheckoutDate.Value.Date == DateTime.Today);

        // Total Registered Patients: Kayıtlı toplam hasta sayısı
        int totalRegisteredPatients = allPatients.Count();

        // Rooms Available: Boş ve temizlenmiş odalar (odanın doluluk durumu kapasiteye göre ve durumu "Cleaned" ise)
        int roomsAvailable = allRooms.Count(r => r.CurrentPatientCount < r.Capacity && r.Status == "Cleaned");

        var model = new ReceptionistDashboardViewModel
        {
            TodaysAdmissions = todaysAdmissions,
            TodaysCheckouts = todaysCheckouts,
            RoomsAvailable = roomsAvailable,
            TotalRegisteredPatients = totalRegisteredPatients
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

    [HttpPost]
    public IActionResult CheckoutPatient([FromBody] int patientId)
    {
        var patient = _patientManager.GetById(patientId);
        if (patient == null)
            return NotFound(new { success = false, message = "Patient not found." });

        if (patient.CheckoutDate.HasValue)
            return BadRequest(new { success = false, message = "Patient already checked out." });

        patient.CheckoutDate = DateTime.UtcNow; // veya DateTime.Now
        _patientManager.Update(patient);

        return Ok(new 
        { 
            success = true, 
            message = "Patient checked out successfully.", 
            checkoutDate = patient.CheckoutDate?.ToString("yyyy-MM-dd")
        });
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
            Role = HttpContext.Session.GetString("UserRole") ?? "Account" // Varsayılan bir rol değeri
        };

        return View(model);
    }
}