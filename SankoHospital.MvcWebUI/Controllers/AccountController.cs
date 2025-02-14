using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SankoHospital.MvcWebUI.Controllers.Base;
using SankoHospital.MvcWebUI.Models;
using SankoHospital.MvcWebUI.Models.AuthModels;

namespace SankoHospital.MvcWebUI.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : BaseController
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;

        public AccountController(IHttpClientFactory httpClientFactory, IConfiguration configuration) : base()
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        // GET /account/login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST /account/login
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Örnek: "ApiSettings:BaseUrl" = "http://localhost:5165"
            var baseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5261";

            // 1. Web API'ye istek atarak kullanıcıyı doğrula (/Auth/login)
            using var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseUrl);

            var response = await client.PostAsJsonAsync("/Auth/login", new
            {
                Username = model.Username,
                Password = model.Password
            });

            if (!response.IsSuccessStatusCode)
            {
                ModelState.AddModelError("", "Wrong username or password.");
                return View(model);
            }

            // 2. Token bilgisini al ({ "token": "eyJhb..." } gibi)
            var tokenResponse = await response.Content.ReadFromJsonAsync<JwtTokenResponse>();
            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.Token))
            {
                ModelState.AddModelError("", "Invalid token.");
                return View(model);
            }

            // 3. Token'ı Session'da sakla veya cookie kullanabilirsin
            HttpContext.Session.SetString("jwtToken", tokenResponse.Token);
            HttpContext.Session.SetString("Username", model.Username);

            // 4. Giriş başarılı, ana sayfaya yönlendir

            // Token'ı decode edin, role = "Admin" mi bakın
            var role = DecodeTokenAndGetRole(tokenResponse.Token);

            // Burada rol bilgisini de Session'a kaydedelim:
            HttpContext.Session.SetString("UserRole", role);

            switch (role)
            {
                case "Admin":
                    return RedirectToAction("Dashboard", "Admin");
                case "User":
                    return RedirectToAction("Dashboard", "User");
                case "Nurse":
                    return RedirectToAction("Dashboard", "Nurse");
                case "Cleaner":
                    return RedirectToAction("Dashboard", "Cleaner");
                case "Receptionist":
                    return RedirectToAction("Dashboard", "Receptionist");
                default:
                    // Varsayılan olarak User controller'a yönlendirme yapabilirsiniz.
                    return RedirectToAction("Dashboard", "User");
            }
        }

        // GET /account/register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST /account/register
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "Unmatched passwords.");
                return View(model);
            }

            string baseUrl = _configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5165";

            // 1. Kayıt için Web API'ye POST isteği (/Auth/register)
            using var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseUrl);

            // Gönderilecek user datası
            var newUser = new
            {
                Username = model.Username,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword // Sunucu tarafında PBKDF2 ile hash'lenir
            };

            var response = await client.PostAsJsonAsync("/Auth/register", newUser);
            if (response.IsSuccessStatusCode) return RedirectToAction("login", "account");

            var errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", $"Error: {errorMessage}");
            return View(model);

            // Kayıt başarılı -> Login sayfasına yönlendir
        }

        [HttpGet]
        public IActionResult Logout()
        {
            // Session'dan JWT token ve kullanıcı adını temizle
            HttpContext.Session.Remove("jwtToken");
            HttpContext.Session.Remove("Username");

            // Eğer Cookie Authentication kullanıyorsanız, 
            // await HttpContext.SignOutAsync(); ekleyebilirsiniz.

            // Login sayfasına yönlendir
            return RedirectToAction("Login", "Account");
        }

        private static string DecodeTokenAndGetRole(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var roleClaim = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            return roleClaim?.Value;
        }
    }
}