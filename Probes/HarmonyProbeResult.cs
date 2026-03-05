using System;
using System.Collections.Generic;
using System.Linq;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Probes
{
    public class HarmonyProbeResult : ProbeResult
    {
        public HarmonyPatchInfo PatchInfo { get; }
        public bool Detailed { get; }

        public HarmonyProbeResult(HarmonyPatchInfo info, bool detailed = false)
        {
            PatchInfo = info;
            Detailed = detailed;
            Id = info.PatchClassName;
            Name = info.PatchClassName.Split('.').Last();
            Type = info.PatchType.ToString();
            Source = info.AssemblyName;
            Location = info.TargetType ?? "";
        }

        public override void PrintDetails()
        {
            Console.WriteLine($"\n--- Harmony Patch: {PatchInfo.PatchClassName} ---");
            Console.WriteLine($"Assembly: {PatchInfo.AssemblyName}");
            Console.WriteLine($"Patch Type: {PatchInfo.PatchType}");
            if (!string.IsNullOrEmpty(PatchInfo.TargetType))
            {
                Console.WriteLine($"Target Type: {PatchInfo.TargetType}");
            }
            if (!string.IsNullOrEmpty(PatchInfo.TargetMethod))
            {
                Console.WriteLine($"Target Method: {PatchInfo.TargetMethod}");
            }
            if (!string.IsNullOrEmpty(PatchInfo.Priority))
            {
                Console.WriteLine($"Priority: {PatchInfo.Priority}");
            }
        }
    }
}
