using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SankoHospital.MvcWebUI.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Session'dan Username ve UserRole değerlerini alıyoruz.
            // Eğer değer bulunamazsa, varsayılan değerler de ayarlanabilir.
            ViewData["Username"] = HttpContext.Session.GetString("Username") ?? "Guest";
            ViewData["UserRole"] = HttpContext.Session.GetString("UserRole") ?? "User";
            
            base.OnActionExecuting(context);
        }
    }
}