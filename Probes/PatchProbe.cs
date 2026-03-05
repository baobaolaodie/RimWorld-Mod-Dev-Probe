using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Probes
{
    public class PatchProbe : IProbe
    {
        public string Name => "patch";
        private ProbeContext _context;
        private ConcurrentBag<PatchInfo> _patchIndex;

        public void Initialize(ProbeContext context)
        {
            _context = context;
        }

        private void EnsureIndexBuilt()
        {
            if (_patchIndex != null) return;

            _patchIndex = new ConcurrentBag<PatchInfo>();
            var paths = GetPatchPaths();

            Parallel.ForEach(paths, path =>
            {
                try
                {
                    var doc = XDocument.Load(path);
                    var patchNode = doc.Root;
                    if (patchNode?.Name.LocalName != "Patch") return;

                    foreach (var op in patchNode.Elements())
                    {
                        var info = ParseOperation(op, path);
                        if (info != null)
                        {
                            _patchIndex.Add(info);
                        }
                    }
                }
                catch { }
            });
        }

        private IEnumerable<string> GetPatchPaths()
        {
            var paths = new List<string>();

            if (_context.GameDataPath != null && Directory.Exists(_context.GameDataPath))
            {
                paths.AddRange(Directory.GetFiles(_context.GameDataPath, "*.xml", SearchOption.AllDirectories)
                    .Where(p => p.Contains("Patch") || p.Contains("patch")));
            }

            return paths;
        }

        private PatchInfo ParseOperation(XElement op, string filePath)
        {
            var opType = op.Name.LocalName;
            if (!opType.StartsWith("Operation")) return null;

            var info = new PatchInfo
            {
                OperationType = opType,
                FileName = Path.GetFileName(filePath),
                FilePath = filePath
            };

            var xpath = op.Element("xpath")?.Value;
            if (!string.IsNullOrEmpty(xpath))
            {
                info.XPath = xpath;
                info.TargetDef = ExtractDefName(xpath);
            }

            var value = op.Element("value");
            if (value != null)
            {
                info.Value = FormatValueElement(value);
            }

            var order = op.Attribute("Order")?.Value;
            if (!string.IsNullOrEmpty(order))
            {
                info.Order = order;
            }

            var priority = op.Attribute("Priority")?.Value;
            if (!string.IsNullOrEmpty(priority))
            {
                info.Priority = priority;
            }

            return info;
        }

        private string ExtractDefName(string xpath)
        {
            var parts = xpath.Split('/');
            foreach (var part in parts)
            {
                if (part.Contains("[@Name=\"") || part.Contains("[defName=\""))
                {
                    var start = part.IndexOf("\"") + 1;
                    var end = part.LastIndexOf("\"");
                    if (start > 0 && end > start)
                    {
                        return part.Substring(start, end - start);
                    }
                }
            }
            return null;
        }

        private string FormatValueElement(XElement value)
        {
            var sb = new StringBuilder();
            foreach (var child in value.Elements())
            {
                FormatElement(child, sb, 0);
            }
            return sb.ToString();
        }

        private void FormatElement(XElement element, StringBuilder sb, int indent)
        {
            var prefix = new string(' ', indent * 2);
            sb.AppendLine($"{prefix}<{element.Name.LocalName}>");
            foreach (var child in element.Elements())
            {
                FormatElement(child, sb, indent + 1);
            }
            if (!element.HasElements)
            {
                sb.AppendLine($"{prefix}  {element.Value}");
            }
            sb.AppendLine($"{prefix}</{element.Name.LocalName}>");
        }

        public IEnumerable<ProbeResult> Search(string query, SearchOptions options)
        {
            EnsureIndexBuilt();

            var comparison = options.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            var results = new List<PatchProbeResult>();

            foreach (var patch in _patchIndex)
            {
                bool match = options.ExactMatch
                    ? (patch.TargetDef?.Equals(query, comparison) ?? false) ||
                      patch.OperationType.Equals(query, comparison)
                    : (patch.TargetDef?.Contains(query, comparison) ?? false) ||
                      (patch.XPath?.Contains(query, comparison) ?? false) ||
                      patch.OperationType.Contains(query, comparison);

                if (match)
                {
                    results.Add(new PatchProbeResult(patch));
                    if (results.Count >= options.MaxResults) break;
                }
            }

            return results;
        }

        public ProbeResult GetDetails(string id)
        {
            EnsureIndexBuilt();

            var patch = _patchIndex.FirstOrDefault(p => p.TargetDef == id);
            if (patch != null)
            {
                return new PatchProbeResult(patch, true);
            }
            return null;
        }

        public void ClearCache()
        {
            _patchIndex = null;
        }

        public IEnumerable<PatchInfo> GetPatchesForDef(string defName)
        {
            EnsureIndexBuilt();
            return _patchIndex.Where(p => p.TargetDef == defName);
        }
    }

    public class PatchInfo
    {
        public string OperationType { get; set; }
        public string XPath { get; set; }
        public string TargetDef { get; set; }
        public string Value { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Order { get; set; }
        public string Priority { get; set; }
    }
}
