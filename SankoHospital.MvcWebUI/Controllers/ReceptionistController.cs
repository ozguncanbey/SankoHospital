using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SankoHospital.Business.Abstract;
using SankoHospital.Core.Security;
using SankoHospital.Entities.Concrete;
using SankoHospital.MvcWebUI.Controllers.Base;
using SankoHospital.MvcWebUI.Models.CleanerModel;
using SankoHospital.MvcWebUI.Models.FilterModels;
using SankoHospital.MvcWebUI.Models.NurseModel;
using SankoHospital.MvcWebUI.Models.ReceptionistModel;
using SankoHospital.MvcWebUI.Models.UserModels;

namespace SankoHospital.MvcWebUI.Controllers;

[Route("[controller]/[action]")]
public class ReceptionistController : BaseController
{
    private readonly IPatientService _patientManager;
    private readonly IRoomService _roomManager;
    private readonly IBedService _bedManager;
    private readonly IRoomOccupancyService _roomOccupancyManager;

    public ReceptionistController(IPatientService patientManager, IRoomService roomManager, IUserService userManager,
        IPasswordHasher passwordHasher, IRoomOccupancyService roomOccupancyManager, IBedService bedManager) : base(userManager, passwordHasher)
    {
        _patientManager = patientManager;
        _roomManager = roomManager;
        _roomOccupancyManager = roomOccupancyManager;
        _bedManager = bedManager;
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
        int todaysCheckouts =
            allPatients.Count(p => p.CheckoutDate.HasValue && p.CheckoutDate.Value.Date == DateTime.Today);

        // Total Registered Patients: Kayıtlı toplam hasta sayısı
        int totalRegisteredPatients = allPatients.Count();

        // Rooms Available: Boş ve temizlenmiş odalar (odanın doluluk durumu kapasiteye göre ve durumu "Cleaned" ise)
        int roomsAvailable = allRooms.Count(r => r.CurrentPatientCount < r.Capacity);

        var model = new ReceptionistDashboardViewModel
        {
            TodaysAdmissions = todaysAdmissions,
            TodaysCheckouts = todaysCheckouts,
            RoomsAvailable = roomsAvailable,
            TotalRegisteredPatients = totalRegisteredPatients
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Patients(PatientListViewModel model)
    {
        // Mevcut hastaları filtreleme metoduyla çekin
        var filteredPatients = _patientManager.GetFilteredPatients(
            model.Id,
            model.Name,
            model.Surname,
            model.BloodType,
            model.AdmissionDate,
            model.CheckoutDate, // Duruma göre burada null gönderebilirsiniz.
            model.RoomId
        );

        if (model.RoomNumber is > 0)
        {
            filteredPatients.Where(p => _roomManager.GetById(p.RoomId)?.RoomNumber == model.RoomNumber.Value);
        }

        // Filtrelenmiş hastaları PatientViewModel'e dönüştürün
        var patients = filteredPatients.Select(p => new PatientViewModel
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
            RoomNumber = _roomManager.GetById(p.RoomId)?.RoomNumber
        }).ToList();

        // Filtre formunda kullanılmak üzere, eğer RoomId verilmişse ilgili odanın RoomNumber'ını alalım:
        int? filterRoomNumber = model.RoomNumber;
        /*if (model.RoomId is > 0)
        {
            var room = _roomManager.GetById(model.RoomId.Value);
            if (room != null)
            {
                filterRoomNumber = room.RoomNumber;
            }
        }*/

        // AvailableRooms listesini de modelinize ekleyin:
        var availableRooms = _roomManager.GetAll()
            .Where(r => r.CurrentPatientCount < r.Capacity)
            .Select(r => new RoomViewModel
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                Capacity = r.Capacity,
                CurrentPatientCount = r.CurrentPatientCount
            }).ToList();

        // View modelimizi dolduralım:
        model.Patients = patients;
        model.RoomNumber = filterRoomNumber;
        model.AvailableRooms = availableRooms;

        // BloodTypeList'i de dolduralım:
        if (model.BloodTypeList == null || model.BloodTypeList.Count == 0)
        {
            model.BloodTypeList = new List<SelectListItem>
            {
                new SelectListItem { Value = "A+", Text = "A+" },
                new SelectListItem { Value = "A-", Text = "A-" },
                new SelectListItem { Value = "B+", Text = "B+" },
                new SelectListItem { Value = "B-", Text = "B-" },
                new SelectListItem { Value = "AB+", Text = "AB+" },
                new SelectListItem { Value = "AB-", Text = "AB-" },
                new SelectListItem { Value = "O+", Text = "O+" },
                new SelectListItem { Value = "O-", Text = "O-" }
            };
        }

        return View("Patients", model);
    }

