using Microsoft.AspNetCore.Mvc;
using SankoHospital.MvcWebUI.Models;
using System.Net.Http;
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

        // GET /admin
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            // 1. Web API’den tüm kullanıcıları çek (Only Admin can do it).
            // 2. View’da listele.
            
            // JWT Token’ı session’dan al
            var token = HttpContext.Session.GetString("jwtToken");
            if (string.IsNullOrEmpty(token))
            {
                // Henüz login değil, login'e yönlendir
                return RedirectToAction("login", "account");
            }

            var baseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5261";
            using var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseUrl);

            // Token’ı Authorization header’a ekle
            client.DefaultRequestHeaders.Authorization 
                = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Web API’de /admin/users benzeri bir endpoint olduğunu varsayıyoruz
            var response = await client.GetAsync("/admin/users");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Kullanıcıları çekerken hata oluştu veya yetkiniz yok.";
                return View(new List<UserViewModel>());
            }

            var users = await response.Content.ReadFromJsonAsync<List<UserViewModel>>();
            return View(users);
        }

        // Örnek: Rol değiştirme sayfası
        [HttpGet("assign-role/{userId}")]
        public IActionResult AssignRole(int userId)
        {
            // Bir form göstererek rolu seçeceği bir view döndür
            var model = new AssignRoleViewModel { UserId = userId };
            return View(model);
        }

        // POST: /admin/assign-role
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRolePost(AssignRoleViewModel model)
        {
            var token = HttpContext.Session.GetString("jwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("login", "account");
            }

            var baseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5261";
            using var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Authorization 
                = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Web API’de /admin/assign-role benzeri bir endpoint olduğunu varsayıyoruz
            var response = await client.PostAsJsonAsync("/admin/assign-role", new 
            {
                UserId = model.UserId,
                Role = model.SelectedRole
            });

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError("", $"Rol ataması hatası: {errorMessage}");
                return View("AssignRole", model);
            }

            // Başarılı olunca Admin/Index’e dön
            return RedirectToAction("Index");
        }
    }
}
