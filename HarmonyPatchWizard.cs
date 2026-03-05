using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using RimWorldModDevProbe.Core;
using RimWorldModDevProbe.Analysis;
using RimWorldModDevProbe.Wizards.Core;

namespace RimWorldModDevProbe
{
    public class WelcomeStep : WizardStepBase
    {
        public override string Title => "欢迎";
        public override string Description => "Harmony Patch 创建向导";
        public override bool CanSkip => false;

        public WelcomeStep() : base("欢迎", "Harmony Patch 创建向导")
        {
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();

            Console.WriteLine("  本向导将帮助您创建 Harmony Patch:");
            Console.WriteLine();
            Console.WriteLine("  1. 输入要修改的功能描述");
            Console.WriteLine("  2. 查看推荐的目标方法");
            Console.WriteLine("  3. 选择 Patch 类型 (Prefix/Postfix/Transpiler/Finalizer)");
            Console.WriteLine("  4. 生成完整的 Patch 代码");
            Console.WriteLine("  5. 获取注册指导");
            Console.WriteLine();
            Console.WriteLine("  Harmony Patch 类型说明:");
            Console.WriteLine("  - Prefix:  在原方法执行前运行，可阻止原方法执行");
            Console.WriteLine("  - Postfix: 在原方法执行后运行，可修改返回值");
            Console.WriteLine("  - Transpiler: 修改方法的 IL 代码，实现深度定制");
            Console.WriteLine("  - Finalizer: 处理方法执行过程中的异常");
            Console.WriteLine();

            Pause("按任意键继续...");
        }
    }

    public class FeatureDescriptionStep : WizardStepBase
    {
        public override string Title => "功能描述";
        public override string Description => "输入要修改的功能描述";
        public override bool CanSkip => false;

        public FeatureDescriptionStep() : base("功能描述", "输入要修改的功能描述")
        {
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();

            Console.WriteLine("  请描述您想要实现的功能或修改的行为。");
            Console.WriteLine("  描述越详细，推荐结果越准确。");
            Console.WriteLine();
            Console.WriteLine("  示例描述:");
            Console.WriteLine("  - 修改角色死亡时的音效");
            Console.WriteLine("  - 阻止敌人生成");
            Console.WriteLine("  - 修改武器伤害计算");
            Console.WriteLine("  - 添加自定义日志记录");
            Console.WriteLine("  - 修改交易价格");
            Console.WriteLine();

            string description;
            while (true)
            {
                description = ReadInput("请输入功能描述");
                if (string.IsNullOrWhiteSpace(description))
                {
                    ShowError("功能描述不能为空，请重新输入。");
                    continue;
                }
                break;
            }

            context.SetData("FeatureDescription", description);
            ShowSuccess($"已记录功能描述: {description}");
        }
    }

    public class TargetRecommendationStep : WizardStepBase
    {
        private readonly PatchRecommender _recommender;
        public override string Title => "目标推荐";
        public override string Description => "分析并推荐目标方法";
        public override bool CanSkip => false;

        public TargetRecommendationStep(PatchRecommender recommender) 
            : base("目标推荐", "分析并推荐目标方法")
        {
            _recommender = recommender;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();

            var description = context.GetData<string>("FeatureDescription");
            ShowInfo($"正在分析: {description}");
            Console.WriteLine();

            var recommendations = _recommender.Recommend(description);
            context.SetData("Recommendations", recommendations);

            if (recommendations == null || recommendations.Count == 0)
            {
                ShowWarning("未找到匹配的推荐结果。");
                Console.WriteLine("  您可以在下一步手动输入目标类型和方法名称。");
                return;
            }

            Console.WriteLine($"  找到 {recommendations.Count} 个推荐结果:");
            Console.WriteLine();

            for (int i = 0; i < recommendations.Count; i++)
            {
                var rec = recommendations[i];
                Console.WriteLine($"  [{i + 1}] {rec.TargetMethod?.DeclaringType?.Name}.{rec.TargetMethod?.Name}");
                Console.WriteLine($"      类型: {rec.TargetMethod?.DeclaringType?.FullName}");
                Console.WriteLine($"      签名: {rec.GetMethodSignature()}");
                Console.WriteLine($"      推荐类型: {rec.GetPatchTypeName()}");
                Console.WriteLine($"      置信度: {rec.ConfidenceScore}/100");
                Console.WriteLine($"      理由: {rec.Reason}");
                Console.WriteLine();
            }

            ShowInfo("您可以查看推荐列表，或在下一步手动输入目标。");
        }
    }

