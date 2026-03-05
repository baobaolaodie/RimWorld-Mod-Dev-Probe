using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorldModDevProbe.Core;
using RimWorldModDevProbe.Utils;

namespace RimWorldModDevProbe.Probes
{
    public class DllProbe : IProbe
    {
        public string Name => "dll";
        private ProbeContext _context;

        public void Initialize(ProbeContext context)
        {
            _context = context;
            _context.LoadGameAssemblies();
        }

        public IEnumerable<ProbeResult> Search(string query, SearchOptions options)
        {
            var results = new List<DllProbeResult>();
            var comparison = options.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            foreach (var asm in _context.LoadedAssemblies)
            {
                foreach (var type in IlHelper.GetTypesSafe(asm))
                {
                    bool match = options.ExactMatch
                        ? type.Name.Equals(query, comparison)
                        : type.Name.Contains(query, comparison);

                    if (match)
                    {
                        results.Add(new DllProbeResult(type));
                        if (results.Count >= options.MaxResults) break;
                    }
                }
                if (results.Count >= options.MaxResults) break;
            }

            return results;
        }

        public IEnumerable<MethodSearchResult> SearchMethods(string query, SearchOptions options)
        {
            var results = new List<MethodSearchResult>();
            var comparison = options.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            foreach (var asm in _context.LoadedAssemblies)
            {
                foreach (var type in IlHelper.GetTypesSafe(asm))
                {
                    var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
                    foreach (var method in type.GetMethods(flags))
                    {
                        if (method.IsSpecialName) continue;

                        bool match = options.ExactMatch
                            ? method.Name.Equals(query, comparison)
                            : method.Name.Contains(query, comparison);

                        if (match)
                        {
                            results.Add(new MethodSearchResult(method));
                            if (results.Count >= options.MaxResults) return results;
                        }
                    }
                }
            }
            return results;
        }

        public IEnumerable<FieldSearchResult> SearchFields(string query, SearchOptions options)
        {
            var results = new List<FieldSearchResult>();
            var comparison = options.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

            foreach (var asm in _context.LoadedAssemblies)
            {
                foreach (var type in IlHelper.GetTypesSafe(asm))
                {
                    var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
                    foreach (var field in type.GetFields(flags))
                    {
                        bool match = options.ExactMatch
                            ? field.Name.Equals(query, comparison)
                            : field.Name.Contains(query, comparison);

                        if (match)
                        {
                            results.Add(new FieldSearchResult(field));
                            if (results.Count >= options.MaxResults) return results;
                        }
                    }
                }
            }
            return results;
        }

        public List<string> GetInheritanceChain(Type type)
        {
            var chain = new List<string>();
            var current = type;
            while (current != null)
            {
                chain.Add(current.Name);
                current = current.BaseType;
            }
            return chain;
        }

        public ProbeResult GetDetails(string id)
        {
            foreach (var asm in _context.LoadedAssemblies)
            {
                foreach (var type in IlHelper.GetTypesSafe(asm))
                {
                    if (type.FullName == id || type.Name == id)
                    {
                        return new DllProbeResult(type, true);
                    }
                }
            }
            return null;
        }

        public void ClearCache()
        {
        }
    }
}
