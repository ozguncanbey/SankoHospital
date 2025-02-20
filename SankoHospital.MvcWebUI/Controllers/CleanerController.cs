using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SankoHospital.Business.Abstract;
using SankoHospital.Core.Security;
using SankoHospital.MvcWebUI.Controllers.Base;
using SankoHospital.MvcWebUI.Models.CleanerModel;
using SankoHospital.MvcWebUI.Models.FilterModels;
using SankoHospital.MvcWebUI.Models.UserModels;

namespace SankoHospital.MvcWebUI.Controllers;

[Route("[controller]/[action]")]
public class CleanerController : BaseController
{
    private readonly IRoomService _roomManager;
    private readonly IBedService _bedManager;
    private readonly IPatientService _patientManager;

    public CleanerController(IRoomService roomManager, IUserService userManager, IPasswordHasher passwordHasher,
        IBedService bedManager, IPatientService patientManager) : base(userManager, passwordHasher)
    {
        _roomManager = roomManager;
        _bedManager = bedManager;
        _patientManager = patientManager;
    }

    [HttpGet("")]
    public IActionResult Dashboard()
    {
        // Tüm odaları çekiyoruz.
        var rooms = _roomManager.GetAll();

        // Bugün temizlenmesi gereken odaları; 
        // Örneğin: Eğer son temizlenme tarihi yok veya bugüne ait değilse temizlik işi var sayalım.
        var todaysCleaningTasks =
            rooms.Count(r => !r.LastCleanedDate.HasValue || r.LastCleanedDate.Value.Date != DateTime.Today);

        // Bugün temizlenen odalar
        var completedTasks = rooms.Count(r => r.Status == "Cleaned");

        // Pending, temizlik işi olan fakat henüz tamamlanmamış odalar
        var pendingTasks = todaysCleaningTasks - completedTasks;

        var model = new CleanerDashboardViewModel
        {
            TodaysCleaningTasks = todaysCleaningTasks,
            CompletedTasks = completedTasks,
            PendingTasks = pendingTasks
        };

        return View(model);
    }


    [HttpGet]
    public IActionResult Rooms()
    {
        var rooms = _roomManager.GetAll().Select(r => new RoomViewModel
        {
            Id = r.Id,
            RoomNumber = r.RoomNumber,
            Capacity = r.Capacity,
            CurrentPatientCount = r.CurrentPatientCount,
            Status = r.Status,
            LastCleanedDate = r.LastCleanedDate
        }).ToList();

        return View("Rooms", rooms);
    }
    
    [HttpGet]
    public IActionResult Beds(
        int? id,
        int? roomNumber,
        int? bedNumber,
        int? patientId,
        string status,
        DateTime? lastCleanedDate)
    {
        // Filtre parametrelerine göre yatakları getiriyoruz.
        var filteredBeds = _bedManager.GetFilteredBeds(id, roomNumber, bedNumber, patientId, status, lastCleanedDate)
            .Select(r => new BedViewModel
            {
                Id = r.Id,
                RoomId = r.RoomId,
                RoomNumber = _roomManager.GetById(r.RoomId)?.RoomNumber.ToString() ?? string.Empty,
                BedNumber = r.BedNumber,
                PatientId = r.PatientId,
                PatientName = r.PatientId.HasValue ? _patientManager.GetById(r.PatientId.Value)?.Name : string.Empty,
                PatientSurname = r.PatientId.HasValue ? _patientManager.GetById(r.PatientId.Value)?.Surname : string.Empty,
                Status = r.Status,
                LastCleanedDate = r.LastCleanedDate,
                CreatedAt = r.CreatedAt
            }).ToList();

        // Yeni view modelimizi dolduralım
        var viewModel = new BedListViewModel
        {
            Beds = filteredBeds,
            Id = id,
            RoomNumber = roomNumber,
            BedNumber = bedNumber,
            PatientId = patientId,
            SelectedStatus = status,
            LastCleanedDate = lastCleanedDate,
            StatusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "Cleaned", Text = "Temizlendi" },
                new SelectListItem { Value = "Cleaning", Text = "Temizleniyor" },
                new SelectListItem { Value = "In Care", Text = "Bakımda" },
                new SelectListItem { Value = "Waiting", Text = "Bekliyor" }
            }
        };

        return View("Beds", viewModel);
    }
    
    [HttpPost]
    public IActionResult UpdateRoomStatus(int id, string status)
    {
        try
        {
            var room = _roomManager.GetById(id);
            if (room == null)
                return NotFound(new { message = "Room not found" });

            // Eğer oda "Cleaned" durumuna geçiyorsa, LastCleanedDate güncellensin
            if (status == "Cleaned")
            {
                room.LastCleanedDate = DateTime.UtcNow; // UTC kullanarak saat farklarını önlüyoruz
            }

            room.Status = status;
            _roomManager.Update(room);

            // Değişikliklerin veritabanına yansıdığını doğrulamak için tekrar çekelim
            var updatedRoom = _roomManager.GetById(id);

            return Ok(new
            {
                message = "Room status updated successfully",
                newStatus = updatedRoom.Status,
                lastCleanedDate = updatedRoom.LastCleanedDate?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while updating room status.", error = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult UpdateBedStatus(int id, string status)
    {
        try
        {
            var bed = _bedManager.GetById(id);
            if (bed == null)
                return NotFound(new { message = "Bed not found" });

            // Eğer yatak "Cleaned" durumuna geçiyorsa, LastCleanedDate güncellensin
            if (status == "Cleaned")
            {
                bed.LastCleanedDate = DateTime.UtcNow; // UTC kullanarak saat farklarını önlüyoruz
            }

            bed.Status = status;
            _bedManager.Update(bed);

            // Değişikliklerin veritabanına yansıdığını doğrulamak için tekrar çekelim
            var updatedBed = _bedManager.GetById(id);

            return Ok(new
            {
                message = "Bed status updated successfully",
                newStatus = updatedBed.Status,
                lastCleanedDate = updatedBed.LastCleanedDate?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500,
                new { message = "An error occurred while updating bed status.", error = ex.Message });
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