using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ABC.Navigation
{
    [DebuggerDisplay("UrlPattern: {UrlPattern}, PageType: {PageType}, TargetType: {TargetType}")]
    public class Route
    {
        public string UrlPattern { get; }
        public Type PageType { get; }
        public Func<object> PageViewModelFactory { get; }
        public Type TargetType { get; }

        public Route(string urlpattern, Type pageType, Type targetType, Func<object> pageViewModelFactory)
        {
            PageViewModelFactory = pageViewModelFactory;
            UrlPattern = urlpattern;
            TargetType = targetType;
            PageType = pageType;
        }

        public List<UrlSegment> CreateSegments()
        {
            var segments = new List<UrlSegment>();
            foreach (var segment in UrlPattern.Split('/'))
            {
                if (segment.StartsWith("{") && segment.EndsWith("}"))
                {
                    segments.Add(new UrlSegment { Segment = segment.Replace("{", "").Replace("}", ""), IsVariable = true });
                }
                else
                {
                    segments.Add(new UrlSegment { Segment = segment });
                }
            }
            return segments;
        }

        public bool IsMatch(string url)
        {
            if (String.IsNullOrEmpty(UrlPattern))
            {
                return true;
            }

            var pattern = Regex.Replace(UrlPattern.ToLowerInvariant(), @"{(.*?)}", @"(.*?)");
            var match = Regex.Match(url.ToLowerInvariant(), pattern);
            return match.Success;
        }
    }
}
