using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using govuk_dotnet_core_template.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace govuk_dotnet_core_template.Controllers
{
    public class ApplicationController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private const string WhiteList = "publicapi.payments.service.gov.uk";

        public ApplicationController(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Start()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Details()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Details(DetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Address");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Address()
        {
            var model = new AddressViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Address(AddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Pay");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Pay()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(BaseViewModel model)
        {
            string baseUrl = $"{this.Request.Scheme}://{this.Request.Host}/application/complete";

            GovUkPay payment = new()
            {
                Amount = 3000,
                Reference = DateTime.Now.ToString(new CultureInfo("en-GB")),
                Description = "Renew your driving licence",
                ReturnUrl = baseUrl
            };

            var client = _clientFactory.CreateClient("GovPay");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["govPaySecretKey"]);

            client.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = await client.PostAsync("payments", new StringContent(JsonConvert.SerializeObject(payment), Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                using HttpContent content = response.Content;
                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var dataObj = JObject.Parse(responseBody);

                var nextUrl = dataObj["_links"]?["next_url"]?["href"]?.ToString();

                Response.Cookies.Append(
                    "paymentUrl",
                    dataObj["_links"]?["self"]?["href"]?.ToString() ?? string.Empty,
                    new CookieOptions()
                    {
                        Path = "/",
                        HttpOnly = true,
                        Secure = true
                    }
                );

                if (nextUrl != null) return Redirect(nextUrl);
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Complete()
        {
            //reading the cookie so we can make a call to see the status of the payment from gov pay.
            string? paymentUrl = Request.Cookies["paymentUrl"];

            // Match the incoming URL against a whitelist
            if (paymentUrl == null || !paymentUrl.Contains(WhiteList))
            {
                return BadRequest();
            }

            var client = _clientFactory.CreateClient("GovPay");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _configuration["govPaySecretKey"]);

            client.DefaultRequestHeaders.Add("Accept", "application/json");

            var response = await client.GetAsync(paymentUrl);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                var dataObj = JObject.Parse(responseBody);

                var status = dataObj["state"]?["status"]?.ToString();
                ViewData["status"] = status;

                //if not success we assume payment has failed
                if (status != "success")
                {
                    var message = dataObj["state"]?["message"]?.ToString();
                    var code = dataObj["state"]?["code"]?.ToString();
                    if (code != null && message != null)
                        ModelState.AddModelError(code, message);
                }
                else
                {
                    // Flash message of success.
                    TempData["Success"] = "Payment has been received with thanks";
                }

                Response.Cookies.Delete("paymentUrl");
            }
            return View();
        }
    }
}