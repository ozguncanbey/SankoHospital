using Microsoft.AspNetCore.Mvc;
using SankoHospital.MvcWebUI.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace SankoHospital.MvcWebUI.Controllers
{
    [Route("admin")]
    public class AdminController : Controller
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
        public IActionResult Dashboard()
        {
            // Token kontrolü
            var token = HttpContext.Session.GetString("jwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            // Şimdilik dashboard için Web API'ye istek atmaya gerek yoksa 
            // doğrudan view döndürebilirsiniz:
            return View("Dashboard"); 
            
            // Eğer "dashboard" istatistiklerini Web API'den çekiyorsanız, 
            // buraya benzer HttpClient kodu ekleyip model oluşturabilirsiniz.
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

        // GET /admin/assign-role/{userId}
        [HttpGet("assign-role/{userId}")]
        public IActionResult AssignRole(int userId)
        {
            // Rol seçme formu
            var model = new AssignRoleViewModel { UserId = userId };
            return View(model);
        }

        // POST /admin/assign-role
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRolePost(AssignRoleViewModel model)
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

            // Web API’de /admin/assign-role endpoint
            var response = await client.PostAsJsonAsync("/admin/assign-role", new {
                UserId = model.UserId,
                Role = model.SelectedRole
            });

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Role assignment error: {errorMessage}");
                return View("AssignRole", model);
            }

            // Başarılı -> kullanıcı listesine dön
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

    }
}
