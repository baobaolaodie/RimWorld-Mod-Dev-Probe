using System;
using System.Linq;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Commands
{
    public class ExampleCommand : CommandBase
    {
        public ExampleCommand(ProbeContext context, ServiceContainer services) 
            : base(context, services)
        {
        }

        public override string Name => "example";

        public override string Description => "查看示例代码";

        public override void Execute(string[] args)
        {
            if (args.Length == 0)
            {
                ListExamples();
            }
            else
            {
                var featureName = string.Join(" ", args);
                ShowExample(featureName);
            }
        }

        private void ListExamples()
        {
            if (!_services.TryResolve<ExampleLibrary>(out var exampleLibrary))
            {
                Console.WriteLine("ExampleLibrary not available.");
                return;
            }

            var examples = exampleLibrary.GetAllExamples();

            Console.WriteLine($"\n{'='} 示例库 ({examples.Count} 个示例) {'='}");

            var xmlDefExamples = examples.Where(e =>
                e.Feature.Contains("武器") || e.Feature.Contains("建筑") ||
                e.Feature.Contains("消耗品") || e.Feature.Contains("资源") ||
                e.Feature.Contains("apparel") || e.Feature.Contains("动物") ||
                e.Feature == "XML Def").ToList();

            var xmlPatchExamples = examples.Where(e =>
                e.Feature.Contains("Patch") || e.Feature.Contains("修改") ||
                e.Feature == "XML Patch").ToList();

            var harmonyExamples = examples.Where(e =>
                e.Feature.Contains("Harmony") || e.Feature.Contains("Prefix") ||
                e.Feature.Contains("Postfix") || e.Feature.Contains("Transpiler") ||
                e.Feature.Contains("音效")).ToList();

            var modExamples = examples.Where(e =>
                e.Feature.Contains("Race") || e.Feature.Contains("种族") ||
                e.Feature.Contains("Incident") || e.Feature.Contains("事件") ||
                e.Feature.Contains("Research") || e.Feature.Contains("研究") ||
                e.Feature.Contains("Trader") || e.Feature.Contains("商人")).ToList();

            if (xmlDefExamples.Count > 0)
            {
                Console.WriteLine($"\n--- XML Def 示例 ({xmlDefExamples.Count}) ---");
                foreach (var ex in xmlDefExamples)
                {
                    Console.WriteLine($"  • {ex.Title} [{ex.Feature}]");
                }
            }

            if (xmlPatchExamples.Count > 0)
            {
                Console.WriteLine($"\n--- XML Patch 示例 ({xmlPatchExamples.Count}) ---");
                foreach (var ex in xmlPatchExamples)
                {
                    Console.WriteLine($"  • {ex.Title} [{ex.Feature}]");
                }
            }

            if (harmonyExamples.Count > 0)
            {
                Console.WriteLine($"\n--- Harmony Patch 示例 ({harmonyExamples.Count}) ---");
                foreach (var ex in harmonyExamples)
                {
                    Console.WriteLine($"  • {ex.Title} [{ex.Feature}]");
                }
            }

            if (modExamples.Count > 0)
            {
                Console.WriteLine($"\n--- 复杂 Mod 示例 ({modExamples.Count}) ---");
                foreach (var ex in modExamples)
                {
                    Console.WriteLine($"  • {ex.Title} [{ex.Feature}]");
                }
            }

            Console.WriteLine($"\n使用 'example <关键词>' 查看详细示例");
            Console.WriteLine($"分类快捷命令: example xml / example patch / example harmony / example mod");
        }

        private void ShowExample(string featureName)
        {
            if (!_services.TryResolve<ExampleLibrary>(out var exampleLibrary))
            {
                Console.WriteLine("ExampleLibrary not available.");
                return;
            }

            var lowerName = featureName.ToLowerInvariant();

            if (lowerName == "xml" || lowerName == "xmldef" || lowerName == "def")
            {
                PrintExamplesByCategory(exampleLibrary, new[] { "武器", "建筑", "消耗品", "资源", "apparel", "动物", "XML Def" });
                return;
            }

            if (lowerName == "patch" || lowerName == "xmlpatch")
            {
                PrintExamplesByCategory(exampleLibrary, new[] { "Patch", "修改", "XML Patch" });
                return;
            }

            if (lowerName == "harmony")
            {
                PrintExamplesByCategory(exampleLibrary, new[] { "Harmony", "Prefix", "Postfix", "Transpiler", "音效" });
                return;
            }

            if (lowerName == "mod" || lowerName == "complex")
            {
                PrintExamplesByCategory(exampleLibrary, new[] { "Race", "种族", "Incident", "事件", "Research", "研究", "Trader", "商人" });
                return;
            }

            exampleLibrary.PrintExample(featureName);
        }

        private void PrintExamplesByCategory(ExampleLibrary exampleLibrary, string[] keywords)
        {
            var examples = exampleLibrary.GetAllExamples();
            var filtered = examples.Where(e =>
                keywords.Any(k => e.Feature.IndexOf(k, StringComparison.OrdinalIgnoreCase) >= 0)).ToList();

            if (filtered.Count == 0)
            {
                Console.WriteLine("未找到匹配的示例。");
                return;
            }

            Console.WriteLine($"\n{'='} 分类示例 ({filtered.Count} 个) {'='}");
            for (int i = 0; i < filtered.Count; i++)
            {
                var ex = filtered[i];
                Console.WriteLine($"\n[{i + 1}] {ex.Title}");
                Console.WriteLine($"    功能: {ex.Feature}");
                Console.WriteLine($"    文件数: {ex.Files.Count}");
                Console.WriteLine($"    步骤数: {ex.Steps.Count}");
            }

            Console.Write("\n输入编号查看详情 (或按 Enter 跳过): ");
            var selection = Console.ReadLine();
            if (int.TryParse(selection, out var index) && index >= 1 && index <= filtered.Count)
            {
                filtered[index - 1].PrintDetails();
            }
        }
    }
}
