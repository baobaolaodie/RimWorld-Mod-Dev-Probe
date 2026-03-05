using System;
using System.Linq;
using RimWorldModDevProbe.Core;
using RimWorldModDevProbe.Probes;

namespace RimWorldModDevProbe.Commands
{
    public class MethodCommand : CommandBase
    {
        public MethodCommand(ProbeContext context, ServiceContainer services) 
            : base(context, services)
        {
        }

        public override string Name => "method";

        public override string Description => "搜索方法";

        public override void Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: method <query>");
                return;
            }

            var query = string.Join(" ", args);
            SearchMethods(query);
        }

        private void SearchMethods(string query)
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
            var results = dllProbe.SearchMethods(query, options).ToList();

            if (results.Count == 0)
            {
                Console.WriteLine("No methods found.");
                return;
            }

            Console.WriteLine($"\nFound {results.Count} methods:");
            for (int i = 0; i < Math.Min(results.Count, 20); i++)
            {
                var r = results[i];
                Console.WriteLine($"  [{i}] {r.Name}() - {r.Source}");
            }

            if (results.Count > 20)
            {
                Console.WriteLine($"  ... and {results.Count - 20} more.");
            }

            Console.Write("\nEnter number to view details (or press Enter to skip): ");
            var selection = Console.ReadLine();
            if (int.TryParse(selection, out var index) && index >= 0 && index < results.Count)
            {
                results[index].PrintDetails();
            }
        }
    }
}
