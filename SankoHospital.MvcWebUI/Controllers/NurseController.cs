using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SankoHospital.Business.Abstract;
using SankoHospital.Core.Security;
using SankoHospital.MvcWebUI.Controllers.Base;
using SankoHospital.MvcWebUI.Models.FilterModels;
using SankoHospital.MvcWebUI.Models.NurseModel;
using SankoHospital.MvcWebUI.Models.UserModels;
using System.Linq;
using SankoHospital.Entities.Concrete;


namespace SankoHospital.MvcWebUI.Controllers
{
    [Route("[controller]/[action]")]
    public class NurseController : BaseController
    {
        private readonly IUserService _userManager;
        private readonly IPatientService _patientManager;
        private readonly IRoomService _roomManager;
        private readonly IPatientDailyRecordService _patientDailyRecordManager;

        public NurseController(IPatientService patientManager, IRoomService roomManager, IUserService userManager,
            IPasswordHasher passwordHasher, IPatientDailyRecordService patientDailyRecordManager)
            : base(userManager, passwordHasher)
        {
            _patientManager = patientManager;
            _roomManager = roomManager;
            _userManager = userManager;
            _patientDailyRecordManager = patientDailyRecordManager;
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
        public IActionResult Patients(
            int? id,
            string name,
            string surname,
            string bloodType,
            DateTime? admissionDate,
            int? roomNumber)
        {
            // Sadece çıkışı yapılmamış hastaları alıyoruz.
            var patientsQuery = _patientManager.GetAll().Where(p => p.CheckoutDate == null);

            // Filtre uygulamaları:
            if (id.HasValue)
            {
                patientsQuery = patientsQuery.Where(p => p.Id == id.Value);
            }

            if (!string.IsNullOrEmpty(name))
            {
                patientsQuery = patientsQuery.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(surname))
            {
                patientsQuery =
                    patientsQuery.Where(p => p.Surname.Contains(surname, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(bloodType))
            {
                patientsQuery =
                    patientsQuery.Where(p => p.BloodType.Equals(bloodType, StringComparison.OrdinalIgnoreCase));
            }

            if (admissionDate.HasValue)
            {
                patientsQuery = patientsQuery.Where(p => p.AdmissionDate.Date == admissionDate.Value.Date);
            }

            // Oda numarası filtrelemesi: Gelen roomNumber parametresi ile, her hastanın bağlı olduğu odanın RoomNumber'ı karşılaştırılıyor.
            if (roomNumber.HasValue && roomNumber.Value > 0)
            {
                patientsQuery =
                    patientsQuery.Where(p => _roomManager.GetById(p.RoomId)?.RoomNumber == roomNumber.Value);
            }

            var patients = patientsQuery.Select(p => new PatientViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Surname = p.Surname,
                BloodType = p.BloodType,
                AdmissionDate = p.AdmissionDate,
                CheckoutDate = p.CheckoutDate,
                Checked = p.Checked,
                BloodPressure = p.BloodPressure,
                Pulse = p.Pulse,
                BloodSugar = p.BloodSugar,
                RoomId = p.RoomId,
                // Listeleme için oda numarası string olarak çekiliyor:
                RoomNumber = _roomManager.GetById(p.RoomId)?.RoomNumber
            }).ToList();

            // Filtre formunda kullanılmak üzere, direkt roomNumber parametresi kullanılacak:
            int? filterRoomNumber = roomNumber;

            // View modelimizi dolduralım:
            var viewModel = new PatientListViewModel
            {
                Patients = patients,
                Id = id,
                Name = name,
                Surname = surname,
                BloodType = bloodType,
                AdmissionDate = admissionDate,
                RoomId = null, // Artık oda numarası üzerinden filtreleme yapıyoruz
                RoomNumber = filterRoomNumber, // Gerçek oda numarası (int) filtre parametresi
                BloodTypeList = new List<SelectListItem>
                {
                    new SelectListItem { Value = "A+", Text = "A+" },
                    new SelectListItem { Value = "A-", Text = "A-" },
                    new SelectListItem { Value = "B+", Text = "B+" },
                    new SelectListItem { Value = "B-", Text = "B-" },
                    new SelectListItem { Value = "AB+", Text = "AB+" },
                    new SelectListItem { Value = "AB-", Text = "AB-" },
                    new SelectListItem { Value = "O+", Text = "O+" },
                    new SelectListItem { Value = "O-", Text = "O-" }
                }
            };

            return View("Patients", viewModel);
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

            var dailyRecord = new PatientDailyRecord
            {
                PatientId = patient.Id,
                RecordDate = DateTime.Now, // Veya uygun bir tarih değeri, örneğin model üzerinden de gelebilir.
                BloodPressure = model.BloodPressure,
                Pulse = model.Pulse,
                BloodSugar = model.BloodSugar,
                CreatedAt = DateTime.Now
            };

            _patientDailyRecordManager.Add(dailyRecord);

            return Ok(new { success = true, message = "Patient data saved successfully." });
        }

        [HttpGet("{id:int}")]
        public IActionResult Records(int id, string sortOrder)
        {
            // Retrieve the patient information
            var patient = _patientManager.GetById(id);

            // Set up sorting parameters for view
            ViewBag.CurrentSort = sortOrder;

            // Set up toggling parameters for each column
            ViewBag.IdSortParam = sortOrder == "id_asc" ? "id_desc" : "id_asc";
            ViewBag.DateSortParam = sortOrder == "date_asc" ? "date_desc" : "date_asc";
            ViewBag.BpSortParam = sortOrder == "bp_asc" ? "bp_desc" : "bp_asc";
            ViewBag.PulseSortParam = sortOrder == "p_asc" ? "p_desc" : "p_asc";
            ViewBag.BsSortParam = sortOrder == "bs_asc" ? "bs_desc" : "bs_asc";

            // Retrieve the daily records for this patient
            var dailyRecords = _patientDailyRecordManager.GetByPatientDailyRecords(patient.Id);

            // Sıralama işlemi
            switch (sortOrder)
            {
                case "date_desc":
                    dailyRecords = dailyRecords.OrderByDescending(r => r.RecordDate).ToList();
                    break;
                case "date_asc":
                    dailyRecords = dailyRecords.OrderBy(r => r.RecordDate).ToList();
                    break;
                case "id_desc":
                    dailyRecords = dailyRecords.OrderByDescending(r => r.Id).ToList();
                    break;
                case "id_asc":
                    dailyRecords = dailyRecords.OrderBy(r => r.Id).ToList();
                    break;
                case "bp_desc":
                    dailyRecords = dailyRecords.OrderByDescending(r => r.BloodPressure).ToList();
                    break;
                case "bp_asc":
                    dailyRecords = dailyRecords.OrderBy(r => r.BloodPressure).ToList();
                    break;
                case "p_desc":
                    dailyRecords = dailyRecords.OrderByDescending(r => r.Pulse).ToList();
                    break;
                case "p_asc":
                    dailyRecords = dailyRecords.OrderBy(r => r.Pulse).ToList();
                    break;
                case "bs_desc":
                    dailyRecords = dailyRecords.OrderByDescending(r => r.BloodSugar).ToList();
                    break;
                case "bs_asc":
                    dailyRecords = dailyRecords.OrderBy(r => r.BloodSugar).ToList();
                    break;
                default:
                    dailyRecords = dailyRecords.OrderByDescending(p => p.RecordDate).ToList();
                    break;
            }

            // Project the daily records into the RecordsViewModel
            var records = dailyRecords.Select(r => new RecordsViewModel
            {
                Id = r.Id,
                PatientId = r.PatientId,
                BloodPressure = r.BloodPressure,
                Pulse = r.Pulse,
                BloodSugar = r.BloodSugar,
                RecordDate = r.RecordDate
            }).ToList();

            // Build the composite view model
            var model = new PatientRecordsViewModel
            {
                PatientInfo = new PatientViewModel
                {
                    Id = patient.Id,
                    Name = patient.Name,
                    Surname = patient.Surname,
                    BloodType = patient.BloodType,
                    // Add other properties as needed
                },
                Records = records
            };

            return View("Records", model);
        }

        [HttpGet]
        public IActionResult Profile()
        {
            var username = HttpContext.Session.GetString("Username") ?? "DefaultUser";

            var user = _userManager.GetByUsername(username);
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }

            var model = new UserProfileViewModel
            {
                Username = user.Username,
                Role = user.Role,
                CreatedDate = user.CreatedAt
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