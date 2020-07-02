using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace LongUrl.Core
{
    public class UrlParsedType
    {
        public UrlParsedType(string url)
        {
            _current = url;
            _next = url;
            Url = Get(url);
        }

        private string _current;
        private string _next;
        public string Url { get; }

        private string Get(string currentUri)
        {
            try
            {
                Uri uri;
                try
                {
                    currentUri =
                        (currentUri.StartsWith("https://", StringComparison.CurrentCultureIgnoreCase) ||
                         currentUri.StartsWith("http://", StringComparison.CurrentCultureIgnoreCase))
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
                    if (string.IsNullOrEmpty(item.Value))
                    {
                        _next = Get(item.Value);
                        _current = currentUri;
                        continue;
                    }
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
