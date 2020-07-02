using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using LongUrl.Models;
using Microsoft.AspNetCore.Http;

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
            _responseUrl = new ResponseUrl();
        }

        public async Task<ResponseUrl> Get()
        {
            try
            {
                foreach (string uri in _requestUrl.UrlList)
                {
                    _urlItem = new UrlItem(uri);
                    List<string> list = new List<string>();
                    do
                    {
                        UrlParsedType urlParsed = null;
                        urlParsed = new UrlParsedType(_urlItem.Next);
                        _urlItem.Next = urlParsed.Url;
                        await GetLongUrlFromShort(_urlItem.Next);
                        if (_urlItem.Uncovered != null) list.Add(_urlItem.Uncovered);
                    } while (!string.IsNullOrEmpty(_urlItem.Next));

                    if (_requestUrl.MultiUrl) _responseUrl.Url.Add(list.Last());
                    else _responseUrl.Url = list;
                }

                if ((_responseUrl.Success) && (_requestUrl.Antivirus))
                {
                    await AntivirusCheck(_responseUrl.Url.Last());
                }

                return _responseUrl;
            }
            catch
            {
                return _responseUrl;
            }
        }

        private async Task GetLongUrlFromShort(string url)
        {
            _urlItem.Success = false;
            _urlItem.Next = null;
            _urlItem.Uncovered = null;
            Uri uri;
            try
            {
                uri = new Uri(url);
            }
            catch (UriFormatException)
            {
                return;
            }

            HttpClient http = new HttpClient();
            HttpResponseMessage response = await http.GetAsync(uri.ToString());
            
            if ((response.StatusCode == HttpStatusCode.Redirect)                // 302
                || (response.StatusCode == HttpStatusCode.Moved)                // 301
                || (response.StatusCode == HttpStatusCode.RedirectKeepVerb)     // 307
                || (response.StatusCode == HttpStatusCode.RedirectMethod)       // 303
                || (response.StatusCode == HttpStatusCode.PermanentRedirect)    // 308
                || (response.StatusCode == HttpStatusCode.TemporaryRedirect)    // 307
                )
            {
                _urlItem.Success = true;
                string ret = response.Headers.Location.OriginalString.Trim(new[] {' ', '/'});
                _urlItem.Next = ret;
                _urlItem.Uncovered = ret;
                return;
            }

            if (!string.Equals(response.RequestMessage.RequestUri.OriginalString.Trim(new[] { ' ', '/' }), url, StringComparison.InvariantCultureIgnoreCase))
            {
                _urlItem.Success = true;
                string ret = response.RequestMessage.RequestUri.OriginalString.Trim(new[] {' ', '/'});
                _urlItem.Next = ret;
                _urlItem.Uncovered = ret;
                return;
            }

            if (response.StatusCode == HttpStatusCode.OK)                       // 200
            {
                _urlItem.Success = true;
            }
        }

        private async Task AntivirusCheck(string url)
        {
            HttpClient http = new HttpClient();
            string request = "http://online.drweb.com/result/?url=" + url;
            HttpResponseMessage response = await http.GetAsync(request);

            if (response.StatusCode != HttpStatusCode.OK) return;

            string responseMessage = response.Content.ReadAsStringAsync().Result;
            int posStart = responseMessage.IndexOf("<!-- scan report begin -->");
            posStart = posStart < 0 ? posStart : posStart + "<!-- scan report begin -->".Length;
            int posEnd = responseMessage.LastIndexOf("<!-- scan report end -->");

            if (responseMessage.Contains("<!-- X_SCAN_STATE: CLEAN -->"))
                _responseUrl.AntivirusStatus = AntivirusStatusType.Clear;
            else if (responseMessage.Contains("<!-- X_SCAN_STATE: INFECTED -->"))
                _responseUrl.AntivirusStatus = AntivirusStatusType.Infected;
            else _responseUrl.AntivirusStatus = AntivirusStatusType.Error;

            if (posStart < 0 || posEnd < 0) return;

            _responseUrl.AntivirusMessage = responseMessage.Substring(posStart, posEnd - posStart);
        }
    }
}
