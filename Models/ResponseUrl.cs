using System.Collections.Generic;

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
            SafeBrowsing = false;
        }

        public bool Success => Url.Count > 0;
        public List<string> Url { get; set; }
        public AntivirusStatusType AntivirusStatus { get; set; }
        public string AntivirusMessage { get; set; }
        public bool SafeBrowsing { get; set; }
    }
}
