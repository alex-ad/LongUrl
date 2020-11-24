using System;

namespace LongUrl.Models
{
    /// <summary>
    /// Model UrlItem is used for storing intermediate data while processing chain of URLs-decoding
    /// (because of ShortURL is consist of chain of URLs often)
    /// </summary>
    public class UrlItem
    {
        public UrlItem(string url)
        {
            if (string.IsNullOrEmpty(url) || url.Length < 4)
                throw new ArgumentException("The URL must be greater then 4 symbols", nameof(url));
            Success = false;
            Next = url.Trim();
            Uncovered = null;
        }

        /// <summary>
        /// Flag: if the last URL (in the chain) decoding is successful
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// Stores decoded URL of the last successful decoding-operation
        /// </summary>
        public string Uncovered { get; set; }
        /// <summary>
        /// Stores next URl for decoding int the chain
        /// </summary>
        public string Next { get; set; }
    }
}