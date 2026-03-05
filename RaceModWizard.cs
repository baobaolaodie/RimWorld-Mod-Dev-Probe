using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorldModDevProbe.Core;
using RimWorldModDevProbe.Wizards.Core;

namespace RimWorldModDevProbe
{
    public enum RaceType
    {
        Humanlike,
        Animal,
        Mechanoid,
        Custom
    }

    public enum RaceIntelligence
    {
        ToolUser,
        Humanlike,
        Animal
    }

    public class RaceModConfig
    {
        public string ModName { get; set; } = "MyRaceMod";
        public string AuthorName { get; set; } = "YourName";
        public string Description { get; set; } = "Custom race mod";
        public string RaceDefName { get; set; } = "CustomRace";
        public string RaceLabelZh { get; set; } = "自定义种族";
        public string RaceLabelEn { get; set; } = "Custom Race";
        public string RaceDescriptionZh { get; set; } = "一个自定义种族";
        public string RaceDescriptionEn { get; set; } = "A custom race";
        public RaceType RaceType { get; set; } = RaceType.Humanlike;
        public RaceIntelligence Intelligence { get; set; } = RaceIntelligence.Humanlike;
        public float BaseHealthScale { get; set; } = 1.0f;
        public int BaseBodySize { get; set; } = 1;
        public float BaseHungerRate { get; set; } = 1.0f;
        public float BaseMoveSpeed { get; set; } = 4.6f;
        public int BaseMeleeDodgeChance { get; set; } = 10;
        public int BaseMeleeHitChance { get; set; } = 60;
        public List<string> BodyParts { get; set; } = new List<string>();
        public string BodyType { get; set; } = "Human";
        public List<SkillConfig> Skills { get; set; } = new List<SkillConfig>();
        public List<ApparelConfig> Apparels { get; set; } = new List<ApparelConfig>();
        public bool UseBilingual { get; set; } = true;
        public string PawnKindDefName { get; set; } = "CustomRacePawnKind";
        public string FactionDefName { get; set; } = "CustomRaceFaction";
        public string BackstoryCategory { get; set; } = "Civil";
    }

    public class SkillConfig
    {
        public string SkillName { get; set; }
        public int Level { get; set; }
        public string Passion { get; set; } = "None";
    }

    public class ApparelConfig
    {
        public string ApparelDefName { get; set; }
        public string LabelZh { get; set; }
        public string LabelEn { get; set; }
        public string DescriptionZh { get; set; }
        public string DescriptionEn { get; set; }
        public string Layer { get; set; } = "OnSkin";
        public List<string> BodyPartGroups { get; set; } = new List<string>();
        public float ArmorRating { get; set; }
        public float InsulationCold { get; set; }
        public float InsulationHeat { get; set; }
    }

    public class RaceTypeInfo
    {
        public RaceType Type { get; set; }
        public string NameZh { get; set; }
        public string NameEn { get; set; }
        public string DescriptionZh { get; set; }
        public string DescriptionEn { get; set; }
        public List<string> DefaultBodyParts { get; set; } = new List<string>();
        public string DefaultBodyType { get; set; }
        public List<string> DefaultApparelLayers { get; set; } = new List<string>();
    }

    public class RaceModWizard
    {
        private readonly ProbeContext _context;
        private readonly DevWizard _innerWizard;
        private readonly RaceModConfig _config;
        private readonly List<RaceTypeInfo> _raceTypes;
        private bool _useChinese = true;

        public RaceModWizard(ProbeContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _config = new RaceModConfig();
            _raceTypes = InitializeRaceTypes();
            _innerWizard = new DevWizard(context);

            SetupWizardSteps();
        }

        private List<RaceTypeInfo> InitializeRaceTypes()
        {
            return new List<RaceTypeInfo>
            {
                new RaceTypeInfo
                {
                    Type = RaceType.Humanlike,
                    NameZh = "类人生物",
                    NameEn = "Humanlike",
                    DescriptionZh = "类似人类的智能生物，可以使用武器、穿着装备、进行社交活动",
                    DescriptionEn = "Intelligent humanoid creatures that can use weapons, wear apparel, and engage in social activities",
                    DefaultBodyParts = new List<string> { "Head", "Torso", "LeftArm", "RightArm", "LeftLeg", "RightLeg" },
                    DefaultBodyType = "Human",
                    DefaultApparelLayers = new List<string> { "OnSkin", "MiddleLayer", "Shell", "Overhead", "Eyes" }
                },
                new RaceTypeInfo
                {
                    Type = RaceType.Animal,
                    NameZh = "动物",
                    NameEn = "Animal",
                    DescriptionZh = "普通动物，通常不能使用武器或穿着装备",
                    DescriptionEn = "Regular animals that typically cannot use weapons or wear apparel",
                    DefaultBodyParts = new List<string> { "Head", "Torso", "FrontLeftLeg", "FrontRightLeg", "RearLeftLeg", "RearRightLeg", "Tail" },
                    DefaultBodyType = "QuadrupedAnimal",
                    DefaultApparelLayers = new List<string>()
                },
                new RaceTypeInfo
                {
                    Type = RaceType.Mechanoid,
                    NameZh = "机械族",
                    NameEn = "Mechanoid",
                    DescriptionZh = "机械生物，通常不需要食物，具有特殊能力",
                    DescriptionEn = "Mechanical beings that typically don't need food and have special abilities",
                    DefaultBodyParts = new List<string> { "Head", "Torso", "LeftArm", "RightArm", "LeftLeg", "RightLeg" },
                    DefaultBodyType = "Human",
                    DefaultApparelLayers = new List<string>()
                },
                new RaceTypeInfo
                {
                    Type = RaceType.Custom,
                    NameZh = "自定义",
                    NameEn = "Custom",
                    DescriptionZh = "完全自定义的种族类型",
                    DescriptionEn = "Fully custom race type",
                    DefaultBodyParts = new List<string>(),
                    DefaultBodyType = "Human",
                    DefaultApparelLayers = new List<string>()
                }
            };
        }

