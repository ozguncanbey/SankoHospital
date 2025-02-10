using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SankoHospital.MvcWebUI.Controllers;

public class BaseController : Controller
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        ViewData["Username"] = HttpContext.Session.GetString("Username");
        base.OnActionExecuting(context);
    }
}