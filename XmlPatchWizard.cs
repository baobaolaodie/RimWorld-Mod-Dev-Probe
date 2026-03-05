using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorldModDevProbe.Core;
using RimWorldModDevProbe.Wizards.Core;

namespace RimWorldModDevProbe
{
    public enum PatchOperationType
    {
        Add,
        Replace,
        Remove,
        Insert,
        Conditional,
        Sequence,
        Find,
        Multiply,
        Set
    }

    public enum DefType
    {
        ThingDef,
        SoundDef,
        ResearchProjectDef,
        RecipeDef,
        WorkGiverDef,
        JobDef,
        FactionDef,
        PawnKindDef,
        HediffDef,
        BodyDef,
        BiomeDef,
        TerrainDef,
        WeatherDef,
        IncidentDef,
        Custom
    }

    public class XmlPatchConfig
    {
        public string ModName { get; set; } = "MyXmlPatchMod";
        public string AuthorName { get; set; } = "YourName";
        public string Description { get; set; } = "XML Patch Mod";
        public DefType TargetDefType { get; set; } = DefType.ThingDef;
        public string TargetDefName { get; set; } = "TargetDef";
        public PatchOperationType OperationType { get; set; } = PatchOperationType.Replace;
        public string XPath { get; set; } = "";
        public List<PatchValue> Values { get; set; } = new List<PatchValue>();
        public string ConditionalXPath { get; set; } = "";
        public List<string> SequenceOperations { get; set; } = new List<string>();
        public bool UseBilingual { get; set; } = true;
        public string CustomDefType { get; set; } = "";
    }

    public class PatchValue
    {
        public string ElementName { get; set; }
        public string Value { get; set; }
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
        public List<PatchValue> Children { get; set; } = new List<PatchValue>();
    }

    public class DefTypeInfo
    {
        public DefType Type { get; set; }
        public string NameZh { get; set; }
        public string NameEn { get; set; }
        public string DescriptionZh { get; set; }
        public string DescriptionEn { get; set; }
        public List<string> CommonXPaths { get; set; } = new List<string>();
        public List<string> ExampleDefNames { get; set; } = new List<string>();
    }

    public class PatchOperationInfo
    {
        public PatchOperationType Type { get; set; }
        public string NameZh { get; set; }
        public string NameEn { get; set; }
        public string DescriptionZh { get; set; }
        public string DescriptionEn { get; set; }
        public string UsageExample { get; set; }
    }

    public class XmlPatchWizard
    {
        private readonly ProbeContext _context;
        private readonly DevWizard _innerWizard;
        private readonly XmlPatchConfig _config;
        private readonly List<DefTypeInfo> _defTypes;
        private readonly List<PatchOperationInfo> _operationTypes;
        private bool _useChinese = true;

        public XmlPatchWizard(ProbeContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _config = new XmlPatchConfig();
            _defTypes = InitializeDefTypes();
            _operationTypes = InitializeOperationTypes();
            _innerWizard = new DevWizard(context);

            SetupWizardSteps();
        }