    public class TargetConfirmationStep : WizardStepBase
    {
        private readonly PatchRecommender _recommender;
        public override string Title => "目标确认";
        public override string Description => "确认或手动输入目标方法";
        public override bool CanSkip => false;

        public TargetConfirmationStep(PatchRecommender recommender)
            : base("目标确认", "确认或手动输入目标方法")
        {
            _recommender = recommender;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();

            var recommendations = context.GetData<List<PatchRecommendation>>("Recommendations");
            PatchRecommendation selectedRecommendation = null;

            if (recommendations != null && recommendations.Count > 0)
            {
                Console.WriteLine("  选择方式:");
                Console.WriteLine("  [1] 从推荐列表中选择");
                Console.WriteLine("  [2] 手动输入目标类型和方法");
                Console.WriteLine();

                var choice = ReadChoice("请选择", new[] { "从推荐列表选择", "手动输入" }, "从推荐列表选择");

                if (choice == "从推荐列表选择")
                {
                    var index = ReadInt("请输入推荐项编号", 1, 1, recommendations.Count);
                    selectedRecommendation = recommendations[index - 1];
                }
                else
                {
                    selectedRecommendation = GetManualInput(context);
                }
            }
            else
            {
                ShowInfo("没有推荐结果，请手动输入目标。");
                selectedRecommendation = GetManualInput(context);
            }

            if (selectedRecommendation != null)
            {
                context.SetData("SelectedRecommendation", selectedRecommendation);
                ShowSuccess($"已选择目标方法: {selectedRecommendation.TargetMethod?.DeclaringType?.Name}.{selectedRecommendation.TargetMethod?.Name}");
            }
        }

        private PatchRecommendation GetManualInput(WizardContext context)
        {
            Console.WriteLine();
            var typeName = ReadInput("请输入目标类型完整名称 (如: RimWorld.Pawn)");
            if (string.IsNullOrWhiteSpace(typeName))
            {
                ShowError("类型名称不能为空。");
                return null;
            }

            var methodName = ReadInput("请输入目标方法名称");
            if (string.IsNullOrWhiteSpace(methodName))
            {
                ShowError("方法名称不能为空。");
                return null;
            }

            context.ProbeContext.LoadGameAssemblies();
            Type targetType = null;
            MethodInfo targetMethod = null;

            foreach (var asm in context.ProbeContext.LoadedAssemblies)
            {
                try
                {
                    targetType = asm.GetType(typeName);
                    if (targetType != null)
                    {
                        var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
                        targetMethod = targetType.GetMethod(methodName, flags);
                        if (targetMethod != null)
                        {
                            break;
                        }
                    }
                }
                catch { }
            }

            if (targetMethod == null)
            {
                ShowError($"未找到方法: {typeName}.{methodName}");
                return null;
            }

            var recommendation = _recommender.RecommendPatchType(targetMethod);
            recommendation.FeatureDescription = context.GetData<string>("FeatureDescription");
            return recommendation;
        }
    }

    public class PatchTypeSelectionStep : WizardStepBase
    {
        public override string Title => "Patch 类型选择";
        public override string Description => "选择要使用的 Patch 类型";
        public override bool CanSkip => false;

        public PatchTypeSelectionStep() : base("Patch 类型选择", "选择要使用的 Patch 类型")
        {
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();

            var recommendation = context.GetData<PatchRecommendation>("SelectedRecommendation");
            if (recommendation == null)
            {
                ShowError("未找到选中的推荐。");
                return;
            }

            Console.WriteLine($"  目标方法: {recommendation.TargetMethod?.DeclaringType?.Name}.{recommendation.TargetMethod?.Name}");
            Console.WriteLine($"  推荐类型: {recommendation.GetPatchTypeName()}");
            Console.WriteLine($"  推荐理由: {recommendation.Reason}");
            Console.WriteLine();

            Console.WriteLine("  Patch 类型说明:");
            Console.WriteLine("  [1] Prefix     - 在原方法执行前运行，返回 false 可阻止原方法执行");
            Console.WriteLine("  [2] Postfix    - 在原方法执行后运行，可访问和修改返回值");
            Console.WriteLine("  [3] Transpiler - 修改方法的 IL 代码，实现深度定制");
            Console.WriteLine("  [4] Finalizer  - 处理方法执行过程中的异常");
            Console.WriteLine();

            var defaultType = recommendation.GetPatchTypeName();
            var selectedType = ReadChoice("请选择 Patch 类型", 
                new[] { "Prefix", "Postfix", "Transpiler", "Finalizer" }, 
                defaultType);

            if (Enum.TryParse<RecommendedPatchType>(selectedType, out var patchType))
            {
                recommendation.RecommendedType = patchType;
                ShowSuccess($"已选择 Patch 类型: {selectedType}");
            }
        }
    }