        private void SetupWizardSteps()
        {
            _innerWizard.AddStep(new RaceWelcomeStep(this));
            _innerWizard.AddStep(new RaceLanguageSelectStep(this));
            _innerWizard.AddStep(new RaceInfoStep(this));
            _innerWizard.AddStep(new RaceTypeSelectStep(this));
            _innerWizard.AddStep(new RaceStatsStep(this));
            _innerWizard.AddStep(new RaceSkillsStep(this));
            _innerWizard.AddStep(new RaceApparelStep(this));
            _innerWizard.AddStep(new RaceCodeGenerationStep(this));
            _innerWizard.AddStep(new RaceTestSuggestionStep(this));
        }

        public WizardResult Run()
        {
            return _innerWizard.Run();
        }

        public RaceModConfig Config => _config;
        public bool UseChinese => _useChinese;
        public List<RaceTypeInfo> RaceTypes => _raceTypes;
        public ProbeContext Context => _context;

        public void SetLanguage(bool useChinese)
        {
            _useChinese = useChinese;
            _config.UseBilingual = true;
        }

        public string T(string zh, string en)
        {
            return _useChinese ? zh : en;
        }
    }

    public class RaceWelcomeStep : WizardStepBase
    {
        private readonly RaceModWizard _wizard;

        public RaceWelcomeStep(RaceModWizard wizard)
            : base(wizard.T("欢迎 - 种族 Mod 创建向导", "Welcome - Race Mod Wizard"),
                   wizard.T("本向导将引导您完成种族 Mod 的创建流程", "This wizard will guide you through the race mod creation process"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();

            Console.WriteLine(_wizard.T(
                "欢迎使用种族 Mod 创建向导！\n\n" +
                "本向导将帮助您：\n" +
                "  1. 配置种族基本信息（名称、描述）\n" +
                "  2. 选择种族类型（类人生物、动物、机械族）\n" +
                "  3. 配置基础属性（生命值、体型、移动速度等）\n" +
                "  4. 配置技能特性\n" +
                "  5. 配置服装装备\n" +
                "  6. 生成完整的 ThingDef 和 PawnKindDef XML\n",
                "Welcome to the Race Mod Wizard!\n\n" +
                "This wizard will help you:\n" +
                "  1. Configure basic race information (name, description)\n" +
                "  2. Select race type (Humanlike, Animal, Mechanoid)\n" +
                "  3. Configure base stats (health, body size, move speed, etc.)\n" +
                "  4. Configure skill traits\n" +
                "  5. Configure apparel\n" +
                "  6. Generate complete ThingDef and PawnKindDef XML\n"));

            Pause(_wizard.T("按任意键继续...", "Press any key to continue..."));
        }
    }

    public class RaceLanguageSelectStep : WizardStepBase
    {
        private readonly RaceModWizard _wizard;

        public RaceLanguageSelectStep(RaceModWizard wizard)
            : base(wizard.T("语言选择", "Language Selection"),
                   wizard.T("选择向导显示语言", "Select wizard display language"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();

            Console.WriteLine(_wizard.T("请选择语言 / Please select language:", "Please select language:"));
            Console.WriteLine("  [1] 中文 (Chinese)");
            Console.WriteLine("  [2] English (英文)");

            var choice = ReadChoice("", new[] { "中文", "English" }, "中文");
            _wizard.SetLanguage(choice == "中文");

            ShowSuccess(_wizard.T("已选择中文", "English selected"));
        }
    }

    public class RaceInfoStep : WizardStepBase
    {
        private readonly RaceModWizard _wizard;

        public RaceInfoStep(RaceModWizard wizard)
            : base(wizard.T("种族信息配置", "Race Information Configuration"),
                   wizard.T("配置种族的基本信息", "Configure basic race information"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();

            Console.WriteLine(_wizard.T("=== Mod 基本信息 ===", "=== Mod Basic Information ==="));
            Console.WriteLine();

            _wizard.Config.ModName = ReadInput(_wizard.T("Mod 名称", "Mod name"), "MyRaceMod");
            _wizard.Config.AuthorName = ReadInput(_wizard.T("作者名称", "Author name"), "YourName");
            _wizard.Config.Description = ReadInput(_wizard.T("Mod 描述", "Mod description"),
                _wizard.T("自定义种族 Mod", "Custom race mod"));

            Console.WriteLine();
            Console.WriteLine(_wizard.T("=== 种族定义信息 ===", "=== Race Definition Information ==="));
            Console.WriteLine();

            _wizard.Config.RaceDefName = ReadInput(_wizard.T("种族 DefName (英文标识符)", "Race DefName (English identifier)"), "CustomRace");
            _wizard.Config.RaceLabelZh = ReadInput(_wizard.T("种族名称 (中文)", "Race name (Chinese)"), "自定义种族");
            _wizard.Config.RaceLabelEn = ReadInput(_wizard.T("种族名称 (英文)", "Race name (English)"), "Custom Race");
            _wizard.Config.RaceDescriptionZh = ReadInput(_wizard.T("种族描述 (中文)", "Race description (Chinese)"), "一个自定义种族");
            _wizard.Config.RaceDescriptionEn = ReadInput(_wizard.T("种族描述 (英文)", "Race description (English)"), "A custom race");

            _wizard.Config.PawnKindDefName = ReadInput(_wizard.T("PawnKind DefName", "PawnKind DefName"), _wizard.Config.RaceDefName + "PawnKind");
            _wizard.Config.FactionDefName = ReadInput(_wizard.T("派系 DefName (可选)", "Faction DefName (optional)"), _wizard.Config.RaceDefName + "Faction");

            context.SetData("RaceInfoConfigured", true);
            ShowSuccess(_wizard.T("种族信息配置完成", "Race information configured"));
        }
    }