        private List<DefTypeInfo> InitializeDefTypes()
        {
            return new List<DefTypeInfo>
            {
                new DefTypeInfo
                {
                    Type = DefType.ThingDef,
                    NameZh = "物品/建筑定义",
                    NameEn = "ThingDef",
                    DescriptionZh = "定义物品、建筑、武器、角色等游戏实体",
                    DescriptionEn = "Defines items, buildings, weapons, pawns and other game entities",
                    CommonXPaths = new List<string>
                    {
                        "/Defs/ThingDef[defName=\"XXX\"]/label",
                        "/Defs/ThingDef[defName=\"XXX\"]/description",
                        "/Defs/ThingDef[defName=\"XXX\"]/statBases/MaxHitPoints",
                        "/Defs/ThingDef[defName=\"XXX\"]/statBases/Beauty",
                        "/Defs/ThingDef[defName=\"XXX\"]/costList",
                        "/Defs/ThingDef[defName=\"XXX\"]/recipeMaker"
                    },
                    ExampleDefNames = new List<string> { "Steel", "WoodLog", "Gun_Pistol", "Bed_Simple", "Wall" }
                },
                new DefTypeInfo
                {
                    Type = DefType.SoundDef,
                    NameZh = "音效定义",
                    NameEn = "SoundDef",
                    DescriptionZh = "定义游戏中的各种音效",
                    DescriptionEn = "Defines various sound effects in the game",
                    CommonXPaths = new List<string>
                    {
                        "/Defs/SoundDef[defName=\"XXX\"]/subSounds",
                        "/Defs/SoundDef[defName=\"XXX\"]/eventNames"
                    },
                    ExampleDefNames = new List<string> { "Pawn_Melee_Punch_HitPawn", "Interact_Rope" }
                },
                new DefTypeInfo
                {
                    Type = DefType.ResearchProjectDef,
                    NameZh = "研究项目定义",
                    NameEn = "ResearchProjectDef",
                    DescriptionZh = "定义研究项目和解锁内容",
                    DescriptionEn = "Defines research projects and unlocked content",
                    CommonXPaths = new List<string>
                    {
                        "/Defs/ResearchProjectDef[defName=\"XXX\"]/label",
                        "/Defs/ResearchProjectDef[defName=\"XXX\"]/description",
                        "/Defs/ResearchProjectDef[defName=\"XXX\"]/baseCost",
                        "/Defs/ResearchProjectDef[defName=\"XXX\"]/prerequisites"
                    },
                    ExampleDefNames = new List<string> { "Machining", "MicroelectronicsBasics", "GunTurrets" }
                },
                new DefTypeInfo
                {
                    Type = DefType.RecipeDef,
                    NameZh = "配方定义",
                    NameEn = "RecipeDef",
                    DescriptionZh = "定义制作配方和工作任务",
                    DescriptionEn = "Defines crafting recipes and work tasks",
                    CommonXPaths = new List<string>
                    {
                        "/Defs/RecipeDef[defName=\"XXX\"]/label",
                        "/Defs/RecipeDef[defName=\"XXX\"]/workAmount",
                        "/Defs/RecipeDef[defName=\"XXX\"]/ingredients"
                    },
                    ExampleDefNames = new List<string> { "Make_SimpleMeal", "Make_Pistol" }
                },
                new DefTypeInfo
                {
                    Type = DefType.FactionDef,
                    NameZh = "派系定义",
                    NameEn = "FactionDef",
                    DescriptionZh = "定义游戏中的各种派系",
                    DescriptionEn = "Defines various factions in the game",
                    CommonXPaths = new List<string>
                    {
                        "/Defs/FactionDef[defName=\"XXX\"]/label",
                        "/Defs/FactionDef[defName=\"XXX\"]/description",
                        "/Defs/FactionDef[defName=\"XXX\"]/pawnKindMembers"
                    },
                    ExampleDefNames = new List<string> { "OutlanderCivil", "Pirate", "Mechanoid" }
                },
                new DefTypeInfo
                {
                    Type = DefType.PawnKindDef,
                    NameZh = "角色类型定义",
                    NameEn = "PawnKindDef",
                    DescriptionZh = "定义角色的类型和属性",
                    DescriptionEn = "Defines pawn types and their properties",
                    CommonXPaths = new List<string>
                    {
                        "/Defs/PawnKindDef[defName=\"XXX\"]/label",
                        "/Defs/PawnKindDef[defName=\"XXX\"]/race",
                        "/Defs/PawnKindDef[defName=\"XXX\"]/defaultFactionType"
                    },
                    ExampleDefNames = new List<string> { "Colonist", "Drifter", "Scyther" }
                },
                new DefTypeInfo
                {
                    Type = DefType.HediffDef,
                    NameZh = "健康状态定义",
                    NameEn = "HediffDef",
                    DescriptionZh = "定义疾病、伤势、增益状态等",
                    DescriptionEn = "Defines diseases, injuries, buffs and other health conditions",
                    CommonXPaths = new List<string>
                    {
                        "/Defs/HediffDef[defName=\"XXX\"]/label",
                        "/Defs/HediffDef[defName=\"XXX\"]/description",
                        "/Defs/HediffDef[defName=\"XXX\"]/stages"
                    },
                    ExampleDefNames = new List<string> { "Flu", "Plague", "HeartAttack" }
                },
                new DefTypeInfo
                {
                    Type = DefType.BiomeDef,
                    NameZh = "生态群落定义",
                    NameEn = "BiomeDef",
                    DescriptionZh = "定义地图的生态类型",
                    DescriptionEn = "Defines biome types for maps",
                    CommonXPaths = new List<string>
                    {
                        "/Defs/BiomeDef[defName=\"XXX\"]/label",
                        "/Defs/BiomeDef[defName=\"XXX\"]/description",
                        "/Defs/BiomeDef[defName=\"XXX\"]/baseWeatherCommonalities"
                    },
                    ExampleDefNames = new List<string> { "TemperateForest", "Desert", "IceSheet" }
                },
                new DefTypeInfo
                {
                    Type = DefType.TerrainDef,
                    NameZh = "地形定义",
                    NameEn = "TerrainDef",
                    DescriptionZh = "定义地面地形类型",
                    DescriptionEn = "Defines terrain types for ground surfaces",
                    CommonXPaths = new List<string>
                    {
                        "/Defs/TerrainDef[defName=\"XXX\"]/label",
                        "/Defs/TerrainDef[defName=\"XXX\"]/statBases/Beauty",
                        "/Defs/TerrainDef[defName=\"XXX\"]/costList"
                    },
                    ExampleDefNames = new List<string> { "Soil", "Concrete", "WoodPlankFloor" }
                },
                new DefTypeInfo
                {
                    Type = DefType.Custom,
                    NameZh = "自定义 Def 类型",
                    NameEn = "Custom Def Type",
                    DescriptionZh = "输入自定义的 Def 类型名称",
                    DescriptionEn = "Enter a custom Def type name",
                    CommonXPaths = new List<string>(),
                    ExampleDefNames = new List<string>()
                }
            };
        }

