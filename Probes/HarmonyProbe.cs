using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using RimWorldModDevProbe.Core;
using RimWorldModDevProbe.Utils;

namespace RimWorldModDevProbe.Probes
{
    public class HarmonyProbe : IProbe
    {
        public string Name => "harmony";
        private ProbeContext _context;
        private ConcurrentBag<HarmonyPatchInfo> _patchIndex;

        public void Initialize(ProbeContext context)
        {
            _context = context;
        }

        private void EnsureIndexBuilt()
        {
            if (_patchIndex != null) return;

            _patchIndex = new ConcurrentBag<HarmonyPatchInfo>();

            Parallel.ForEach(_context.LoadedAssemblies, asm =>
            {
                try
                {
                    foreach (var type in IlHelper.GetTypesSafe(asm))
                    {
                        if (IsHarmonyPatchClass(type))
                        {
                            var patches = ExtractPatchInfo(type);
                            foreach (var patch in patches)
                            {
                                _patchIndex.Add(patch);
                            }
                        }
                    }
                }
                catch { }
            });
        }

        private bool IsHarmonyPatchClass(Type type)
        {
            return type.GetCustomAttributes(false)
                .Any(a => a.GetType().FullName?.Contains("HarmonyPatch") == true);
        }

        private IEnumerable<HarmonyPatchInfo> ExtractPatchInfo(Type type)
        {
            var results = new List<HarmonyPatchInfo>();
            var baseInfo = new HarmonyPatchInfo
            {
                PatchClassName = type.FullName,
                AssemblyName = type.Assembly.GetName().Name
            };

            var harmonyAttr = type.GetCustomAttributes(false)
                .FirstOrDefault(a => a.GetType().FullName?.Contains("HarmonyPatch") == true);

            if (harmonyAttr != null)
            {
                var attrType = harmonyAttr.GetType();
                var infoField = attrType.GetField("info");
                if (infoField != null)
                {
                    var info = infoField.GetValue(harmonyAttr);
                    if (info != null)
                    {
                        var methodField = info.GetType().GetField("method");
                        var declaringTypeField = info.GetType().GetField("declaringType");

                        if (methodField != null)
                        {
                            baseInfo.TargetMethod = methodField.GetValue(info)?.ToString();
                        }
                        if (declaringTypeField != null)
                        {
                            baseInfo.TargetType = declaringTypeField.GetValue(info)?.ToString();
                        }
                    }
                }
            }

            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
            {
                var methodName = method.Name;
                PatchType patchType = PatchType.Unknown;

                if (methodName == "Prefix") patchType = PatchType.Prefix;
                else if (methodName == "Postfix") patchType = PatchType.Postfix;
                else if (methodName == "Transpiler") patchType = PatchType.Transpiler;
                else if (methodName == "Finalizer") patchType = PatchType.Finalizer;
                else continue;

                var patch = new HarmonyPatchInfo
                {
                    PatchClassName = baseInfo.PatchClassName,
                    AssemblyName = baseInfo.AssemblyName,
                    TargetType = baseInfo.TargetType,
                    TargetMethod = baseInfo.TargetMethod,
                    PatchType = patchType,
                    PatchMethodName = methodName
                };

                var priorityAttr = method.GetCustomAttributes(false)
                    .FirstOrDefault(a => a.GetType().FullName?.Contains("Priority") == true);
                if (priorityAttr != null)
                {
                    var priorityField = priorityAttr.GetType().GetField("priority");
                    if (priorityField != null)
                    {
                        patch.Priority = priorityField.GetValue(priorityAttr)?.ToString();
                    }
                }

                results.Add(patch);
            }

            return results;
        }

        public IEnumerable<ProbeResult> Search(string query, SearchOptions options)
        {
            EnsureIndexBuilt();

            var comparison = options.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
            var results = new List<HarmonyProbeResult>();

            foreach (var patch in _patchIndex)
            {
                bool match = options.ExactMatch
                    ? (patch.TargetMethod?.Equals(query, comparison) ?? false) ||
                      (patch.TargetType?.Equals(query, comparison) ?? false) ||
                      patch.PatchClassName.Contains(query, comparison)
                    : (patch.TargetMethod?.Contains(query, comparison) ?? false) ||
                      (patch.TargetType?.Contains(query, comparison) ?? false) ||
                      patch.PatchClassName.Contains(query, comparison);

                if (match)
                {
                    results.Add(new HarmonyProbeResult(patch));
                    if (results.Count >= options.MaxResults) break;
                }
            }

            return results;
        }

        public ProbeResult GetDetails(string id)
        {
            EnsureIndexBuilt();

            var patch = _patchIndex.FirstOrDefault(p => p.PatchClassName == id);
            if (patch != null)
            {
                return new HarmonyProbeResult(patch, true);
            }
            return null;
        }

        public void ClearCache()
        {
            _patchIndex = null;
        }
    }

    public enum PatchType
    {
        Unknown,
        Prefix,
        Postfix,
        Transpiler,
        Finalizer
    }

    public class HarmonyPatchInfo
    {
        public string PatchClassName { get; set; }
        public string AssemblyName { get; set; }
        public string TargetType { get; set; }
        public string TargetMethod { get; set; }
        public PatchType PatchType { get; set; }
        public string PatchMethodName { get; set; }
        public string Priority { get; set; }
    }
}
