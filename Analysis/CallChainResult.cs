using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Analysis
{
    public class CallChainResult : ProbeResult
    {
        public MethodInfo MethodInfo { get; }
        public Type DeclaringType { get; }
        public string RelationType { get; }

        public CallChainResult(MethodInfo method, Type declaringType, string relationType)
        {
            MethodInfo = method;
            DeclaringType = declaringType;
            RelationType = relationType;

            Id = $"{method.DeclaringType.FullName}.{method.Name}";
            Name = method.Name;
            Type = "Method";
            Source = method.DeclaringType.Assembly.GetName().Name;
            Location = method.DeclaringType.FullName;

            Metadata["RelationType"] = relationType;
            Metadata["DeclaringType"] = declaringType.FullName;
            Metadata["IsStatic"] = method.IsStatic;
            Metadata["IsVirtual"] = method.IsVirtual;
            Metadata["IsGenericMethod"] = method.IsGenericMethod;
            Metadata["IsPublic"] = method.IsPublic;
        }

        public override void PrintDetails()
        {
            var visibility = MethodInfo.IsPublic ? "public" : MethodInfo.IsPrivate ? "private" : MethodInfo.IsFamily ? "protected" : "internal";
            var staticStr = MethodInfo.IsStatic ? "static " : "";
            var params_ = string.Join(", ", MethodInfo.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));

            Console.WriteLine($"\n--- {MethodInfo.Name} ({RelationType}) ---");
            Console.WriteLine($"Declaring Type: {MethodInfo.DeclaringType.FullName}");
            Console.WriteLine($"Signature: [{visibility}] {staticStr}{MethodInfo.ReturnType.Name} {MethodInfo.Name}({params_})");
            Console.WriteLine($"Assembly: {MethodInfo.DeclaringType.Assembly.GetName().Name}");
            Console.WriteLine($"Relation: {RelationType}");

            if (MethodInfo.IsGenericMethod)
            {
                var genericArgs = MethodInfo.GetGenericArguments();
                Console.WriteLine($"Generic Arguments: {string.Join(", ", genericArgs.Select(a => a.Name))}");
            }

            if (MethodInfo.IsVirtual)
            {
                Console.WriteLine($"Virtual Method: Yes");
                var baseMethod = MethodInfo.GetBaseDefinition();
                if (baseMethod != null && baseMethod != MethodInfo)
                {
                    Console.WriteLine($"Base Definition: {baseMethod.DeclaringType.FullName}.{baseMethod.Name}");
                }
            }

            var attributes = MethodInfo.GetCustomAttributes(false);
            if (attributes.Length > 0)
            {
                Console.WriteLine($"Attributes: {string.Join(", ", attributes.Select(a => a.GetType().Name))}");
            }
        }

        public string GetSignature()
        {
            var visibility = MethodInfo.IsPublic ? "public" : MethodInfo.IsPrivate ? "private" : MethodInfo.IsFamily ? "protected" : "internal";
            var staticStr = MethodInfo.IsStatic ? "static " : "";
            var params_ = string.Join(", ", MethodInfo.GetParameters().Select(p => p.ParameterType.Name));
            return $"[{visibility}] {staticStr}{MethodInfo.ReturnType.Name} {MethodInfo.Name}({params_})";
        }
    }
}
