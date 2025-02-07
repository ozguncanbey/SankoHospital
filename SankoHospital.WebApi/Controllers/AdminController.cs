using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SankoHospital.Business.Abstract;

namespace SankoHospital.WebApi.Controllers;

[ApiController]
[Authorize(Roles="Admin")]
[Route("admin")]
public class AdminController : Controller
{
    private readonly IUserService _userService;

    public AdminController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("users")]
    public IActionResult GetAllUsers()
    {
        var users = _userService.GetAll();
        // Map to a simpler DTO if needed
        return Ok(users);
    }

    // POST /admin/assign-role
    [HttpPost("assign-role")]
    public IActionResult AssignRole([FromBody] AssignRoleDto dto)
    {
        // 1. Kullanıcıyı bul
        var user = _userService.GetById(dto.UserId);
        if (user == null) return NotFound("Kullanıcı bulunamadı");

        // 2. Rolü güncelle
        user.Role = dto.Role;
        _userService.Update(user);

        return Ok("Rol atandı");
    }
}

public class AssignRoleDto
{
    public int UserId { get; set; }
    public string Role { get; set; }
}
