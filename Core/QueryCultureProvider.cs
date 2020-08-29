using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Primitives;

namespace LongUrl.Core
{
    public class QueryCultureProvider : IRequestCultureProvider
    {
        private readonly CultureInfo _defaultCulture;

        public QueryCultureProvider(RequestCulture requestCulture)
        {
            _defaultCulture = requestCulture.Culture;
        }

        public Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            var query = httpContext.Request.Query;
            if (!query.ContainsKey("lang") ||
                !query.TryGetValue("lang", out var lang) ||
                !Regex.IsMatch(lang, @"^[a-z]{2}(-[A-Z]{2})*$"))
            {
                if (httpContext.Request.Cookies.TryGetValue(CookieRequestCultureProvider.DefaultCookieName,
                    out var cookie))
                {
                    var cultureString = cookie.Split("|");
                    var culture = cultureString.FirstOrDefault(x => Regex.IsMatch(x, @"(c=)([a-z]{2})"))?.Substring(2);
                    return string.IsNullOrEmpty(culture)
                        ? Task.FromResult(new ProviderCultureResult(_defaultCulture.TwoLetterISOLanguageName))
                        : Task.FromResult(new ProviderCultureResult(new StringSegment(culture)));
                }

                return Task.FromResult(new ProviderCultureResult(_defaultCulture.TwoLetterISOLanguageName));
            }

            httpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(lang)),
                new CookieOptions {Expires = DateTimeOffset.UtcNow.AddYears(1)}
            );

            return Task.FromResult(new ProviderCultureResult(new StringSegment(lang)));
        }
    }
}