        private List<PatchOperationInfo> InitializeOperationTypes()
        {
            return new List<PatchOperationInfo>
            {
                new PatchOperationInfo
                {
                    Type = PatchOperationType.Add,
                    NameZh = "添加",
                    NameEn = "Add",
                    DescriptionZh = "在指定位置添加新的 XML 元素",
                    DescriptionEn = "Add new XML elements at the specified location",
                    UsageExample = "<Operation Class=\"PatchOperationAdd\"><xpath>...</xpath><value>...</value></Operation>"
                },
                new PatchOperationInfo
                {
                    Type = PatchOperationType.Replace,
                    NameZh = "替换",
                    NameEn = "Replace",
                    DescriptionZh = "替换指定位置的 XML 元素内容",
                    DescriptionEn = "Replace content of XML elements at the specified location",
                    UsageExample = "<Operation Class=\"PatchOperationReplace\"><xpath>...</xpath><value>...</value></Operation>"
                },
                new PatchOperationInfo
                {
                    Type = PatchOperationType.Remove,
                    NameZh = "移除",
                    NameEn = "Remove",
                    DescriptionZh = "移除指定位置的 XML 元素",
                    DescriptionEn = "Remove XML elements at the specified location",
                    UsageExample = "<Operation Class=\"PatchOperationRemove\"><xpath>...</xpath></Operation>"
                },
                new PatchOperationInfo
                {
                    Type = PatchOperationType.Insert,
                    NameZh = "插入",
                    NameEn = "Insert",
                    DescriptionZh = "在指定位置插入新的 XML 元素（保留同级元素）",
                    DescriptionEn = "Insert new XML elements at the specified location (preserving siblings)",
                    UsageExample = "<Operation Class=\"PatchOperationInsert\"><xpath>...</xpath><value>...</value></Operation>"
                },
                new PatchOperationInfo
                {
                    Type = PatchOperationType.Conditional,
                    NameZh = "条件",
                    NameEn = "Conditional",
                    DescriptionZh = "根据条件执行不同的 Patch 操作",
                    DescriptionEn = "Execute different patch operations based on conditions",
                    UsageExample = "<Operation Class=\"PatchOperationConditional\"><xpath>...</xpath><match>...</match><nomatch>...</nomatch></Operation>"
                },
                new PatchOperationInfo
                {
                    Type = PatchOperationType.Sequence,
                    NameZh = "序列",
                    NameEn = "Sequence",
                    DescriptionZh = "按顺序执行多个 Patch 操作",
                    DescriptionEn = "Execute multiple patch operations in sequence",
                    UsageExample = "<Operation Class=\"PatchOperationSequence\"><operations>...</operations></Operation>"
                },
                new PatchOperationInfo
                {
                    Type = PatchOperationType.Find,
                    NameZh = "查找",
                    NameEn = "Find",
                    DescriptionZh = "查找匹配的元素并执行操作",
                    DescriptionEn = "Find matching elements and perform operations",
                    UsageExample = "<Operation Class=\"PatchOperationFindMod\"><mod>...</mod>...</Operation>"
                },
                new PatchOperationInfo
                {
                    Type = PatchOperationType.Multiply,
                    NameZh = "乘法",
                    NameEn = "Multiply",
                    DescriptionZh = "将数值乘以指定倍数",
                    DescriptionEn = "Multiply numeric values by a specified factor",
                    UsageExample = "<Operation Class=\"PatchOperationMultiply\"><xpath>...</xpath><value>2.0</value></Operation>"
                },
                new PatchOperationInfo
                {
                    Type = PatchOperationType.Set,
                    NameZh = "设置",
                    NameEn = "Set",
                    DescriptionZh = "设置指定位置的值",
                    DescriptionEn = "Set the value at the specified location",
                    UsageExample = "<Operation Class=\"PatchOperationSet\"><xpath>...</xpath><value>...</value></Operation>"
                }
            };
        }

        private void SetupWizardSteps()
        {
            _innerWizard.AddStep(new XmlPatchWelcomeStep(this));
            _innerWizard.AddStep(new XmlPatchLanguageSelectStep(this));
            _innerWizard.AddStep(new XmlPatchModInfoStep(this));
            _innerWizard.AddStep(new XmlPatchTargetSelectStep(this));
            _innerWizard.AddStep(new XmlPatchOperationSelectStep(this));
            _innerWizard.AddStep(new XmlPatchValueConfigStep(this));
            _innerWizard.AddStep(new XmlPatchCodeGenerationStep(this));
            _innerWizard.AddStep(new XmlPatchTestSuggestionStep(this));
        }

        public WizardResult Run()
        {
            return _innerWizard.Run();
        }

        public XmlPatchConfig Config => _config;
        public bool UseChinese => _useChinese;
        public List<DefTypeInfo> DefTypes => _defTypes;
        public List<PatchOperationInfo> OperationTypes => _operationTypes;
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

    public class XmlPatchWelcomeStep : WizardStepBase
    {
        private readonly XmlPatchWizard _wizard;

