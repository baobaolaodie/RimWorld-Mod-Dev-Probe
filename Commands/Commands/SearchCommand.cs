using System;
using System.Linq;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Commands
{
    public class SearchCommand : CommandBase
    {
        public SearchCommand(ProbeContext context, ServiceContainer services) 
            : base(context, services)
        {
        }

        public override string Name => "search";

        public override string Description => "在当前模式下搜索";

        public override void Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: search <query>");
                return;
            }

            var query = string.Join(" ", args);
            SearchCurrentMode(query);
        }

        private void SearchCurrentMode(string query)
        {
            var currentMode = _services.Resolve<CurrentMode>();
            
            if (!_services.TryResolve<IProbe>(currentMode.Mode, out var probe))
            {
                Console.WriteLine($"Unknown mode: {currentMode.Mode}");
                return;
            }

            var options = new SearchOptions();
            var results = probe.Search(query, options).ToList();

            if (results.Count == 0)
            {
                Console.WriteLine("No results found.");
                return;
            }

            if (results.Count == 1)
            {
                results[0].PrintDetails();
                return;
            }

            Console.WriteLine($"\nFound {results.Count} results:");
            for (int i = 0; i < Math.Min(results.Count, 20); i++)
            {
                var r = results[i];
                Console.WriteLine($"  [{i}] {r.Name} ({r.Type}) - {r.Source}");
            }

            if (results.Count > 20)
            {
                Console.WriteLine($"  ... and {results.Count - 20} more.");
            }

            Console.Write("\nEnter number to view details (or press Enter to skip): ");
            var selection = Console.ReadLine();
            if (int.TryParse(selection, out var index) && index >= 0 && index < results.Count)
            {
                var detailed = probe.GetDetails(results[index].Id);
                detailed?.PrintDetails();
            }
        }
    }
}
