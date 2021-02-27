using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LongUrl.Core;
using LongUrl.Data;
using LongUrl.Models;
using LongUrl.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace LongUrl.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStringLocalizer<Locale> _locale;
        private readonly ITokensRepository _tokensRepository;

        public HomeController(IStringLocalizer<Locale> locale, ITokensRepository tokens)
        {
            _locale = locale;
            _tokensRepository = tokens;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestToken(AccessToken request)
        {
	        if (ModelState.IsValid)
	        {
		        request.ResultMessage = await AddToken(request);
            }
	        else
	        {
		        request.ResultMessage = _locale["token_error"];
	        }
	        return RedirectToAction("Api", new{ request.ResultMessage});
        }

        [HttpGet]
        public IActionResult Api(AccessToken request)
        {
            return View(request);
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

        private async Task<string> AddToken(AccessToken request)
        {
	        var result = string.Empty;

	        await Task.Run(() =>
	        {
		        if ( request == null )
			        throw new ArgumentNullException(nameof(request));

		        request.Email = request.Email.ToLower();
		        if ( _tokensRepository.AccessTokens.Any() )
		        {
			        var t = _tokensRepository.AccessTokens.FirstOrDefaultAsync(x =>
				        x.Email.Equals(request.Email)).Result;

			        if ( t != null )
				        result = _locale["token_exists"];
		        }

			    try
			    {
				    request.GenerateNew();
				    _tokensRepository.AddToken(request);
					Mailer.Send(request.Email, request.Token);

				    result = _locale["token_success"];
			    }
			    catch (Exception ex)
			    {
				    throw new ArgumentNullException("Error: ", ex.InnerException);
			    }
			        
            });

	        return result;
        }
    }
}