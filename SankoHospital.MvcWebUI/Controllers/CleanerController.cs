using Microsoft.AspNetCore.Mvc;
using SankoHospital.Business.Abstract;
using SankoHospital.MvcWebUI.Controllers.Base;
using SankoHospital.MvcWebUI.Models;
using System.Linq;

namespace SankoHospital.MvcWebUI.Controllers;

[Route("[controller]/[action]")]
public class CleanerController : BaseController
{
    private readonly IRoomService _roomService;

    public CleanerController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet("")]
    public IActionResult Dashboard()
    {
        return View("Dashboard");
    }

    [HttpGet]
    public IActionResult Rooms()
    {
        var rooms = _roomService.GetAll().Select(r => new RoomViewModel
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

    [HttpPost]
    public IActionResult UpdateRoomStatus(int id, string status)
    {
        try
        {
            var room = _roomService.GetById(id);
            if (room == null) 
                return NotFound(new { message = "Room not found" });

            // Eğer oda "Cleaned" durumuna geçiyorsa, LastCleanedDate güncellensin
            if (status == "Cleaned")
            {
                room.LastCleanedDate = DateTime.UtcNow; // UTC kullanarak saat farklarını önlüyoruz
            }

            room.Status = status;
            _roomService.Update(room);

            // Değişikliklerin veritabanına yansıdığını doğrulamak için tekrar çekelim
            var updatedRoom = _roomService.GetById(id);

            return Ok(new
            {
                message = "Room status updated successfully",
                newStatus = updatedRoom.Status,
                lastCleanedDate = updatedRoom.LastCleanedDate?.ToString("yyyy-MM-dd HH:mm") ?? "N/A"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating room status.", error = ex.Message });
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
        return View();
    }
}