using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Probes
{
    public class DefProbeResult : ProbeResult
    {
        public DefInfo DefInfo { get; }
        public bool Detailed { get; }

        public DefProbeResult(DefInfo info, bool detailed = false)
        {
            DefInfo = info;
            Detailed = detailed;
            Id = info.DefName;
            Name = info.DefName;
            Type = info.DefType;
            Source = info.FileName;
            Location = info.FilePath;
        }

        public override void PrintDetails()
        {
            Console.WriteLine($"\n{'='} {DefInfo.DefName} ({DefInfo.DefType}) {'='}");
            Console.WriteLine($"File: {DefInfo.FileName}");
            Console.WriteLine($"Path: {DefInfo.FilePath}");
            if (!string.IsNullOrEmpty(DefInfo.ParentName))
            {
                Console.WriteLine($"Parent: {DefInfo.ParentName}");
            }

            if (Detailed && DefInfo.Element != null)
            {
                Console.WriteLine("\n--- Fields ---");
                foreach (var child in DefInfo.Element.Elements())
                {
                    if (child.Name.LocalName == "defName") continue;
                    PrintElement(child, 0);
                }
            }
        }

        private void PrintElement(XElement element, int indent)
        {
            var prefix = new string(' ', indent * 2);
            var attrs = element.Attributes()
                .Where(a => a.Name.LocalName != "ParentName")
                .Select(a => $"{a.Name.LocalName}=\"{a.Value}\"");
            var attrStr = attrs.Any() ? $" [{string.Join(", ", attrs)}]" : "";

            if (!element.HasElements)
            {
                var value = element.Value.Trim();
                if (value.Contains("\n"))
                {
                    Console.WriteLine($"{prefix}{element.Name.LocalName}{attrStr}:");
                    foreach (var line in value.Split('\n'))
                    {
                        Console.WriteLine($"{prefix}  {line.Trim()}");
                    }
                }
                else
                {
                    Console.WriteLine($"{prefix}{element.Name.LocalName}{attrStr}: {value}");
                }
            }
            else if (element.Elements().All(e => e.Name.LocalName == "li"))
            {
                Console.WriteLine($"{prefix}{element.Name.LocalName}{attrStr}:");
                int index = 0;
                foreach (var li in element.Elements("li"))
                {
                    if (li.HasElements)
                    {
                        Console.WriteLine($"{prefix}  [{index}]");
                        foreach (var child in li.Elements())
                        {
                            PrintElement(child, indent + 2);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"{prefix}  [{index}] {li.Value}");
                    }
                    index++;
                }
            }
            else
            {
                Console.WriteLine($"{prefix}{element.Name.LocalName}{attrStr}:");
                foreach (var child in element.Elements())
                {
                    PrintElement(child, indent + 1);
                }
            }
        }
    }
}
