using System;
using System.Collections.Generic;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Commands
{
    public class ModeCommand : CommandBase
    {
        public ModeCommand(ProbeContext context, ServiceContainer services) 
            : base(context, services)
        {
        }

        public override string Name => "mode";

        public override string Description => "切换搜索模式";

        public override void Execute(string[] args)
        {
            var currentMode = _services.Resolve<CurrentMode>();
            
            if (args.Length == 0)
            {
                Console.WriteLine($"Current mode: {currentMode.Mode}");
                Console.WriteLine("Available modes: dll, def, patch, harmony, mod");
                return;
            }

            var mode = args[0].ToLowerInvariant();
            SwitchMode(mode, args.Length > 1 ? string.Join(" ", args, 1, args.Length - 1) : null);
        }

        private void SwitchMode(string mode, string query)
        {
            if (!_services.IsRegistered<IProbe>(mode))
            {
                Console.WriteLine($"Unknown mode: {mode}");
                Console.WriteLine("Available modes: dll, def, patch, harmony, mod");
                return;
            }

            var currentMode = _services.Resolve<CurrentMode>();
            currentMode.Mode = mode;
            Console.WriteLine($"Switched to {mode} mode.");

            if (!string.IsNullOrEmpty(query))
            {
                SearchCurrentMode(query);
            }
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
            var results = probe.Search(query, options);

            var resultList = new List<ProbeResult>();
            foreach (var result in results)
            {
                resultList.Add(result);
                if (resultList.Count >= 20) break;
            }

            if (resultList.Count == 0)
            {
                Console.WriteLine("No results found.");
                return;
            }

            Console.WriteLine($"\nFound {resultList.Count} results:");
            for (int i = 0; i < resultList.Count; i++)
            {
                var r = resultList[i];
                Console.WriteLine($"  [{i}] {r.Name} ({r.Type}) - {r.Source}");
            }

            Console.Write("\nEnter number to view details (or press Enter to skip): ");
            var selection = Console.ReadLine();
            if (int.TryParse(selection, out var index) && index >= 0 && index < resultList.Count)
            {
                var detailed = probe.GetDetails(resultList[index].Id);
                detailed?.PrintDetails();
            }
        }
    }
}
