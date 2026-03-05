using System;
using System.Linq;
using System.Reflection;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Probes
{
    public class DllProbeResult : ProbeResult
    {
        public Type TypeInfo { get; }
        public bool Detailed { get; }

        public DllProbeResult(Type type, bool detailed = false)
        {
            TypeInfo = type;
            Detailed = detailed;
            Id = type.FullName;
            Name = type.Name;
            Type = type.IsEnum ? "Enum" : type.IsInterface ? "Interface" : type.IsValueType ? "Struct" : "Class";
            Source = type.Assembly.GetName().Name;
            Location = type.Namespace ?? "";
        }

        public override void PrintDetails()
        {
            Console.WriteLine($"\n--- {TypeInfo.FullName} ({TypeInfo.Assembly.GetName().Name}) ---");

            if (TypeInfo.BaseType != null)
            {
                Console.WriteLine($"Base Type: {TypeInfo.BaseType.FullName}");
            }
            var interfaces = TypeInfo.GetInterfaces();
            if (interfaces.Length > 0)
            {
                Console.WriteLine($"Interfaces: {string.Join(", ", interfaces.Take(5).Select(i => i.Name))}");
                if (interfaces.Length > 5) Console.WriteLine($"  ... and {interfaces.Length - 5} more");
            }

            if (TypeInfo.IsEnum)
            {
                Console.WriteLine("Enum Values:");
                foreach (var name in Enum.GetNames(TypeInfo))
                {
                    Console.WriteLine($"  {name}");
                }
            }
            else
            {
                var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

                Console.WriteLine("Fields:");
                foreach (var field in TypeInfo.GetFields(flags))
                {
                    var visibility = field.IsPublic ? "public" : field.IsPrivate ? "private" : field.IsFamily ? "protected" : "internal";
                    Console.WriteLine($"  [{visibility}] {field.FieldType.Name} {field.Name}");
                }

                Console.WriteLine("\nProperties:");
                foreach (var prop in TypeInfo.GetProperties(flags))
                {
                    Console.WriteLine($"  {prop.PropertyType.Name} {prop.Name}");
                }

                Console.WriteLine("\nMethods:");
                foreach (var method in TypeInfo.GetMethods(flags | BindingFlags.DeclaredOnly))
                {
                    if (method.IsSpecialName) continue;
                    var visibility = method.IsPublic ? "public" : method.IsPrivate ? "private" : method.IsFamily ? "protected" : "internal";
                    var staticStr = method.IsStatic ? "static " : "";
                    var params_ = string.Join(", ", method.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
                    Console.WriteLine($"  [{visibility}] {staticStr}{method.ReturnType.Name} {method.Name}({params_})");
                }
            }
        }
    }
}
