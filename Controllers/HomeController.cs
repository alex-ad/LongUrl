using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LongUrl.Core;
using LongUrl.Data;
using LongUrl.Models;
using LongUrl.ViewModels;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using MimeKit;

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

        /// <summary>
        /// Main Page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Action-method for processing entered data (i.e. short URL) via WebUI
        /// </summary>
        /// <param name="data">MainPage ViewModel</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(IndexViewModel data)
        {
            // Set Model error: if mode MultiURL is On, but  URL-list is empty
            if (data.InMultiUrl && (data.InUrlList == null || !data.InUrlList.Any()))
                ModelState.AddModelError(nameof(data.InUrlList), _locale["validation_set_urls"]);

            // Set Model error: if mode SingleURL is On, but URL is empty of Length < 4
            if (!data.InMultiUrl && (string.IsNullOrEmpty(data.InUrlSingle) || data.InUrlSingle.Length < 4))
                ModelState.AddModelError(nameof(data.InUrlSingle), _locale["validation_set_url"]);

            // Ser model error: if mode MultiURL is On, and URL-list is set
            if (data.InMultiUrl && data.InUrlList != null && data.InUrlList.Any())
            {
                // Split MultiURL multi-line text into a List
                var list = data.InUrlList.Last()?.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                    .Distinct().ToList();
                // Set Model error: if result URL-list is empty
                if (list == null || !list.Any())
                    ModelState.AddModelError(nameof(data.InUrlList), _locale["validation_set_urls"]);
            }

            if (ModelState.IsValid)
            {
                // Start main process of shortURL decoding
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

        /// <summary>
        /// API Page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Api()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Api(AccessToken request)
        {
			if ( ModelState.IsValid )
			{
				request.ResultMessage = await AddToken(request);
                if (!string.IsNullOrEmpty(request.Token)) await SendEmail(request);
			} else
			{
				request.ResultMessage = _locale["token_error"];
			}

			return View(request);
        }

        /// <summary>
        /// About Page
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        /// <summary>
        /// Action-method for define UI language via main menu
        /// </summary>
        /// <param name="culture">UI language (en, ru)</param>
        /// <param name="returnUrl">Returning URL after UI language changing</param>
        /// <returns></returns>
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

        /// <summary>
        /// Base Action-method for starting ShortURL-decoding process
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Task ResponseUrl</returns>
        [NonAction]
        private async Task<ResponseUrl> GetLong(IndexViewModel request)
        {
            try
            {
                return await Task.Run(async () =>
                {
                    var requestUrl = new RequestUrl(request);   // forming RequestUrl Model
                    var longUri = new LongUri(requestUrl);
                    var responseUrl = await longUri.Get();      // decoding ShortURL
                    return responseUrl;
                });
            }
            catch (Exception)
            {
                var responseUrl = new ResponseUrl();
                return responseUrl;
            }
        }

        [NonAction]
        private async Task<string> AddToken(AccessToken request)
        {
	        var result = string.Empty;

            await Task.Run(() =>
	        {
		        if ( request == null )
			        throw new ArgumentNullException(nameof(request));

				request.Email = request.Email.ToLower();
				if ( _tokensRepository.AccessTokens != null && _tokensRepository.AccessTokens.Any() )
				{
					var t = _tokensRepository.AccessTokens.FirstOrDefaultAsync(x =>
						x.Email.Equals(request.Email)).Result;

					if ( t != null )
						result = _locale["token_exists"];
				}

				if ( string.IsNullOrEmpty(result) )
				{
					try
					{
						request.GenerateNew();
						_tokensRepository.AddToken(request);
						result = _locale["token_success"];
					} catch ( Exception ex )
					{
						throw new ArgumentNullException("Error: ", ex.InnerException);
					}
				}
			});

	        return result;
        }

        [NonAction]
        private async Task SendEmail(AccessToken token)
        {
	        var emailMessage = new MimeMessage();

	        emailMessage.From.Add(new MailboxAddress("LongURL Mailer", "longurl.info@gmail.com"));
	        emailMessage.To.Add(new MailboxAddress("", token.Email));
	        emailMessage.Subject = "LongURL API Token";
	        emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
	        {
		        Text = "Hello. Your token is: " + token.Token
	        };

			using var client = new SmtpClient();
			await client.ConnectAsync("smtp.gmail.com", 465, SecureSocketOptions.Auto);
			await client.AuthenticateAsync("longurl.info@gmail.com", "fgh5zj76k5ukf");
			await client.SendAsync(emailMessage);

			await client.DisconnectAsync(true);
		}
    }
}