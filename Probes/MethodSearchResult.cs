using System;
using System.Linq;
using System.Reflection;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Probes
{
    public class MethodSearchResult : ProbeResult
    {
        public MethodInfo MethodInfo { get; }

        public MethodSearchResult(MethodInfo method)
        {
            MethodInfo = method;
            Id = $"{method.DeclaringType.FullName}.{method.Name}";
            Name = method.Name;
            Type = "Method";
            Source = method.DeclaringType.Assembly.GetName().Name;
            Location = method.DeclaringType.FullName;
        }

        public override void PrintDetails()
        {
            var visibility = MethodInfo.IsPublic ? "public" : MethodInfo.IsPrivate ? "private" : MethodInfo.IsFamily ? "protected" : "internal";
            var staticStr = MethodInfo.IsStatic ? "static " : "";
            var params_ = string.Join(", ", MethodInfo.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
            Console.WriteLine($"\n--- {MethodInfo.Name} ---");
            Console.WriteLine($"Declaring Type: {MethodInfo.DeclaringType.FullName}");
            Console.WriteLine($"Signature: [{visibility}] {staticStr}{MethodInfo.ReturnType.Name} {MethodInfo.Name}({params_})");
            Console.WriteLine($"Assembly: {MethodInfo.DeclaringType.Assembly.GetName().Name}");

            var parameters = MethodInfo.GetParameters();
            if (parameters.Length > 0)
            {
                Console.WriteLine("\nParameters:");
                foreach (var param in parameters)
                {
                    var optional = param.IsOptional ? " = " + (param.DefaultValue?.ToString() ?? "null") : "";
                    Console.WriteLine($"  {param.ParameterType.Name} {param.Name}{optional}");
                }
            }

            if (MethodInfo.IsVirtual)
            {
                Console.WriteLine($"\nVirtual Method: Yes");
                var baseMethod = MethodInfo.GetBaseDefinition();
                if (baseMethod != null && baseMethod != MethodInfo)
                {
                    Console.WriteLine($"Base Definition: {baseMethod.DeclaringType.FullName}.{baseMethod.Name}");
                }
            }

            var attributes = MethodInfo.GetCustomAttributes(false);
            if (attributes.Length > 0)
            {
                Console.WriteLine($"\nAttributes: {string.Join(", ", attributes.Select(a => a.GetType().Name))}");
            }
        }
    }
}
