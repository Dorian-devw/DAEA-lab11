using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Eventos.Web.Models;

namespace Eventos.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return RedirectToAction("Index", "Eventos");
    }

    public IActionResult ToggleModo(string? returnUrl)
    {
        var esAdmin = HttpContext.Session.GetString("ModoAdmin") == "true";
        HttpContext.Session.SetString("ModoAdmin", (!esAdmin).ToString().ToLower());

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Eventos");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
