using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Commands
{
    public class CommandRegistry
    {
        private readonly Dictionary<string, ICommand> _commands = new Dictionary<string, ICommand>(StringComparer.OrdinalIgnoreCase);
        private readonly ProbeContext _context;
        private readonly ServiceContainer _services;

        public CommandRegistry(ProbeContext context, ServiceContainer services)
        {
            _context = context;
            _services = services;
        }

        public void RegisterCommand(ICommand command)
        {
            if (command == null)
                throw new ArgumentNullException(nameof(command));

            if (string.IsNullOrWhiteSpace(command.Name))
                throw new ArgumentException("Command name cannot be null or empty.");

            _commands[command.Name.ToLowerInvariant()] = command;
        }

        public void RegisterCommandsFromAssembly(Assembly assembly)
        {
            var commandTypes = assembly.GetTypes()
                .Where(t => typeof(ICommand).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

            foreach (var type in commandTypes)
            {
                try
                {
                    ICommand command = null;

                    if (typeof(CommandBase).IsAssignableFrom(type))
                    {
                        var constructor = type.GetConstructor(new[] { typeof(ProbeContext), typeof(ServiceContainer) });
                        if (constructor != null)
                        {
                            command = (ICommand)constructor.Invoke(new object[] { _context, _services });
                        }
                    }

                    if (command == null)
                    {
                        command = (ICommand)Activator.CreateInstance(type);
                    }

                    RegisterCommand(command);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to register command {type.Name}: {ex.Message}");
                }
            }
        }

        public bool TryGetCommand(string name, out ICommand command)
        {
            return _commands.TryGetValue(name, out command);
        }

        public IEnumerable<ICommand> GetAllCommands()
        {
            return _commands.Values;
        }

        public IEnumerable<string> GetCommandNames()
        {
            return _commands.Keys;
        }

        public bool ExecuteCommand(string name, string[] args)
        {
            if (TryGetCommand(name, out var command))
            {
                command.Execute(args);
                return true;
            }
            return false;
        }

        public void Clear()
        {
            _commands.Clear();
        }
    }
}