    public class CodeGenerationStep : WizardStepBase
    {
        public override string Title => "代码生成";
        public override string Description => "生成完整的 Patch 代码";
        public override bool CanSkip => false;

        public CodeGenerationStep() : base("代码生成", "生成完整的 Patch 代码")
        {
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();

            var recommendation = context.GetData<PatchRecommendation>("SelectedRecommendation");
            if (recommendation == null)
            {
                ShowError("未找到选中的推荐。");
                return;
            }

            var code = recommendation.GeneratePatchCode();
            context.SetData("GeneratedCode", code);

            Console.WriteLine("  生成的 Patch 代码:");
            Console.WriteLine();
            Console.WriteLine(new string('-', 60));

            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(code);
            Console.ForegroundColor = originalColor;

            Console.WriteLine(new string('-', 60));
            Console.WriteLine();

            ShowSuccess("代码生成完成！");
        }
    }

    public class RegistrationGuideStep : WizardStepBase
    {
        public override string Title => "注册指导";
        public override string Description => "如何在 Mod 中注册 Patch";
        public override bool CanSkip => false;

        public RegistrationGuideStep() : base("注册指导", "如何在 Mod 中注册 Patch")
        {
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();

            var recommendation = context.GetData<PatchRecommendation>("SelectedRecommendation");
            var generatedCode = context.GetData<string>("GeneratedCode");

            Console.WriteLine("  在 Mod 中注册 Harmony Patch 的步骤:");
            Console.WriteLine();

            Console.WriteLine("  步骤 1: 创建 Mod 主类");
            Console.WriteLine(new string('-', 50));
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(GenerateModMainClass());
            Console.ForegroundColor = originalColor;
            Console.WriteLine();

            Console.WriteLine("  步骤 2: 添加 HarmonyLib 引用");
            Console.WriteLine("  - 在项目中引用 0Harmony.dll (通常位于 RimWorld 安装目录)");
            Console.WriteLine("  - 或通过 NuGet 安装 Lib.Harmony 包");
            Console.WriteLine();

            Console.WriteLine("  步骤 3: 项目结构");
            Console.WriteLine(new string('-', 50));
            Console.WriteLine(GenerateProjectStructure());
            Console.WriteLine();

            Console.WriteLine("  步骤 4: 编译和部署");
            Console.WriteLine("  1. 编译项目生成 DLL 文件");
            Console.WriteLine("  2. 将 DLL 放入 Mod 的 Assemblies 文件夹");
            Console.WriteLine("  3. 确保 About.xml 正确配置");
            Console.WriteLine();

            Console.WriteLine("  步骤 5: 调试技巧");
            Console.WriteLine("  - 使用 Log.Message() 输出调试信息");
            Console.WriteLine("  - 检查 RimWorld 的输出日志 (Player.log)");
            Console.WriteLine("  - 使用 HarmonyDebug 特性获取详细信息");
            Console.WriteLine();

            ShowAdditionalTips(recommendation);

            Console.WriteLine();
            ShowSuccess("向导完成！您现在可以复制生成的代码并开始开发。");

            var saveToFile = ReadBool("是否将代码保存到文件?", true);
            if (saveToFile)
            {
                SaveCodeToFile(context);
            }
        }

        private string GenerateModMainClass()
        {
            var sb = new StringBuilder();
            sb.AppendLine("using HarmonyLib;");
            sb.AppendLine("using Verse;");
            sb.AppendLine();
            sb.AppendLine("namespace YourModNamespace");
            sb.AppendLine("{");
            sb.AppendLine("    public class YourMod : Mod");
            sb.AppendLine("    {");
            sb.AppendLine("        public YourMod(ModContentPack content) : base(content)");
            sb.AppendLine("        {");
            sb.AppendLine("            var harmony = new Harmony(\"YourName.YourMod\");");
            sb.AppendLine("            harmony.PatchAll();");
            sb.AppendLine("            Log.Message(\"[YourMod] Harmony patches applied successfully.\");");
            sb.AppendLine("        }");
            sb.AppendLine("    }");
            sb.AppendLine("}");
            return sb.ToString();
        }

