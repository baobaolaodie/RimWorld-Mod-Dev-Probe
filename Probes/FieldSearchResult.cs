using System;
using System.Linq;
using System.Reflection;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Probes
{
    public class FieldSearchResult : ProbeResult
    {
        public FieldInfo FieldInfo { get; }

        public FieldSearchResult(FieldInfo field)
        {
            FieldInfo = field;
            Id = $"{field.DeclaringType.FullName}.{field.Name}";
            Name = field.Name;
            Type = "Field";
            Source = field.DeclaringType.Assembly.GetName().Name;
            Location = field.DeclaringType.FullName;
        }

        public override void PrintDetails()
        {
            var visibility = FieldInfo.IsPublic ? "public" : FieldInfo.IsPrivate ? "private" : FieldInfo.IsFamily ? "protected" : "internal";
            var staticStr = FieldInfo.IsStatic ? "static " : "";
            Console.WriteLine($"\n--- {FieldInfo.Name} ---");
            Console.WriteLine($"Declaring Type: {FieldInfo.DeclaringType.FullName}");
            Console.WriteLine($"Signature: [{visibility}] {staticStr}{FieldInfo.FieldType.Name} {FieldInfo.Name}");
            Console.WriteLine($"Assembly: {FieldInfo.DeclaringType.Assembly.GetName().Name}");

            if (FieldInfo.FieldType.IsGenericType)
            {
                var genericArgs = string.Join(", ", FieldInfo.FieldType.GetGenericArguments().Select(t => t.Name));
                Console.WriteLine($"Generic Arguments: {genericArgs}");
            }

            if (FieldInfo.IsLiteral && !FieldInfo.IsInitOnly)
            {
                var value = FieldInfo.GetValue(null);
                Console.WriteLine($"Constant Value: {value}");
            }

            var attributes = FieldInfo.GetCustomAttributes(false);
            if (attributes.Length > 0)
            {
                Console.WriteLine($"Attributes: {string.Join(", ", attributes.Select(a => a.GetType().Name))}");
            }
        }
    }
}
