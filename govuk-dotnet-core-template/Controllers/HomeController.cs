using System.Diagnostics;
using govuk_dotnet_core_template.Models;
using Microsoft.AspNetCore.Mvc;

namespace govuk_dotnet_core_template.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Cookies()
    {
        return View();
    }

    public IActionResult Accessibility()
    {
        return View();
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