    // Receptionist için Rooms sayfası (sadece görüntüleme)
    [HttpGet("{roomId:int}")]
    public IActionResult RoomOccupancy(int roomId, string sortOrder = "id_desc", string activeView = "current")
    {
        // Odayı getir
        var room = _roomManager.GetById(roomId);
        if (room == null)
        {
            return NotFound();
        }

        // İlgili odanın doluluk kayıtlarını getir
        var roomOccupancy = _roomOccupancyManager.GetByRoomOccupancy(room.Id);

        // Her doluluk kaydı için ilgili hasta bilgilerini de ekleyelim
        var occupancy = roomOccupancy.Select(o =>
        {
            var patient = _patientManager.GetById(o.PatientId);
            var bed = _bedManager.GetByPatientId(patient?.Id ?? 0);
            return new OccupancyViewModel
            {
                Id = o.Id,
                RoomId = o.RoomId,
                PatientId = o.PatientId,
                AdmissionDate = o.AdmissionDate,
                CheckoutDate = o.CheckoutDate,
                PatientName = patient?.Name ?? string.Empty,
                PatientSurname = patient?.Surname ?? string.Empty,
                BedNumber = bed != null ? bed.BedNumber : 0,
                BloodType = patient?.BloodType ?? string.Empty
            };
        }).ToList();

        // Sıralama işlemi
        switch (sortOrder)
        {
            case "id_desc":
                occupancy = occupancy.OrderByDescending(o => o.Id).ToList();
                break;
            case "id_asc":
                occupancy = occupancy.OrderBy(o => o.Id).ToList();
                break;
            case "pid_desc":
                occupancy = occupancy.OrderByDescending(o => o.PatientId).ToList();
                break;
            case "pid_asc":
                occupancy = occupancy.OrderBy(o => o.PatientId).ToList();
                break;
            case "name_desc":
                occupancy = occupancy.OrderByDescending(o => o.PatientName)
                    .ThenByDescending(o => o.PatientSurname).ToList();
                break;
            case "name_asc":
                occupancy = occupancy.OrderBy(o => o.PatientName)
                    .ThenBy(o => o.PatientSurname).ToList();
                break;
            case "bn_desc":
                occupancy = occupancy.OrderByDescending(o => o.BedNumber).ToList();
                break;
            case "bn_asc":
                occupancy = occupancy.OrderBy(o => o.BedNumber).ToList();
                break;
            case "admission_desc":
                occupancy = occupancy.OrderByDescending(o => o.AdmissionDate).ToList();
                break;
            case "admission_asc":
                occupancy = occupancy.OrderBy(o => o.AdmissionDate).ToList();
                break;
            default:
                occupancy = occupancy.OrderByDescending(o => o.Id).ToList();
                break;
        }

        // ActiveView bilgisini ViewBag’e aktaralım
        ViewBag.ActiveView = activeView;

        // View model oluştur
        var model = new RoomOccupancyViewModel
        {
            RoomInfo = new RoomViewModel
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                Capacity = room.Capacity,
                CurrentPatientCount = room.CurrentPatientCount,
                LastCleanedDate = room.LastCleanedDate,
                Status = room.Status
            },
            Occupancy = occupancy
        };

        return View("RoomOccupancy", model);
    }

    [HttpGet]
    public IActionResult Rooms(RoomListViewModel model)
    {
        // Eğer model null ise yeni bir instance oluşturuyoruz.
        if (model == null)
        {
            model = new RoomListViewModel();
        }

        // Filtreleme metodunu kullanarak odaları çekelim:
        var filteredRooms = _roomManager.GetFilteredRooms(
            model.Id,
            model.RoomNumber,
            model.Capacity,
            model.CurrentPatientCount,
            model.Status,
            model.LastCleanedDate,
            null // Doluluk (occupancy) isteğe bağlı; burada eklenmedi
        );

        // Filtrelenmiş odaları RoomViewModel'e dönüştürelim:
        var rooms = filteredRooms.Select(r => new RoomViewModel
        {
            Id = r.Id,
            RoomNumber = r.RoomNumber,
            Capacity = r.Capacity,
            CurrentPatientCount = r.CurrentPatientCount,
            LastCleanedDate = r.LastCleanedDate,
            Status = r.Status
        }).ToList();

        // View modelimize odaları atıyoruz:
        model.Rooms = rooms;

        // Eğer StatusList henüz doldurulmamışsa, dropdown seçeneklerini ekleyelim:
        if (model.StatusList == null || model.StatusList.Count == 0)
        {
            model.StatusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "Cleaned", Text = "Temizlendi" },
                new SelectListItem { Value = "Cleaning", Text = "Temizleniyor" },
                new SelectListItem { Value = "In Care", Text = "Bakımda" },
                new SelectListItem { Value = "Waiting", Text = "Bekliyor" }
            };
        }

        return View(model);
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

        {
            var room = _roomManager.GetById(patient.RoomId);
            room.CurrentPatientCount--;
            _roomManager.Update(room);
        }

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
            {
                var room = _roomManager.GetById(patient.RoomId);
                room.CurrentPatientCount--;
                _roomManager.Update(room);
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