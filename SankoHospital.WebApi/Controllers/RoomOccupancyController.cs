using Microsoft.AspNetCore.Mvc;
using SankoHospital.Business.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class RoomOccupancyController : Controller
{
    private readonly IRoomOccupancyService _roomOccupancyManager;

    public RoomOccupancyController(IRoomOccupancyService roomOccupancyManager)
    {
        _roomOccupancyManager = roomOccupancyManager;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var occupancy = _roomOccupancyManager.GetAll();
        if (occupancy == null) return NotFound("No result found.");
        return Ok(occupancy);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var occupancy = _roomOccupancyManager.GetById(id);
        if (occupancy == null) return NotFound($"Result with id {id} not found.");
        return Ok(occupancy);
    }

    [HttpPost]
    public IActionResult Add([FromBody] RoomOccupancy occupancy)
    {
        if (!ModelState.IsValid) return BadRequest("Invalid data.");
        _roomOccupancyManager.Add(occupancy);
        return CreatedAtAction(nameof(GetById), new { id = occupancy.Id }, occupancy);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] RoomOccupancy occupancy)
    {
        if (id != occupancy.Id) return BadRequest("Result id mismatch.");

        if (!ModelState.IsValid) return BadRequest(ModelState);

        var existingOccupancy = _roomOccupancyManager.GetById(id);
        if (existingOccupancy == null) return NotFound($"Result with id {id} not found.");

        _roomOccupancyManager.Update(occupancy);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        try
        {
            var occupancy = _roomOccupancyManager.GetById(id);
            if (occupancy == null) return NotFound($"Result with id {id} not found.");

            _roomOccupancyManager.Delete(occupancy);
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}