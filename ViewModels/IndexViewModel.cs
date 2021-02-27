using System.Collections.Generic;
using LongUrl.Models;

namespace LongUrl.ViewModels
{
    /// <summary>
    /// Main-Page ViewModel: used for getting and setting data
    /// </summary>
    public class IndexViewModel
    {
        /// <summary>
        /// Input {set} field: whether Antivirus checking is On, either Off
        /// </summary>
        public bool InAntivirus { get; set; }
        /// <summary>
        /// Input {set} field: whether MultiURL mode is On, either Off
        /// </summary>
        public bool InMultiUrl { get; set; }
        /// <summary>
        /// Input {set} field: URLs-list in the MultiURL mode
        /// </summary>
        public List<string> InUrlList { get; set; }
        /// <summary>
        /// Input {set} field: URL in the SingleURL mode
        /// </summary>
        public string InUrlSingle { get; set; }
        public bool OutSuccess { get; set; }
        /// <summary>
        /// Output {get} field: decoded short URL
        /// </summary>
        public List<string> OutUrl { get; set; }
        /// <summary>
        /// Output {get} field: result status of the Antivirus checking
        /// </summary>
        public AntivirusStatusType OutAntivirusStatus { get; set; }
        /// <summary>
        /// Output {get} field: result message of the Antivirus checking
        /// </summary>
        public string OutAntivirusMessage { get; set; }
        /// <summary>
        /// Output {get} field: result status of the SafeBrowsing checking
        /// </summary>
        public bool OutSafeBrowsing { get; set; }
    }
}