        public XmlPatchWelcomeStep(XmlPatchWizard wizard)
            : base(wizard.T("欢迎 - XML Patch 创建向导", "Welcome - XML Patch Wizard"),
                   wizard.T("本向导将引导您完成 XML Patch 的创建流程", "This wizard will guide you through the XML patch creation process"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();

            Console.WriteLine(_wizard.T(
                "欢迎使用 XML Patch 创建向导！\n\n" +
                "本向导将帮助您：\n" +
                "  1. 选择要修改的 Def 类型\n" +
                "  2. 指定目标 Def 名称\n" +
                "  3. 选择 Patch 操作类型 (Add/Replace/Remove 等)\n" +
                "  4. 配置 XPath 和修改值\n" +
                "  5. 生成完整的 PatchOperation XML\n",
                "Welcome to the XML Patch Wizard!\n\n" +
                "This wizard will help you:\n" +
                "  1. Select the Def type to modify\n" +
                "  2. Specify the target Def name\n" +
                "  3. Select Patch operation type (Add/Replace/Remove, etc.)\n" +
                "  4. Configure XPath and modification values\n" +
                "  5. Generate complete PatchOperation XML\n"));

            Console.WriteLine(_wizard.T("=== PatchOperation 类型说明 ===", "=== PatchOperation Types ==="));
            Console.WriteLine();
            Console.WriteLine(_wizard.T("常用操作类型:", "Common operation types:"));
            Console.WriteLine("  - PatchOperationAdd: " + _wizard.T("添加新元素", "Add new elements"));
            Console.WriteLine("  - PatchOperationReplace: " + _wizard.T("替换元素内容", "Replace element content"));
            Console.WriteLine("  - PatchOperationRemove: " + _wizard.T("移除元素", "Remove elements"));
            Console.WriteLine("  - PatchOperationConditional: " + _wizard.T("条件执行", "Conditional execution"));
            Console.WriteLine();

            Pause(_wizard.T("按任意键继续...", "Press any key to continue..."));
        }
    }

    public class XmlPatchLanguageSelectStep : WizardStepBase
    {
        private readonly XmlPatchWizard _wizard;

        public XmlPatchLanguageSelectStep(XmlPatchWizard wizard)
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

    public class XmlPatchModInfoStep : WizardStepBase
    {
        private readonly XmlPatchWizard _wizard;

        public XmlPatchModInfoStep(XmlPatchWizard wizard)
            : base(wizard.T("Mod 信息配置", "Mod Information Configuration"),
                   wizard.T("配置 Mod 的基本信息", "Configure basic mod information"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();

            Console.WriteLine(_wizard.T("=== Mod 基本信息 ===", "=== Mod Basic Information ==="));
            Console.WriteLine();

            _wizard.Config.ModName = ReadInput(_wizard.T("Mod 名称", "Mod name"), "MyXmlPatchMod");
            _wizard.Config.AuthorName = ReadInput(_wizard.T("作者名称", "Author name"), "YourName");
            _wizard.Config.Description = ReadInput(_wizard.T("Mod 描述", "Mod description"),
                _wizard.T("XML Patch 修改 Mod", "XML Patch modification mod"));

            context.SetData("ModInfoConfigured", true);
            ShowSuccess(_wizard.T("Mod 信息配置完成", "Mod information configured"));
        }
    }

    public class XmlPatchTargetSelectStep : WizardStepBase
    {
        private readonly XmlPatchWizard _wizard;

        public XmlPatchTargetSelectStep(XmlPatchWizard wizard)
            : base(wizard.T("目标选择", "Target Selection"),
                   wizard.T("选择要修改的 Def 类型和名称", "Select the Def type and name to modify"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();

            Console.WriteLine(_wizard.T("=== Def 类型选择 ===", "=== Def Type Selection ==="));
            Console.WriteLine();

            var defTypes = _wizard.DefTypes;
            for (int i = 0; i < defTypes.Count; i++)
            {
                var type = defTypes[i];
                var name = _wizard.UseChinese ? type.NameZh : type.NameEn;
                var desc = _wizard.UseChinese ? type.DescriptionZh : type.DescriptionEn;
                Console.WriteLine($"  [{i + 1}] {name}");
                Console.WriteLine($"      {desc}");
                Console.WriteLine();
            }

            var options = defTypes.Select(t => _wizard.UseChinese ? t.NameZh : t.NameEn).ToList();
            var selectedName = ReadChoice(_wizard.T("请选择 Def 类型", "Please select Def type"), options);

            var selectedIndex = options.IndexOf(selectedName);
            var selectedType = defTypes[selectedIndex];

            _wizard.Config.TargetDefType = selectedType.Type;
            context.SetData("SelectedDefType", selectedType);

            Console.WriteLine();
            Console.WriteLine(_wizard.T("=== 目标 Def 名称 ===", "=== Target Def Name ==="));
            Console.WriteLine();

            if (selectedType.ExampleDefNames.Count > 0)
            {
                Console.WriteLine(_wizard.T("示例 Def 名称:", "Example Def names:"));
                foreach (var example in selectedType.ExampleDefNames)
                {
                    Console.WriteLine($"  - {example}");
                }
                Console.WriteLine();
            }

            _wizard.Config.TargetDefName = ReadInput(_wizard.T("请输入目标 Def 的 defName", "Enter the defName of the target Def"), 
                selectedType.ExampleDefNames.FirstOrDefault() ?? "TargetDef");

            Console.WriteLine();
            Console.WriteLine(_wizard.T("=== 常用 XPath 示例 ===", "=== Common XPath Examples ==="));
            Console.WriteLine();

            if (selectedType.CommonXPaths.Count > 0)
            {
                foreach (var xpath in selectedType.CommonXPaths)
                {
                    var displayXPath = xpath.Replace("XXX", _wizard.Config.TargetDefName);
                    Console.WriteLine($"  {displayXPath}");
                }
                Console.WriteLine();
            }

            ShowSuccess(_wizard.T($"已选择目标: {_wizard.Config.TargetDefName}", $"Selected target: {_wizard.Config.TargetDefName}"));
        }
    }

