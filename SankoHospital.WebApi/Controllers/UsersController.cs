using Microsoft.AspNetCore.Mvc;
using SankoHospital.Business.Abstract;
using SankoHospital.Entities.Concrete;

namespace SankoHospital.WebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController : Controller
{
    private readonly IUserService _userManager;

    public UsersController(IUserService userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var users = _userManager.GetAll();
        if (users == null || !users.Any()) return NotFound("No users found.");
        return Ok(users);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var user = _userManager.GetById(id);
        if (user == null) return NotFound($"User with id {id} not found.");
        return Ok(user);
    }

    [HttpPost]
    public IActionResult Add([FromBody] User user)
    {
        if (!ModelState.IsValid) return BadRequest("Invalid data.");
        _userManager.Add(user);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] User user)
    {
        if (id != user.Id) return BadRequest("User id mismatch.");
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var existingUser = _userManager.GetById(id);
        if (existingUser == null) return NotFound($"User with id {id} not found.");

        _userManager.Update(user);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        try
        {
            var user = _userManager.GetById(id);
            if (user == null) return NotFound($"User with id {id} not found.");

            _userManager.Delete(user);
            return NoContent();
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}