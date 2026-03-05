using System;
using System.Collections.Generic;
using System.Linq;
using RimWorldModDevProbe.Core;
using RimWorldModDevProbe.Examples;

namespace RimWorldModDevProbe
{
    public enum FileType
    {
        CSharp,
        Xml,
        Text
    }

    public class ExampleFile
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string Content { get; set; }
        public FileType Type { get; set; }

        public ExampleFile(string fileName, string filePath, string content, FileType type)
        {
            FileName = fileName;
            FilePath = filePath;
            Content = content;
            Type = type;
        }
    }

    public class Example
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Feature { get; set; }
        public List<string> Keywords { get; set; } = new List<string>();
        public List<ExampleFile> Files { get; set; } = new List<ExampleFile>();
        public List<string> Steps { get; set; } = new List<string>();

        public void PrintDetails()
        {
            Console.WriteLine($"\n{'='} {Title} {'='}");
            Console.WriteLine($"\n功能类型: {Feature}");
            Console.WriteLine($"\n描述:\n  {Description}");

            if (Files.Count > 0)
            {
                Console.WriteLine($"\n示例文件 ({Files.Count}):");
                foreach (var file in Files)
                {
                    Console.WriteLine($"\n  [{file.Type}] {file.FilePath}");
                    Console.WriteLine($"  {new string('-', 60)}");
                    var lines = file.Content.Split('\n');
                    foreach (var line in lines.Take(30))
                    {
                        Console.WriteLine($"  {line}");
                    }
                    if (lines.Length > 30)
                    {
                        Console.WriteLine($"  ... ({lines.Length - 30} more lines)");
                    }
                }
            }

            if (Steps.Count > 0)
            {
                Console.WriteLine($"\n实现步骤:");
                for (int i = 0; i < Steps.Count; i++)
                {
                    Console.WriteLine($"  {i + 1}. {Steps[i]}");
                }
            }
        }
    }

    public class ExampleLibrary
    {
        private readonly Dictionary<string, Example> _examples;
        private readonly ProbeContext _context;

        public ExampleLibrary(ProbeContext context)
        {
            _context = context;
            _examples = new Dictionary<string, Example>(StringComparer.OrdinalIgnoreCase);
            InitializeExamples();
        }

        private void InitializeExamples()
        {
            var allExamples = new List<Example>();

            allExamples.AddRange(SoundExamples.GetExamples());
            allExamples.AddRange(WeaponExamples.GetExamples());
            allExamples.AddRange(BuildingExamples.GetExamples());
            allExamples.AddRange(ConsumableExamples.GetExamples());
            allExamples.AddRange(PatchExamples.GetExamples());
            allExamples.AddRange(HarmonyExamples.GetExamples());
            allExamples.AddRange(RaceExamples.GetExamples());
            allExamples.AddRange(IncidentExamples.GetExamples());

            foreach (var example in allExamples)
            {
                RegisterExample(example);
            }
        }

        private void RegisterExample(Example example)
        {
            if (example.Keywords != null)
            {
                foreach (var keyword in example.Keywords)
                {
                    if (!_examples.ContainsKey(keyword))
                    {
                        _examples[keyword] = example;
                    }
                }
            }

            if (!string.IsNullOrEmpty(example.Title) && !_examples.ContainsKey(example.Title))
            {
                _examples[example.Title] = example;
            }

            if (!string.IsNullOrEmpty(example.Feature) && !_examples.ContainsKey(example.Feature))
            {
                _examples[example.Feature] = example;
            }
        }

        public Example GetExample(string featureName)
        {
            if (string.IsNullOrEmpty(featureName))
            {
                return null;
            }

            if (_examples.TryGetValue(featureName, out var example))
            {
                return example;
            }

            var matchingKey = _examples.Keys.FirstOrDefault(k => 
                k.IndexOf(featureName, StringComparison.OrdinalIgnoreCase) >= 0);

            if (matchingKey != null)
            {
                return _examples[matchingKey];
            }

            return null;
        }

        public List<Example> GetAllExamples()
        {
            return _examples.Values.Distinct().ToList();
        }

        public List<string> GetAvailableFeatures()
        {
            return _examples.Keys.OrderBy(k => k).ToList();
        }

        public List<Example> SearchExamples(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return GetAllExamples();
            }

            var results = new List<Example>();
            var seenExamples = new HashSet<Example>();

            foreach (var kvp in _examples)
            {
                if (kvp.Key.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (seenExamples.Add(kvp.Value))
                    {
                        results.Add(kvp.Value);
                    }
                }
            }

            foreach (var example in _examples.Values.Distinct())
            {
                if (example.Title.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    example.Description.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    example.Feature.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    if (seenExamples.Add(example))
                    {
                        results.Add(example);
                    }
                }
            }

            return results;
        }

        public void PrintAllExamples()
        {
            var examples = GetAllExamples();
            Console.WriteLine($"\n{'='} 示例库 ({examples.Count} 个示例) {'='}");

            for (int i = 0; i < examples.Count; i++)
            {
                var example = examples[i];
                Console.WriteLine($"\n[{i + 1}] {example.Title}");
                Console.WriteLine($"    功能: {example.Feature}");
                Console.WriteLine($"    文件数: {example.Files.Count}");
                Console.WriteLine($"    步骤数: {example.Steps.Count}");
            }

            Console.WriteLine($"\n使用 'example <名称>' 查看详细示例");
            Console.WriteLine($"可用关键词: {string.Join(", ", GetAvailableFeatures())}");
        }

        public void PrintExample(string featureName)
        {
            var example = GetExample(featureName);
            if (example == null)
            {
                Console.WriteLine($"未找到 '{featureName}' 相关的示例。");
                Console.WriteLine($"可用关键词: {string.Join(", ", GetAvailableFeatures())}");
                return;
            }

            example.PrintDetails();
        }

        public string GenerateExampleCode(string featureName, string modName = "YourMod")
        {
            var example = GetExample(featureName);
            if (example == null)
            {
                return $"// 未找到 '{featureName}' 相关的示例";
            }

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"// {example.Title}");
            sb.AppendLine($"// {example.Description}");
            sb.AppendLine();

            foreach (var file in example.Files)
            {
                sb.AppendLine($"// ========== {file.FilePath} ==========");
                sb.AppendLine(file.Content);
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}
