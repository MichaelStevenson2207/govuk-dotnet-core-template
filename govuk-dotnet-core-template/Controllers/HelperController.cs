using Microsoft.AspNetCore.Mvc;

namespace govuk_dotnet_core_template.Controllers
{
    public class HelperController : Controller
    {
        public RedirectToActionResult TimeoutResult()
        {
            return RedirectToAction("Timeout", "Helper");
        }

        [HttpGet]
        public IActionResult Timeout()
        {
            return View();
        }
    }
}