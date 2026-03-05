using System;
using System.Collections.Generic;
using System.Linq;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Probes
{
    public class ModProbeResult : ProbeResult
    {
        public ModInfo ModInfo { get; }
        public bool Detailed { get; }

        public ModProbeResult(ModInfo info, bool detailed = false)
        {
            ModInfo = info;
            Detailed = detailed;
            Id = info.PackageId ?? info.Name;
            Name = info.Name;
            Type = "Mod";
            Source = info.Author ?? "Unknown";
            Location = info.DirectoryPath;
        }

        public override void PrintDetails()
        {
            Console.WriteLine($"\n--- Mod: {ModInfo.Name} ---");
            Console.WriteLine($"PackageId: {ModInfo.PackageId}");
            Console.WriteLine($"Author: {ModInfo.Author}");
            if (!string.IsNullOrEmpty(ModInfo.Version))
            {
                Console.WriteLine($"Version: {ModInfo.Version}");
            }
            Console.WriteLine($"Path: {ModInfo.DirectoryPath}");

            if (Detailed)
            {
                Console.WriteLine($"\nResources:");
                Console.WriteLine($"  DLLs: {ModInfo.DllFiles.Count}");
                if (ModInfo.DllFiles.Count > 0 && ModInfo.DllFiles.Count <= 10)
                {
                    foreach (var dll in ModInfo.DllFiles)
                    {
                        Console.WriteLine($"    - {dll}");
                    }
                }
                Console.WriteLine($"  Defs: {ModInfo.DefCount} files");
                if (ModInfo.DefFiles.Count > 0 && ModInfo.DefFiles.Count <= 10)
                {
                    foreach (var def in ModInfo.DefFiles)
                    {
                        Console.WriteLine($"    - {def}");
                    }
                }
                Console.WriteLine($"  Patches: {ModInfo.PatchCount} files");
                if (ModInfo.PatchFiles.Count > 0 && ModInfo.PatchFiles.Count <= 10)
                {
                    foreach (var patch in ModInfo.PatchFiles)
                    {
                        Console.WriteLine($"    - {patch}");
                    }
                }
            }
        }
    }
}
