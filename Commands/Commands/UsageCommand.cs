using System;
using System.Linq;
using System.Reflection;
using RimWorldModDevProbe.Analysis;
using RimWorldModDevProbe.Core;
using RimWorldModDevProbe.Probes;

namespace RimWorldModDevProbe.Commands
{
    public class UsageCommand : CommandBase
    {
        public UsageCommand(ProbeContext context, ServiceContainer services) 
            : base(context, services)
        {
        }

        public override string Name => "usage";

        public override string Description => "分析字段的使用情况";

        public override void Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: usage <field>");
                return;
            }

            var fieldName = string.Join(" ", args);
            AnalyzeFieldUsage(fieldName);
        }

        private void AnalyzeFieldUsage(string fieldName)
        {
            if (!_services.TryResolve<FieldUsageAnalyzer>(out var analyzer))
            {
                Console.WriteLine("FieldUsageAnalyzer not available.");
                return;
            }

            if (!_services.TryResolve<IProbe>("dll", out var probe))
            {
                Console.WriteLine("DLL probe not available.");
                return;
            }
            var dllProbe = probe as DllProbe;
            if (dllProbe == null)
            {
                Console.WriteLine("DLL probe not available.");
                return;
            }

            var fields = dllProbe.SearchFields(fieldName, new SearchOptions()).ToList();
            if (fields.Count == 0)
            {
                Console.WriteLine($"No fields found matching '{fieldName}'.");
                return;
            }

            FieldInfo targetField = null;
            if (fields.Count == 1)
            {
                targetField = fields[0].FieldInfo;
            }
            else
            {
                Console.WriteLine($"\nFound {fields.Count} fields:");
                for (int i = 0; i < Math.Min(fields.Count, 20); i++)
                {
                    Console.WriteLine($"  [{i}] {fields[i].Name} - {fields[i].Source}");
                }

                Console.Write("\nSelect field to analyze (or press Enter to skip): ");
                var selection = Console.ReadLine();
                if (int.TryParse(selection, out var index) && index >= 0 && index < fields.Count)
                {
                    targetField = fields[index].FieldInfo;
                }
            }

            if (targetField == null)
            {
                Console.WriteLine("No field selected.");
                return;
            }

            var result = analyzer.Analyze(targetField);
            result.PrintDetails();
        }
    }
}
