using System;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Commands
{
    public class InfoCommand : CommandBase
    {
        public InfoCommand(ProbeContext context, ServiceContainer services) 
            : base(context, services)
        {
        }

        public override string Name => "info";

        public override string Description => "显示系统信息";

        public override void Execute(string[] args)
        {
            PrintInfo();
        }

        private void PrintInfo()
        {
            Console.WriteLine("\nSystem Info:");
            Console.WriteLine($"  GameDll Path: {_context.GameDllPath ?? "Not found"}");
            Console.WriteLine($"  GameData Path: {_context.GameDataPath ?? "Not found"}");
            Console.WriteLine($"  Mods Path: {_context.ModsPath ?? "Not found"}");
            Console.WriteLine($"  Loaded Assemblies: {_context.LoadedAssemblies.Count}");
        }
    }
}