    public class RaceTypeSelectStep : WizardStepBase
    {
        private readonly RaceModWizard _wizard;

        public RaceTypeSelectStep(RaceModWizard wizard)
            : base(wizard.T("种族类型选择", "Race Type Selection"),
                   wizard.T("选择种族的基本类型", "Select the basic type of the race"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();

            Console.WriteLine(_wizard.T("可用种族类型:", "Available race types:"));
            Console.WriteLine();

            var raceTypes = _wizard.RaceTypes;
            for (int i = 0; i < raceTypes.Count; i++)
            {
                var type = raceTypes[i];
                var name = _wizard.UseChinese ? type.NameZh : type.NameEn;
                var desc = _wizard.UseChinese ? type.DescriptionZh : type.DescriptionEn;
                Console.WriteLine($"  [{i + 1}] {name}");
                Console.WriteLine($"      {desc}");
                Console.WriteLine();
            }

            var options = raceTypes.Select(t => _wizard.UseChinese ? t.NameZh : t.NameEn).ToList();
            var selectedName = ReadChoice(_wizard.T("请选择种族类型", "Please select race type"), options);

            var selectedIndex = options.IndexOf(selectedName);
            var selectedType = raceTypes[selectedIndex];

            _wizard.Config.RaceType = selectedType.Type;
            _wizard.Config.BodyType = selectedType.DefaultBodyType;
            _wizard.Config.BodyParts = new List<string>(selectedType.DefaultBodyParts);

            context.SetData("SelectedRaceType", selectedType);

            Console.WriteLine();
            Console.WriteLine(_wizard.T("=== 智能等级 ===", "=== Intelligence Level ==="));
            var intelligenceOptions = new[] {
                _wizard.T("工具使用者", "ToolUser"),
                _wizard.T("类人智能", "Humanlike"),
                _wizard.T("动物智能", "Animal")
            };
            var selectedIntelligence = ReadChoice(_wizard.T("选择智能等级", "Select intelligence level"), intelligenceOptions, intelligenceOptions[1]);

            if (selectedIntelligence == intelligenceOptions[0])
                _wizard.Config.Intelligence = RaceIntelligence.ToolUser;
            else if (selectedIntelligence == intelligenceOptions[1])
                _wizard.Config.Intelligence = RaceIntelligence.Humanlike;
            else
                _wizard.Config.Intelligence = RaceIntelligence.Animal;

            ShowSuccess(_wizard.T($"已选择: {selectedName}", $"Selected: {selectedName}"));
        }
    }

    public class RaceStatsStep : WizardStepBase
    {
        private readonly RaceModWizard _wizard;

        public RaceStatsStep(RaceModWizard wizard)
            : base(wizard.T("属性配置", "Stats Configuration"),
                   wizard.T("配置种族的基础属性", "Configure base stats for the race"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();

            var raceType = context.GetData<RaceTypeInfo>("SelectedRaceType");
            Console.WriteLine(_wizard.T($"配置: {(_wizard.UseChinese ? raceType?.NameZh : raceType?.NameEn)} 的基础属性",
                                       $"Configuring stats for: {raceType?.NameEn}"));
            Console.WriteLine();

            Console.WriteLine(_wizard.T("=== 生存属性 ===", "=== Survival Stats ==="));
            _wizard.Config.BaseHealthScale = ReadFloat(_wizard.T("生命值倍率 (默认 1.0)", "Health scale (default 1.0)"), 1.0f, 0.1f, 10.0f);
            _wizard.Config.BaseBodySize = ReadInt(_wizard.T("体型大小 (默认 1)", "Body size (default 1)"), 1, 1, 10);
            _wizard.Config.BaseHungerRate = ReadFloat(_wizard.T("饥饿速率 (默认 1.0)", "Hunger rate (default 1.0)"), 1.0f, 0.1f, 10.0f);

            Console.WriteLine();
            Console.WriteLine(_wizard.T("=== 移动属性 ===", "=== Movement Stats ==="));
            _wizard.Config.BaseMoveSpeed = ReadFloat(_wizard.T("移动速度 (默认 4.6)", "Move speed (default 4.6)"), 4.6f, 0.5f, 20.0f);

            Console.WriteLine();
            Console.WriteLine(_wizard.T("=== 战斗属性 ===", "=== Combat Stats ==="));
            _wizard.Config.BaseMeleeDodgeChance = ReadInt(_wizard.T("近战闪避率 (0-100, 默认 10)", "Melee dodge chance (0-100, default 10)"), 10, 0, 100);
            _wizard.Config.BaseMeleeHitChance = ReadInt(_wizard.T("近战命中率 (0-100, 默认 60)", "Melee hit chance (0-100, default 60)"), 60, 0, 100);

            context.SetData("StatsConfigured", true);
            ShowSuccess(_wizard.T("属性配置完成", "Stats configured"));
        }

        private float ReadFloat(string prompt, float defaultValue, float minValue, float maxValue)
        {
            while (true)
            {
                var input = ReadInput(prompt, defaultValue.ToString());
                if (string.IsNullOrEmpty(input))
                    return defaultValue;

                if (float.TryParse(input, out var result))
                {
                    if (result < minValue || result > maxValue)
                    {
                        ShowError(_wizard.T($"值必须在 {minValue} 到 {maxValue} 之间", $"Value must be between {minValue} and {maxValue}"));
                        continue;
                    }
                    return result;
                }

                ShowError(_wizard.T("请输入有效的数字", "Please enter a valid number"));
            }
        }
    }

