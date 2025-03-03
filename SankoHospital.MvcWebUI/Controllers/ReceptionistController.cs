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
    private readonly IBedOccupancyService _bedOccupancyManager;

    public ReceptionistController(IPatientService patientManager, IRoomService roomManager, IUserService userManager,
        IPasswordHasher passwordHasher, IRoomOccupancyService roomOccupancyManager, IBedService bedManager,
        IBedOccupancyService bedOccupancyManager) : base(userManager, passwordHasher)
    {
        _patientManager = patientManager;
        _roomManager = roomManager;
        _roomOccupancyManager = roomOccupancyManager;
        _bedManager = bedManager;
        _bedOccupancyManager = bedOccupancyManager;
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

    [HttpGet("{roomId:int}")]
    public IActionResult BedOccupancy(int roomId, int? bedId, string sortOrder = "id_desc",
        string activeView = "current")
    {
        // Odayı getir
        var room = _roomManager.GetById(roomId);
        if (room == null)
        {
            return NotFound();
        }

        // Odadaki tüm yatakları getir
        var beds = _bedManager.GetAllByRoom(roomId);
        if (beds == null || !beds.Any())
        {
            return NotFound("Bu odada yatak bulunmamaktadır.");
        }

        // Eğer bedId parametresi verilmemişse, odadaki ilk yatağı seç
        var selectedBed = bedId.HasValue
            ? beds.FirstOrDefault(b => b.Id == bedId.Value)
            : beds.First();

        if (selectedBed == null)
        {
            return NotFound("Seçili yatak bulunamadı.");
        }

        // Seçilen yatağın doluluk kayıtlarını getir
        var occupancyRecords = _bedOccupancyManager.GetByBedOccupancy(selectedBed.Id);

        // Sıralama işlemi
        switch (sortOrder)
        {
            case "id_desc":
                occupancyRecords = occupancyRecords.OrderByDescending(o => o.Id).ToList();
                break;
            case "id_asc":
                occupancyRecords = occupancyRecords.OrderBy(o => o.Id).ToList();
                break;
            case "pid_desc":
                occupancyRecords = occupancyRecords.OrderByDescending(o => o.PatientId).ToList();
                break;
            case "pid_asc":
                occupancyRecords = occupancyRecords.OrderBy(o => o.PatientId).ToList();
                break;
            case "admission_desc":
                occupancyRecords = occupancyRecords.OrderByDescending(o => o.AdmissionDate).ToList();
                break;
            case "admission_asc":
                occupancyRecords = occupancyRecords.OrderBy(o => o.AdmissionDate).ToList();
                break;
        }

        var occupancy = occupancyRecords.Select(o =>
        {
            var patient = _patientManager.GetById(o.PatientId);
            return new OccupancyViewModel
            {
                Id = o.Id,
                RoomId = o.RoomId,
                PatientId = o.PatientId,
                AdmissionDate = o.AdmissionDate,
                CheckoutDate = o.CheckoutDate,
                PatientName = patient?.Name ?? string.Empty,
                PatientSurname = patient?.Surname ?? string.Empty,
                BloodType = patient?.BloodType ?? string.Empty
            };
        }).ToList();

        // Odadaki tüm yatakları modele ekle
        var bedViewModels = beds.Select(b => new BedViewModel
        {
            Id = b.Id,
            RoomId = b.RoomId,
            BedNumber = b.BedNumber,
            Status = b.Status,
            LastCleanedDate = b.LastCleanedDate,
            RoomNumber = room.RoomNumber.ToString() // Oda numarasını da ekle
        }).ToList();

        // ViewModel oluştur
        var model = new BedOccupancyViewModel
        {
            BedInfo = new BedViewModel
            {
                Id = selectedBed.Id,
                RoomId = selectedBed.RoomId,
                BedNumber = selectedBed.BedNumber,
                Status = selectedBed.Status,
                LastCleanedDate = selectedBed.LastCleanedDate,
                RoomNumber = room.RoomNumber.ToString()
            },
            Occupancy = occupancy,
            Beds = bedViewModels
        };

        // Sıralama ve görünüm parametrelerini ViewBag'e ekle
        ViewBag.CurrentSort = sortOrder;
        ViewBag.IdSortParam = sortOrder == "id_asc" ? "id_desc" : "id_asc";
        ViewBag.PatientIdSortParam = sortOrder == "pid_asc" ? "pid_desc" : "pid_asc";
        ViewBag.NameSortParam = sortOrder == "name_asc" ? "name_desc" : "name_asc";
        ViewBag.SurnameSortParam = sortOrder == "surname_asc" ? "surname_desc" : "surname_asc";
        ViewBag.AdmissionSortParam = sortOrder == "admission_asc" ? "admission_desc" : "admission_asc";
        ViewBag.ActiveView = activeView;

        return View("BedOccupancy", model);
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

            // *** Yeni: Bu hastaya ait RoomOccupancy kaydı oluşturuluyor ***
            // RoomOccupancy tablosu; patientin oda kayıt geçmişini tutuyor
            var roomOccupancyRecord = new RoomOccupancy
            {
                RoomId = newPatient.RoomId,
                PatientId = newPatient.Id,
                AdmissionDate = newPatient.AdmissionDate,
                CheckoutDate = null,
                CreatedAt = DateTime.UtcNow
            };
            _roomOccupancyManager.Add(roomOccupancyRecord);

            // *** New: Create a BedOccupancy record for this patient ***
            // (Assuming GetByRoomId returns an available bed from that room)
            var availableBed = _bedManager.GetAvailableBedForRoom(newPatient.RoomId);
            if (availableBed != null)
            {
                var bedOccupancyRecord = new BedOccupancy
                {
                    RoomId = newPatient.RoomId,
                    PatientId = newPatient.Id,
                    BedId = availableBed.Id,
                    AdmissionDate = newPatient.AdmissionDate,
                    CheckoutDate = null,
                    CreatedAt = DateTime.UtcNow
                };
                _bedOccupancyManager.Add(bedOccupancyRecord);
            }
            else
            {
                // Eğer uygun yatak bulunamazsa, hata döndürebilir veya alternatif işlem yapabilirsiniz.
                return BadRequest("No available bed found for the room.");
            }

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

            // Eğer oda değişikliği varsa:
            if (existingPatient.RoomId != model.RoomId)
            {
                // 1. Eski odadaki işlemler:
                var oldRoom = _roomManager.GetById(existingPatient.RoomId);
                if (oldRoom != null)
                {
                    // Eski odanın mevcut hasta sayısını 1 azalt
                    oldRoom.CurrentPatientCount = Math.Max(0, oldRoom.CurrentPatientCount - 1);
                    _roomManager.Update(oldRoom);
                }

                // Eski odadaki aktif BedOccupancy kaydını (CheckoutDate null olan) kapat
                var oldBedOccupancy = _bedOccupancyManager.GetOpenRecordByPatientId(existingPatient.Id);
                if (oldBedOccupancy != null)
                {
                    oldBedOccupancy.CheckoutDate = DateTime.UtcNow;
                    _bedOccupancyManager.Update(oldBedOccupancy);
                }

                // 2. Yeni odadaki işlemler:
                var newRoom = _roomManager.GetById(model.RoomId);
                if (newRoom == null)
                {
                    return NotFound(new { success = false, message = "New room not found." });
                }

                if (newRoom.CurrentPatientCount >= newRoom.Capacity)
                {
                    return BadRequest(new { success = false, message = "New room is full." });
                }

                // Yeni odanın mevcut hasta sayısını artır
                newRoom.CurrentPatientCount++;
                _roomManager.Update(newRoom);

                // Yeni odanın RoomOccupancy kaydını oluştur (yeni giriş tarihi ile)
                var newRoomOccupancy = new RoomOccupancy
                {
                    RoomId = newRoom.Id,
                    PatientId = existingPatient.Id,
                    AdmissionDate = model.AdmissionDate,
                    CheckoutDate = null,
                    CreatedAt = DateTime.UtcNow
                };
                _roomOccupancyManager.Add(newRoomOccupancy);

                // Yeni odada müsait yatağı bul (IBedService içindeki GetAvailableBedForRoom metodu kullanılarak)
                var availableBed = _bedManager.GetAvailableBedForRoom(newRoom.Id);
                if (availableBed == null)
                {
                    return BadRequest(new { success = false, message = "No available bed in the new room." });
                }

                // Yeni BedOccupancy kaydı oluştur
                var newBedOccupancy = new BedOccupancy
                {
                    RoomId = newRoom.Id,
                    PatientId = existingPatient.Id,
                    BedId = availableBed.Id,
                    AdmissionDate = model.AdmissionDate,
                    CheckoutDate = null,
                    CreatedAt = DateTime.UtcNow
                };
                _bedOccupancyManager.Add(newBedOccupancy);
            }

            // Hastanın diğer bilgilerini güncelle
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
        // Hastayı alıyoruz
        var patient = _patientManager.GetById(patientId);
        if (patient == null)
        {
            return NotFound("Patient not found.");
        }

        // Hasta zaten çıkış yaptıysa hata döndürüyoruz.
        if (patient.CheckoutDate.HasValue)
        {
            return BadRequest(new { success = false, message = "Patient already checked out." });
        }

        // Hastanın odasını alıyoruz
        var room = _roomManager.GetById(patient.RoomId);
        if (room == null)
        {
            return NotFound("Room not found.");
        }

        // Hastaya ait aktif BedOccupancy kaydını getiriyoruz.
        // Not: Bu metodu IBedOccupancyService içinde GetActiveRecordByPatientId şeklinde tanımlamanız gerekiyor.
        var bedOccupancyRecord = _bedOccupancyManager.GetOpenRecordByPatientId(patientId);
        if (bedOccupancyRecord == null)
        {
            return NotFound("Active bed occupancy record not found.");
        }

        // Çıkış tarihi belirleniyor
        patient.CheckoutDate = DateTime.UtcNow; // veya DateTime.Now
        _patientManager.Update(patient);

        // Oda doluluk bilgisini güncelliyoruz (mevcut hasta sayısını 1 azaltıyoruz)
        room.CurrentPatientCount--;
        _roomManager.Update(room);

        // *** Yeni: İlgili RoomOccupancy kaydını güncelliyoruz ***
        // Aktif RoomOccupancy kaydını getiriyoruz (CheckoutDate alanı null olan)
        var roomOccupancyRecord = _roomOccupancyManager.GetOpenRecordByPatientId(patientId);
        if (roomOccupancyRecord != null)
        {
            roomOccupancyRecord.CheckoutDate = patient.CheckoutDate;
            _roomOccupancyManager.Update(roomOccupancyRecord);
        }

        // *** Yeni: İlgili BedOccupancy kaydını güncelliyoruz ***
        // Alınan aktif BedOccupancy kaydının CheckoutDate alanını güncelliyoruz
        bedOccupancyRecord.CheckoutDate = patient.CheckoutDate;
        _bedOccupancyManager.Update(bedOccupancyRecord);

        return Ok(new
        {
            success = true,
            message = "Patient checked out successfully.",
            checkoutDate = patient.CheckoutDate?.ToString("yyyy-MM-dd")
        });
    }

    [HttpDelete("{id:int}")]
    public IActionResult DeletePatient(int id)
    {
        try
        {
            // Hastayı getir
            var patient = _patientManager.GetById(id);
            if (patient == null)
                return NotFound("Patient not found.");

            // İlgili odanın bilgisini al
            var room = _roomManager.GetById(patient.RoomId);

            // Hem RoomOccupancy hem de BedOccupancy tablosundaki bu hasta ile ilişkili tüm kayıtları getirip sil
            // (Tüm kayıtlar siliniyor, aktif (checkoutDate null) ya da kapalı farketmeksizin)
            var roomOccupancyRecords = _roomOccupancyManager.GetAll().Where(r => r.PatientId == patient.Id).ToList();
            foreach (var record in roomOccupancyRecords)
            {
                _roomOccupancyManager.Delete(record);
            }

            var bedOccupancyRecords = _bedOccupancyManager.GetAll().Where(b => b.PatientId == patient.Id).ToList();
            foreach (var record in bedOccupancyRecords)
            {
                _bedOccupancyManager.Delete(record);
            }

            // Oda bilgisi varsa oda doluluk sayısını güncelle (sıfırın altına düşmesin)
            if (room != null)
            {
                room.CurrentPatientCount = Math.Max(0, room.CurrentPatientCount - 1);
                _roomManager.Update(room);
            }

            // Hastayı sil
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