using System;
using RimWorldModDevProbe.Core;
using RimWorldModDevProbe.Probes;

namespace RimWorldModDevProbe
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = new ProbeContext();
            var router = new CommandRouter(context);
            
            router.RegisterProbe(new DllProbe());
            router.RegisterProbe(new DefsProbe());
            router.RegisterProbe(new PatchProbe());
            router.RegisterProbe(new HarmonyProbe());
            router.RegisterProbe(new ModProbe());
            
            router.Initialize();
            router.Run(args);
        }
    }
}