    public class RaceSkillsStep : WizardStepBase
    {
        private readonly RaceModWizard _wizard;

        public RaceSkillsStep(RaceModWizard wizard)
            : base(wizard.T("技能配置", "Skills Configuration"),
                   wizard.T("配置种族的技能特性", "Configure skill traits for the race"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();

            if (_wizard.Config.Intelligence == RaceIntelligence.Animal)
            {
                ShowInfo(_wizard.T("动物智能种族通常不需要配置技能", "Animal intelligence races typically don't need skill configuration"));
                return;
            }

            Console.WriteLine(_wizard.T("=== 技能配置 ===", "=== Skills Configuration ==="));
            Console.WriteLine(_wizard.T("为种族配置初始技能等级和热情", "Configure initial skill levels and passions for the race"));
            Console.WriteLine();

            var skills = new List<string>
            {
                "Shooting", "Melee", "Cooking", "Construction", "Mining",
                "Plants", "Animals", "Crafting", "Artistic", "Medical",
                "Social", "Intellectual"
            };

            var addMore = true;
            while (addMore)
            {
                Console.WriteLine();
                Console.WriteLine(_wizard.T("可用技能:", "Available skills:"));
                for (int i = 0; i < skills.Count; i++)
                {
                    Console.WriteLine($"  [{i + 1}] {skills[i]}");
                }

                var selectedSkill = ReadChoice(_wizard.T("选择要配置的技能", "Select skill to configure"), skills);
                var level = ReadInt(_wizard.T("技能等级 (0-20)", "Skill level (0-20)"), 5, 0, 20);

                Console.WriteLine(_wizard.T("热情类型:", "Passion type:"));
                var passionOptions = new[] { "None", "Minor", "Major" };
                var passion = ReadChoice(_wizard.T("选择热情类型", "Select passion type"), passionOptions, "None");

                _wizard.Config.Skills.Add(new SkillConfig
                {
                    SkillName = selectedSkill,
                    Level = level,
                    Passion = passion
                });

                ShowSuccess(_wizard.T($"已添加技能: {selectedSkill} (等级 {level}, 热情 {passion})",
                                     $"Added skill: {selectedSkill} (Level {level}, Passion {passion})"));

                addMore = ReadBool(_wizard.T("是否继续添加技能?", "Add more skills?"), false);
            }

            context.SetData("SkillsConfigured", true);
            ShowSuccess(_wizard.T("技能配置完成", "Skills configured"));
        }
    }

    public class RaceApparelStep : WizardStepBase
    {
        private readonly RaceModWizard _wizard;

        public RaceApparelStep(RaceModWizard wizard)
            : base(wizard.T("服装配置", "Apparel Configuration"),
                   wizard.T("配置种族的服装装备", "Configure apparel for the race"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();

            if (_wizard.Config.RaceType == RaceType.Animal)
            {
                ShowInfo(_wizard.T("动物种族通常不需要配置服装", "Animal races typically don't need apparel configuration"));
                return;
            }

            Console.WriteLine(_wizard.T("=== 服装配置 ===", "=== Apparel Configuration ==="));
            Console.WriteLine(_wizard.T("为种族配置专属服装装备", "Configure exclusive apparel for the race"));
            Console.WriteLine();

            var addApparel = ReadBool(_wizard.T("是否添加自定义服装?", "Add custom apparel?"), false);

            while (addApparel)
            {
                var apparel = new ApparelConfig();

                apparel.ApparelDefName = ReadInput(_wizard.T("服装 DefName", "Apparel DefName"), $"{_wizard.Config.RaceDefName}_Apparel");
                apparel.LabelZh = ReadInput(_wizard.T("服装名称 (中文)", "Apparel name (Chinese)"), "自定义服装");
                apparel.LabelEn = ReadInput(_wizard.T("服装名称 (英文)", "Apparel name (English)"), "Custom Apparel");
                apparel.DescriptionZh = ReadInput(_wizard.T("服装描述 (中文)", "Apparel description (Chinese)"), "一件自定义服装");
                apparel.DescriptionEn = ReadInput(_wizard.T("服装描述 (英文)", "Apparel description (English)"), "A custom apparel");

                Console.WriteLine(_wizard.T("服装层级:", "Apparel layer:"));
                var layerOptions = new[] { "OnSkin", "MiddleLayer", "Shell", "Overhead", "Eyes" };
                apparel.Layer = ReadChoice(_wizard.T("选择服装层级", "Select apparel layer"), layerOptions, "OnSkin");

                apparel.ArmorRating = ReadFloat(_wizard.T("护甲值 (默认 0)", "Armor rating (default 0)"), 0, 0, 2);
                apparel.InsulationCold = ReadFloat(_wizard.T("寒冷抗性 (默认 0)", "Cold insulation (default 0)"), 0, -100, 100);
                apparel.InsulationHeat = ReadFloat(_wizard.T("炎热抗性 (默认 0)", "Heat insulation (default 0)"), 0, -100, 100);

                _wizard.Config.Apparels.Add(apparel);
                ShowSuccess(_wizard.T("服装添加完成", "Apparel added"));

                addApparel = ReadBool(_wizard.T("是否继续添加服装?", "Add more apparel?"), false);
            }

            context.SetData("ApparelConfigured", true);
            ShowSuccess(_wizard.T("服装配置完成", "Apparel configured"));
        }

        private float ReadFloat(string prompt, float defaultValue, float minValue, float maxValue)
        {
            while (true)
            {
                var input = ReadInput(prompt, defaultValue.ToString());
                if (string.IsNullOrEmpty(input))
                    return defaultValue;

                if (float.TryParse(input, out var result))
                {
                    if (result < minValue || result > maxValue)
                    {
                        ShowError(_wizard.T($"值必须在 {minValue} 到 {maxValue} 之间", $"Value must be between {minValue} and {maxValue}"));
                        continue;
                    }
                    return result;
                }

                ShowError(_wizard.T("请输入有效的数字", "Please enter a valid number"));
            }
        }
    }

