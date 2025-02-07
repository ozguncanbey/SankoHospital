namespace SankoHospital.WebApi.Models;

public class RegisterModel
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    // public string Role { get; set; } (Opsiyonel veya yetkisi varsa)
}