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
    public class ModProbe : RimWorldModDevProbe.Core.IProbe
    {
        public string Name => "mod";
        private ProbeContext _context;
        private ConcurrentBag<ModInfo> _modIndex;
        private ConcurrentDictionary<string, ModInfo> _modById = new ConcurrentDictionary<string, ModInfo>();

        public void Initialize(ProbeContext context)
        {
            _context = context;
        }

        private void EnsureIndexBuilt()
        {
            if (_modIndex != null) return;

            _modIndex = new ConcurrentBag<ModInfo>();

            if (_context.ModsPath == null || !Directory.Exists(_context.ModsPath)) return;

            var modDirs = Directory.GetDirectories(_context.ModsPath);

            Parallel.ForEach(modDirs, modDir =>
            {
                var aboutFile = Path.Combine(modDir, "About", "About.xml");
                if (File.Exists(aboutFile))
                {
                    try
                    {
                        var doc = XDocument.Load(aboutFile);
                        var modInfo = ParseModInfo(doc, modDir);
                        if (modInfo != null)
                        {
                            _modIndex.Add(modInfo);
                            if (!string.IsNullOrEmpty(modInfo.PackageId))
                            {
                                _modById[modInfo.PackageId] = modInfo;
                            }
                        }
                    }
                    catch { }
                }
            });
        }

        private ModInfo ParseModInfo(XDocument doc, string modDir)
        {
            var root = doc.Root;
            if (root?.Name.LocalName != "ModMetaData") return null;

            var info = new ModInfo
            {
                DirectoryPath = modDir,
                Name = root.Element("name")?.Value,
                Author = root.Element("author")?.Value,
                PackageId = root.Element("packageId")?.Value,
                Description = root.Element("description")?.Value,
                Version = root.Element("modVersion")?.Value
            };

            ScanModResources(info, modDir);

            return info;
        }

        private void ScanModResources(ModInfo info, string modDir)
        {
            var assembliesDir = Path.Combine(modDir, "Assemblies");
            if (Directory.Exists(assembliesDir))
            {
                info.DllFiles = Directory.GetFiles(assembliesDir, "*.dll", SearchOption.AllDirectories)
                    .Select(Path.GetFileName).ToList();
            }

            var defsDir = Path.Combine(modDir, "Defs");
            if (Directory.Exists(defsDir))
            {
                info.DefFiles = Directory.GetFiles(defsDir, "*.xml", SearchOption.AllDirectories)
                    .Select(Path.GetFileName).ToList();
                info.DefCount = info.DefFiles.Count;
            }

            var patchesDir = Path.Combine(modDir, "Patches");
            if (Directory.Exists(patchesDir))
            {
                info.PatchFiles = Directory.GetFiles(patchesDir, "*.xml", SearchOption.AllDirectories)
                    .Select(Path.GetFileName).ToList();
                info.PatchCount = info.PatchFiles.Count;
            }
        }

        public IEnumerable<ProbeResult> Search(string query, SearchOptions options)
        {
            EnsureIndexBuilt();

            var comparison = options.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            var results = new List<ModProbeResult>();

            foreach (var mod in _modIndex)
            {
                bool match = options.ExactMatch
                    ? mod.Name?.Equals(query, comparison) == true ||
                      mod.PackageId?.Equals(query, comparison) == true
                    : mod.Name?.Contains(query, comparison) == true ||
                      mod.Author?.Contains(query, comparison) == true ||
                      mod.PackageId?.Contains(query, comparison) == true;

                if (match)
                {
                    results.Add(new ModProbeResult(mod));
                    if (results.Count >= options.MaxResults) break;
                }
            }

            return results;
        }

        public ProbeResult GetDetails(string id)
        {
            EnsureIndexBuilt();

            if (_modById.TryGetValue(id, out var mod))
            {
                return new ModProbeResult(mod, true);
            }

            mod = _modIndex.FirstOrDefault(m => m.Name == id);
            if (mod != null)
            {
                return new ModProbeResult(mod, true);
            }

            return null;
        }

        public void ClearCache()
        {
            _modIndex = null;
            _modById.Clear();
        }

        public IEnumerable<ModInfo> GetAllMods()
        {
            EnsureIndexBuilt();
            return _modIndex.OrderBy(m => m.Name);
        }
    }

    public class ModInfo
    {
        public string Name { get; set; }
        public string Author { get; set; }
        public string PackageId { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string DirectoryPath { get; set; }
        public List<string> DllFiles { get; set; } = new List<string>();
        public List<string> DefFiles { get; set; } = new List<string>();
        public List<string> PatchFiles { get; set; } = new List<string>();
        public int DefCount { get; set; }
        public int PatchCount { get; set; }
    }
}
