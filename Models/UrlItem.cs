using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LongUrl.Core;

namespace LongUrl.Models
{
    public class UrlItem
    {
        public UrlItem(string url)
        {
            if ((string.IsNullOrEmpty(url)) || (url.Length < 4))
                throw new ArgumentException("The URL must be greater then 4 symbols", nameof(url));
            Success = false;
            Next = url;
            Uncovered = null;
        }

        public bool Success { get; set; }
        public string Uncovered { get; set; }
        public string Next { get; set; }
    }
}
