using System;

namespace RimWorldModDevProbe.Core
{
    public class SearchOptions
    {
        public bool ExactMatch { get; set; } = false;
        public bool CaseSensitive { get; set; } = false;
        public string FilterType { get; set; }
        public int MaxResults { get; set; } = 100;
        public bool IncludeNonPublic { get; set; } = true;
    }
}
