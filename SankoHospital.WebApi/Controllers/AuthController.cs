using Microsoft.AspNetCore.Mvc;
using SankoHospital.Business.Abstract;
using SankoHospital.Entities.Concrete;
using SankoHospital.WebApi.Models;

namespace SankoHospital.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userManager;

        public AuthController(IUserService userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        /// Kullanıcı girişi (login)
        /// </summary>
        /// <param name="model">Kullanıcı adı ve şifre</param>
        /// <returns>JWT Token veya hata</returns>
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid login data.");

            // userManager içinde Authenticate kullanarak token döndürdüğünü varsayıyoruz:
            // public string Authenticate(string username, string password) => "jwt token" 
            var token = _userManager.Authenticate(model.Username, model.Password);
            if (token == null)
            {
                return Unauthorized("Invalid username or password.");
            }

            // Başarılıysa JWT token döndürüyoruz
            return Ok(new { Token = token });
        }

        /// <summary>
        /// Yeni kullanıcı kaydı (register)
        /// </summary>
        /// <param name="user">Kullanıcı bilgileri</param>
        /// <returns>Oluşan kullanıcının verisi veya hata</returns>
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid registration data.");

            if (model.Password != model.ConfirmPassword)
            {
                return Unauthorized("Unmatched passwords.");
            }

            // 1. User entity’si oluştur
            var user = new User
            {
                Username = model.Username,
                PasswordHash = model.Password, // Manager içinde hash'lenecek
                Role = "User", // Sabit rol atayabilirsin, ya da modelden alabilirsin
                CreatedAt = DateTime.Now
            };

            // 2. Kaydet
            _userManager.Add(user);

            // 3. Dönüş
            return CreatedAtAction("GetById", "Users", new { id = user.Id }, user);
        }
    }
}