using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LongUrl.Models;

namespace LongUrl.ViewModels
{
    public class IndexViewModel
    {
        [Required]
        public bool InAntivirus { get; set; }
        [Required]
        public bool InMultiUrl { get; set; }
        [Required] [Url]
        public List<string> InUrlList { get; set; }
        [Required] [Url]
        public string InUrlSingle { get; set; }
        public bool OutSuccess { get; set; }
        public List<string> OutUrl { get; set; }
        public AntivirusStatusType OutAntivirusStatus { get; set; }
        public string OutAntivirusMessage { get; set; }
        public bool OutSafeBrowsing { get; set; }
    }
}
