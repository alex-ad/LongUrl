using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LongUrl.Core;
using LongUrl.ViewModels;

namespace LongUrl.Models
{
    public class RequestUrl
    {
        public RequestUrl(string url, bool antivirus)
        {
            if ((string.IsNullOrEmpty(url)) || (url.Length < 4))
                throw new ArgumentException("The URL must be greater then 4 symbols", nameof(url));
            UrlList = new List<string>{ url };
            Antivirus = antivirus;
            MultiUrl = false;
        }

        public RequestUrl(IndexViewModel data)
        {
            if (data.InUrlList == null || !data.InUrlList.Any())
                throw new ArgumentException("The URL List is empty", nameof(data.InUrlList));
            UrlList = data.InUrlList;
            MultiUrl = data.InMultiUrl;
            Antivirus = data.InMultiUrl ? false : data.InAntivirus;
        }

        public bool Antivirus { get; set; }
        public bool MultiUrl { get; set; }
        public List<string> UrlList { get; set; }
    }
}