    public class RaceCodeGenerationStep : WizardStepBase
    {
        private readonly RaceModWizard _wizard;

        public RaceCodeGenerationStep(RaceModWizard wizard)
            : base(wizard.T("代码生成", "Code Generation"),
                   wizard.T("生成完整的 ThingDef 和 PawnKindDef XML", "Generate complete ThingDef and PawnKindDef XML"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();

            var generatedCode = new StringBuilder();

            generatedCode.AppendLine(_wizard.T("=== 生成的代码 ===", "=== Generated Code ==="));
            generatedCode.AppendLine();

            generatedCode.AppendLine(GenerateModStructure());
            generatedCode.AppendLine();

            generatedCode.AppendLine(GenerateThingDef());
            generatedCode.AppendLine();

            generatedCode.AppendLine(GeneratePawnKindDef());
            generatedCode.AppendLine();

            if (_wizard.Config.Apparels.Count > 0)
            {
                generatedCode.AppendLine(GenerateApparelDefs());
                generatedCode.AppendLine();
            }

            generatedCode.AppendLine(GenerateLanguageFiles());

            Console.WriteLine(generatedCode.ToString());

            context.SetData("GeneratedCode", generatedCode.ToString());

            Pause(_wizard.T("按任意键继续...", "Press any key to continue..."));
        }

        private string GenerateModStructure()
        {
            var sb = new StringBuilder();

            sb.AppendLine(_wizard.T("=== Mod 目录结构 ===", "=== Mod Directory Structure ==="));
            sb.AppendLine();
            sb.AppendLine($"{_wizard.Config.ModName}/");
            sb.AppendLine("|-- About/");
            sb.AppendLine("|   |-- About.xml");
            sb.AppendLine("|   |-- Preview.png");
            sb.AppendLine("|-- Defs/");
            sb.AppendLine("|   |-- RaceDefs/");
            sb.AppendLine("|       |-- ThingDef_Races.xml");
            sb.AppendLine("|       |-- PawnKindDefs.xml");
            if (_wizard.Config.Apparels.Count > 0)
            {
                sb.AppendLine("|   |-- ApparelDefs/");
                sb.AppendLine("|       |-- ApparelDefs.xml");
            }
            sb.AppendLine("|-- Languages/");
            sb.AppendLine("|   |-- ChineseSimplified/");
            sb.AppendLine("|   |   |-- DefInjected/");
            sb.AppendLine("|   |       |-- ThingDef/");
            sb.AppendLine("|   |           |-- Races.xml");
            sb.AppendLine("|   |-- English/");
            sb.AppendLine("|       |-- DefInjected/");
            sb.AppendLine("|           |-- ThingDef/");
            sb.AppendLine("|               |-- Races.xml");
            sb.AppendLine();
            sb.AppendLine(_wizard.T("=== About.xml ===", "=== About.xml ==="));
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<ModMetaData>");
            sb.AppendLine($"    <name>{_wizard.Config.ModName}</name>");
            sb.AppendLine($"    <author>{_wizard.Config.AuthorName}</author>");
            sb.AppendLine($"    <packageId>{_wizard.Config.AuthorName}.{_wizard.Config.ModName.Replace(" ", "")}</packageId>");
            sb.AppendLine($"    <description>{_wizard.Config.Description}</description>");
            sb.AppendLine("    <supportedVersions>");
            sb.AppendLine("        <li>1.5</li>");
            sb.AppendLine("    </supportedVersions>");
            sb.AppendLine("</ModMetaData>");

            return sb.ToString();
        }

        private string GenerateThingDef()
        {
            var sb = new StringBuilder();

            sb.AppendLine(_wizard.T("=== ThingDef (种族定义) ===", "=== ThingDef (Race Definition) ==="));
            sb.AppendLine();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sb.AppendLine("<Defs>");
            sb.AppendLine("    <ThingDef ParentName=\"BasePawn\">");
            sb.AppendLine($"        <defName>{_wizard.Config.RaceDefName}</defName>");
            sb.AppendLine($"        <label>{_wizard.Config.RaceLabelEn}</label>");
            sb.AppendLine($"        <description>{_wizard.Config.RaceDescriptionEn}</description>");
            sb.AppendLine("        <thingClass>Pawn</thingClass>");
            sb.AppendLine($"        <race>");

            switch (_wizard.Config.RaceType)
            {
                case RaceType.Humanlike:
                    sb.AppendLine("            <thinkTreeMain>Humanlike</thinkTreeMain>");
                    sb.AppendLine("            <thinkTreeConstant>HumanlikeConstant</thinkTreeConstant>");
                    sb.AppendLine("            <intelligence>Humanlike</intelligence>");
                    break;
                case RaceType.Animal:
                    sb.AppendLine("            <thinkTreeMain>Animal</thinkTreeMain>");
                    sb.AppendLine("            <thinkTreeConstant>AnimalConstant</thinkTreeConstant>");
                    sb.AppendLine("            <intelligence>Animal</intelligence>");
                    break;
                case RaceType.Mechanoid:
                    sb.AppendLine("            <thinkTreeMain>Mechanoid</thinkTreeMain>");
                    sb.AppendLine("            <thinkTreeConstant>MechanoidConstant</thinkTreeConstant>");
                    sb.AppendLine("            <intelligence>Humanlike</intelligence>");
                    sb.AppendLine("            <isMechanoid>true</isMechanoid>");
                    break;
            }

            sb.AppendLine($"            <body>{_wizard.Config.BodyType}</body>");
            sb.AppendLine($"            <baseBodySize>{_wizard.Config.BaseBodySize}</baseBodySize>");
            sb.AppendLine($"            <baseHealthScale>{_wizard.Config.BaseHealthScale}</baseHealthScale>");
            sb.AppendLine($"            <baseHungerRate>{_wizard.Config.BaseHungerRate}</baseHungerRate>");
            sb.AppendLine($"            <foodType>OmnivoreHuman</foodType>");

            if (_wizard.Config.RaceType == RaceType.Mechanoid)
            {
                sb.AppendLine("            <needsFood>false</needsFood>");
            }

            sb.AppendLine("            <lifeExpectancy>80</lifeExpectancy>");
            sb.AppendLine("            <soundMeleeHitPawn>Pawn_Melee_Punch_HitPawn</soundMeleeHitPawn>");
            sb.AppendLine("            <soundMeleeHitBuilding>Pawn_Melee_Punch_HitBuilding</soundMeleeHitBuilding>");
            sb.AppendLine("            <soundMeleeMiss>Pawn_Melee_Punch_Miss</soundMeleeMiss>");

            sb.AppendLine("            <statBases>");
            sb.AppendLine($"                <MoveSpeed>{_wizard.Config.BaseMoveSpeed}</MoveSpeed>");
            sb.AppendLine($"                <MeleeDodgeChance>{_wizard.Config.BaseMeleeDodgeChance}</MeleeDodgeChance>");
            sb.AppendLine($"                <MeleeHitChance>{_wizard.Config.BaseMeleeHitChance}</MeleeHitChance>");
            sb.AppendLine("            </statBases>");

            if (_wizard.Config.Skills.Count > 0)
            {
                sb.AppendLine("            <skills>");
                foreach (var skill in _wizard.Config.Skills)
                {
                    sb.AppendLine($"                <li>");
                    sb.AppendLine($"                    <def>{skill.SkillName}</def>");
                    sb.AppendLine($"                    <level>{skill.Level}</level>");
                    if (skill.Passion != "None")
                    {
                        sb.AppendLine($"                    <passion>{skill.Passion}</passion>");
                    }
                    sb.AppendLine($"                </li>");
                }
                sb.AppendLine("            </skills>");
            }

            sb.AppendLine("        </race>");
            sb.AppendLine("    </ThingDef>");
            sb.AppendLine("</Defs>");

            return sb.ToString();
        }

        private string GeneratePawnKindDef()
        {
            var sb = new StringBuilder();

            sb.AppendLine(_wizard.T("=== PawnKindDef (角色类型定义) ===", "=== PawnKindDef (Pawn Kind Definition) ==="));
            sb.AppendLine();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sb.AppendLine("<Defs>");
            sb.AppendLine("    <PawnKindDef ParentName=\"BasePawnKind\">");
            sb.AppendLine($"        <defName>{_wizard.Config.PawnKindDefName}</defName>");
            sb.AppendLine($"        <label>{_wizard.Config.RaceLabelEn}</label>");
            sb.AppendLine($"        <race>{_wizard.Config.RaceDefName}</race>");
            sb.AppendLine("        <defaultFactionType>PlayerColony</defaultFactionType>");
            sb.AppendLine($"        <backstoryCategory>{_wizard.Config.BackstoryCategory}</backstoryCategory>");
            sb.AppendLine("        <chemicalAddictionChance>0.05</chemicalAddictionChance>");
            sb.AppendLine("        <apparelIgnoreSeasons>false</apparelIgnoreSeasons>");
            sb.AppendLine("        <forceNormalGearQuality>true</forceNormalGearQuality>");
            sb.AppendLine("        <initialWillRange>1~2</initialWillRange>");
            sb.AppendLine("        <initialResistanceRange>10~18</initialResistanceRange>");
            sb.AppendLine("    </PawnKindDef>");
            sb.AppendLine("</Defs>");

            return sb.ToString();
        }

        private string GenerateApparelDefs()
        {
            var sb = new StringBuilder();

            sb.AppendLine(_wizard.T("=== ApparelDefs (服装定义) ===", "=== ApparelDefs (Apparel Definitions) ==="));
            sb.AppendLine();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sb.AppendLine("<Defs>");

            foreach (var apparel in _wizard.Config.Apparels)
            {
                sb.AppendLine("    <ThingDef ParentName=\"ApparelBase\">");
                sb.AppendLine($"        <defName>{apparel.ApparelDefName}</defName>");
                sb.AppendLine($"        <label>{apparel.LabelEn}</label>");
                sb.AppendLine($"        <description>{apparel.DescriptionEn}</description>");
                sb.AppendLine("        <graphicData>");
                sb.AppendLine("            <texPath>Things/Pawn/Apparel/Apparel</texPath>");
                sb.AppendLine("            <graphicClass>Graphic_Single</graphicClass>");
                sb.AppendLine("        </graphicData>");
                sb.AppendLine("        <statBases>");
                sb.AppendLine("            <WorkToMake>1000</WorkToMake>");
                sb.AppendLine($"            <ArmorRating_Blunt>{apparel.ArmorRating * 0.5f}</ArmorRating_Blunt>");
                sb.AppendLine($"            <ArmorRating_Sharp>{apparel.ArmorRating}</ArmorRating_Sharp>");
                sb.AppendLine($"            <Insulation_Cold>{apparel.InsulationCold}</Insulation_Cold>");
                sb.AppendLine($"            <Insulation_Heat>{apparel.InsulationHeat}</Insulation_Heat>");
                sb.AppendLine("        </statBases>");
                sb.AppendLine("        <apparel>");
                sb.AppendLine($"            <bodyPartGroups>");
                sb.AppendLine($"                <li>Torso</li>");
                sb.AppendLine($"            </bodyPartGroups>");
                sb.AppendLine($"            <layers>");
                sb.AppendLine($"                <li>{apparel.Layer}</li>");
                sb.AppendLine($"            </layers>");
                sb.AppendLine($"            <tags>");
                sb.AppendLine($"                <li>{_wizard.Config.RaceDefName}Apparel</li>");
                sb.AppendLine($"            </tags>");
                sb.AppendLine($"            <defaultOutfitTags>");
                sb.AppendLine($"                <li>{_wizard.Config.RaceDefName}Worker</li>");
                sb.AppendLine($"            </defaultOutfitTags>");
                sb.AppendLine("        </apparel>");
                sb.AppendLine("    </ThingDef>");
            }

            sb.AppendLine("</Defs>");

            return sb.ToString();
        }

        private string GenerateLanguageFiles()
        {
            var sb = new StringBuilder();

            sb.AppendLine(_wizard.T("=== 语言文件 ===", "=== Language Files ==="));
            sb.AppendLine();

            sb.AppendLine(_wizard.T("=== ChineseSimplified/DefInjected/ThingDef/Races.xml ===", "=== ChineseSimplified/DefInjected/ThingDef/Races.xml ==="));
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<LanguageData>");
            sb.AppendLine($"    <{_wizard.Config.RaceDefName}.label>{_wizard.Config.RaceLabelZh}</{_wizard.Config.RaceDefName}.label>");
            sb.AppendLine($"    <{_wizard.Config.RaceDefName}.description>{_wizard.Config.RaceDescriptionZh}</{_wizard.Config.RaceDefName}.description>");
            sb.AppendLine($"    <{_wizard.Config.PawnKindDefName}.label>{_wizard.Config.RaceLabelZh}</{_wizard.Config.PawnKindDefName}.label>");
            foreach (var apparel in _wizard.Config.Apparels)
            {
                sb.AppendLine($"    <{apparel.ApparelDefName}.label>{apparel.LabelZh}</{apparel.ApparelDefName}.label>");
                sb.AppendLine($"    <{apparel.ApparelDefName}.description>{apparel.DescriptionZh}</{apparel.ApparelDefName}.description>");
            }
            sb.AppendLine("</LanguageData>");
            sb.AppendLine();

            sb.AppendLine(_wizard.T("=== English/DefInjected/ThingDef/Races.xml ===", "=== English/DefInjected/ThingDef/Races.xml ==="));
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<LanguageData>");
            sb.AppendLine($"    <{_wizard.Config.RaceDefName}.label>{_wizard.Config.RaceLabelEn}</{_wizard.Config.RaceDefName}.label>");
            sb.AppendLine($"    <{_wizard.Config.RaceDefName}.description>{_wizard.Config.RaceDescriptionEn}</{_wizard.Config.RaceDefName}.description>");
            sb.AppendLine($"    <{_wizard.Config.PawnKindDefName}.label>{_wizard.Config.RaceLabelEn}</{_wizard.Config.PawnKindDefName}.label>");
            foreach (var apparel in _wizard.Config.Apparels)
            {
                sb.AppendLine($"    <{apparel.ApparelDefName}.label>{apparel.LabelEn}</{apparel.ApparelDefName}.label>");
                sb.AppendLine($"    <{apparel.ApparelDefName}.description>{apparel.DescriptionEn}</{apparel.ApparelDefName}.description>");
            }
            sb.AppendLine("</LanguageData>");

            return sb.ToString();
        }
    }