    public class XmlPatchOperationSelectStep : WizardStepBase
    {
        private readonly XmlPatchWizard _wizard;

        public XmlPatchOperationSelectStep(XmlPatchWizard wizard)
            : base(wizard.T("操作类型选择", "Operation Type Selection"),
                   wizard.T("选择要执行的 Patch 操作类型", "Select the Patch operation type to perform"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();

            Console.WriteLine(_wizard.T("=== PatchOperation 类型 ===", "=== PatchOperation Types ==="));
            Console.WriteLine();

            var operationTypes = _wizard.OperationTypes;
            for (int i = 0; i < operationTypes.Count; i++)
            {
                var op = operationTypes[i];
                var name = _wizard.UseChinese ? op.NameZh : op.NameEn;
                var desc = _wizard.UseChinese ? op.DescriptionZh : op.DescriptionEn;
                Console.WriteLine($"  [{i + 1}] {name}");
                Console.WriteLine($"      {desc}");
                Console.WriteLine();
            }

            var options = operationTypes.Select(t => _wizard.UseChinese ? t.NameZh : t.NameEn).ToList();
            var selectedName = ReadChoice(_wizard.T("请选择操作类型", "Please select operation type"), options);

            var selectedIndex = options.IndexOf(selectedName);
            var selectedOp = operationTypes[selectedIndex];

            _wizard.Config.OperationType = selectedOp.Type;
            context.SetData("SelectedOperation", selectedOp);

            ShowSuccess(_wizard.T($"已选择操作: {selectedName}", $"Selected operation: {selectedName}"));
        }
    }

    public class XmlPatchValueConfigStep : WizardStepBase
    {
        private readonly XmlPatchWizard _wizard;

        public XmlPatchValueConfigStep(XmlPatchWizard wizard)
            : base(wizard.T("XPath 和值配置", "XPath and Value Configuration"),
                   wizard.T("配置 XPath 路径和修改值", "Configure XPath path and modification values"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();

            var selectedDefType = context.GetData<DefTypeInfo>("SelectedDefType");
            var selectedOp = context.GetData<PatchOperationInfo>("SelectedOperation");

            Console.WriteLine(_wizard.T($"配置: {_wizard.Config.TargetDefName} 的 {(_wizard.UseChinese ? selectedOp?.NameZh : selectedOp?.NameEn)} 操作",
                                       $"Configuring: {_wizard.Config.TargetDefName} - {selectedOp?.NameEn} operation"));
            Console.WriteLine();

            Console.WriteLine(_wizard.T("=== XPath 配置 ===", "=== XPath Configuration ==="));
            Console.WriteLine();

            var defaultXPath = $"/Defs/{GetDefTypeName()}" +
                              $"[defName=\"{_wizard.Config.TargetDefName}\"]";

            Console.WriteLine(_wizard.T("XPath 基础路径:", "XPath base path:"));
            Console.WriteLine($"  {defaultXPath}");
            Console.WriteLine();

            var xpathSuffix = ReadInput(_wizard.T("请输入 XPath 后缀 (如 /label, /statBases/MaxHitPoints)",
                "Enter XPath suffix (e.g., /label, /statBases/MaxHitPoints)"), "");

            _wizard.Config.XPath = defaultXPath + xpathSuffix;

            Console.WriteLine();
            Console.WriteLine(_wizard.T("完整 XPath:", "Complete XPath:"));
            Console.WriteLine($"  {_wizard.Config.XPath}");
            Console.WriteLine();

            if (_wizard.Config.OperationType != PatchOperationType.Remove)
            {
                ConfigureValues(context);
            }

            if (_wizard.Config.OperationType == PatchOperationType.Conditional)
            {
                ConfigureConditional(context);
            }

            context.SetData("ValuesConfigured", true);
            ShowSuccess(_wizard.T("配置完成", "Configuration completed"));
        }

        private string GetDefTypeName()
        {
            if (_wizard.Config.TargetDefType == DefType.Custom)
            {
                return _wizard.Config.CustomDefType;
            }
            return _wizard.Config.TargetDefType.ToString();
        }

        private void ConfigureValues(WizardContext context)
        {
            Console.WriteLine(_wizard.T("=== 值配置 ===", "=== Value Configuration ==="));
            Console.WriteLine();

            if (_wizard.Config.OperationType == PatchOperationType.Multiply)
            {
                var multiplier = ReadFloat(_wizard.T("请输入乘数", "Enter multiplier"), 1.0f);
                _wizard.Config.Values.Add(new PatchValue { Value = multiplier.ToString() });
                return;
            }

            var addMore = true;
            while (addMore)
            {
                var value = new PatchValue();

                value.ElementName = ReadInput(_wizard.T("元素名称 (如 label, description, MaxHitPoints)",
                    "Element name (e.g., label, description, MaxHitPoints)"), "value");

                value.Value = ReadInput(_wizard.T("元素值", "Element value"), "");

                var hasAttribute = ReadBool(_wizard.T("是否添加属性?", "Add attribute?"), false);
                if (hasAttribute)
                {
                    value.AttributeName = ReadInput(_wizard.T("属性名称", "Attribute name"), "");
                    value.AttributeValue = ReadInput(_wizard.T("属性值", "Attribute value"), "");
                }

                _wizard.Config.Values.Add(value);
                ShowSuccess(_wizard.T($"已添加值: {value.ElementName} = {value.Value}",
                                     $"Added value: {value.ElementName} = {value.Value}"));

                addMore = ReadBool(_wizard.T("是否继续添加值?", "Add more values?"), false);
            }
        }

        private void ConfigureConditional(WizardContext context)
        {
            Console.WriteLine();
            Console.WriteLine(_wizard.T("=== 条件配置 ===", "=== Conditional Configuration ==="));
            Console.WriteLine();

            _wizard.Config.ConditionalXPath = ReadInput(
                _wizard.T("条件 XPath (检查条件是否满足)", "Conditional XPath (check if condition is met)"),
                _wizard.Config.XPath);

            Console.WriteLine();
            Console.WriteLine(_wizard.T("条件满足时执行的操作:", "Operations when condition is met:"));
            var matchOp = ReadChoice(_wizard.T("选择匹配时操作", "Select match operation"),
                new[] { "Replace", "Add", "Remove" }, "Replace");
            context.SetData("MatchOperation", matchOp);

            var hasNoMatch = ReadBool(_wizard.T("是否配置不匹配时的操作?", "Configure no-match operation?"), false);
            if (hasNoMatch)
            {
                var noMatchOp = ReadChoice(_wizard.T("选择不匹配时操作", "Select no-match operation"),
                    new[] { "Add", "None" }, "Add");
                context.SetData("NoMatchOperation", noMatchOp);
            }
        }

        private float ReadFloat(string prompt, float defaultValue)
        {
            while (true)
            {
                var input = ReadInput(prompt, defaultValue.ToString());
                if (string.IsNullOrEmpty(input))
                    return defaultValue;

                if (float.TryParse(input, out var result))
                    return result;

                ShowError(_wizard.T("请输入有效的数字", "Please enter a valid number"));
            }
        }
    }

