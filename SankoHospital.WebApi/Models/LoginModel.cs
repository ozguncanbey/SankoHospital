namespace SankoHospital.WebApi.Models;

/// <summary>
/// Login işlemi için sadece Username ve Password
/// </summary>
public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}