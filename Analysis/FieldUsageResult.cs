using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Analysis
{
    public class FieldUsageResult : ProbeResult
    {
        public FieldInfo FieldInfo { get; }
        public List<FieldUsageLocation> ReadPositions { get; } = new List<FieldUsageLocation>();
        public List<FieldUsageLocation> WritePositions { get; } = new List<FieldUsageLocation>();
        public List<FieldUsageLocation> AllUsages { get; } = new List<FieldUsageLocation>();
        public int TotalReadCount => ReadPositions.Count;
        public int TotalWriteCount => WritePositions.Count;
        public int TotalUsageCount => AllUsages.Count;

        public FieldUsageResult(FieldInfo field)
        {
            FieldInfo = field;
            Id = $"{field.DeclaringType.FullName}.{field.Name}";
            Name = field.Name;
            Type = "FieldUsage";
            Source = field.DeclaringType.Assembly.GetName().Name;
            Location = field.DeclaringType.FullName;
        }

        public override void PrintDetails()
        {
            Console.WriteLine($"\n=== Field Usage Analysis: {FieldInfo.Name} ===");
            Console.WriteLine($"Declaring Type: {FieldInfo.DeclaringType.FullName}");
            Console.WriteLine($"Field Type: {FieldInfo.FieldType.Name}");
            var visibility = FieldInfo.IsPublic ? "public" : FieldInfo.IsPrivate ? "private" : FieldInfo.IsFamily ? "protected" : "internal";
            var staticStr = FieldInfo.IsStatic ? "static " : "";
            Console.WriteLine($"Signature: [{visibility}] {staticStr}{FieldInfo.FieldType.Name} {FieldInfo.Name}");

            Console.WriteLine($"\n--- Summary ---");
            Console.WriteLine($"Total Read: {TotalReadCount}");
            Console.WriteLine($"Total Write: {TotalWriteCount}");
            Console.WriteLine($"Total Usages: {TotalUsageCount}");

            if (ReadPositions.Count > 0)
            {
                Console.WriteLine($"\n--- Read Positions ({ReadPositions.Count}) ---");
                var groupedByType = ReadPositions.GroupBy(p => p.TypeName);
                foreach (var group in groupedByType)
                {
                    Console.WriteLine($"  {group.Key}:");
                    foreach (var pos in group)
                    {
                        Console.WriteLine($"    {pos.MethodName} @ IL_{pos.ILOffset:X4}");
                    }
                }
            }

            if (WritePositions.Count > 0)
            {
                Console.WriteLine($"\n--- Write Positions ({WritePositions.Count}) ---");
                var groupedByType = WritePositions.GroupBy(p => p.TypeName);
                foreach (var group in groupedByType)
                {
                    Console.WriteLine($"  {group.Key}:");
                    foreach (var pos in group)
                    {
                        Console.WriteLine($"    {pos.MethodName} @ IL_{pos.ILOffset:X4}");
                    }
                }
            }
        }
    }
}
