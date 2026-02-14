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
            LoadAssemblies();

            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    InspectType(arg);
                }
            }
            else
            {
                Console.WriteLine("Please provide type names as arguments.");
            }
        }

        static void InspectType(string typeName)
        {
            Console.WriteLine($"\n--- Searching for: {typeName} ---");
            var types = loadedAssemblies.SelectMany(a => GetTypesSafe(a)).Where(t => t.Name.Contains(typeName)).ToList();
            
            if (types.Count == 0)
            {
                Console.WriteLine("No types found.");
                return;
            }

            foreach (var type in types)
            {
                Console.WriteLine($"\nType: {type.FullName} ({type.Assembly.GetName().Name})");
                
                if (type.IsEnum)
                {
                    Console.WriteLine("Enum Values:");
                    foreach (var name in Enum.GetNames(type))
                    {
                        Console.WriteLine($"  {name}");
                    }
                }
                else
                {
                    Console.WriteLine("Fields:");
                    foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
                    {
                        Console.WriteLine($"  {field.FieldType.Name} {field.Name} ({(field.IsPublic ? "public" : "non-public")})");
                    }
                    
                    Console.WriteLine("Properties:");
                    foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance))
                    {
                        Console.WriteLine($"  {prop.PropertyType.Name} {prop.Name}");
                    }

                    Console.WriteLine("Methods:");
                    foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                    {
                        Console.WriteLine($"  {method.ReturnType.Name} {method.Name}({string.Join(", ", method.GetParameters().Select(p => p.ParameterType.Name + " " + p.Name))})");
                    }
                }
            }
        }

        static IEnumerable<Type> GetTypesSafe(Assembly asm)
        {
            try { return asm.GetTypes(); }
            catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null); }
            catch { return Enumerable.Empty<Type>(); }
        }

        static void LoadAssemblies()
        {
            try
            {
                string gameDllPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "GameDll"));
                if (!System.IO.Directory.Exists(gameDllPath))
                {
                     gameDllPath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "GameDll"));
                }
                
                if (System.IO.Directory.Exists(gameDllPath))
                {
                    foreach (var dll in System.IO.Directory.GetFiles(gameDllPath, "*.dll"))
                    {
                        try { loadedAssemblies.Add(Assembly.LoadFrom(dll)); } catch {}
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading assemblies: {ex.Message}");
            }
        }
    }
}
