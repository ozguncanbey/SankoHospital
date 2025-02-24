using Microsoft.AspNetCore.Mvc;
using SankoHospital.Business.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PatientDailyRecordController : Controller
{
    private readonly IPatientDailyRecordService _patientDailyRecordManager;

    public PatientDailyRecordController(IPatientDailyRecordService patientDailyRecordManager)
    {
        _patientDailyRecordManager = patientDailyRecordManager;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var records = _patientDailyRecordManager.GetAll();
        if (records == null) return NotFound("No records found.");
        return Ok(records);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var records = _patientDailyRecordManager.GetById(id);
        if (records == null) return NotFound($"Record with id {id} not found.");
        return Ok(records);
    }

    [HttpPost]
    public IActionResult Add([FromBody] PatientDailyRecord record)
    {
        if (!ModelState.IsValid) return BadRequest("Invalid data.");
        _patientDailyRecordManager.Add(record);
        return CreatedAtAction(nameof(GetById), new { id = record.Id }, record);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] PatientDailyRecord record)
    {
        if (id != record.Id) return BadRequest("Record id mismatch.");

        if (!ModelState.IsValid) return BadRequest(ModelState);

        var existingRecord = _patientDailyRecordManager.GetById(id);
        if (existingRecord == null) return NotFound($"Record with id {id} not found.");

        _patientDailyRecordManager.Update(record);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        try
        {
            var record = _patientDailyRecordManager.GetById(id);
            if (record == null) return NotFound($"Record with id {id} not found.");

            _patientDailyRecordManager.Delete(record);
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}