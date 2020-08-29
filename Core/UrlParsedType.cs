using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.WebUtilities;

namespace LongUrl.Core
{
    public class UrlParsedType
    {
        private string _current;
        private string _next;

        public UrlParsedType(string url)
        {
            _current = url;
            _next = url;
            Url = Get(url);
        }

        public string Url { get; }

        private string Get(string currentUri)
        {
            try
            {
                Uri uri;
                try
                {
                    currentUri =
                        currentUri.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                        currentUri.StartsWith("http://", StringComparison.OrdinalIgnoreCase)
                            ? currentUri
                            : "http://" + currentUri;
                    uri = new Uri(currentUri);
                }
                catch (UriFormatException)
                {
                    _next = null;
                    return currentUri;
                }

                var query = QueryHelpers.ParseNullableQuery(uri.Query);

                if (query == null || !query.Any())
                {
                    _next = null;
                    return currentUri;
                }

                var items = query
                    .SelectMany(x => x.Value, (col, value) => new KeyValuePair<string, string>(col.Key, value))
                    .ToList();
                foreach (var item in items)
                {
                    if (string.IsNullOrEmpty(item.Value)) continue;
                    _next = Get(item.Value);
                    _current = _next ?? currentUri;
                }

                return _current;
            }
            catch (StackOverflowException)
            {
                return null;
            }
        }
    }
}