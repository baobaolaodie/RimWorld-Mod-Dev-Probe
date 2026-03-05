using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Probes
{
    public class DefsProbe : IProbe
    {
        public string Name => "def";
        private ProbeContext _context;
        private ConcurrentBag<DefInfo> _defIndex;
        private ConcurrentDictionary<string, List<DefInfo>> _defTypeIndex = new ConcurrentDictionary<string, List<DefInfo>>();

        public void Initialize(ProbeContext context)
        {
            _context = context;
        }

        private void EnsureIndexBuilt()
        {
            if (_defIndex != null) return;

            _defIndex = new ConcurrentBag<DefInfo>();
            var paths = GetDefPaths();

            Parallel.ForEach(paths, path =>
            {
                try
                {
                    var doc = XDocument.Load(path);
                    foreach (var element in doc.Root?.Elements() ?? Enumerable.Empty<XElement>())
                    {
                        var defName = element.Element("defName")?.Value;
                        if (!string.IsNullOrEmpty(defName))
                        {
                            var info = new DefInfo
                            {
                                DefType = element.Name.LocalName,
                                DefName = defName,
                                ParentName = element.Attribute("ParentName")?.Value,
                                FileName = Path.GetFileName(path),
                                FilePath = path,
                                Element = element
                            };
                            _defIndex.Add(info);

                            var typeList = _defTypeIndex.GetOrAdd(info.DefType, _ => new List<DefInfo>());
                            lock (typeList) { typeList.Add(info); }
                        }
                    }
                }
                catch { }
            });
        }

        private IEnumerable<string> GetDefPaths()
        {
            var paths = new List<string>();

            if (_context.GameDataPath != null && Directory.Exists(_context.GameDataPath))
            {
                paths.AddRange(Directory.GetFiles(_context.GameDataPath, "*.xml", SearchOption.AllDirectories));
            }

            if (_context.GameDllPath != null && Directory.Exists(_context.GameDllPath))
            {
                paths.AddRange(Directory.GetFiles(_context.GameDllPath, "*.xml", SearchOption.AllDirectories));
            }

            return paths;
        }

        public IEnumerable<ProbeResult> Search(string query, SearchOptions options)
        {
            EnsureIndexBuilt();

            var comparison = options.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            var results = new List<DefProbeResult>();

            IEnumerable<DefInfo> searchBase = string.IsNullOrEmpty(options.FilterType)
                ? _defIndex
                : _defTypeIndex.TryGetValue(options.FilterType, out var typed) ? typed : Enumerable.Empty<DefInfo>();

            foreach (var def in searchBase)
            {
                bool match = options.ExactMatch
                    ? def.DefName.Equals(query, comparison)
                    : def.DefName.Contains(query, comparison) || def.DefType.Contains(query, comparison);

                if (match)
                {
                    results.Add(new DefProbeResult(def));
                    if (results.Count >= options.MaxResults) break;
                }
            }

            return results;
        }

        public ProbeResult GetDetails(string id)
        {
            EnsureIndexBuilt();

            var def = _defIndex.FirstOrDefault(d => d.DefName == id);
            if (def != null)
            {
                return new DefProbeResult(def, true);
            }
            return null;
        }

        public void ClearCache()
        {
            _defIndex = null;
            _defTypeIndex.Clear();
        }

        public IEnumerable<string> GetDefTypes()
        {
            EnsureIndexBuilt();
            return _defTypeIndex.Keys.OrderBy(k => k);
        }
    }

    public class DefInfo
    {
        public string DefType { get; set; }
        public string DefName { get; set; }
        public string ParentName { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public XElement Element { get; set; }
    }
}
