using System;
using RimWorldModDevProbe.Core;
using RimWorldModDevProbe.Probes;

namespace RimWorldModDevProbe.Commands
{
    public class ModsCommand : CommandBase
    {
        public ModsCommand(ProbeContext context, ServiceContainer services) 
            : base(context, services)
        {
        }

        public override string Name => "mods";

        public override string Description => "列出所有 Mods";

        public override void Execute(string[] args)
        {
            if (_services.TryResolve<IProbe>("mod", out var probe))
            {
                var modProbe = probe as ModProbe;
                if (modProbe != null)
                {
                    var mods = modProbe.GetAllMods();
                    Console.WriteLine("\nMods:");
                    foreach (var mod in mods)
                    {
                        Console.WriteLine($"  {mod.Name} ({mod.PackageId})");
                        if (mod.DllFiles.Count > 0) Console.WriteLine($"    DLLs: {mod.DllFiles.Count}");
                        if (mod.DefCount > 0) Console.WriteLine($"    Defs: {mod.DefCount}");
                        if (mod.PatchCount > 0) Console.WriteLine($"    Patches: {mod.PatchCount}");
                    }
                    return;
                }
            }
            Console.WriteLine("Mod probe not available.");
        }
    }
}
