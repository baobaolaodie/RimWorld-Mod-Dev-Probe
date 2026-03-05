using System;
using System.Linq;
using System.Reflection;
using System.Text;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Utils
{
    /// <summary>
    /// 代码生成器 - 根据探测结果生成 Harmony Patch 和 XML Patch 代码模板
    /// </summary>
    public class CodeGenerator
    {
        private readonly CodeValidator _validator;

        public CodeGenerator()
        {
            _validator = null;
        }

        public CodeGenerator(ProbeContext context)
        {
            _validator = new CodeValidator(context);
        }

        public CodeGenerator(CodeValidator validator)
        {
            _validator = validator;
        }

        /// <summary>
        /// 生成 Harmony Patch 代码
        /// </summary>
        public string GenerateHarmonyPatch(MethodInfo method, string patchType = "Postfix")
        {
            var sb = new StringBuilder();
            var declaringType = method.DeclaringType;
            var methodName = method.Name;
            var patchClassName = $"{declaringType.Name}_{methodName}_{patchType}Patch";
            
            sb.AppendLine("using HarmonyLib;");
            sb.AppendLine($"using {declaringType.Namespace};");
            sb.AppendLine();
            sb.AppendLine($"namespace YourModNamespace");
            sb.AppendLine("{");
            sb.AppendLine($"    /// <summary>");
            sb.AppendLine($"    /// Harmony {patchType} patch for {declaringType.FullName}.{methodName}");
            sb.AppendLine($"    /// </summary>");
            sb.AppendLine($"    [HarmonyPatch(typeof({declaringType.Name}), \"{methodName}\")]");
            sb.AppendLine($"    public static class {patchClassName}");
            sb.AppendLine("    {");
            
            // 生成 Patch 方法
            var parameters = method.GetParameters();
            var paramList = string.Join(", ", parameters.Select(p => $"{GetParameterTypeName(p.ParameterType)} {p.Name}"));
            
            if (patchType == "Prefix")
            {
                sb.AppendLine($"        public static bool Prefix({paramList})");
                sb.AppendLine("        {");
                sb.AppendLine("            // 返回 false 阻止原方法执行");
                sb.AppendLine("            // 返回 true 继续执行原方法");
                sb.AppendLine("            return true;");
                sb.AppendLine("        }");
            }
            else if (patchType == "Postfix")
            {
                var returnParam = method.ReturnType != typeof(void) ? $", {GetParameterTypeName(method.ReturnType)} __result" : "";
                sb.AppendLine($"        public static void Postfix({paramList}{returnParam})");
                sb.AppendLine("        {");
                sb.AppendLine("            // 在原方法执行后执行");
                sb.AppendLine("        }");
            }
            else if (patchType == "Transpiler")
            {
                sb.AppendLine("        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)");
                sb.AppendLine("        {");
                sb.AppendLine("            // 使用 CodeMatcher 修改 IL 代码");
                sb.AppendLine("            var matcher = new CodeMatcher(instructions);");
                sb.AppendLine("            // matcher.MatchStartForward(...).Insert(...);");
                sb.AppendLine("            return matcher.InstructionEnumeration();");
                sb.AppendLine("        }");
            }
            
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }

        /// <summary>
        /// 生成 Harmony Patch 代码并验证
        /// </summary>
        public (string code, ValidationResult validation) GenerateAndValidateHarmonyPatch(MethodInfo method, string patchType = "Postfix")
        {
            var code = GenerateHarmonyPatch(method, patchType);
            var validation = _validator?.ValidateHarmonyPatch(
                method.DeclaringType?.Name,
                method.Name,
                patchType,
                method.GetParameters(),
                method);

            return (code, validation);
        }

        /// <summary>
        /// 验证已生成的代码
        /// </summary>
        public ValidationResult ValidateGeneratedCode(string generatedCode)
        {
            return _validator?.ValidateGeneratedCode(generatedCode) ?? new ValidationResult();
        }

        /// <summary>
        /// 生成 XML Patch 代码
        /// </summary>
        public string GenerateXmlPatch(string defName, string defType, string operation = "Replace")
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sb.AppendLine("<Patch>");
            sb.AppendLine("    <Operation Class=\"PatchOperation{operation}\">");
            sb.AppendLine($"        <xpath>/Defs/{defType}[defName=\"{defName}\"]</xpath>");
            sb.AppendLine("        <value>");
            sb.AppendLine("            <!-- 你的修改内容 -->");
            sb.AppendLine("        </value>");
            sb.AppendLine("    </Operation>");
            sb.AppendLine("</Patch>");
            
            return sb.ToString();
        }
        
        /// <summary>
        /// 生成音效修改 Patch 代码（特定功能）
        /// </summary>
        public string GenerateSoundPatch(string targetType, string soundField, string newSoundDef)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine("using HarmonyLib;");
            sb.AppendLine("using RimWorld;");
            sb.AppendLine("using Verse;");
            sb.AppendLine();
            sb.AppendLine("namespace YourModNamespace");
            sb.AppendLine("{");
            sb.AppendLine($"    /// <summary>");
            sb.AppendLine($"    /// 修改 {targetType} 的音效");
            sb.AppendLine($"    /// </summary>");
            sb.AppendLine($"    [HarmonyPatch(typeof({targetType}))]");
            sb.AppendLine($"    public static class {targetType}_SoundPatch");
            sb.AppendLine("    {");
            sb.AppendLine("        // 示例：修改死亡音效");
            sb.AppendLine("        // [HarmonyPatch(\"Die\"]");
            sb.AppendLine("        // public static void Prefix(Pawn __instance)");
            sb.AppendLine("        // {");
            sb.AppendLine("        //     // 播放自定义音效");
            sb.AppendLine($"        //     {newSoundDef}.PlayOneShot(SoundInfo.InMap(__instance));");
            sb.AppendLine("        // }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            
            return sb.ToString();
        }
        
        /// <summary>
        /// 生成完整的 Mod 结构
        /// </summary>
        public string GenerateModStructure(string modName)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine($"Mod 结构: {modName}");
            sb.AppendLine();
            sb.AppendLine($"{modName}/");
            sb.AppendLine("├── About/");
            sb.AppendLine("│   ├── About.xml");
            sb.AppendLine("│   └── Preview.png");
            sb.AppendLine("├── Assemblies/");
            sb.AppendLine("│   └── (编译后的 DLL)");
            sb.AppendLine("├── Defs/");
            sb.AppendLine("│   └── (自定义 Defs)");
            sb.AppendLine("├── Patches/");
            sb.AppendLine("│   └── (XML Patches)");
            sb.AppendLine("└── Source/");
            sb.AppendLine("    └── (C# 源代码)");
            sb.AppendLine();
            sb.AppendLine("About.xml 示例:");
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<ModMetaData>");
            sb.AppendLine($"    <name>{modName}</name>");
            sb.AppendLine("    <author>YourName</author>");
            sb.AppendLine("    <packageId>YourName.YourMod</packageId>");
            sb.AppendLine("    <description>Mod 描述</description>");
            sb.AppendLine("    <supportedVersions>");
            sb.AppendLine("        <li>1.5</li>");
            sb.AppendLine("    </supportedVersions>");
            sb.AppendLine("</ModMetaData>");
            
            return sb.ToString();
        }
        
        private string GetParameterTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                var genericName = type.Name.Split('`')[0];
                var args = string.Join(", ", type.GetGenericArguments().Select(GetParameterTypeName));
                return $"{genericName}<{args}>";
            }
            return type.Name;
        }
    }
}
