using Microsoft.AspNetCore.Mvc;
using SankoHospital.Business.Abstract;
using SankoHospital.Core.Security;
using SankoHospital.MvcWebUI.Controllers.Base;
using SankoHospital.MvcWebUI.Models.NurseModel;
using SankoHospital.MvcWebUI.Models.UserModels;

namespace SankoHospital.MvcWebUI.Controllers
{
    [Route("[controller]/[action]")]
    public class NurseController : BaseController
    {
        private readonly IPatientService _patientManager;

        public NurseController(IPatientService patientManager, IUserService userManager, IPasswordHasher passwordHasher) 
            : base(userManager, passwordHasher)
        {
            _patientManager = patientManager;
        }

        [HttpGet("")]
        public IActionResult Dashboard()
        {
            // Sadece çıkışı yapılmamış hastaları alıyoruz
            var allPatients = _patientManager.GetAll().Where(p => p.CheckoutDate == null);

            // Toplam hasta sayısı
            var todaysPatients = allPatients.Count();

            // Kontrol edilmiş hastaların sayısı
            var completedTasks = allPatients.Count(p => p.Checked);

            // Kontrol edilmemiş hastaların sayısı
            var pendingTasks = allPatients.Count(p => !p.Checked && (p.CheckoutDate == null));

            var model = new NurseDashboardViewModel
            {
                TodaysPatients = todaysPatients,
                CompletedTasks = completedTasks,
                PendingTasks = pendingTasks
            };

            return View(model);
        }
        
        [HttpGet]
        public IActionResult Patients()
        {
            // Sadece çıkışı yapılmamış hastaları getiriyoruz
            var patients = _patientManager.GetAll()
                .Where(p => p.CheckoutDate == null)
                .Select(p => new PatientViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Surname = p.Surname,
                    BloodType = p.BloodType,
                    AdmissionDate = p.AdmissionDate,
                    CheckoutDate = p.CheckoutDate,
                    Checked = p.Checked,
                    BloodPressure = p.BloodPressure, // Yeni eklenen alan
                    Pulse = p.Pulse,                 // Yeni eklenen alan
                    BloodSugar = p.BloodSugar        // Yeni eklenen alan
                }).ToList();

            return View("Patients", patients);
        }

        [HttpPost]
        public IActionResult MarkChecked(int id)
        {
            var patient = _patientManager.GetById(id);
            if (patient == null)
            {
                return NotFound();
            }

            patient.Checked = true;
            _patientManager.Update(patient);
            return Ok();
        }

        [HttpPost]
        public IActionResult SavePatientData([FromBody] PatientViewModel model)
        {
            var patient = _patientManager.GetById(model.Id);
            if (patient == null)
            {
                return NotFound(new { success = false, message = "Patient not found." });
            }

            // Yeni alanları güncelle
            patient.BloodPressure = model.BloodPressure;
            patient.Pulse = model.Pulse;
            patient.BloodSugar = model.BloodSugar;
    
            // Kaydetme işlemi aynı zamanda kontrol edildi olarak işaretleyebilir
            patient.Checked = model.Checked;

            _patientManager.Update(patient);

            return Ok(new { success = true, message = "Patient data saved successfully." });
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
                Role = HttpContext.Session.GetString("UserRole") ?? "Account"
            };

            return View(model);
        }
    }
}