    public class XmlPatchCodeGenerationStep : WizardStepBase
    {
        private readonly XmlPatchWizard _wizard;

        public XmlPatchCodeGenerationStep(XmlPatchWizard wizard)
            : base(wizard.T("代码生成", "Code Generation"),
                   wizard.T("生成完整的 PatchOperation XML", "Generate complete PatchOperation XML"))
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

            generatedCode.AppendLine(GeneratePatchXml());
            generatedCode.AppendLine();

            generatedCode.AppendLine(GenerateAboutXml());

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
            sb.AppendLine("|-- Patches/");
            sb.AppendLine("|   |-- Patch_*.xml");
            sb.AppendLine();

            return sb.ToString();
        }

        private string GeneratePatchXml()
        {
            var sb = new StringBuilder();

            sb.AppendLine(_wizard.T("=== Patch XML ===", "=== Patch XML ==="));
            sb.AppendLine();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sb.AppendLine("<Patch>");

            switch (_wizard.Config.OperationType)
            {
                case PatchOperationType.Add:
                    GenerateAddOperation(sb);
                    break;
                case PatchOperationType.Replace:
                    GenerateReplaceOperation(sb);
                    break;
                case PatchOperationType.Remove:
                    GenerateRemoveOperation(sb);
                    break;
                case PatchOperationType.Insert:
                    GenerateInsertOperation(sb);
                    break;
                case PatchOperationType.Conditional:
                    GenerateConditionalOperation(sb);
                    break;
                case PatchOperationType.Sequence:
                    GenerateSequenceOperation(sb);
                    break;
                case PatchOperationType.Multiply:
                    GenerateMultiplyOperation(sb);
                    break;
                case PatchOperationType.Set:
                    GenerateSetOperation(sb);
                    break;
                default:
                    GenerateReplaceOperation(sb);
                    break;
            }

            sb.AppendLine("</Patch>");

            return sb.ToString();
        }

        private void GenerateAddOperation(StringBuilder sb)
        {
            sb.AppendLine($"    <!-- {_wizard.T("添加元素到", "Add elements to")}: {_wizard.Config.XPath} -->");
            sb.AppendLine("    <Operation Class=\"PatchOperationAdd\">");
            sb.AppendLine($"        <xpath>{_wizard.Config.XPath}</xpath>");
            sb.AppendLine("        <value>");

            foreach (var value in _wizard.Config.Values)
            {
                GenerateValueElement(sb, value, 3);
            }

            sb.AppendLine("        </value>");
            sb.AppendLine("    </Operation>");
        }

