using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LongUrl.Models;

namespace LongUrl.Core
{
    public class LongUri
    {
        private UrlItem _urlItem;
        private readonly RequestUrl _requestUrl;
        private readonly ResponseUrl _responseUrl;

        public LongUri(RequestUrl requestUrl)
        {
            _requestUrl = requestUrl;
            if (!_requestUrl.MultiUrl) _requestUrl.UrlList = new List<string>{ _requestUrl.UrlSingle };
            _responseUrl = new ResponseUrl();
        }

        public async Task<ResponseUrl> Get()
        {
            List<string> list = new List<string>();
            try
            {
                if (_requestUrl.UrlList.Count > 25)
                    _requestUrl.UrlList.RemoveRange(25, _requestUrl.UrlList.Count-25);
                foreach (string uri in _requestUrl.UrlList)
                {
                    if (string.IsNullOrEmpty(uri)) continue;
                    _urlItem = new UrlItem(uri);
                    list = new List<string>();
                    do
                    {
                        UrlParsedType urlParsed = new UrlParsedType(_urlItem.Next);
                        _urlItem.Next = urlParsed.Url;
                        await GetLongUrlFromShort(_urlItem.Next);
                        if (_urlItem.Uncovered != null) list.Add(_urlItem.Uncovered);
                    } while (!string.IsNullOrEmpty(_urlItem.Next));

                    if (_requestUrl.MultiUrl) _responseUrl.Url.Add(list.Last());
                    else _responseUrl.Url = list;
                }

                if ((_responseUrl.Success) && (_requestUrl.Antivirus))
                {
                    var mw = new MalwareScanner(_responseUrl);
                    await mw.Go();
                }

                return _responseUrl;
            }
            catch
            {
                if (list.Any())
                {
                    if (_requestUrl.MultiUrl) _responseUrl.Url.Add(list.Last());
                    else _responseUrl.Url = list;
                }
                return _responseUrl;
            }
        }

        private async Task GetLongUrlFromShort(string url)
        {
            _urlItem.Success = false;
            _urlItem.Next = null;
            _urlItem.Uncovered = null;
            url = url.Trim(new[] {' ', '/'});
            Uri uri;
            try
            {
                uri = new Uri(url);
            }
            catch (UriFormatException)
            {
                return;
            }

            ServicePointManager.SecurityProtocol = SecurityProtocolType.SystemDefault;
            HttpClient http = new HttpClient();
            http.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.89 Safari/537.36");
            http.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            http.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            http.DefaultRequestHeaders.Add("Connection", "keep-alive");
            http.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
            HttpResponseMessage response;
            try
            {
                response = await http.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
            }
            catch (HttpRequestException ex)
            {
                return;
            }
            //HttpResponseMessage response = await http.GetAsync(uri.ToString());

            if ((response.StatusCode == HttpStatusCode.Redirect)                // 302
                || (response.StatusCode == HttpStatusCode.Moved)                // 301
                || (response.StatusCode == HttpStatusCode.RedirectKeepVerb)     // 307
                || (response.StatusCode == HttpStatusCode.RedirectMethod)       // 303
                || (response.StatusCode == HttpStatusCode.PermanentRedirect)    // 308
                || (response.StatusCode == HttpStatusCode.TemporaryRedirect)    // 307
                )
            {
                _urlItem.Success = true;

                UrlParsedType urlParsed = new UrlParsedType(response.RequestMessage.RequestUri.OriginalString.Trim(new[] { ' ', '/' }));
                var parserUri = urlParsed.Url;

                if (!string.Equals(parserUri, url, StringComparison.OrdinalIgnoreCase) && Uri.TryCreate(parserUri, UriKind.RelativeOrAbsolute, out Uri newUri) && IsUrlValid(newUri.OriginalString))
                {
                    _urlItem.Next = newUri.OriginalString;
                    _urlItem.Uncovered = newUri.OriginalString;
                    return;
                }

                var ret = response.Headers.Location.OriginalString.Trim(new[] { ' ', '/' });

                _urlItem.Next = ret;
                _urlItem.Uncovered = ret;
                return;
            }

            if (response.StatusCode == HttpStatusCode.OK)                       // 200
            {
                if (!string.Equals(response.RequestMessage.RequestUri.OriginalString.Trim(new[] { ' ', '/' }), url, StringComparison.OrdinalIgnoreCase))
                {
                    var ret = response.RequestMessage.RequestUri.OriginalString.Trim(new[] { ' ', '/' });
                    _urlItem.Next = ret;
                    _urlItem.Uncovered = ret;
                }
                _urlItem.Success = true;
                return;
            }

            if ((response.StatusCode == HttpStatusCode.BadGateway)               // 502
                || (response.StatusCode == HttpStatusCode.NotFound))             // 404
            {
                var ret = response.RequestMessage.RequestUri.OriginalString.Trim(new[] { ' ', '/' });
                _urlItem.Success = true;
                _urlItem.Next = null;
                _urlItem.Uncovered = ret;
            }
        }

        private bool IsUrlValid(string url) => Regex.IsMatch(url, @"(ftp|http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
    }
}
