using Microsoft.AspNetCore.Mvc;
using SankoHospital.MvcWebUI.Models;
using System.Net.Http.Headers;
using SankoHospital.Business.DTOs;
using SankoHospital.MvcWebUI.Controllers.Base;

namespace SankoHospital.MvcWebUI.Controllers
{
    [Route("admin")]
    public class AdminController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AdminController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
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
        [HttpGet("users")]
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
        [HttpPost("inline-update-role")]
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
            var response = await client.PostAsJsonAsync("/admin/assign-role", new {
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
        [HttpGet("delete/{id}")]
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

        [HttpGet("profile")]
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

        [HttpGet("settings")]
        public IActionResult Settings()
        {
            return View();
        }
    }
}
