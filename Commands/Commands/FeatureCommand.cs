using System;
using System.Linq;
using RimWorldModDevProbe.Analysis;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Commands
{
    public class FeatureCommand : CommandBase
    {
        public FeatureCommand(ProbeContext context, ServiceContainer services) 
            : base(context, services)
        {
        }

        public override string Name => "feature";

        public override string Description => "通过关键词搜索功能";

        public override void Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: feature <keyword>");
                return;
            }

            var keyword = string.Join(" ", args);
            SearchFeatures(keyword);
        }

        private void SearchFeatures(string keyword)
        {
            if (!_services.TryResolve<FeatureKeywordMap>(out var featureMap))
            {
                Console.WriteLine("FeatureKeywordMap not available.");
                return;
            }

            var results = featureMap.Search(keyword, new SearchOptions()).ToList();
            if (results.Count == 0)
            {
                Console.WriteLine($"No features found matching '{keyword}'.");
                return;
            }

            Console.WriteLine($"\n=== Feature Search: {keyword} ===");
            Console.WriteLine($"Found {results.Count} results:\n");

            for (int i = 0; i < results.Count; i++)
            {
                var r = results[i] as FeatureKeywordResult;
                if (r == null) continue;

                Console.WriteLine($"[{i + 1}] {r.Entry.TargetName} ({r.Entry.RecommendationType})");
                Console.WriteLine($"    {r.Entry.Description}");
                Console.WriteLine($"    Matched: {r.MatchedKeyword} (Score: {r.MatchScore})");
                Console.WriteLine();
            }
        }
    }
}
