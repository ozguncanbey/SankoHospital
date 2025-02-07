using Microsoft.AspNetCore.Mvc;
using SankoHospital.MvcWebUI.Models;

namespace SankoHospital.MvcWebUI.Controllers;

public class AccountController : Controller
{
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }
    
    [HttpPost]
    public IActionResult Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // 1. Web API'ye veya Business Katmanına istek atarak kullanıcıyı doğrula.
        // 2. Eğer doğruysa, JWT token alıp session'a kaydedebilir ya da cookie oluşturabilirsin.
        // 3. Yanlışsa, model hata mesajı ile tekrar View'a dön.

        // Örnek:
        // var token = _authService.Authenticate(model.Username, model.Password);
        // if (token == null) 
        // {
        //     ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
        //     return View(model);
        // }
        // HttpContext.Session.SetString("jwtToken", token);

        return RedirectToAction("Index", "Home"); // Giriş başarılı ise ana sayfaya yönlendir
    }
    
    // GET: /Account/Register
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    
    // POST: /Account/Register
    [HttpPost]
    public IActionResult Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        if (model.Password != model.ConfirmPassword)
        {
            ModelState.AddModelError("", "Şifreler uyuşmuyor.");
            return View(model);
        }

        // 1. Web API'ye veya Business Katmanına yeni kullanıcı ekleme isteği at.
        // var newUser = new User { Username = model.Username, PasswordHash = model.Password ... };
        // _userService.Add(newUser);

        // 2. Giriş sayfasına yönlendir veya otomatik giriş yap.
        return RedirectToAction("Login");
    }
}