using System;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Commands
{
    public class HelpCommand : CommandBase
    {
        public HelpCommand(ProbeContext context, ServiceContainer services) 
            : base(context, services)
        {
        }

        public override string Name => "help";

        public override string Description => "显示帮助信息";

        public override void Execute(string[] args)
        {
            PrintHelp();
        }

        private void PrintHelp()
        {
            Console.WriteLine("\nCommands:");
            Console.WriteLine("  dll [query]     - Switch to DLL mode / search types");
            Console.WriteLine("  def [query]     - Switch to Def mode / search defs");
            Console.WriteLine("  patch [query]   - Switch to Patch mode / search patches");
            Console.WriteLine("  harmony [query] - Switch to Harmony mode / search patches");
            Console.WriteLine("  mod [query]     - Switch to Mod mode / search mods");
            Console.WriteLine("  type <query>    - Search types by name");
            Console.WriteLine("  method <query>  - Search methods by name");
            Console.WriteLine("  field <query>   - Search fields by name");
            Console.WriteLine("  inherit <type>  - Show inheritance chain");
            Console.WriteLine("  types           - List all DefTypes");
            Console.WriteLine("  mods            - List all Mods");
            Console.WriteLine("");
            Console.WriteLine("Advanced Commands:");
            Console.WriteLine("  calls <method>  - Analyze call chain (callers/callees)");
            Console.WriteLine("  usage <field>   - Analyze field usage (read/write positions)");
            Console.WriteLine("  xml <type>      - Show XML structure for Def type");
            Console.WriteLine("  feature <key>   - Search features by keyword");
            Console.WriteLine("  recommend <key> - Get Patch recommendations for feature");
            Console.WriteLine("  relate <type>   - Show related resources for type");
            Console.WriteLine("");
            Console.WriteLine("Wizard Commands:");
            Console.WriteLine("  wizard          - Start development wizard");
            Console.WriteLine("                    - [1] Harmony Patch Wizard");
            Console.WriteLine("                    - [2] Sound Mod Wizard");
            Console.WriteLine("                    - [3] Weapon Mod Wizard");
            Console.WriteLine("                    - [4] Building Mod Wizard");
            Console.WriteLine("                    - [5] Race Mod Wizard");
            Console.WriteLine("                    - [6] XML Patch Wizard");
            Console.WriteLine("");
            Console.WriteLine("Example Commands:");
            Console.WriteLine("  example         - Show all examples by category");
            Console.WriteLine("  example <key>   - Show specific example details");
            Console.WriteLine("  example xml     - Show XML Def examples");
            Console.WriteLine("  example patch   - Show XML Patch examples");
            Console.WriteLine("  example harmony - Show Harmony Patch examples");
            Console.WriteLine("  example mod     - Show complex Mod examples");
            Console.WriteLine("");
            Console.WriteLine("General Commands:");
            Console.WriteLine("  clear           - Clear all caches");
            Console.WriteLine("  info            - Show system info");
            Console.WriteLine("  help / ?        - Show this help");
            Console.WriteLine("  exit / quit     - Exit program");
            Console.WriteLine("\nSearch tips:");
            Console.WriteLine("  - Enter search term to search in current mode");
            Console.WriteLine("  - Use 'method Death' to find all methods containing 'Death'");
            Console.WriteLine("  - Use 'field Sound' to find all fields containing 'Sound'");
            Console.WriteLine("  - Use 'feature 音效' to find features related to sound");
            Console.WriteLine("  - Use 'recommend 死亡音效' to get patch recommendations");
        }
    }
}