        private string GenerateProjectStructure()
        {
            var sb = new StringBuilder();
            sb.AppendLine("YourMod/");
            sb.AppendLine("├── About/");
            sb.AppendLine("│   ├── About.xml");
            sb.AppendLine("│   └── Preview.png");
            sb.AppendLine("├── Assemblies/");
            sb.AppendLine("│   └── YourMod.dll");
            sb.AppendLine("├── Defs/");
            sb.AppendLine("│   └── (自定义 Defs)");
            sb.AppendLine("├── Patches/");
            sb.AppendLine("│   └── (XML Patches)");
            sb.AppendLine("└── Source/");
            sb.AppendLine("    ├── YourMod.cs");
            sb.AppendLine("    └── (Patch 文件)");
            return sb.ToString();
        }

        private void ShowAdditionalTips(PatchRecommendation recommendation)
        {
            if (recommendation == null) return;

            Console.WriteLine("  针对您选择的 Patch 的额外提示:");
            Console.WriteLine(new string('-', 50));

            if (recommendation.ParameterHandlingSuggestions.Count > 0)
            {
                Console.WriteLine("  参数处理建议:");
                foreach (var suggestion in recommendation.ParameterHandlingSuggestions)
                {
                    Console.WriteLine($"  - {suggestion}");
                }
                Console.WriteLine();
            }

            if (recommendation.Notes.Count > 0)
            {
                Console.WriteLine("  注意事项:");
                foreach (var note in recommendation.Notes)
                {
                    Console.WriteLine($"  ! {note}");
                }
                Console.WriteLine();
            }

            Console.WriteLine("  常用 Harmony 特殊参数:");
            Console.WriteLine("  - __instance: 访问当前实例 (非静态方法)");
            Console.WriteLine("  - __result:   访问/修改返回值 (Postfix)");
            Console.WriteLine("  - __state:    在 Prefix 和 Postfix 之间传递数据");
            Console.WriteLine("  - __originalMethod: 获取原始方法信息");
        }

        private void SaveCodeToFile(WizardContext context)
        {
            var generatedCode = context.GetData<string>("GeneratedCode");
            var recommendation = context.GetData<PatchRecommendation>("SelectedRecommendation");

            var defaultFileName = $"Patch_{recommendation?.TargetMethod?.DeclaringType?.Name}_{recommendation?.TargetMethod?.Name}_{recommendation?.GetPatchTypeName()}.cs";
            var fileName = ReadInput("请输入文件名", defaultFileName);

            try
            {
                var filePath = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), fileName);
                System.IO.File.WriteAllText(filePath, generatedCode);
                ShowSuccess($"代码已保存到: {filePath}");
            }
            catch (Exception ex)
            {
                ShowError($"保存文件失败: {ex.Message}");
            }
        }
    }

    public class HarmonyPatchWizard
    {
        private readonly DevWizard _wizard;
        private readonly PatchRecommender _recommender;

        public HarmonyPatchWizard(ProbeContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _recommender = new PatchRecommender(context);
            _wizard = new DevWizard(context);

            InitializeSteps();
        }

        private void InitializeSteps()
        {
            _wizard.AddStep(new WelcomeStep());
            _wizard.AddStep(new FeatureDescriptionStep());
            _wizard.AddStep(new TargetRecommendationStep(_recommender));
            _wizard.AddStep(new TargetConfirmationStep(_recommender));
            _wizard.AddStep(new PatchTypeSelectionStep());
            _wizard.AddStep(new CodeGenerationStep());
            _wizard.AddStep(new RegistrationGuideStep());
        }

        public WizardResult Run()
        {
            return _wizard.Run();
        }

        public static void RunQuickWizard(ProbeContext context)
        {
            var wizard = new HarmonyPatchWizard(context);
            var result = wizard.Run();

            Console.WriteLine();
            result.PrintSummary();

            if (result.Success)
            {
                var code = result.GetData<string>("GeneratedCode");
                if (!string.IsNullOrEmpty(code))
                {
                    Console.WriteLine("\n生成的代码:");
                    Console.WriteLine(new string('-', 50));
                    Console.WriteLine(code);
                }
            }
        }
    }
}
