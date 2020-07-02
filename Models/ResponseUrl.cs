using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LongUrl.Models
{
    public enum AntivirusStatusType
    {
        Clear,
        Infected,
        Error
    }

    public class ResponseUrl
    {
        public ResponseUrl()
        {
            Url = new List<string>();
            AntivirusStatus = AntivirusStatusType.Error;
            AntivirusMessage = string.Empty;
        }

        public bool Success => Url.Count > 0;
        public List<string> Url { get; set; }
        public AntivirusStatusType AntivirusStatus { get; set; }
        public string AntivirusMessage { get; set; }
    }
}
