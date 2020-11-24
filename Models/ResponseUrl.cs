using System.Collections.Generic;

namespace LongUrl.Models
{
    /// <summary>
    /// Enum-type for Antivirus checking status
    /// </summary>
    public enum AntivirusStatusType
    {
        /// <summary>
        /// Antivirus status: Clear
        /// </summary>
        Clear,
        /// <summary>
        /// Antivirus status: Infected
        /// </summary>
        Infected,
        /// <summary>
        /// Antivirus status: Error (eg. "requested source is unreachable)"
        /// </summary>
        Error
    }

    /// <summary>
    /// Model for initializing Response data
    /// </summary>
    public class ResponseUrl
    {
        public ResponseUrl()
        {
            Url = new List<string>();
            AntivirusStatus = AntivirusStatusType.Error;
            AntivirusMessage = string.Empty;
            SafeBrowsing = false;
        }

        /// <summary>
        /// Response: if decoding process is successful
        /// </summary>
        public bool Success => Url.Count > 0;
        /// <summary>
        /// Response: List (one or more) of the decoded URLs
        /// </summary>
        public List<string> Url { get; set; }
        /// <summary>
        /// Response: Antivirus checking status
        /// </summary>
        public AntivirusStatusType AntivirusStatus { get; set; }
        /// <summary>
        /// Response: Antivirus checking message
        /// </summary>
        public string AntivirusMessage { get; set; }
        /// <summary>
        /// Response: SafeBrowsing checking status
        /// </summary>
        public bool SafeBrowsing { get; set; }
    }
}