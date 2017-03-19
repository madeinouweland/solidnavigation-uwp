using System.Diagnostics;

namespace ABC.Navigation
{
    [DebuggerDisplay("Segment: {Segment}, IsVariable: {IsVariable}")]
    public class UrlSegment
    {
        public string Segment { get; set; }
        public bool IsVariable { get; set; }
    }
}
