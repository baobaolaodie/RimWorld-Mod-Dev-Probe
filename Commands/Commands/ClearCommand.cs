using System;
using System.Collections.Generic;
using RimWorldModDevProbe.Analysis;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Commands
{
    public class ClearCommand : CommandBase
    {
        public ClearCommand(ProbeContext context, ServiceContainer services) 
            : base(context, services)
        {
        }

        public override string Name => "clear";

        public override string Description => "清除所有缓存";

        public override void Execute(string[] args)
        {
            ClearCache();
        }

        private void ClearCache()
        {
            var probeNames = new[] { "dll", "def", "patch", "harmony", "mod" };
            
            foreach (var name in probeNames)
            {
                if (_services.TryResolve<IProbe>(name, out var probe))
                {
                    probe.ClearCache();
                }
            }

            _context.ClearCache();

            if (_services.TryResolve<CallChainAnalyzer>(out var analyzer))
            {
                analyzer.ClearCache();
            }

            Console.WriteLine("All caches cleared.");
        }
    }
}
