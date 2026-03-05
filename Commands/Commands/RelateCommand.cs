using System;
using System.Linq;
using RimWorldModDevProbe.Analysis;
using RimWorldModDevProbe.Core;
using RimWorldModDevProbe.Probes;

namespace RimWorldModDevProbe.Commands
{
    public class RelateCommand : CommandBase
    {
        public RelateCommand(ProbeContext context, ServiceContainer services) 
            : base(context, services)
        {
        }

        public override string Name => "relate";

        public override string Description => "显示类型的相关资源";

        public override void Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: relate <type>");
                return;
            }

            var typeName = string.Join(" ", args);
            ShowRelatedResources(typeName);
        }

        private void ShowRelatedResources(string typeName)
        {
            if (!_services.TryResolve<ResourceRecommender>(out var recommender))
            {
                Console.WriteLine("ResourceRecommender not available.");
                return;
            }

            if (!_services.TryResolve<TypeDefMapper>(out var typeDefMapper))
            {
                Console.WriteLine("TypeDefMapper not available.");
                return;
            }

            var type = typeDefMapper.GetTypeByDefName(typeName);
            if (type == null)
            {
                if (_services.TryResolve<IProbe>("dll", out var probe) && probe is DllProbe dllProbe)
                {
                    var results = dllProbe.Search(typeName, new SearchOptions()).ToList();
                    if (results.Count > 0)
                    {
                        type = (results[0] as DllProbeResult)?.TypeInfo;
                    }
                }
            }

            if (type == null)
            {
                Console.WriteLine($"Type '{typeName}' not found.");
                return;
            }

            var result = recommender.RecommendForType(type);
            result.PrintDetails();
        }
    }
}
