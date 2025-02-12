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
        var availableRooms = _roomManager.GetAll()
            .Where(r => r.CurrentPatientCount < r.Capacity)
            .Select(r => new RoomViewModel
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                Capacity = r.Capacity,
                CurrentPatientCount = r.CurrentPatientCount
            }).ToList();

        ViewBag.AvailableRooms = availableRooms;
        var patients = _patientManager.GetAll().Select(p => new PatientViewModel
        {
            Id = p.Id,
            Name = p.Name,
            Surname = p.Surname,
            BloodType = p.BloodType,
            AdmissionDate = p.AdmissionDate,
            CheckoutDate = p.CheckoutDate,
            Checked = p.Checked,
            RoomNumber = p.Room != null ? p.Room.RoomNumber.ToString() : "Not Assigned"

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
    public IActionResult AddPatient([FromBody] PatientViewModel model)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        try 
        {
            // First check if room exists and has capacity
            var room = _roomManager.GetById(model.RoomId);
            if (room == null) return NotFound("Room not found");
            if (room.CurrentPatientCount >= room.Capacity)
                return BadRequest("Room is full");
        
            var newPatient = new Patient
            {
                Name = model.Name,
                Surname = model.Surname,
                BloodType = model.BloodType,
                AdmissionDate = model.AdmissionDate,
                RoomId = model.RoomId,  // Explicitly set the RoomId
                Room = room,  // Set the Room navigation property
                Checked = false
            };

            _patientManager.Add(newPatient);
            
            // Update room's patient count
            room.CurrentPatientCount++;
            _roomManager.Update(room);
            
            return Ok(newPatient);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    // Receptionist hastayı güncelleyebilir
    [HttpPost]
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

            // If room is being changed, update room counts
            if (existingPatient.RoomId != model.RoomId)
            {
                // Decrease count in old room if it exists
                if (existingPatient.RoomId.HasValue)
                {
                    var oldRoom = _roomManager.GetById(existingPatient.RoomId.Value);
                    if (oldRoom != null)
                    {
                        oldRoom.CurrentPatientCount--;
                        _roomManager.Update(oldRoom);
                    }
                }

                // Increase count in new room
                if (model.RoomId != null)
                {
                    var newRoom = _roomManager.GetById(model.RoomId);
                    if (newRoom != null)
                    {
                        if (newRoom.CurrentPatientCount >= newRoom.Capacity)
                        {
                            return BadRequest("Selected room is at full capacity");
                        }
                        newRoom.CurrentPatientCount++;
                        _roomManager.Update(newRoom);
                    }
                }
            }

            existingPatient.Name = model.Name;
            existingPatient.Surname = model.Surname;
            existingPatient.BloodType = model.BloodType;
            existingPatient.AdmissionDate = model.AdmissionDate;
            existingPatient.RoomId = model.RoomId;

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

        var room = _roomManager.GetById(patient.RoomId.Value);
        if (room != null)
        {
            room.CurrentPatientCount--;
            _roomManager.Update(room);
        }
        
        patient.CheckoutDate = DateTime.Now;
        patient.RoomId = null;
        _patientManager.Update(patient);
        
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