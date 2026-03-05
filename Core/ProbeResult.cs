using System;
using System.Collections.Generic;

namespace RimWorldModDevProbe.Core
{
    public abstract class ProbeResult
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Source { get; set; }
        public string Location { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

        public abstract void PrintDetails();
    }
}
