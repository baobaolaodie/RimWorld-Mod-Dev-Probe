using System;
using System.Linq;
using RimWorldModDevProbe.Core;
using RimWorldModDevProbe.Probes;

namespace RimWorldModDevProbe.Commands
{
    public class TypesCommand : CommandBase
    {
        public TypesCommand(ProbeContext context, ServiceContainer services) 
            : base(context, services)
        {
        }

        public override string Name => "types";

        public override string Description => "列出所有 DefTypes";

        public override void Execute(string[] args)
        {
            if (_services.TryResolve<IProbe>("def", out var probe))
            {
                var defProbe = probe as DefsProbe;
                if (defProbe != null)
                {
                    var types = defProbe.GetDefTypes();
                    Console.WriteLine("\nDefTypes:");
                    foreach (var type in types)
                    {
                        Console.WriteLine($"  {type}");
                    }
                    return;
                }
            }
            Console.WriteLine("Defs probe not available.");
        }
    }
}
