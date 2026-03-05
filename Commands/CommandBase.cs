using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Commands
{
    public abstract class CommandBase : ICommand
    {
        protected readonly ProbeContext _context;
        protected readonly ServiceContainer _services;

        public CommandBase(ProbeContext context, ServiceContainer services)
        {
            _context = context;
            _services = services;
        }

        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract void Execute(string[] args);
    }
}
