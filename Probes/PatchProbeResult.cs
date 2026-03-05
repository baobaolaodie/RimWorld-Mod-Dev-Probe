using System;
using System.Collections.Generic;
using System.Linq;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Probes
{
    public class PatchProbeResult : ProbeResult
    {
        public PatchInfo PatchInfo { get; }
        public bool Detailed { get; }

        public PatchProbeResult(PatchInfo info, bool detailed = false)
        {
            PatchInfo = info;
            Detailed = detailed;
            Id = $"{info.FileName}:{info.OperationType}";
            Name = info.TargetDef ?? info.OperationType;
            Type = info.OperationType;
            Source = info.FileName;
            Location = info.FilePath;
        }

        public override void PrintDetails()
        {
            Console.WriteLine($"\n{'='} Patch: {PatchInfo.OperationType} {'='}");
            Console.WriteLine($"File: {PatchInfo.FileName}");
            Console.WriteLine($"Path: {PatchInfo.FilePath}");
            if (!string.IsNullOrEmpty(PatchInfo.TargetDef))
            {
                Console.WriteLine($"Target Def: {PatchInfo.TargetDef}");
            }
            Console.WriteLine($"XPath: {PatchInfo.XPath}");
            if (!string.IsNullOrEmpty(PatchInfo.Order))
            {
                Console.WriteLine($"Order: {PatchInfo.Order}");
            }
            if (!string.IsNullOrEmpty(PatchInfo.Priority))
            {
                Console.WriteLine($"Priority: {PatchInfo.Priority}");
            }

            if (Detailed && !string.IsNullOrEmpty(PatchInfo.Value))
            {
                Console.WriteLine("\n--- Value ---");
                Console.WriteLine(PatchInfo.Value);
            }
        }
    }
}
