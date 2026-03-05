using System;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Commands
{
    public class WizardCommand : CommandBase
    {
        public WizardCommand(ProbeContext context, ServiceContainer services) 
            : base(context, services)
        {
        }

        public override string Name => "wizard";

        public override string Description => "启动开发向导";

        public override void Execute(string[] args)
        {
            StartWizard();
        }

        private void StartWizard()
        {
            Console.WriteLine("\n=== Development Wizard ===");
            Console.WriteLine("Available wizards:");
            Console.WriteLine("  [1] Harmony Patch Wizard");
            Console.WriteLine("  [2] Sound Mod Wizard");
            Console.WriteLine("  [3] Weapon Mod Wizard");
            Console.WriteLine("  [4] Building Mod Wizard");
            Console.WriteLine("  [5] Race Mod Wizard");
            Console.WriteLine("  [6] XML Patch Wizard");
            Console.Write("\nSelect wizard (or press Enter to cancel): ");

            var selection = Console.ReadLine();
            switch (selection)
            {
                case "1":
                    RunHarmonyPatchWizard();
                    break;
                case "2":
                    RunSoundModWizard();
                    break;
                case "3":
                    RunWeaponModWizard();
                    break;
                case "4":
                    RunBuildingModWizard();
                    break;
                case "5":
                    RunRaceModWizard();
                    break;
                case "6":
                    RunXmlPatchWizard();
                    break;
                default:
                    Console.WriteLine("Wizard cancelled.");
                    break;
            }
        }

        private void RunHarmonyPatchWizard()
        {
            if (_services.TryResolve<HarmonyPatchWizard>(out var wizard))
            {
                var result = wizard.Run();
                result.PrintSummary();
            }
            else
            {
                Console.WriteLine("HarmonyPatchWizard not available.");
            }
        }

        private void RunSoundModWizard()
        {
            if (_services.TryResolve<SoundModWizard>(out var wizard))
            {
                var result = wizard.Run();
                result.PrintSummary();
            }
            else
            {
                Console.WriteLine("SoundModWizard not available.");
            }
        }

        private void RunWeaponModWizard()
        {
            if (_services.TryResolve<WeaponModWizard>(out var wizard))
            {
                var result = wizard.Run();
                result.PrintSummary();
            }
            else
            {
                Console.WriteLine("WeaponModWizard not available.");
            }
        }

        private void RunBuildingModWizard()
        {
            if (_services.TryResolve<BuildingModWizard>(out var wizard))
            {
                var result = wizard.Run();
                result.PrintSummary();
            }
            else
            {
                Console.WriteLine("BuildingModWizard not available.");
            }
        }

        private void RunRaceModWizard()
        {
            if (_services.TryResolve<RaceModWizard>(out var wizard))
            {
                var result = wizard.Run();
                result.PrintSummary();
            }
            else
            {
                Console.WriteLine("RaceModWizard not available.");
            }
        }

        private void RunXmlPatchWizard()
        {
            if (_services.TryResolve<XmlPatchWizard>(out var wizard))
            {
                var result = wizard.Run();
                result.PrintSummary();
            }
            else
            {
                Console.WriteLine("XmlPatchWizard not available.");
            }
        }
    }
}
