using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Analysis
{
    public enum RecommendedPatchType
    {
        Prefix,
        Postfix,
        Transpiler,
        Finalizer
    }

    public class PatchRecommendation
    {
        public MethodInfo TargetMethod { get; set; }
        public RecommendedPatchType RecommendedType { get; set; }
        public string Reason { get; set; }
        public List<string> ParameterHandlingSuggestions { get; set; } = new List<string>();
        public List<string> Notes { get; set; } = new List<string>();
        public int ConfidenceScore { get; set; }
        public string FeatureDescription { get; set; }

        public string GetPatchTypeName()
        {
            return RecommendedType.ToString();
        }

        public string GetMethodSignature()
        {
            if (TargetMethod == null) return "";
            var visibility = TargetMethod.IsPublic ? "public" : TargetMethod.IsPrivate ? "private" : TargetMethod.IsFamily ? "protected" : "internal";
            var staticStr = TargetMethod.IsStatic ? "static " : "";
            var params_ = string.Join(", ", TargetMethod.GetParameters().Select(p => $"{p.ParameterType.Name} {p.Name}"));
            return $"[{visibility}] {staticStr}{TargetMethod.ReturnType.Name} {TargetMethod.Name}({params_})";
        }

        public void PrintDetails()
        {
            Console.WriteLine($"\n{'='} Patch 推荐 {'='}");
            Console.WriteLine($"\n功能描述: {FeatureDescription}");
            Console.WriteLine($"\n目标方法:");
            Console.WriteLine($"  类型: {TargetMethod?.DeclaringType?.FullName}");
            Console.WriteLine($"  签名: {GetMethodSignature()}");
            Console.WriteLine($"\n推荐 Patch 类型: {GetPatchTypeName()}");
            Console.WriteLine($"置信度: {ConfidenceScore}/100");
            Console.WriteLine($"\n推荐理由:");
            Console.WriteLine($"  {Reason}");

            if (ParameterHandlingSuggestions.Count > 0)
            {
                Console.WriteLine($"\n参数处理建议:");
                foreach (var suggestion in ParameterHandlingSuggestions)
                {
                    Console.WriteLine($"  - {suggestion}");
                }
            }

            if (Notes.Count > 0)
            {
                Console.WriteLine($"\n注意事项:");
                foreach (var note in Notes)
                {
                    Console.WriteLine($"  ! {note}");
                }
            }
        }

        public string GeneratePatchCode()
        {
            if (TargetMethod == null) return "";

            var sb = new StringBuilder();
            var declaringType = TargetMethod.DeclaringType;
            var methodName = TargetMethod.Name;
            var patchClassName = $"{declaringType.Name}_{methodName}_{GetPatchTypeName()}Patch";

            sb.AppendLine("using HarmonyLib;");
            if (declaringType.Namespace != null)
            {
                sb.AppendLine($"using {declaringType.Namespace};");
            }
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Reflection.Emit;");
            sb.AppendLine();
            sb.AppendLine($"namespace YourModNamespace");
            sb.AppendLine("{");
            sb.AppendLine($"    /// <summary>");
            sb.AppendLine($"    /// {FeatureDescription}");
            sb.AppendLine($"    /// {Reason}");
            sb.AppendLine($"    /// </summary>");
            sb.AppendLine($"    [HarmonyPatch(typeof({declaringType.Name}), \"{methodName}\")]");
            sb.AppendLine($"    public static class {patchClassName}");
            sb.AppendLine("    {");

            var parameters = TargetMethod.GetParameters();
            var paramList = string.Join(", ", parameters.Select(p => $"{GetTypeName(p.ParameterType)} {p.Name}"));

            switch (RecommendedType)
            {
                case RecommendedPatchType.Prefix:
                    GeneratePrefixCode(sb, paramList);
                    break;
                case RecommendedPatchType.Postfix:
                    GeneratePostfixCode(sb, paramList);
                    break;
                case RecommendedPatchType.Transpiler:
                    GenerateTranspilerCode(sb);
                    break;
                case RecommendedPatchType.Finalizer:
                    GenerateFinalizerCode(sb, paramList);
                    break;
            }

            sb.AppendLine("    }");
            sb.AppendLine("}");

            return sb.ToString();
        }

        private void GeneratePrefixCode(StringBuilder sb, string paramList)
        {
            if (!TargetMethod.IsStatic)
            {
                paramList = $"{TargetMethod.DeclaringType.Name} __instance" + (string.IsNullOrEmpty(paramList) ? "" : ", " + paramList);
            }
            sb.AppendLine($"        public static bool Prefix({paramList})");
            sb.AppendLine("        {");
            sb.AppendLine("            // 返回 false 阻止原方法执行");
            sb.AppendLine("            // 返回 true 继续执行原方法");
            foreach (var suggestion in ParameterHandlingSuggestions)
            {
                sb.AppendLine($"            // {suggestion}");
            }
            sb.AppendLine("            return true;");
            sb.AppendLine("        }");
        }

        private void GeneratePostfixCode(StringBuilder sb, string paramList)
        {
            if (!TargetMethod.IsStatic)
            {
                paramList = $"{TargetMethod.DeclaringType.Name} __instance" + (string.IsNullOrEmpty(paramList) ? "" : ", " + paramList);
            }
            var returnParam = TargetMethod.ReturnType != typeof(void) ? $", {GetTypeName(TargetMethod.ReturnType)} __result" : "";
            sb.AppendLine($"        public static void Postfix({paramList}{returnParam})");
            sb.AppendLine("        {");
            sb.AppendLine("            // 在原方法执行后执行");
            if (TargetMethod.ReturnType != typeof(void))
            {
                sb.AppendLine("            // 可以通过 __result 参数访问和修改返回值");
            }
            foreach (var suggestion in ParameterHandlingSuggestions)
            {
                sb.AppendLine($"            // {suggestion}");
            }
            sb.AppendLine("        }");
        }

        private void GenerateTranspilerCode(StringBuilder sb)
        {
            sb.AppendLine("        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)");
            sb.AppendLine("        {");
            sb.AppendLine("            // 使用 CodeMatcher 修改 IL 代码");
            sb.AppendLine("            var matcher = new CodeMatcher(instructions);");
            sb.AppendLine("            ");
            sb.AppendLine("            // 示例：查找并替换特定指令");
            sb.AppendLine("            // matcher.MatchStartForward(");
            sb.AppendLine("            //     new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(SomeType), \"SomeMethod\"))");
            sb.AppendLine("            // );");
            sb.AppendLine("            // if (matcher.IsValid)");
            sb.AppendLine("            // {");
            sb.AppendLine("            //     matcher.Set(OpCodes.Call, AccessTools.Method(typeof(YourClass, \"YourReplacementMethod\"));");
            sb.AppendLine("            // }");
            sb.AppendLine("            ");
            sb.AppendLine("            return matcher.InstructionEnumeration();");
            sb.AppendLine("        }");
        }

        private void GenerateFinalizerCode(StringBuilder sb, string paramList)
        {
            if (!TargetMethod.IsStatic)
            {
                paramList = $"{TargetMethod.DeclaringType.Name} __instance" + (string.IsNullOrEmpty(paramList) ? "" : ", " + paramList);
            }
            sb.AppendLine($"        public static void Finalizer({paramList}, Exception __exception)");
            sb.AppendLine("        {");
            sb.AppendLine("            // 在方法结束时执行（无论是否抛出异常）");
            sb.AppendLine("            // __exception 参数包含异常信息（如果有）");
            sb.AppendLine("            // 可以通过设置 __exception = null 来吞掉异常");
            foreach (var suggestion in ParameterHandlingSuggestions)
            {
                sb.AppendLine($"            // {suggestion}");
            }
            sb.AppendLine("        }");
        }

        private static string GetTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                var genericName = type.Name.Split('`')[0];
                var args = string.Join(", ", type.GetGenericArguments().Select(GetTypeName));
                return $"{genericName}<{args}>";
            }
            return type.Name;
        }
    }

    public class PatchRecommender
    {
        private readonly ProbeContext _context;
        private readonly Dictionary<string, List<string>> _featureKeywords;

        public PatchRecommender(ProbeContext context)
        {
            _context = context;
            _featureKeywords = InitializeFeatureKeywords();
        }

        private Dictionary<string, List<string>> InitializeFeatureKeywords()
        {
            return new Dictionary<string, List<string>>
            {
                { "阻止", new List<string> { "阻止", "阻止执行", "取消", "跳过", "拦截", "屏蔽", "禁止" } },
                { "修改返回值", new List<string> { "修改返回值", "改变结果", "修改结果", "调整返回", "替换返回" } },
                { "修改逻辑", new List<string> { "修改逻辑", "改变行为", "替换逻辑", "重写逻辑", "修改内部", "改变内部" } },
                { "监听", new List<string> { "监听", "观察", "记录", "日志", "追踪", "监控" } },
                { "添加功能", new List<string> { "添加功能", "增加功能", "扩展", "附加", "增强" } },
                { "验证", new List<string> { "验证", "检查", "校验", "判断", "条件" } },
                { "异常处理", new List<string> { "异常", "错误处理", "捕获异常", "处理错误" } },
                { "音效", new List<string> { "音效", "声音", "播放声音", "sound", "audio" } },
                { "动画", new List<string> { "动画", "animation", "播放动画" } },
                { "UI", new List<string> { "UI", "界面", "窗口", "窗口", "窗口显示", "界面显示" } },
                { "伤害", new List<string> { "伤害", "damage", "攻击", "受伤" } },
                { "死亡", new List<string> { "死亡", "death", "die", "kill" } },
                { "生成", new List<string> { "生成", "spawn", "创建", "create", "实例化" } },
                { "装备", new List<string> { "装备", "equipment", "穿戴", "武器" } },
                { "技能", new List<string> { "技能", "skill", "ability", "能力" } },
                { "任务", new List<string> { "任务", "quest", "mission" } },
                { "交易", new List<string> { "交易", "trade", "买卖", "商店" } },
                { "建造", new List<string> { "建造", "build", "建筑", "放置" } },
                { "研究", new List<string> { "研究", "research", "科技" } },
                { "事件", new List<string> { "事件", "event", "incident" } }
            };
        }

        public List<PatchRecommendation> Recommend(string featureDescription)
        {
            var recommendations = new List<PatchRecommendation>();
            var detectedIntents = DetectIntents(featureDescription);
            var candidateMethods = FindCandidateMethods(featureDescription);

            foreach (var method in candidateMethods)
            {
                var recommendation = CreateRecommendation(method, featureDescription, detectedIntents);
                if (recommendation != null)
                {
                    recommendations.Add(recommendation);
                }
            }

            return recommendations.OrderByDescending(r => r.ConfidenceScore).Take(10).ToList();
        }

        public PatchRecommendation RecommendPatchType(MethodInfo method)
        {
            if (method == null) return null;

            var recommendation = new PatchRecommendation
            {
                TargetMethod = method,
                FeatureDescription = "分析现有方法"
            };

            AnalyzeMethodForPatchType(method, recommendation);
            AddParameterHandlingSuggestions(method, recommendation);
            AddMethodSpecificNotes(method, recommendation);

            return recommendation;
        }

        private List<string> DetectIntents(string description)
        {
            var intents = new List<string>();
            var lowerDesc = description.ToLower();

            foreach (var kvp in _featureKeywords)
            {
                foreach (var keyword in kvp.Value)
                {
                    if (lowerDesc.Contains(keyword.ToLower()))
                    {
                        if (!intents.Contains(kvp.Key))
                        {
                            intents.Add(kvp.Key);
                        }
                        break;
                    }
                }
            }

            return intents;
        }

        private List<MethodInfo> FindCandidateMethods(string description)
        {
            var methods = new List<MethodInfo>();
            _context.LoadGameAssemblies();

            var keywords = ExtractKeywords(description);
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

            foreach (var asm in _context.LoadedAssemblies)
            {
                try
                {
                    foreach (var type in asm.GetTypes())
                    {
                        if (!IsRimWorldType(type)) continue;

                        foreach (var method in type.GetMethods(flags))
                        {
                            if (method.IsSpecialName) continue;

                            var relevanceScore = CalculateRelevanceScore(method, keywords);
                            if (relevanceScore > 0)
                            {
                                methods.Add(method);
                            }
                        }
                    }
                }
                catch { }
            }

            return methods.Distinct().ToList();
        }

        private List<string> ExtractKeywords(string description)
        {
            var keywords = new List<string>();
            var words = description.Split(new[] { ' ', ',', '.', '、', '：', ':', '(', ')', '[', ']' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                if (word.Length >= 2)
                {
                    keywords.Add(word.ToLower());
                }
            }

            foreach (var kvp in _featureKeywords)
            {
                foreach (var keyword in kvp.Value)
                {
                    if (description.ToLower().Contains(keyword.ToLower()))
                    {
                        keywords.Add(keyword.ToLower());
                    }
                }
            }

            return keywords.Distinct().ToList();
        }

        private bool IsRimWorldType(Type type)
        {
            if (type.Namespace == null) return false;

            return type.Namespace.StartsWith("RimWorld") ||
                   type.Namespace.StartsWith("Verse") ||
                   type.Namespace.StartsWith("UnityEngine") ||
                   type.Namespace.Contains("RimWorld");
        }

        private int CalculateRelevanceScore(MethodInfo method, List<string> keywords)
        {
            var score = 0;
            var methodName = method.Name.ToLower();
            var typeName = method.DeclaringType?.Name?.ToLower() ?? "";

            foreach (var keyword in keywords)
            {
                if (methodName.Contains(keyword))
                {
                    score += 10;
                }
                if (typeName.Contains(keyword))
                {
                    score += 5;
                }

                foreach (var param in method.GetParameters())
                {
                    if (param.Name?.ToLower().Contains(keyword) == true)
                    {
                        score += 3;
                    }
                    if (param.ParameterType.Name.ToLower().Contains(keyword))
                    {
                        score += 2;
                    }
                }
            }

            return score;
        }

        private PatchRecommendation CreateRecommendation(MethodInfo method, string description, List<string> intents)
        {
            var recommendation = new PatchRecommendation
            {
                TargetMethod = method,
                FeatureDescription = description
            };

            AnalyzeMethodForPatchType(method, recommendation, intents);
            AddParameterHandlingSuggestions(method, recommendation);
            AddMethodSpecificNotes(method, recommendation);

            recommendation.ConfidenceScore = CalculateConfidenceScore(method, intents);

            return recommendation;
        }

        private void AnalyzeMethodForPatchType(MethodInfo method, PatchRecommendation recommendation, List<string> intents = null)
        {
            intents = intents ?? new List<string>();

            if (intents.Contains("阻止") || intents.Contains("验证"))
            {
                recommendation.RecommendedType = RecommendedPatchType.Prefix;
                recommendation.Reason = "需要在原方法执行前进行拦截或验证，使用 Prefix 可以阻止或控制原方法的执行。";
                recommendation.ConfidenceScore = 90;
            }
            else if (intents.Contains("修改返回值") || intents.Contains("添加功能") || intents.Contains("监听"))
            {
                recommendation.RecommendedType = RecommendedPatchType.Postfix;
                recommendation.Reason = "需要在原方法执行后处理结果或添加额外功能，使用 Postfix 可以访问和修改返回值。";
                recommendation.ConfidenceScore = 85;
            }
            else if (intents.Contains("修改逻辑") || intents.Contains("修改内部"))
            {
                recommendation.RecommendedType = RecommendedPatchType.Transpiler;
                recommendation.Reason = "需要修改方法内部的执行逻辑，使用 Transpiler 可以直接修改 IL 代码实现深度定制。";
                recommendation.ConfidenceScore = 75;
            }
            else if (intents.Contains("异常处理"))
            {
                recommendation.RecommendedType = RecommendedPatchType.Finalizer;
                recommendation.Reason = "需要处理方法执行过程中的异常，使用 Finalizer 可以捕获和处理异常。";
                recommendation.ConfidenceScore = 80;
            }
            else
            {
                if (method.ReturnType == typeof(void))
                {
                    recommendation.RecommendedType = RecommendedPatchType.Postfix;
                    recommendation.Reason = "该方法无返回值，建议使用 Postfix 在方法执行后添加功能。";
                    recommendation.ConfidenceScore = 60;
                }
                else if (method.ReturnType == typeof(bool))
                {
                    recommendation.RecommendedType = RecommendedPatchType.Prefix;
                    recommendation.Reason = "该方法返回布尔值，可能用于条件判断，建议使用 Prefix 控制执行流程。";
                    recommendation.ConfidenceScore = 65;
                }
                else
                {
                    recommendation.RecommendedType = RecommendedPatchType.Postfix;
                    recommendation.Reason = "建议使用 Postfix 在方法执行后处理返回值。";
                    recommendation.ConfidenceScore = 55;
                }
            }

            if (method.IsVirtual && !method.IsFinal)
            {
                recommendation.Notes.Add("这是一个虚方法，也可以考虑通过继承重写来实现功能。");
            }
        }

        private void AddParameterHandlingSuggestions(MethodInfo method, PatchRecommendation recommendation)
        {
            var parameters = method.GetParameters();

            if (!method.IsStatic)
            {
                recommendation.ParameterHandlingSuggestions.Add("使用 __instance 参数访问当前实例");
            }

            foreach (var param in parameters)
            {
                if (param.ParameterType.IsByRef)
                {
                    recommendation.ParameterHandlingSuggestions.Add($"参数 '{param.Name}' 是引用类型，可以直接修改其值");
                }
            }

            if (method.ReturnType != typeof(void) && recommendation.RecommendedType == RecommendedPatchType.Postfix)
            {
                recommendation.ParameterHandlingSuggestions.Add("使用 __result 参数访问和修改返回值");
            }

            if (parameters.Any(p => p.IsOptional))
            {
                recommendation.ParameterHandlingSuggestions.Add("方法有可选参数，Patch 中可以只声明需要的参数");
            }
        }

        private void AddMethodSpecificNotes(MethodInfo method, PatchRecommendation recommendation)
        {
            if (!method.IsPublic)
            {
                var visibility = method.IsPrivate ? "private" : method.IsFamily ? "protected" : "internal";
                recommendation.Notes.Add($"该方法是 {visibility} 的，需要确保 Patch 类能够访问它");
            }

            if (method.IsStatic)
            {
                recommendation.Notes.Add("这是一个静态方法，Patch 方法也应该是静态的");
            }

            if (method.IsGenericMethod)
            {
                recommendation.Notes.Add("这是一个泛型方法，可能需要特殊处理泛型参数");
            }

            var attributes = method.GetCustomAttributes(false);
            foreach (var attr in attributes)
            {
                var attrName = attr.GetType().Name;
                if (attrName.Contains("Obsolete"))
                {
                    recommendation.Notes.Add("该方法已标记为过时，未来版本可能被移除");
                }
                if (attrName.Contains("Conditional"))
                {
                    recommendation.Notes.Add("该方法有条件编译特性，可能不会在所有情况下执行");
                }
            }

            if (method.DeclaringType?.IsSealed == true)
            {
                recommendation.Notes.Add("声明类型是 sealed 的，无法通过继承来实现功能");
            }
        }

        private int CalculateConfidenceScore(MethodInfo method, List<string> intents)
        {
            var score = 50;

            if (method.IsPublic) score += 10;
            if (method.IsStatic) score += 5;
            if (method.ReturnType != typeof(void)) score += 5;

            if (intents.Count > 0)
            {
                score += intents.Count * 10;
            }

            var methodName = method.Name.ToLower();
            foreach (var intent in intents)
            {
                if (_featureKeywords.TryGetValue(intent, out var keywords))
                {
                    foreach (var keyword in keywords)
                    {
                        if (methodName.Contains(keyword.ToLower()))
                        {
                            score += 15;
                            break;
                        }
                    }
                }
            }

            return Math.Min(score, 100);
        }

        public List<PatchRecommendation> GetRecommendationsForType(Type type, string featureDescription)
        {
            var recommendations = new List<PatchRecommendation>();
            var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
            var intents = DetectIntents(featureDescription);

            foreach (var method in type.GetMethods(flags))
            {
                if (method.IsSpecialName) continue;

                var recommendation = CreateRecommendation(method, featureDescription, intents);
                if (recommendation != null)
                {
                    recommendations.Add(recommendation);
                }
            }

            return recommendations.OrderByDescending(r => r.ConfidenceScore).ToList();
        }

        public void PrintRecommendations(List<PatchRecommendation> recommendations)
        {
            if (recommendations == null || recommendations.Count == 0)
            {
                Console.WriteLine("未找到匹配的 Patch 推荐结果。");
                return;
            }

            Console.WriteLine($"\n找到 {recommendations.Count} 个推荐结果:\n");

            for (int i = 0; i < recommendations.Count; i++)
            {
                Console.WriteLine($"\n[{i + 1}]");
                recommendations[i].PrintDetails();
            }
        }
    }
}
