using System;
using System.Linq;
using RimWorldModDevProbe.Analysis;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Commands
{
    public class RecommendCommand : CommandBase
    {
        public RecommendCommand(ProbeContext context, ServiceContainer services) 
            : base(context, services)
        {
        }

        public override string Name => "recommend";

        public override string Description => "获取Patch推荐";

        public override void Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: recommend <feature>");
                return;
            }

            var feature = string.Join(" ", args);
            RecommendPatches(feature);
        }

        private void RecommendPatches(string feature)
        {
            if (!_services.TryResolve<PatchRecommender>(out var recommender))
            {
                Console.WriteLine("PatchRecommender not available.");
                return;
            }

            var recommendations = recommender.Recommend(feature);
            if (recommendations.Count == 0)
            {
                Console.WriteLine($"No patch recommendations found for '{feature}'.");
                return;
            }

            recommender.PrintRecommendations(recommendations);

            Console.Write("\nView patch code for a recommendation? (Enter number or press Enter to skip): ");
            var selection = Console.ReadLine();
            if (int.TryParse(selection, out var index) && index >= 1 && index <= recommendations.Count)
            {
                var rec = recommendations[index - 1];
                Console.WriteLine("\n--- Generated Patch Code ---");
                Console.WriteLine(rec.GeneratePatchCode());
            }
        }
    }
}
