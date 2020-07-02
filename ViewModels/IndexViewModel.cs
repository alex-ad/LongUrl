using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LongUrl.Models;

namespace LongUrl.ViewModels
{
    public class IndexViewModel
    {
        [Required]
        public bool InAntivirus { get; set; }
        [Required]
        public bool InMultiUrl { get; set; }
        [Required][Url]
        public List<string> InUrlList { get; set; }
        public bool OutSuccess { get; set; }
        public List<string> OutUrl { get; set; }
        public AntivirusStatusType OutAntivirusStatus { get; set; }
        public string OutAntivirusMessage { get; set; }
    }
}
