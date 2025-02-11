using Microsoft.AspNetCore.Mvc;
using SankoHospital.Business.Abstract;
using SankoHospital.Entities.Concrete;
using SankoHospital.MvcWebUI.Controllers.Base;
using SankoHospital.MvcWebUI.Models;

namespace SankoHospital.MvcWebUI.Controllers;

[Route("[controller]/[action]")]
public class ReceptionistController : BaseController
{
    private readonly IPatientService _patientManager;
    private readonly IRoomService _roomManager;

    public ReceptionistController(IPatientService patientManager, IRoomService roomManager)
    {
        _patientManager = patientManager;
        _roomManager = roomManager;
    }

    [HttpGet("")]
    public IActionResult Dashboard()
    {
        return View("Dashboard");
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
                Checked = p.Checked
            }).ToList();

        return View(patients);
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
    public IActionResult AddPatient([FromBody] PatientViewModel model)  // [FromBody] ekleyin
    {
        if (!ModelState.IsValid) 
        {
            return BadRequest(ModelState); // Hata detaylarını görmek için ModelState'i döndürün
        }

        var newPatient = new Patient
        {
            Name = model.Name,
            Surname = model.Surname,
            BloodType = model.BloodType,
            AdmissionDate = model.AdmissionDate,
            Checked = false  // Yeni hasta olduğu için false
        };

        try 
        {
            _patientManager.Add(newPatient);
            return Ok(newPatient);  // Başarılı sonuç döndür
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Receptionist hastayı güncelleyebilir
    [HttpPost]
    //[Route("UpdatePatient")] // Route ekleyelim
    public IActionResult UpdatePatient([FromBody] PatientViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try 
        {
            var existingPatient = _patientManager.GetById(model.Id);
            if (existingPatient == null)
            {
                return NotFound("Patient not found.");
            }

            existingPatient.Name = model.Name;
            existingPatient.Surname = model.Surname;
            existingPatient.BloodType = model.BloodType;
            existingPatient.AdmissionDate = model.AdmissionDate;

            _patientManager.Update(existingPatient);
        
            return Ok(existingPatient);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Receptionist hasta silebilir
    [HttpPost]
    public IActionResult DeletePatient(int id)
    {
        var patient = _patientManager.GetById(id);
        if (patient == null) return NotFound("Patient not found.");

        _patientManager.Delete(patient);
        return RedirectToAction("Patients");
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
        return View();
    }
}