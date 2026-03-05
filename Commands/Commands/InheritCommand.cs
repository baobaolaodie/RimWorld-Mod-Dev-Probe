using System;
using System.Linq;
using RimWorldModDevProbe.Core;
using RimWorldModDevProbe.Probes;

namespace RimWorldModDevProbe.Commands
{
    public class InheritCommand : CommandBase
    {
        public InheritCommand(ProbeContext context, ServiceContainer services) 
            : base(context, services)
        {
        }

        public override string Name => "inherit";

        public override string Description => "显示类型的继承链";

        public override void Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: inherit <typename>");
                return;
            }

            var typeName = string.Join(" ", args);
            ShowInheritance(typeName);
        }

        private void ShowInheritance(string typeName)
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

            var options = new SearchOptions { ExactMatch = true };
            var typeResult = dllProbe.Search(typeName, options).FirstOrDefault() as DllProbeResult;

            if (typeResult == null)
            {
                options.ExactMatch = false;
                typeResult = dllProbe.Search(typeName, options).FirstOrDefault() as DllProbeResult;
            }

            if (typeResult == null)
            {
                Console.WriteLine($"Type '{typeName}' not found.");
                return;
            }

            var chain = dllProbe.GetInheritanceChain(typeResult.TypeInfo);
            Console.WriteLine($"\nInheritance chain for {typeName}:");
            for (int i = 0; i < chain.Count; i++)
            {
                var indent = new string(' ', i * 2);
                var arrow = i < chain.Count - 1 ? " →" : "";
                Console.WriteLine($"{indent}{chain[i]}{arrow}");
            }
        }
    }
}