        private void GenerateReplaceOperation(StringBuilder sb)
        {
            sb.AppendLine($"    <!-- {_wizard.T("替换元素", "Replace element")}: {_wizard.Config.XPath} -->");
            sb.AppendLine("    <Operation Class=\"PatchOperationReplace\">");
            sb.AppendLine($"        <xpath>{_wizard.Config.XPath}</xpath>");
            sb.AppendLine("        <value>");

            foreach (var value in _wizard.Config.Values)
            {
                GenerateValueElement(sb, value, 3);
            }

            sb.AppendLine("        </value>");
            sb.AppendLine("    </Operation>");
        }

        private void GenerateRemoveOperation(StringBuilder sb)
        {
            sb.AppendLine($"    <!-- {_wizard.T("移除元素", "Remove element")}: {_wizard.Config.XPath} -->");
            sb.AppendLine("    <Operation Class=\"PatchOperationRemove\">");
            sb.AppendLine($"        <xpath>{_wizard.Config.XPath}</xpath>");
            sb.AppendLine("    </Operation>");
        }

        private void GenerateInsertOperation(StringBuilder sb)
        {
            sb.AppendLine($"    <!-- {_wizard.T("插入元素到", "Insert elements at")}: {_wizard.Config.XPath} -->");
            sb.AppendLine("    <Operation Class=\"PatchOperationInsert\">");
            sb.AppendLine($"        <xpath>{_wizard.Config.XPath}</xpath>");
            sb.AppendLine("        <value>");

            foreach (var value in _wizard.Config.Values)
            {
                GenerateValueElement(sb, value, 3);
            }

            sb.AppendLine("        </value>");
            sb.AppendLine("    </Operation>");
        }

        private void GenerateConditionalOperation(StringBuilder sb)
        {
            sb.AppendLine($"    <!-- {_wizard.T("条件操作", "Conditional operation")}: {_wizard.Config.ConditionalXPath} -->");
            sb.AppendLine("    <Operation Class=\"PatchOperationConditional\">");
            sb.AppendLine($"        <xpath>{_wizard.Config.ConditionalXPath}</xpath>");
            sb.AppendLine("        <match Class=\"PatchOperationReplace\">");
            sb.AppendLine($"            <xpath>{_wizard.Config.XPath}</xpath>");
            sb.AppendLine("            <value>");

            foreach (var value in _wizard.Config.Values)
            {
                GenerateValueElement(sb, value, 4);
            }

            sb.AppendLine("            </value>");
            sb.AppendLine("        </match>");
            sb.AppendLine("    </Operation>");
        }

        private void GenerateSequenceOperation(StringBuilder sb)
        {
            sb.AppendLine($"    <!-- {_wizard.T("序列操作", "Sequence operation")} -->");
            sb.AppendLine("    <Operation Class=\"PatchOperationSequence\">");
            sb.AppendLine("        <operations>");
            sb.AppendLine($"            <!-- {_wizard.T("在此添加多个操作", "Add multiple operations here")} -->");
            sb.AppendLine($"            <li Class=\"PatchOperationReplace\">");
            sb.AppendLine($"                <xpath>{_wizard.Config.XPath}</xpath>");
            sb.AppendLine("                <value>");

            foreach (var value in _wizard.Config.Values)
            {
                GenerateValueElement(sb, value, 5);
            }

            sb.AppendLine("                </value>");
            sb.AppendLine("            </li>");
            sb.AppendLine("        </operations>");
            sb.AppendLine("    </Operation>");
        }

        private void GenerateMultiplyOperation(StringBuilder sb)
        {
            var multiplier = _wizard.Config.Values.FirstOrDefault()?.Value ?? "1.0";
            sb.AppendLine($"    <!-- {_wizard.T("乘法操作", "Multiply operation")}: {_wizard.Config.XPath} × {multiplier} -->");
            sb.AppendLine("    <Operation Class=\"PatchOperationMultiply\">");
            sb.AppendLine($"        <xpath>{_wizard.Config.XPath}</xpath>");
            sb.AppendLine($"        <value>{multiplier}</value>");
            sb.AppendLine("    </Operation>");
        }

        private void GenerateSetOperation(StringBuilder sb)
        {
            sb.AppendLine($"    <!-- {_wizard.T("设置值", "Set value")}: {_wizard.Config.XPath} -->");
            sb.AppendLine("    <Operation Class=\"PatchOperationSet\">");
            sb.AppendLine($"        <xpath>{_wizard.Config.XPath}</xpath>");
            sb.AppendLine("        <value>");

            foreach (var value in _wizard.Config.Values)
            {
                GenerateValueElement(sb, value, 3);
            }

            sb.AppendLine("        </value>");
            sb.AppendLine("    </Operation>");
        }

        private void GenerateValueElement(StringBuilder sb, PatchValue value, int indentLevel)
        {
            var indent = new string(' ', indentLevel * 4);

            if (!string.IsNullOrEmpty(value.AttributeName))
            {
                sb.AppendLine($"{indent}<{value.ElementName} {value.AttributeName}=\"{value.AttributeValue}\">{value.Value}</{value.ElementName}>");
            }
            else
            {
                sb.AppendLine($"{indent}<{value.ElementName}>{value.Value}</{value.ElementName}>");
            }

            foreach (var child in value.Children)
            {
                GenerateValueElement(sb, child, indentLevel + 1);
            }
        }

