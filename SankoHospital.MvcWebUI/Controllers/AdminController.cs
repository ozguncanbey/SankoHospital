using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Mvc.Rendering;
using SankoHospital.Business.Abstract;
using SankoHospital.Business.DTOs;
using SankoHospital.Core.Security;
using SankoHospital.MvcWebUI.Controllers.Base;
using SankoHospital.MvcWebUI.Models.FilterModels;
using SankoHospital.MvcWebUI.Models.UserModels;

namespace SankoHospital.MvcWebUI.Controllers
{
    [Route("[controller]/[action]")]
    public class AdminController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        
        private readonly IUserService _userManager;

        // GET /admin  -> Dashboard görünümü (kartlar, istatistikler vs.)
        public AdminController(IUserService userManager, IPasswordHasher passwordHasher,
            IHttpClientFactory httpClientFactory, IConfiguration configuration) : base(userManager, passwordHasher)
        {
            _userManager = userManager;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

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
        public async Task<IActionResult> Users(UserListViewModel model)
        {
            var token = HttpContext.Session.GetString("jwtToken");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account");
            }

            var baseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5261";
            using var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Eğer model boşsa yeni bir instance oluşturun.
            if (model == null)
            {
                model = new UserListViewModel();
            }

            // Kullanıcı filtreleme fonksiyonunu kullanıyoruz.
            // Modeldeki Id, Username ve SelectedRole alanları, GetFilteredUsers metoduna gönderiliyor.
            var filteredUsers = _userManager.GetFilteredUsers(model.Id, model.Username, model.SelectedRole);

            // Filtrelenmiş kullanıcı listesini UserViewModel'e map ediyoruz.
            model.Users = filteredUsers.Select(u => new UserViewModel
            {
                Id = u.Id,
                Username = u.Username,
                Role = u.Role
            }).ToList();

            // Rol seçeneklerini dolduralım.
            model.RoleList = new List<SelectListItem>
            {
                new SelectListItem { Value = "Admin", Text = "Yönetici" },
                new SelectListItem { Value = "Nurse", Text = "Hemşire" },
                new SelectListItem { Value = "Cleaner", Text = "Temizlik Görevlisi" },
                new SelectListItem { Value = "Receptionist", Text = "Resepsiyonist" },
                new SelectListItem { Value = "User", Text = "Kullanıcı" }
            };

            return View("Users", model);
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
        public async Task<IActionResult> Delete(int id)
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
            var username = HttpContext.Session.GetString("Username") ?? "DefaultUser";

            var user = _userManager.GetByUsername(username);
            if (user == null)
            {
                return NotFound("Kullanıcı bulunamadı.");
            }
            
            var model = new UserProfileViewModel
            {
                Username = user.Username,
                Role = user.Role,
                CreatedDate = user.CreatedAt
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult Settings()
        {
            var model = new UserSettingsViewModel
            {
                Username = HttpContext.Session.GetString("Username") ?? "DefaultUser",
                Role = HttpContext.Session.GetString("UserRole") ?? "Account" // Varsayılan bir rol değeri
            };

            return View(model);
        }
    }
}