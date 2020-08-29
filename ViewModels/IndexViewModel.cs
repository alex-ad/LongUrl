using System.Collections.Generic;
using LongUrl.Models;

namespace LongUrl.ViewModels
{
    public class IndexViewModel
    {
        public bool InAntivirus { get; set; }
        public bool InMultiUrl { get; set; }
        public List<string> InUrlList { get; set; }
        public string InUrlSingle { get; set; }
        public bool OutSuccess { get; set; }
        public List<string> OutUrl { get; set; }
        public AntivirusStatusType OutAntivirusStatus { get; set; }
        public string OutAntivirusMessage { get; set; }
        public bool OutSafeBrowsing { get; set; }
    }
}