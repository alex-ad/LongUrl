using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LongUrl.Models;

namespace LongUrl.Core
{
    /// <summary>
    /// Main class for decoding ShortURL
    /// </summary>
    public class LongUri
    {
        private readonly RequestUrl _requestUrl;
        private readonly ResponseUrl _responseUrl;
        /// <summary>
        /// Current decoding URL in the chain of ShortURLs
        /// What is it the chain? ShortURL is consist of chain of URLs often,
        /// eg. http://short1.url redirects to http://short2.url, this one redirects to another,
        /// and so on... til the "real" (LongURL) will be found
        /// </summary>
        private UrlItem _urlItem;

        public LongUri(RequestUrl requestUrl)
        {
            _requestUrl = requestUrl;
            if (!_requestUrl.MultiUrl) _requestUrl.UrlList = new List<string> {_requestUrl.UrlSingle};
            _responseUrl = new ResponseUrl();
        }

        /// <summary>
        /// Base loop to decode URLs one by one in the chain
        /// </summary>
        /// <returns></returns>
        public async Task<ResponseUrl> Get()
        {
            var list = new List<string>();
            try
            {
                // 25 - is a maximum count of ShortURLs in the mode MultiURL (switching via WebUI)
                if (_requestUrl.UrlList.Count > 25)
                    _requestUrl.UrlList.RemoveRange(25, _requestUrl.UrlList.Count - 25);
                foreach (var uri in _requestUrl.UrlList)
                {
                    if (string.IsNullOrEmpty(uri)) continue;
                    _urlItem = new UrlItem(uri);
                    list = new List<string>();
                    do
                    {
                        var urlParsed = new UrlParsedType(_urlItem.Next);               // parsing URL
                        _urlItem.Next = urlParsed.Url;                                  // ... i.e. searching any URL (if exists) int the request string
                        await GetLongUrlFromShort(_urlItem.Next);
                        if (_urlItem.Uncovered != null) list.Add(_urlItem.Uncovered);
                    } while (!string.IsNullOrEmpty(_urlItem.Next));

                    if (_requestUrl.MultiUrl) _responseUrl.Url.Add(list.Last());
                    else _responseUrl.Url = list;
                }

                if (_responseUrl.Success && _requestUrl.Antivirus)
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

        /// <summary>
        /// Core method to decode ShortURL
        /// Algorithm is primitive and dumb: make http-request and analyze if there is some redirect or not
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private async Task GetLongUrlFromShort(string url)
        {
            _urlItem.Success = false;
            _urlItem.Next = null;
            _urlItem.Uncovered = null;
            url = url.Trim(' ', '/');
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
            var http = new HttpClient();
            http.DefaultRequestHeaders.Add("User-Agent",
                "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.89 Safari/537.36");
            http.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate, br");
            http.DefaultRequestHeaders.Add("Accept",
                "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
            http.DefaultRequestHeaders.Add("Connection", "keep-alive");
            HttpResponseMessage response;
            try
            {
                response = await http.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
            }
            catch (HttpRequestException)
            {
                return;
            }

            if (response.StatusCode == HttpStatusCode.Redirect // 302
                || response.StatusCode == HttpStatusCode.Moved // 301
                || response.StatusCode == HttpStatusCode.RedirectKeepVerb // 307
                || response.StatusCode == HttpStatusCode.RedirectMethod // 303
                || response.StatusCode == HttpStatusCode.PermanentRedirect // 308
                || response.StatusCode == HttpStatusCode.TemporaryRedirect // 307
            )
            {
                _urlItem.Success = true;

                var urlParsed = new UrlParsedType(response.RequestMessage.RequestUri.OriginalString.Trim(' ', '/'));
                var parserUri = urlParsed.Url;

                if (!string.Equals(parserUri, url, StringComparison.OrdinalIgnoreCase) &&
                    Uri.TryCreate(parserUri, UriKind.RelativeOrAbsolute, out var newUri) &&
                    IsUrlValid(newUri.OriginalString))
                {
                    _urlItem.Next = newUri.OriginalString;
                    _urlItem.Uncovered = newUri.OriginalString;
                    return;
                }

                var ret = response.Headers.Location.OriginalString.Trim(' ', '/');

                _urlItem.Next = ret;
                _urlItem.Uncovered = ret;
                return;
            }

            if (response.StatusCode == HttpStatusCode.OK) // 200
            {
                if (!string.Equals(response.RequestMessage.RequestUri.OriginalString.Trim(' ', '/'), url,
                    StringComparison.OrdinalIgnoreCase))
                {
                    var ret = response.RequestMessage.RequestUri.OriginalString.Trim(' ', '/');
                    _urlItem.Next = ret;
                    _urlItem.Uncovered = ret;
                }

                _urlItem.Success = true;
                return;
            }

            if (response.StatusCode == HttpStatusCode.BadGateway // 502
                || response.StatusCode == HttpStatusCode.NotFound) // 404
            {
                var ret = response.RequestMessage.RequestUri.OriginalString.Trim(' ', '/');
                _urlItem.Success = true;
                _urlItem.Next = null;
                _urlItem.Uncovered = ret;
            }
        }

        /// <summary>
        /// Validates URL format
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private bool IsUrlValid(string url) =>
            Regex.IsMatch(url, @"(ftp|http|https)://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
    }
}