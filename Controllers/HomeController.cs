using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LongUrl.Core;
using LongUrl.Models;
using LongUrl.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace LongUrl.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStringLocalizer<Locale> _locale;

        public HomeController(IStringLocalizer<Locale> locale)
        {
            _locale = locale;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(IndexViewModel data)
        {
            if (data.InMultiUrl && (data.InUrlList == null || !data.InUrlList.Any()))
                ModelState.AddModelError(nameof(data.InUrlList), _locale["validation_set_urls"]);

            if (!data.InMultiUrl && (string.IsNullOrEmpty(data.InUrlSingle) || data.InUrlSingle.Length < 4))
                ModelState.AddModelError(nameof(data.InUrlSingle), _locale["validation_set_url"]);

            if (data.InMultiUrl && data.InUrlList != null && data.InUrlList.Any())
            {
                var list = data.InUrlList.Last()?.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                    .Distinct().ToList();
                if (list == null || !list.Any())
                    ModelState.AddModelError(nameof(data.InUrlList), _locale["validation_set_urls"]);
            }

            if (ModelState.IsValid)
            {
                var responseUrl = await GetLong(data);
                data.OutAntivirusMessage = responseUrl.AntivirusMessage;
                data.OutAntivirusStatus = responseUrl.AntivirusStatus;
                data.OutSuccess = responseUrl.Success;
                data.OutUrl = responseUrl.Url;
                data.OutSafeBrowsing = responseUrl.SafeBrowsing;
                return View(data);
            }

            return View(data);
        }

        [HttpGet]
        public IActionResult Api()
        {
            return View();
        }

        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions {Expires = DateTimeOffset.UtcNow.AddYears(1)}
            );

            return LocalRedirect(returnUrl);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }

        [NonAction]
        private async Task<ResponseUrl> GetLong(IndexViewModel request)
        {
            try
            {
                return await Task.Run(async () =>
                {
                    var requestUrl = new RequestUrl(request);
                    var longUri = new LongUri(requestUrl);
                    var responseUrl = await longUri.Get();
                    return responseUrl;
                });
            }
            catch (Exception)
            {
                var responseUrl = new ResponseUrl();
                return responseUrl;
            }
        }
    }
}