using Microsoft.AspNetCore.Mvc;
using SankoHospital.Business.Abstract;
using SankoHospital.Core.Security;
using SankoHospital.MvcWebUI.Controllers.Base;
using SankoHospital.MvcWebUI.Models.NurseModel;
using SankoHospital.MvcWebUI.Models.UserModels;

namespace SankoHospital.MvcWebUI.Controllers;

[Route("[controller]/[action]")]
public class NurseController : BaseController
{
    private readonly IPatientService _patientService;

    public NurseController(IPatientService patientService, IUserService userManager, IPasswordHasher passwordHasher)
    {
        _patientService = patientService;
    }

    [HttpGet("")]
    public IActionResult Dashboard()
    {
        var model = new NurseDashboardViewModel
        {
            TodaysPatients = 10, // Örnek değer
            PendingTasks = 5, // Örnek değer
            AssignedWards = 2, // Örnek değer
            MedicationsToAdminister = 3 // Örnek değer
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
            Role = HttpContext.Session.GetString("UserRole") ?? "Account" // Varsayılan bir rol değeri
        };

        return View(model);
    }
}