        private string GenerateAboutXml()
        {
            var sb = new StringBuilder();

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
    }

    public class XmlPatchTestSuggestionStep : WizardStepBase
    {
        private readonly XmlPatchWizard _wizard;

        public XmlPatchTestSuggestionStep(XmlPatchWizard wizard)
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
            Console.WriteLine(_wizard.T("   * 将 Patch XML 文件放入 Patches 文件夹", "   * Place Patch XML files in Patches folder"));
            Console.WriteLine();

            Console.WriteLine(_wizard.T("2. 验证 XML 语法", "2. Validate XML syntax"));
            Console.WriteLine(_wizard.T("   * 确保 XML 标签正确闭合", "   * Ensure XML tags are properly closed"));
            Console.WriteLine(_wizard.T("   * 检查特殊字符是否正确转义", "   * Check if special characters are properly escaped"));
            Console.WriteLine();

            Console.WriteLine(_wizard.T("3. 游戏内测试", "3. In-game testing"));
            Console.WriteLine(_wizard.T("   * 启动游戏并加载 Mod", "   * Start game and load the mod"));
            Console.WriteLine(_wizard.T("   * 检查修改是否生效", "   * Check if modifications are applied"));
            Console.WriteLine(_wizard.T("   * 查看错误日志确认无报错", "   * Check error log for any errors"));
            Console.WriteLine();

            Console.WriteLine(_wizard.T("=== XPath 技巧 ===", "=== XPath Tips ==="));
            Console.WriteLine();
            Console.WriteLine(_wizard.T("常用 XPath 选择器:", "Common XPath selectors:"));
            Console.WriteLine("  * // : " + _wizard.T("选择所有匹配的元素", "Select all matching elements"));
            Console.WriteLine("  * @ : " + _wizard.T("选择属性", "Select attribute"));
            Console.WriteLine("  * [] : " + _wizard.T("条件过滤", "Conditional filter"));
            Console.WriteLine("  * | : " + _wizard.T("选择多个路径", "Select multiple paths"));
            Console.WriteLine();
            Console.WriteLine(_wizard.T("示例:", "Examples:"));
            Console.WriteLine("  /Defs/ThingDef[defName=\"Steel\"]/label");
            Console.WriteLine("  /Defs/ThingDef[@ParentName=\"BaseWeapon\"]/statBases/MaxHitPoints");
            Console.WriteLine("  /Defs/ThingDef[defName=\"Steel\"] | /Defs/ThingDef[defName=\"WoodLog\"]");
            Console.WriteLine();

            Console.WriteLine(_wizard.T("=== 常见问题 ===", "=== Common Issues ==="));
            Console.WriteLine();
            Console.WriteLine(_wizard.T("1. Patch 没有生效", "1. Patch not working"));
            Console.WriteLine(_wizard.T("   * 检查 XPath 是否正确", "   * Check if XPath is correct"));
            Console.WriteLine(_wizard.T("   * 确认 Def 名称拼写正确", "   * Confirm Def name is spelled correctly"));
            Console.WriteLine(_wizard.T("   * 检查 Mod 加载顺序", "   * Check mod load order"));
            Console.WriteLine();
            Console.WriteLine(_wizard.T("2. 游戏报错", "2. Game errors"));
            Console.WriteLine(_wizard.T("   * 查看 Player.log 获取详细错误信息", "   * Check Player.log for detailed error info"));
            Console.WriteLine(_wizard.T("   * 检查 XML 语法是否正确", "   * Check if XML syntax is correct"));
            Console.WriteLine(_wizard.T("   * 确认 value 结构与目标匹配", "   * Confirm value structure matches target"));
            Console.WriteLine();

            Console.WriteLine(_wizard.T("=== 调试技巧 ===", "=== Debug Tips ==="));
            Console.WriteLine();
            Console.WriteLine(_wizard.T("查看错误日志:", "View error log:"));
            Console.WriteLine("  - Windows: %USERPROFILE%\\AppData\\LocalLow\\Ludeon Studios\\RimWorld by Ludeon Studios\\Player.log");
            Console.WriteLine();
            Console.WriteLine(_wizard.T("使用开发模式:", "Use development mode:"));
            Console.WriteLine("  - " + _wizard.T("启用开发模式后可查看 Def 信息", "Enable dev mode to view Def information"));
            Console.WriteLine("  - " + _wizard.T("使用 Debug Actions 菜单测试功能", "Use Debug Actions menu to test features"));
            Console.WriteLine();

            Pause(_wizard.T("按任意键完成向导...", "Press any key to finish wizard..."));

            ShowSuccess(_wizard.T("向导完成！", "Wizard completed!"));
            Console.WriteLine();
            Console.WriteLine(_wizard.T("生成的代码已保存到上下文中，您可以复制使用。", "Generated code is saved in context, you can copy and use it."));
        }
    }
}
