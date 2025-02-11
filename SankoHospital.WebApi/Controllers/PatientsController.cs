using Microsoft.AspNetCore.Mvc;
using SankoHospital.Business.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PatientsController : Controller
{
    private readonly IPatientService _patientManager;

    public PatientsController(IPatientService patientManager)
    {
        _patientManager = patientManager;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var patients = _patientManager.GetAll();
        if (patients == null) return NotFound("No patients found.");
        return Ok(patients);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var patient = _patientManager.GetById(id);
        if (patient == null) return NotFound($"Patient with id {id} not found.");
        return Ok(patient);
    }

    // HEMŞİRENİN SADECE CHECK İŞARETLEMESİNE İZİN VERİLEN ENDPOINT
    [HttpPatch("{id:int}/check")]
    public IActionResult MarkChecked(int id)
    {
        var patient = _patientManager.GetById(id);
        if (patient == null) return NotFound($"Patient with id {id} not found.");

        patient.Checked = true;
        _patientManager.Update(patient);

        return Ok(new { message = "Patient checked successfully." });
    }
    
    [HttpPost]
    public IActionResult Add([FromBody] Patient patient)
    {
        if (!ModelState.IsValid) return BadRequest("Invalid data.");
        _patientManager.Add(patient);
        return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] Patient patient)
    {
        if (id != patient.Id) return BadRequest("Patient id mismatch.");

        if (!ModelState.IsValid) return BadRequest(ModelState);
            
        var existingPatient = _patientManager.GetById(id);
        if (existingPatient == null) return NotFound($"City with id {id} not found.");

        _patientManager.Update(patient);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        try
        {
            var patient = _patientManager.GetById(id);
            if (patient == null) return NotFound($"City with id {id} not found.");

            _patientManager.Delete(patient);
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}