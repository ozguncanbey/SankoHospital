using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SankoHospital.Business.Abstract;

namespace SankoHospital.WebApi.Controllers;

[ApiController]
[Authorize(Roles="Admin")]
[Route("admin")]
public class AdminController : Controller
{
    private readonly IUserService _userManager;

    public AdminController(IUserService userManager)
    {
        _userManager = userManager;
    }

    [HttpGet("users")]
    public IActionResult GetAllUsers()
    {
        var users = _userManager.GetAll();
        // Map to a simpler DTO if needed
        return Ok(users);
    }

    // POST /admin/assign-role
    [HttpPost("assign-role")]
    public IActionResult AssignRole([FromBody] AssignRoleDto dto)
    {
        // 1. Kullanıcıyı bul
        var user = _userManager.GetById(dto.UserId);
        if (user == null) return NotFound("User not found");

        // 2. Rolü güncelle
        user.Role = dto.Role;
        _userManager.Update(user);

        return Ok("Role assigned");
    }
    
    // DELETE /admin/users/{id}
    [HttpDelete("users/{id:int}")]
    public IActionResult DeleteUser(int id)
    {
        var user = _userManager.GetById(id);
        if (user == null)
        {
            return NotFound($"User (ID: {id}) not found.");
        }

        _userManager.Delete(user);
        return Ok($"User (ID: {id}) deleted.");
    }
}

public class AssignRoleDto
{
    public int UserId { get; set; }
    public string Role { get; set; }
}
