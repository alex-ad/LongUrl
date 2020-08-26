using System;
using System.Collections.Generic;
using System.Linq;
using LongUrl.ViewModels;

namespace LongUrl.Models
{
    public class RequestUrl
    {
        public RequestUrl(string url, bool antivirus)
        {
            if ((string.IsNullOrEmpty(url)) || (url.Length < 4))
                throw new ArgumentException("The URL must be greater then 4 symbols", nameof(url));
            UrlList = null;
            UrlSingle = url;
            Antivirus = antivirus;
            MultiUrl = false;
        }

        public RequestUrl(IndexViewModel data)
        {
            UrlList = data.InUrlList?.Last()?.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
            MultiUrl = data.InMultiUrl;
            UrlSingle = data.InUrlSingle;
            Antivirus = data.InMultiUrl ? false : data.InAntivirus;
        }

        public bool Antivirus { get; set; }
        public bool MultiUrl { get; set; }
        public List<string> UrlList { get; set; }
        public string UrlSingle { get; set; }
    }
}
