using Microsoft.AspNetCore.Mvc;
using SankoHospital.Business.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class BedsController : Controller
{
    private readonly IBedService _bedManager;

    public BedsController(IBedService bedManager)
    {
        _bedManager = bedManager;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var beds = _bedManager.GetAll();
        if (beds == null || !beds.Any()) return NotFound("No beds found.");
        return Ok(beds);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var room = _bedManager.GetById(id);
        if (room == null) return NotFound($"Bed with id {id} not found.");
        return Ok(room);
    }

    [HttpPost]
    public IActionResult Add([FromBody] Bed bed)
    {
        if (!ModelState.IsValid) return BadRequest("Invalid data.");
        _bedManager.Add(bed);
        return CreatedAtAction(nameof(GetById), new { id = bed.Id }, bed);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Bed bed)
    {
        if (id != bed.Id) return BadRequest("Bed id mismatch.");
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var existingBed = _bedManager.GetById(id);
        if (existingBed == null) return NotFound($"Bed with id {id} not found.");

        _bedManager.Update(bed);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        try
        {
            var bed = _bedManager.GetById(id);
            if (bed == null) return NotFound($"Bed with id {id} not found.");

            _bedManager.Delete(bed);
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}