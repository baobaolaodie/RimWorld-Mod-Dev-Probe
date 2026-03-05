using System;
using System.Reflection;
using RimWorldModDevProbe.Commands;
using RimWorldModDevProbe.Core;
using RimWorldModDevProbe.Analysis;
using RimWorldModDevProbe.Probes;

namespace RimWorldModDevProbe
{
    public class CommandRouter
    {
        private readonly ProbeContext _context;
        private readonly ServiceContainer _services;
        private readonly CommandRegistry _registry;
        private readonly CurrentMode _currentMode;

        public CommandRouter(ProbeContext context)
        {
            _context = context;
            _services = new ServiceContainer();
            _registry = new CommandRegistry(_context, _services);
            _currentMode = new CurrentMode();
        }

        public void RegisterProbe(IProbe probe)
        {
            probe.Initialize(_context);
            _services.RegisterSingleton(probe.Name, probe);
        }

        public void Initialize()
        {
            InitializeServices();
            RegisterCommands();
        }

        private void InitializeServices()
        {
            _services.RegisterSingleton(_currentMode);

            var typeDefMapper = new TypeDefMapper(_context);
            typeDefMapper.Initialize();
            _services.RegisterSingleton(typeDefMapper);

            var featureKeywordMap = new FeatureKeywordMap();
            featureKeywordMap.Initialize(_context);
            _services.RegisterSingleton(featureKeywordMap);

            var callChainAnalyzer = new CallChainAnalyzer(_context);
            _services.RegisterSingleton(callChainAnalyzer);

            var fieldUsageAnalyzer = new FieldUsageAnalyzer(_context);
            _services.RegisterSingleton(fieldUsageAnalyzer);

            var exampleLibrary = new ExampleLibrary(_context);
            _services.RegisterSingleton(exampleLibrary);

            if (_services.TryResolve<IProbe>("def", out var defProbe) && defProbe is DefsProbe defsProbe)
            {
                var resourceRecommender = new ResourceRecommender(_context, defsProbe, typeDefMapper);
                _services.RegisterSingleton(resourceRecommender);
            }

            var patchRecommender = new PatchRecommender(_context);
            _services.RegisterSingleton(patchRecommender);

            var harmonyPatchWizard = new HarmonyPatchWizard(_context);
            _services.RegisterSingleton(harmonyPatchWizard);

            var soundModWizard = new SoundModWizard(_context);
            _services.RegisterSingleton(soundModWizard);

            var weaponModWizard = new WeaponModWizard(_context);
            _services.RegisterSingleton(weaponModWizard);

            var buildingModWizard = new BuildingModWizard(_context);
            _services.RegisterSingleton(buildingModWizard);

            var raceModWizard = new RaceModWizard(_context);
            _services.RegisterSingleton(raceModWizard);

            var xmlPatchWizard = new XmlPatchWizard(_context);
            _services.RegisterSingleton(xmlPatchWizard);
        }

        private void RegisterCommands()
        {
            _registry.RegisterCommandsFromAssembly(Assembly.GetExecutingAssembly());
        }

        public void Run(string[] args)
        {
            if (args.Length > 0)
            {
                ProcessBatchCommands(args);
                return;
            }

            PrintWelcome();
            _registry.ExecuteCommand("help", Array.Empty<string>());

            while (true)
            {
                Console.Write($"\n[{_currentMode.Mode}]> ");
                var input = Console.ReadLine()?.Trim();

                if (string.IsNullOrEmpty(input)) continue;
                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                    input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }

                ProcessCommand(input);
            }
        }

        private void ProcessBatchCommands(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                var cmd = args[i].ToLowerInvariant();
                string[] cmdArgs = Array.Empty<string>();

                if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                {
                    cmdArgs = new[] { args[i + 1] };
                    i++;
                }

                ProcessCommand(cmd, cmdArgs);
            }
        }

        private void ProcessCommand(string input)
        {
            var parts = input.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            var cmd = parts[0].ToLowerInvariant();
            var args = parts.Length > 1 ? new[] { parts[1] } : Array.Empty<string>();

            ProcessCommand(cmd, args);
        }

        private void ProcessCommand(string cmd, string[] args)
        {
            if (IsModeSwitchCommand(cmd))
            {
                HandleModeSwitch(cmd, args);
                return;
            }

            if (_registry.ExecuteCommand(cmd, args))
            {
                return;
            }

            HandleDefaultSearch(cmd);
        }

        private bool IsModeSwitchCommand(string cmd)
        {
            return cmd == "dll" || cmd == "def" || cmd == "patch" || 
                   cmd == "harmony" || cmd == "mod";
        }

        private void HandleModeSwitch(string cmd, string[] args)
        {
            _currentMode.Mode = cmd;
            Console.WriteLine($"Switched to {cmd} mode.");

            if (args.Length > 0 && !string.IsNullOrEmpty(args[0]))
            {
                _registry.ExecuteCommand("search", new[] { args[0] });
            }
        }

        private void HandleDefaultSearch(string query)
        {
            if (_services.TryResolve<IProbe>(_currentMode.Mode, out var probe))
            {
                _registry.ExecuteCommand("search", new[] { query });
            }
            else
            {
                Console.WriteLine($"Unknown command: {query}");
            }
        }

        private void PrintWelcome()
        {
            Console.WriteLine("========================================");
            Console.WriteLine("  RimWorld Mod Dev Probe v2.1.0");
            Console.WriteLine("  Multi-resource exploration tool");
            Console.WriteLine("========================================");
        }
    }
}