    public class RaceTestSuggestionStep : WizardStepBase
    {
        private readonly RaceModWizard _wizard;

        public RaceTestSuggestionStep(RaceModWizard wizard)
            : base(wizard.T("测试建议", "Test Suggestions"),
                   wizard.T("提供测试建议和注意事项", "Provide testing suggestions and notes"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();

            Console.WriteLine(_wizard.T("=== 测试步骤 ===", "=== Testing Steps ==="));
            Console.WriteLine();

            Console.WriteLine(_wizard.T("1. 创建 Mod 目录结构", "1. Create mod directory structure"));
            Console.WriteLine(_wizard.T("   * 按照生成的目录结构创建文件夹", "   * Create folders according to generated structure"));
            Console.WriteLine(_wizard.T("   * 将 XML 文件放入对应目录", "   * Place XML files in corresponding directories"));
            Console.WriteLine();

            Console.WriteLine(_wizard.T("2. 准备资源文件", "2. Prepare resource files"));
            Console.WriteLine(_wizard.T("   * 如有自定义贴图，放入 Textures 文件夹", "   * If using custom textures, place in Textures folder"));
            Console.WriteLine(_wizard.T("   * 确保 About.xml 正确配置", "   * Ensure About.xml is correctly configured"));
            Console.WriteLine();

            Console.WriteLine(_wizard.T("3. 游戏内测试", "3. In-game testing"));
            Console.WriteLine(_wizard.T("   * 启动游戏并加载 Mod", "   * Start game and load the mod"));
            Console.WriteLine(_wizard.T("   * 使用开发模式生成测试角色", "   * Use dev mode to spawn test pawns"));
            Console.WriteLine(_wizard.T("   * 检查角色属性是否正确", "   * Check if pawn stats are correct"));
            Console.WriteLine(_wizard.T("   * 测试服装装备是否正常", "   * Test if apparel works correctly"));
            Console.WriteLine();

            Console.WriteLine(_wizard.T("=== 注意事项 ===", "=== Notes ==="));
            Console.WriteLine();

            PrintTypeSpecificNotes();

            Console.WriteLine(_wizard.T("通用注意事项:", "General notes:"));
            Console.WriteLine(_wizard.T("  * DefName 必须全局唯一", "  * DefName must be globally unique"));
            Console.WriteLine(_wizard.T("  * 检查 XML 语法是否正确", "  * Check if XML syntax is correct"));
            Console.WriteLine(_wizard.T("  * 确保语言文件编码为 UTF-8", "  * Ensure language files are UTF-8 encoded"));
            Console.WriteLine(_wizard.T("  * 测试与其他种族 Mod 的兼容性", "  * Test compatibility with other race mods"));
            Console.WriteLine();

            Console.WriteLine(_wizard.T("=== 调试技巧 ===", "=== Debug Tips ==="));
            Console.WriteLine();
            Console.WriteLine(_wizard.T("查看错误日志:", "View error log:"));
            Console.WriteLine("  - Windows: %USERPROFILE%\\AppData\\LocalLow\\Ludeon Studios\\RimWorld by Ludeon Studios\\Player.log");
            Console.WriteLine();
            Console.WriteLine(_wizard.T("常用调试命令:", "Common debug commands:"));
            Console.WriteLine("  - Debug Actions Menu -> Spawn -> Pawn");
            Console.WriteLine($"  - 选择 {_wizard.Config.RaceDefName} 进行测试");
            Console.WriteLine();

            Pause(_wizard.T("按任意键完成向导...", "Press any key to finish wizard..."));

            ShowSuccess(_wizard.T("向导完成！", "Wizard completed!"));
            Console.WriteLine();
            Console.WriteLine(_wizard.T("生成的代码已保存到上下文中，您可以复制使用。", "Generated code is saved in context, you can copy and use it."));
        }

        private void PrintTypeSpecificNotes()
        {
            Console.WriteLine(_wizard.T(_wizard.Config.RaceType.ToString() + " 类型特定注意事项:", _wizard.Config.RaceType.ToString() + " type specific notes:"));

            switch (_wizard.Config.RaceType)
            {
                case RaceType.Humanlike:
                    Console.WriteLine(_wizard.T("  - 确保可以正确穿戴装备", "  - Ensure apparel can be worn correctly"));
                    Console.WriteLine(_wizard.T("  - 测试社交互动", "  - Test social interactions"));
                    Console.WriteLine(_wizard.T("  - 检查背景故事兼容性", "  - Check backstory compatibility"));
                    break;
                case RaceType.Animal:
                    Console.WriteLine(_wizard.T("  - 测试驯服和训练", "  - Test taming and training"));
                    Console.WriteLine(_wizard.T("  - 检查动物行为", "  - Check animal behaviors"));
                    Console.WriteLine(_wizard.T("  - 验证繁殖功能", "  - Verify breeding functionality"));
                    break;
                case RaceType.Mechanoid:
                    Console.WriteLine(_wizard.T("  - 测试机械族特殊能力", "  - Test mechanoid special abilities"));
                    Console.WriteLine(_wizard.T("  - 检查是否需要能源", "  - Check if power is needed"));
                    Console.WriteLine(_wizard.T("  - 验证与敌对派系的关系", "  - Verify relationship with hostile factions"));
                    break;
            }

            Console.WriteLine();
        }
    }
}
