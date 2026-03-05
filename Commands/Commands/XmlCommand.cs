using System;
using System.Linq;
using RimWorldModDevProbe.Analysis;
using RimWorldModDevProbe.Core;
using RimWorldModDevProbe.Probes;

namespace RimWorldModDevProbe.Commands
{
    public class XmlCommand : CommandBase
    {
        public XmlCommand(ProbeContext context, ServiceContainer services) 
            : base(context, services)
        {
        }

        public override string Name => "xml";

        public override string Description => "显示类型的XML结构";

        public override void Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: xml <type>");
                return;
            }

            var typeName = string.Join(" ", args);
            ShowXmlStructure(typeName);
        }

        private void ShowXmlStructure(string typeName)
        {
            if (!_services.TryResolve<TypeDefMapper>(out var typeDefMapper))
            {
                Console.WriteLine("TypeDefMapper not available.");
                return;
            }

            var type = typeDefMapper.GetTypeByDefName(typeName);
            if (type == null)
            {
                if (_services.TryResolve<IProbe>("dll", out var probe) && probe is DllProbe dllProbe)
                {
                    var results = dllProbe.Search(typeName, new SearchOptions()).ToList();
                    if (results.Count > 0)
                    {
                        type = (results[0] as DllProbeResult)?.TypeInfo;
                    }
                }
            }

            if (type == null)
            {
                Console.WriteLine($"Type '{typeName}' not found.");
                return;
            }

            Console.WriteLine($"\n=== XML Structure for {type.Name} ===");
            Console.WriteLine($"C# Type: {type.FullName}");
            Console.WriteLine($"Def Type: {typeDefMapper.GetDefType(type) ?? "N/A"}");

            Console.WriteLine("\n--- XML Template ---");
            Console.WriteLine(typeDefMapper.GetXmlStructure(type));

            Console.WriteLine("\n--- Field Mappings ---");
            var mappings = typeDefMapper.GetFieldMappings(type);
            foreach (var mapping in mappings.Take(20))
            {
                Console.WriteLine($"  {mapping.Key} -> {mapping.Value}");
            }
            if (mappings.Count > 20)
            {
                Console.WriteLine($"  ... and {mappings.Count - 20} more.");
            }
        }
    }
}
