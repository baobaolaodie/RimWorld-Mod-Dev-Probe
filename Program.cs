using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RimWorldApiProbe
{
    class Program
    {
        static List<Assembly> loadedAssemblies = new List<Assembly>();

        static void Main(string[] args)
        {
            Console.WriteLine("Running Automated Probe...");
            LoadAssemblies();

            Console.WriteLine("\n--- Inspecting ThingComp for Gizmo methods ---");
            InspectMethods("ThingComp");

            Console.WriteLine("\n--- Inspecting Apparel for Gizmo methods ---");
            InspectMethods("Apparel");

            Console.WriteLine("\n--- Done ---");
        }

        static void InspectMethods(string typeName)
        {
            var type = loadedAssemblies.SelectMany(a => GetTypesSafe(a)).FirstOrDefault(t => t.Name == typeName);
            if (type != null)
            {
                Console.WriteLine($"Type: {type.FullName}");
                foreach(var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (method.Name.Contains("Gizmo"))
                    {
                        Console.WriteLine($"Method: {method.Name}");
                        foreach(var p in method.GetParameters())
                        {
                            Console.WriteLine($"  Param: {p.ParameterType.Name} {p.Name}");
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"{typeName} not found.");
            }
        }

        static void InspectCompAbilityEffect()
        {
            var type = loadedAssemblies.SelectMany(a => GetTypesSafe(a)).FirstOrDefault(t => t.Name == "CompAbilityEffect");
            if (type != null)
            {
                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                {
                    Console.WriteLine($"Method: {method.Name}");
                    foreach(var p in method.GetParameters())
                    {
                         Console.WriteLine($"  Param: {p.ParameterType.Name} {p.Name}");
                    }
                }
            }
        }

        static void LoadAssemblies()
        {
            try
            {
                // Explicitly load all DLLs from the GameDll directory
                string gameDllPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "GameDll"));
                if (!System.IO.Directory.Exists(gameDllPath))
                {
                    // Try another common location if not found
                     gameDllPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "GameDll"));
                }
                
                Console.WriteLine($"Loading assemblies from: {gameDllPath}");

                if (System.IO.Directory.Exists(gameDllPath))
                {
                    foreach (var dll in System.IO.Directory.GetFiles(gameDllPath, "*.dll"))
                    {
                        try
                        {
                            var asm = Assembly.LoadFrom(dll);
                            loadedAssemblies.Add(asm);
                        }
                        catch {}
                    }
                }
                
                Console.WriteLine($"Ready. Loaded {loadedAssemblies.Count} assemblies.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading assemblies: {ex.Message}");
            }
        }

        static void InspectStatDefOf()
        {
            Type statDefOf = null;
            foreach (var asm in loadedAssemblies)
            {
                try
                {
                    var types = GetTypesSafe(asm);
                    statDefOf = types.FirstOrDefault(t => t.Name == "StatDefOf");
                    if (statDefOf != null) break;
                }
                catch {}
            }

            if (statDefOf != null)
            {
                Console.WriteLine($"Found StatDefOf in {statDefOf.Assembly.GetName().Name}");
                foreach (var field in statDefOf.GetFields(BindingFlags.Public | BindingFlags.Static))
                {
                     if (field.Name.Contains("Vacuum") || field.Name.Contains("Compress") || field.Name.Contains("Toxic"))
                    {
                        Console.WriteLine($"Field: {field.Name}");
                    }
                }
            }
            else
            {
                Console.WriteLine("StatDefOf not found.");
            }
        }

        static void SearchTypes(string query)
        {
            foreach (var asm in loadedAssemblies)
            {
                try
                {
                    var types = GetTypesSafe(asm).Where(t => t.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                    foreach (var t in types)
                    {
                        Console.WriteLine($"Found Type: {t.FullName} in {asm.GetName().Name}");
                    }
                }
                catch {}
            }
        }

        static IEnumerable<Type> GetTypesSafe(Assembly asm)
        {
            try
            {
                return asm.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }

        static void SearchAndInspect(string query)
        {
            var foundTypes = new List<Type>();

            foreach (var asm in loadedAssemblies)
            {
                try
                {
                    var types = asm.GetTypes().Where(t => t.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0).ToList();
                    foundTypes.AddRange(types);
                }
                catch { }
            }

            if (foundTypes.Count == 0)
            {
                Console.WriteLine("No types found matching that name.");
                return;
            }

            if (foundTypes.Count > 1)
            {
                Console.WriteLine($"Found {foundTypes.Count} types:");
                for (int i = 0; i < foundTypes.Count; i++)
                {
                    Console.WriteLine($"[{i}] {foundTypes[i].FullName} ({foundTypes[i].Assembly.GetName().Name})");
                }
                Console.Write("Select ID to inspect (or press Enter to cancel): ");
                string sel = Console.ReadLine();
                if (int.TryParse(sel, out int idx) && idx >= 0 && idx < foundTypes.Count)
                {
                    InspectType(foundTypes[idx]);
                }
            }
            else
            {
                InspectType(foundTypes[0]);
            }
        }

        static void InspectType(Type t)
        {
            Console.WriteLine($"\n--- Inspecting: {t.FullName} ---");
            Console.WriteLine($"Assembly: {t.Assembly.GetName().Name}");
            Console.WriteLine($"BaseType: {t.BaseType?.Name}");

            Console.WriteLine("\n[Fields]");
            foreach (var field in t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
            {
                // Simple filter to avoid too much noise if needed, but for inspection we usually want everything
                Console.WriteLine($"  {(field.IsPublic ? "+" : "-")} {field.FieldType.Name} {field.Name}");
            }

            Console.WriteLine("\n[Properties]");
            foreach (var prop in t.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
            {
                Console.WriteLine($"  {prop.PropertyType.Name} {prop.Name}");
            }

            Console.WriteLine("\n[Methods (Public Static/Instance)]");
            foreach (var method in t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly))
            {
                if (method.IsSpecialName) continue; // Skip getters/setters
                var paramStr = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                Console.WriteLine($"  {method.ReturnType.Name} {method.Name}({paramStr})");
            }
            Console.WriteLine("-----------------------------------");
        }
    }
}
