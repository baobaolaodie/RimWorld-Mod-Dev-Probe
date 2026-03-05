using System;
using System.Linq;
using System.Reflection;
using RimWorldModDevProbe.Analysis;
using RimWorldModDevProbe.Core;
using RimWorldModDevProbe.Probes;

namespace RimWorldModDevProbe.Commands
{
    public class CallsCommand : CommandBase
    {
        public CallsCommand(ProbeContext context, ServiceContainer services) 
            : base(context, services)
        {
        }

        public override string Name => "calls";

        public override string Description => "分析方法的调用链";

        public override void Execute(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: calls <method>");
                return;
            }

            var methodName = string.Join(" ", args);
            AnalyzeCallChain(methodName);
        }

        private void AnalyzeCallChain(string methodName)
        {
            if (!_services.TryResolve<CallChainAnalyzer>(out var analyzer))
            {
                Console.WriteLine("CallChainAnalyzer not available.");
                return;
            }

            if (!_services.TryResolve<IProbe>("dll", out var probe))
            {
                Console.WriteLine("DLL probe not available.");
                return;
            }
            var dllProbe = probe as DllProbe;
            if (dllProbe == null)
            {
                Console.WriteLine("DLL probe not available.");
                return;
            }

            var methods = dllProbe.SearchMethods(methodName, new SearchOptions()).ToList();
            if (methods.Count == 0)
            {
                Console.WriteLine($"No methods found matching '{methodName}'.");
                return;
            }

            MethodInfo targetMethod = null;
            if (methods.Count == 1)
            {
                targetMethod = methods[0].MethodInfo;
            }
            else
            {
                Console.WriteLine($"\nFound {methods.Count} methods:");
                for (int i = 0; i < Math.Min(methods.Count, 20); i++)
                {
                    Console.WriteLine($"  [{i}] {methods[i].Name} - {methods[i].Source}");
                }

                Console.Write("\nSelect method to analyze (or press Enter to skip): ");
                var selection = Console.ReadLine();
                if (int.TryParse(selection, out var index) && index >= 0 && index < methods.Count)
                {
                    targetMethod = methods[index].MethodInfo;
                }
            }

            if (targetMethod == null)
            {
                Console.WriteLine("No method selected.");
                return;
            }

            Console.WriteLine($"\n=== Call Chain Analysis: {targetMethod.Name} ===");

            Console.WriteLine("\n--- Callers (methods that call this method) ---");
            var callers = analyzer.GetCallers(targetMethod);
            if (callers.Count == 0)
            {
                Console.WriteLine("  No callers found.");
            }
            else
            {
                foreach (var caller in callers.Take(20))
                {
                    Console.WriteLine($"  {caller.MethodInfo.DeclaringType.Name}.{caller.MethodInfo.Name}");
                }
                if (callers.Count > 20)
                {
                    Console.WriteLine($"  ... and {callers.Count - 20} more.");
                }
            }

            Console.WriteLine("\n--- Callees (methods called by this method) ---");
            var callees = analyzer.GetCallees(targetMethod);
            if (callees.Count == 0)
            {
                Console.WriteLine("  No callees found.");
            }
            else
            {
                foreach (var callee in callees.Take(20))
                {
                    Console.WriteLine($"  {callee.MethodInfo.DeclaringType.Name}.{callee.MethodInfo.Name}");
                }
                if (callees.Count > 20)
                {
                    Console.WriteLine($"  ... and {callees.Count - 20} more.");
                }
            }
        }
    }
}
