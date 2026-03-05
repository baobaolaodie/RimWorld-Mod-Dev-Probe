using System;
using System.Linq;
using RimWorldModDevProbe.Core;
using RimWorldModDevProbe.Probes;

namespace RimWorldModDevProbe.Commands
{
    public class TypeCommand : CommandBase
    {
        public TypeCommand(ProbeContext context, ServiceContainer services) 
            : base(context, services)
        {
        }

        public override string Name => "type";

        public override string Description => "搜索类型";

        public override void Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: type <query>");
                return;
            }

            var query = string.Join(" ", args);
            SearchTypes(query);
        }

        private void SearchTypes(string query)
        {
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

            var options = new SearchOptions();
            var results = dllProbe.Search(query, options).ToList();

            if (results.Count == 0)
            {
                Console.WriteLine("No types found.");
                return;
            }

            if (results.Count == 1)
            {
                results[0].PrintDetails();
                return;
            }

            Console.WriteLine($"\nFound {results.Count} types:");
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
                var detailed = dllProbe.GetDetails(results[index].Id);
                detailed?.PrintDetails();
            }
        }
    }
}
