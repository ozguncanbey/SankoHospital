using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using SankoHospital.Business.Abstract;
using SankoHospital.Business.DTOs;
using SankoHospital.Core.Helpers;
using SankoHospital.MvcWebUI.Controllers.Base;
using SankoHospital.MvcWebUI.Models.UserModels;

namespace SankoHospital.MvcWebUI.Controllers
{
    [Route("[controller]/[action]")]
    public class AdminController : BaseController
    {
        private readonly IUserService _userManager;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AdminController(
            IUserService userManager, 
            IPasswordHasher passwordHasher, 
            IHttpClientFactory httpClientFactory, 
            IConfiguration configuration)
        {
            _userManager = userManager;
            _passwordHasher = passwordHasher;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // GET /admin  -> Dashboard görünümü (kartlar, istatistikler vs.)
        [HttpGet("")]
        public async Task<IActionResult> Dashboard()
        {
            // JWT Token'ı session'dan al
            var token = HttpContext.Session.GetString("jwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            // Kullanıcı adını Session'dan alıp ViewData'ya ekleyin
            ViewData["Username"] = HttpContext.Session.GetString("Username");

            var baseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5261";
            using var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            // Web API'deki /admin/stats endpoint'ini çağırarak RoleCountsDto modelini elde et
            var response = await client.GetAsync("/admin/stats");
            RoleCountsDto? counts;
            if (response.IsSuccessStatusCode)
            {
                counts = await response.Content.ReadFromJsonAsync<RoleCountsDto>();
            }
            else
            {
                // Hata durumunda varsayılan değerler dönebilirsiniz
                counts = new RoleCountsDto
                {
                    TotalUsers = 0,
                    AdminCount = 0,
                    UserCount = 0,
                    ReceptionistCount = 0,
                    NurseCount = 0,
                    CleanerCount = 0
                };
            }

            return View("Dashboard", counts);
        }

        // GET /admin/users  -> Kullanıcı listesini çeker
        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var token = HttpContext.Session.GetString("jwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var baseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5261";
            using var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseUrl);

            // Token’ı Authorization header’a ekle
            client.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue("Bearer", token);

            // Web API’de /admin/users endpoint’ini çağırıyoruz
            var response = await client.GetAsync("/admin/users");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Failed to retrieve users or no permission.";
                return View("Users", new List<UserViewModel>());
            }

            var users = await response.Content.ReadFromJsonAsync<List<UserViewModel>>();
            return View("Users", users);
        }

        // POST /admin/inline-update-role
        [HttpPost]
        public async Task<IActionResult> InlineUpdateRole(int userId, string selectedRole)
        {
            var token = HttpContext.Session.GetString("jwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var baseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5261";
            using var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue("Bearer", token);

            // Web API’de /admin/assign-role endpointini çağırarak rol atayalım
            var response = await client.PostAsJsonAsync("/admin/assign-role", new
            {
                UserId = userId,
                Role = selectedRole
            });

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                TempData["AdminError"] = $"Role assignment error: {errorMessage}";
            }
            else
            {
                TempData["AdminMessage"] = "Role updated successfully!";
            }

            // Inline güncelleme -> tekrar Users sayfasına dön
            return RedirectToAction("Users");
        }


        // GET /admin/delete/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var token = HttpContext.Session.GetString("jwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var baseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5261";
            using var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Authorization
                = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.DeleteAsync($"/admin/users/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                TempData["AdminError"] = $"Failed to delete user: {error}";
            }
            else
            {
                TempData["AdminMessage"] = "User deleted successfully!";
            }

            return RedirectToAction("Users");
        }

        [HttpGet]
        public IActionResult Profile()
        {
            // Burada, veritabanından veya token'dan kullanıcı bilgilerini çekebilirsiniz.
            // Örneğin, session'dan da alabilirsiniz:
            var username = HttpContext.Session.GetString("Username") ?? "DefaultUser";
            // Kullanıcının rolü de session veya başka bir kaynaktan alınabilir.
            var role = HttpContext.Session.GetString("UserRole") ?? "User";

            var model = new UserProfileViewModel
            {
                Username = username,
                Role = role
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Settings()
        {
            var model = new UserSettingsViewModel
            {
                Username = HttpContext.Session.GetString("Username") ?? "DefaultUser",
                Role = HttpContext.Session.GetString("UserRole") ?? "Account"  // Varsayılan bir rol değeri
            };

            return View(model);
        }
        
        // POST: /admin/change-username
        [HttpPost("change-username")]
        public IActionResult ChangeUsername([FromForm] string newUsername)
        {
            if (string.IsNullOrEmpty(newUsername))
                return BadRequest(new { success = false, message = "New username cannot be empty." });

            // Şu anki kullanıcıyı token ya da HttpContext.User üzerinden alıyoruz.
            var currentUsername = User.Identity?.Name;
            if (string.IsNullOrEmpty(currentUsername))
                return Unauthorized(new { success = false, message = "User not authenticated." });

            // Kullanıcıyı bulun
            var user = _userManager.GetAll().FirstOrDefault(u => u.Username == currentUsername);
            if (user == null)
                return NotFound(new { success = false, message = "User not found." });

            user.Username = newUsername;
            _userManager.Update(user);

            return Ok(new { success = true, message = "Username updated successfully!" });
        }
        
        // POST: /admin/change-password
        [HttpPost("change-password")]
        public IActionResult ChangePassword([FromForm] string currentPassword, [FromForm] string newPassword)
        {
            if (string.IsNullOrEmpty(currentPassword) || string.IsNullOrEmpty(newPassword))
                return BadRequest(new { success = false, message = "Password fields cannot be empty." });

            var currentUsername = User.Identity?.Name;
            if (string.IsNullOrEmpty(currentUsername))
                return Unauthorized(new { success = false, message = "User not authenticated." });

            var user = _userManager.GetAll().FirstOrDefault(u => u.Username == currentUsername);
            if (user == null)
                return NotFound(new { success = false, message = "User not found." });

            // Doğru mevcut şifre kontrolü: Authenticate metodunu kullanarak
            var token = _userManager.Authenticate(user.Username, currentPassword);
            if (token == null)
                return BadRequest(new { success = false, message = "Current password is incorrect." });

            // Yeni şifreyi hashleyin ve güncelleyin
            user.PasswordHash = _passwordHasher.HashPassword(newPassword);
            _userManager.Update(user);

            return Ok(new { success = true, message = "Password updated successfully!" });
        }
        
        // DELETE: /admin/delete-account
        [HttpDelete("delete-account")]
        public IActionResult DeleteAccount()
        {
            var currentUsername = User.Identity?.Name;
            if (string.IsNullOrEmpty(currentUsername))
                return Unauthorized(new { success = false, message = "User not authenticated." });

            var user = _userManager.GetAll().FirstOrDefault(u => u.Username == currentUsername);
            if (user == null)
                return NotFound(new { success = false, message = "User not found." });

            _userManager.Delete(user);

            // Not: Hesap silindikten sonra kullanıcının oturumunu kapatmak gerekebilir.
            return Ok(new { success = true, message = "Account deleted successfully!" });
        }
    }
}