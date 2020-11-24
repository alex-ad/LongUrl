using System;
using System.Collections.Generic;
using System.Linq;
using LongUrl.ViewModels;

namespace LongUrl.Models
{
    /// <summary>
    /// Model for initializing data containing Request params from WebUI
    /// </summary>
    public class RequestUrl
    {
        /// <summary>
        /// Model constructor for API-request
        /// </summary>
        /// <param name="url">Request: ShortURL for decoding</param>
        /// <param name="antivirus">Request: whether antivirus is On, either Off</param>
        public RequestUrl(string url, bool antivirus)
        {
            if (string.IsNullOrEmpty(url) || url.Length < 4)
                throw new ArgumentException("The URL must be greater then 4 symbols", nameof(url));
            UrlList = null;
            UrlSingle = url;
            Antivirus = antivirus;
            MultiUrl = false;
        }

        /// <summary>
        /// Model constructor for Web-request
        /// </summary>
        /// <param name="data">Request: data Model containing request params</param>
        public RequestUrl(IndexViewModel data)
        {
            UrlList = data.InUrlList?.Last()?.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                .Distinct().ToList();
            MultiUrl = data.InMultiUrl;
            UrlSingle = data.InUrlSingle;
            Antivirus = data.InMultiUrl ? false : data.InAntivirus;
        }

        /// <summary>
        /// Input {set} property: whether Antivirus checking is On, either Off
        /// </summary>
        public bool Antivirus { get; set; }
        /// <summary>
        /// Input {set} property: whether MultiURL mode is On, either Off
        /// </summary>
        public bool MultiUrl { get; set; }
        /// <summary>
        /// Input {set} property: URLs-list in the MultiURL mode
        /// </summary>
        public List<string> UrlList { get; set; }
        /// <summary>
        /// Input {set} property: URL in the SingleURL mode
        /// </summary>
        public string UrlSingle { get; set; }
    }
}