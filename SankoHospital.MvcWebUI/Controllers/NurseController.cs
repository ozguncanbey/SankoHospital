using Microsoft.AspNetCore.Mvc;
using SankoHospital.Business.Abstract;
using SankoHospital.MvcWebUI.Controllers.Base;
using SankoHospital.MvcWebUI.Models.NurseModel;
using SankoHospital.MvcWebUI.Models.UserModels;

namespace SankoHospital.MvcWebUI.Controllers;

[Route("[controller]/[action]")]
public class NurseController : BaseController
{
    private readonly IPatientService _patientService;

    public NurseController(IPatientService patientService)
    {
        _patientService = patientService;
    }

    [HttpGet("")]
    public IActionResult Dashboard()
    {
        return View("Dashboard");
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
        return View();
    }
}