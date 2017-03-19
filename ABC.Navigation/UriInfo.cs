using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ABC.Navigation
{
    [DebuggerDisplay("Path: {Path}, Segments: {Segments}")]
    public class UriInfo
    {
        public string Path { get; }
        public string[] Segments { get; }
        public Dictionary<string, string> QueryString { get; }

        public UriInfo(string url)
        {
            if (!url.Contains("://"))
            {
                throw new ArgumentException("The argument 'url' does not contain a scheme!");
            }

            var urlparts = url.Split(new[] { "://" }, StringSplitOptions.None);
            var hierarchy = urlparts[1];
            var pathparts = hierarchy.Split('?');

            Path = pathparts[0].TrimStart('/').TrimEnd('/');
            Segments = Path.Split('/');

            if (pathparts.Length > 1)
            {
                var querystring = pathparts[1];
                var pairs = querystring.Split('&');

                QueryString = pairs
                    .Select(o => o.Split('='))
                    .Where(items => items.Length == 2)
                    .ToDictionary(pair => pair[0], pair => pair[1]);
            }

            if (QueryString == null)
            {
                QueryString = new Dictionary<string, string>();
            }
        }

    }
}
