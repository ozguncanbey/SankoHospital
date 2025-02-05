using Microsoft.AspNetCore.Mvc;
using SankoHospital.Business.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class RoomsController : Controller
{
    private readonly IRoomService _roomManager;

    public RoomsController(IRoomService roomManager)
    {
        _roomManager = roomManager;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var rooms = _roomManager.GetAll();
        if (rooms == null || !rooms.Any()) return NotFound("No rooms found.");
        return Ok(rooms);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var room = _roomManager.GetById(id);
        if (room == null) return NotFound($"Room with id {id} not found.");
        return Ok(room);
    }

    [HttpPost]
    public IActionResult Add([FromBody] Room room)
    {
        if (!ModelState.IsValid) return BadRequest("Invalid data.");
        _roomManager.Add(room);
        return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Room room)
    {
        if (id != room.Id) return BadRequest("Room id mismatch.");
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var existingRoom = _roomManager.GetById(id);
        if (existingRoom == null) return NotFound($"Room with id {id} not found.");

        _roomManager.Update(room);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        try
        {
            var room = _roomManager.GetById(id);
            if (room == null) return NotFound($"Room with id {id} not found.");

            _roomManager.Delete(room);
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}