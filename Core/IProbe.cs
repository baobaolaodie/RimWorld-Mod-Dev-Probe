using System;
using System.Collections.Generic;

namespace RimWorldModDevProbe.Core
{
    public interface IProbe
    {
        string Name { get; }
        void Initialize(ProbeContext context);
        IEnumerable<ProbeResult> Search(string query, SearchOptions options);
        ProbeResult GetDetails(string id);
        void ClearCache();
    }
}
