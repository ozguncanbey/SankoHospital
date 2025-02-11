using Microsoft.AspNetCore.Mvc;
using SankoHospital.Business.Abstract;
using SankoHospital.Entities.Concrete;
using SankoHospital.MvcWebUI.Controllers.Base;
using SankoHospital.MvcWebUI.Models;

namespace SankoHospital.MvcWebUI.Controllers;

[Route("[controller]/[action]")]
public class ReceptionistController : BaseController
{
    private readonly IPatientService _patientService;
    private readonly IRoomService _roomService;

    public ReceptionistController(IPatientService patientService, IRoomService roomService)
    {
        _patientService = patientService;
        _roomService = roomService;
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
        var patients = _patientService.GetAll()
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
        var rooms = _roomService.GetAll()
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
    public IActionResult AddPatient(PatientViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest("Invalid data.");

        var newPatient = new Patient
        {
            Name = model.Name,
            Surname = model.Surname,
            BloodType = model.BloodType,
            AdmissionDate = model.AdmissionDate,
            CheckoutDate = model.CheckoutDate,
            Checked = false
        };

        _patientService.Add(newPatient);
        return RedirectToAction("Patients");
    }

    // Receptionist hastayı güncelleyebilir
    [HttpPost]
    public IActionResult UpdatePatient(PatientViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest("Invalid data.");

        var existingPatient = _patientService.GetById(model.Id);
        if (existingPatient == null) return NotFound("Patient not found.");

        existingPatient.Name = model.Name;
        existingPatient.Surname = model.Surname;
        existingPatient.BloodType = model.BloodType;
        existingPatient.AdmissionDate = model.AdmissionDate;
        existingPatient.CheckoutDate = model.CheckoutDate;

        _patientService.Update(existingPatient);
        return RedirectToAction("Patients");
    }

    // Receptionist hasta silebilir
    [HttpPost]
    public IActionResult DeletePatient(int id)
    {
        var patient = _patientService.GetById(id);
        if (patient == null) return NotFound("Patient not found.");

        _patientService.Delete(patient);
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
