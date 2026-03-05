using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Utils
{
    public class ValidationResult
    {
        public bool IsValid { get; set; } = true;
        public List<string> Errors { get; set; } = new List<string>();
        public List<string> Warnings { get; set; } = new List<string>();
        public List<string> Suggestions { get; set; } = new List<string>();

        public void AddError(string error)
        {
            Errors.Add(error);
            IsValid = false;
        }

        public void AddWarning(string warning)
        {
            Warnings.Add(warning);
        }

        public void AddSuggestion(string suggestion)
        {
            Suggestions.Add(suggestion);
        }

        public void Merge(ValidationResult other)
        {
            if (!other.IsValid)
            {
                IsValid = false;
            }
            Errors.AddRange(other.Errors);
            Warnings.AddRange(other.Warnings);
            Suggestions.AddRange(other.Suggestions);
        }

        public void PrintResults()
        {
            if (IsValid && Warnings.Count == 0 && Suggestions.Count == 0)
            {
                Console.WriteLine("✓ 代码验证通过，未发现问题");
                return;
            }

            if (Errors.Count > 0)
            {
                Console.WriteLine("\n❌ 错误:");
                foreach (var error in Errors)
                {
                    Console.WriteLine($"  • {error}");
                }
            }

            if (Warnings.Count > 0)
            {
                Console.WriteLine("\n⚠ 警告:");
                foreach (var warning in Warnings)
                {
                    Console.WriteLine($"  • {warning}");
                }
            }

            if (Suggestions.Count > 0)
            {
                Console.WriteLine("\n💡 建议:");
                foreach (var suggestion in Suggestions)
                {
                    Console.WriteLine($"  • {suggestion}");
                }
            }
        }
    }

    public class CodeValidator
    {
        private readonly ProbeContext _context;

        public CodeValidator(ProbeContext context)
        {
            _context = context;
        }

        public ValidationResult ValidateHarmonyPatch(
            string targetTypeName,
            string targetMethodName,
            string patchType,
            IEnumerable<ParameterInfo> patchParameters,
            MethodInfo targetMethod = null)
        {
            var result = new ValidationResult();

            if (targetMethod == null)
            {
                targetMethod = FindTargetMethod(targetTypeName, targetMethodName, result);
            }

            if (targetMethod != null)
            {
                ValidateMethodExistence(targetMethod, targetTypeName, targetMethodName, result);
                ValidateParameterTypes(targetMethod, patchType, patchParameters, result);
                ValidateSpecialParameters(targetMethod, patchType, patchParameters, result);
            }
            else
            {
                result.AddError($"无法找到目标方法: {targetTypeName}.{targetMethodName}");
            }

            return result;
        }

        public ValidationResult ValidateGeneratedCode(string generatedCode)
        {
            var result = new ValidationResult();

            var typeName = ExtractAttributeValue(generatedCode, "HarmonyPatch", "typeof");
            var methodName = ExtractAttributeValue(generatedCode, "HarmonyPatch", "\"");
            var patchType = ExtractPatchType(generatedCode);

            if (string.IsNullOrEmpty(typeName))
            {
                result.AddError("无法从生成的代码中提取目标类型名称");
                return result;
            }

            if (string.IsNullOrEmpty(methodName))
            {
                result.AddError("无法从生成的代码中提取目标方法名称");
                return result;
            }

            ValidateNamespaceImports(generatedCode, typeName, result);

            var targetMethod = FindTargetMethod(typeName, methodName, result);
            if (targetMethod != null)
            {
                var patchParameters = ExtractPatchParameters(generatedCode, patchType);
                ValidateMethodExistence(targetMethod, typeName, methodName, result);
                ValidateParameterTypes(targetMethod, patchType, patchParameters, result);
                ValidateSpecialParameters(targetMethod, patchType, patchParameters, result);
            }

            return result;
        }

        private MethodInfo FindTargetMethod(string typeName, string methodName, ValidationResult result)
        {
            foreach (var asm in _context.LoadedAssemblies)
            {
                try
                {
                    foreach (var type in asm.GetTypes())
                    {
                        if (type.Name == typeName || type.FullName == typeName)
                        {
                            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
                            var matchingMethods = methods.Where(m => m.Name == methodName).ToList();

                            if (matchingMethods.Count == 0)
                            {
                                result.AddError($"类型 {typeName} 中不存在方法 {methodName}");
                                return null;
                            }

                            if (matchingMethods.Count == 1)
                            {
                                return matchingMethods[0];
                            }

                            return matchingMethods[0];
                        }
                    }
                }
                catch { }
            }

            result.AddWarning($"无法在已加载的程序集中找到类型: {typeName}");
            return null;
        }

        private void ValidateMethodExistence(MethodInfo targetMethod, string typeName, string methodName, ValidationResult result)
        {
            if (targetMethod == null)
            {
                result.AddError($"目标方法不存在: {typeName}.{methodName}");
                return;
            }

            var declaringType = targetMethod.DeclaringType;
            if (declaringType.Name != typeName && declaringType.FullName != typeName)
            {
                result.AddWarning($"类型名称不匹配: 期望 {typeName}，实际 {declaringType.Name}");
            }

            if (targetMethod.Name != methodName)
            {
                result.AddError($"方法名称不匹配: 期望 {methodName}，实际 {targetMethod.Name}");
            }
        }

        private void ValidateParameterTypes(
            MethodInfo targetMethod,
            string patchType,
            IEnumerable<ParameterInfo> patchParameters,
            ValidationResult result)
        {
            var targetParams = targetMethod.GetParameters();
            var patchParamsList = patchParameters?.ToList() ?? new List<ParameterInfo>();

            if (patchType == "Transpiler")
            {
                ValidateTranspilerParameters(patchParamsList, result);
                return;
            }

            var expectedParams = new List<string>();
            var actualParams = patchParamsList.Select(p => p.Name).ToList();

            if (!targetMethod.IsStatic)
            {
                expectedParams.Add("__instance");
            }

            if (targetMethod.ReturnType != typeof(void) && patchType == "Postfix")
            {
                expectedParams.Add("__result");
            }

            foreach (var param in targetParams)
            {
                expectedParams.Add(param.Name);
            }

            foreach (var actualParam in actualParams)
            {
                var paramName = actualParam.TrimStart('_');
                if (actualParam == "__instance")
                {
                    if (targetMethod.IsStatic)
                    {
                        result.AddError("静态方法不能使用 __instance 参数");
                    }
                    continue;
                }

                if (actualParam == "__result")
                {
                    if (targetMethod.ReturnType == typeof(void))
                    {
                        result.AddError("void 方法不能使用 __result 参数");
                    }
                    else if (patchType != "Postfix")
                    {
                        result.AddWarning($"__result 参数通常只在 Postfix 中使用，当前是 {patchType}");
                    }
                    continue;
                }

                if (actualParam == "__state")
                {
                    continue;
                }

                if (actualParam.StartsWith("__"))
                {
                    continue;
                }

                var matchingTargetParam = targetParams.FirstOrDefault(p => p.Name == actualParam || p.Name == paramName);
                if (matchingTargetParam == null)
                {
                    result.AddWarning($"参数 {actualParam} 在目标方法中不存在");
                }
            }
        }

        private void ValidateTranspilerParameters(List<ParameterInfo> patchParams, ValidationResult result)
        {
            if (patchParams.Count == 0)
            {
                result.AddError("Transpiler 方法必须有参数");
                return;
            }

            var firstParam = patchParams[0];
            var firstParamType = firstParam.ParameterType;

            if (firstParamType.Name != "IEnumerable`1" && 
                !firstParamType.Name.Contains("IEnumerable"))
            {
                result.AddError($"Transpiler 的第一个参数必须是 IEnumerable<CodeInstruction>，实际是 {firstParamType.Name}");
            }

            if (patchParams.Count > 1)
            {
                result.AddWarning("Transpiler 通常只需要一个参数 (IEnumerable<CodeInstruction>)");
            }
        }

        private void ValidateSpecialParameters(
            MethodInfo targetMethod,
            string patchType,
            IEnumerable<ParameterInfo> patchParameters,
            ValidationResult result)
        {
            var targetParams = targetMethod.GetParameters();
            var patchParamsList = patchParameters?.ToList() ?? new List<ParameterInfo>();

            foreach (var targetParam in targetParams)
            {
                if (targetParam.ParameterType.IsByRef)
                {
                    var paramName = targetParam.Name;
                    var matchingPatchParam = patchParamsList.FirstOrDefault(p =>
                        p.Name == paramName ||
                        p.Name == $"__{paramName}" ||
                        p.Name == $"ref{paramName}");

                    if (matchingPatchParam == null)
                    {
                        if (targetParam.IsOut)
                        {
                            result.AddSuggestion($"参数 {paramName} 是 out 参数，请确保正确处理");
                        }
                        else
                        {
                            result.AddSuggestion($"参数 {paramName} 是 ref 参数，在 Patch 中使用时需要添加 ref 关键字或使用 __{paramName} 形式");
                        }
                    }
                    else
                    {
                        if (!matchingPatchParam.ParameterType.IsByRef && matchingPatchParam.Name == paramName)
                        {
                            result.AddWarning($"参数 {paramName} 在目标方法中是 ref/out，但在 Patch 中可能需要特殊处理");
                        }
                    }
                }
            }

            if (!targetMethod.IsStatic)
            {
                var hasInstanceParam = patchParamsList.Any(p => p.Name == "__instance");
                if (!hasInstanceParam)
                {
                    result.AddSuggestion("目标方法是实例方法，可以考虑添加 __instance 参数来访问实例");
                }
            }

            if (targetMethod.ReturnType != typeof(void) && patchType == "Prefix")
            {
                var hasResultParam = patchParamsList.Any(p => p.Name == "__result");
                if (hasResultParam)
                {
                    result.AddWarning("Prefix 中使用 __result 参数时需要使用 ref 关键字");
                }
            }
        }

        private void ValidateNamespaceImports(string code, string typeName, ValidationResult result)
        {
            var usingStatements = ExtractUsingStatements(code);
            var targetNamespace = FindTypeNamespace(typeName);

            if (!string.IsNullOrEmpty(targetNamespace))
            {
                var hasNamespace = usingStatements.Any(u =>
                    u == targetNamespace ||
                    u.EndsWith(".*") && targetNamespace.StartsWith(u.TrimEnd('*', '.')));

                if (!hasNamespace)
                {
                    result.AddWarning($"缺少 using {targetNamespace}; 语句");
                }
            }

            var requiredUsings = new List<string> { "HarmonyLib" };
            foreach (var required in requiredUsings)
            {
                if (!usingStatements.Contains(required))
                {
                    result.AddWarning($"建议添加 using {required};");
                }
            }
        }

        private string FindTypeNamespace(string typeName)
        {
            foreach (var asm in _context.LoadedAssemblies)
            {
                try
                {
                    foreach (var type in asm.GetTypes())
                    {
                        if (type.Name == typeName || type.FullName == typeName)
                        {
                            return type.Namespace;
                        }
                    }
                }
                catch { }
            }
            return null;
        }

        private List<string> ExtractUsingStatements(string code)
        {
            var usings = new List<string>();
            var regex = new Regex(@"^\s*using\s+([^;]+);", RegexOptions.Multiline);

            var matches = regex.Matches(code);
            foreach (Match match in matches)
            {
                usings.Add(match.Groups[1].Value.Trim());
            }

            return usings;
        }

        private string ExtractAttributeValue(string code, string attributeName, string valuePattern)
        {
            var pattern = $@"\[{attributeName}\s*\(\s*{valuePattern}([^)\""]+)";
            var regex = new Regex(pattern);
            var match = regex.Match(code);

            if (match.Success)
            {
                return match.Groups[1].Value.Trim();
            }

            return null;
        }

        private string ExtractPatchType(string code)
        {
            if (code.Contains("public static bool Prefix") || code.Contains("static bool Prefix"))
            {
                return "Prefix";
            }
            if (code.Contains("public static void Postfix") || code.Contains("static void Postfix"))
            {
                return "Postfix";
            }
            if (code.Contains("IEnumerable<CodeInstruction> Transpiler"))
            {
                return "Transpiler";
            }
            return "Unknown";
        }

        private List<ParameterInfo> ExtractPatchParameters(string code, string patchType)
        {
            var parameters = new List<ParameterInfo>();

            var methodPattern = patchType == "Prefix"
                ? @"public\s+static\s+bool\s+Prefix\s*\(([^)]*)\)"
                : patchType == "Postfix"
                    ? @"public\s+static\s+void\s+Postfix\s*\(([^)]*)\)"
                    : @"public\s+static\s+IEnumerable<CodeInstruction>\s+Transpiler\s*\(([^)]*)\)";

            var regex = new Regex(methodPattern);
            var match = regex.Match(code);

            if (match.Success)
            {
                var paramStr = match.Groups[1].Value;
                var paramParts = paramStr.Split(',');

                foreach (var part in paramParts)
                {
                    var trimmed = part.Trim();
                    if (string.IsNullOrEmpty(trimmed)) continue;

                    var tokens = trimmed.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length >= 2)
                    {
                        var paramType = tokens[tokens.Length - 2];
                        var paramName = tokens[tokens.Length - 1];

                        parameters.Add(new MockParameterInfo(paramName, paramType));
                    }
                }
            }

            return parameters;
        }
    }

    internal class MockParameterInfo : ParameterInfo
    {
        private readonly string _name;
        private readonly Type _parameterType;

        public MockParameterInfo(string name, string typeName)
        {
            _name = name;
            _parameterType = new MockType(typeName);
        }

        public override string Name => _name;
        public override Type ParameterType => _parameterType;
    }

    internal class MockType : Type
    {
        private readonly string _name;

        public MockType(string name)
        {
            _name = name;
        }

        public override string Name => _name;
        public override string FullName => _name;

        public override Assembly Assembly => null;
        public override string AssemblyQualifiedName => _name;
        public override Type BaseType => null;
        public override string Namespace => null;
        public override Guid GUID => Guid.Empty;
        public override Module Module => null;
        public override Type UnderlyingSystemType => this;
        public new bool IsByRef => _name.StartsWith("ref ") || _name.EndsWith("&");
        public override bool IsEnum => false;
        public override bool IsGenericType => _name.Contains("<");
        public new bool IsValueType => false;

        public override Type GetElementType() => null;
        public override Type[] GetGenericArguments() => Array.Empty<Type>();
        public override object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, System.Globalization.CultureInfo culture, string[] namedParameters) => null;
        public override Type GetInterface(string name, bool ignoreCase) => null;
        public override Type[] GetInterfaces() => Array.Empty<Type>();
        public override EventInfo GetEvent(string name, BindingFlags bindingAttr) => null;
        public override FieldInfo GetField(string name, BindingFlags bindingAttr) => null;
        public override FieldInfo[] GetFields(BindingFlags bindingAttr) => Array.Empty<FieldInfo>();
        public override MemberInfo[] GetMembers(BindingFlags bindingAttr) => Array.Empty<MemberInfo>();
        public override MethodInfo[] GetMethods(BindingFlags bindingAttr) => Array.Empty<MethodInfo>();
        public override PropertyInfo[] GetProperties(BindingFlags bindingAttr) => Array.Empty<PropertyInfo>();
        public override ConstructorInfo[] GetConstructors(BindingFlags bindingAttr) => Array.Empty<ConstructorInfo>();
        public override Type GetNestedType(string name, BindingFlags bindingAttr) => null;
        public override Type[] GetNestedTypes(BindingFlags bindingAttr) => Array.Empty<Type>();
        public override EventInfo[] GetEvents(BindingFlags bindingAttr) => Array.Empty<EventInfo>();
        protected override TypeAttributes GetAttributeFlagsImpl() => TypeAttributes.Public;
        protected override ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) => null;
        protected override MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers) => null;
        protected override PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers) => null;
        protected override bool HasElementTypeImpl() => false;
        protected override bool IsArrayImpl() => false;
        protected override bool IsByRefImpl() => IsByRef;
        protected override bool IsCOMObjectImpl() => false;
        protected override bool IsPrimitiveImpl() => false;
        protected override bool IsValueTypeImpl() => false;
        protected override bool IsPointerImpl() => false;
        public override object[] GetCustomAttributes(bool inherit) => Array.Empty<object>();
        public override object[] GetCustomAttributes(Type attributeType, bool inherit) => Array.Empty<object>();
        public override bool IsDefined(Type attributeType, bool inherit) => false;
    }
}
