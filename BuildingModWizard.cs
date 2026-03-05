using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorldModDevProbe.Core;
using RimWorldModDevProbe.Wizards.Core;

namespace RimWorldModDevProbe
{
    public enum BuildingType
    {
        Workbench,
        Defense,
        Furniture,
        Power,
        Production,
        Storage,
        Decoration,
        Custom
    }

    public enum BuildingSize
    {
        Small1x1,
        Medium2x2,
        Large3x3,
        ExtraLarge
    }

    public class BuildingModConfig
    {
        public BuildingType BuildingType { get; set; }
        public BuildingSize BuildingSize { get; set; }
        public string ModName { get; set; }
        public string AuthorName { get; set; }
        public string Description { get; set; }
        public string BuildingDefName { get; set; }
        public string BuildingLabelZh { get; set; }
        public string BuildingLabelEn { get; set; }
        public string BuildingDescriptionZh { get; set; }
        public string BuildingDescriptionEn { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int MaxHitPoints { get; set; }
        public float Flammability { get; set; }
        public int MarketValue { get; set; }
        public float Mass { get; set; }
        public int WorkToBuild { get; set; }
        public List<string> DesignationCategory { get; set; } = new List<string>();
        public string MinifiableType { get; set; }
        public float MinifiableWorkToMake { get; set; }
        public int MinifiableSize { get; set; }
        public List<StuffCategoryDef> StuffCategories { get; set; } = new List<StuffCategoryDef>();
        public float PowerConsumption { get; set; }
        public bool NeedsPower { get; set; }
        public float FuelConsumption { get; set; }
        public string FuelType { get; set; }
        public bool NeedsFuel { get; set; }
        public int WorkSpeedFactor { get; set; }
        public List<string> WorkTypes { get; set; } = new List<string>();
        public List<string> Recipes { get; set; } = new List<string>();
        public int StorageCapacity { get; set; }
        public bool IsStorage { get; set; }
        public string GraphicPath { get; set; }
        public string GraphicClass { get; set; }
        public List<string> CostList { get; set; } = new List<string>();
        public List<string> ResearchPrerequisites { get; set; } = new List<string>();
        public string SoundBuildDefName { get; set; }
        public string SoundDestroyDefName { get; set; }
        public string SoundInteractDefName { get; set; }
        public bool UseBilingual { get; set; } = true;
    }

    public class StuffCategoryDef
    {
        public string Name { get; set; }
        public string DisplayNameZh { get; set; }
        public string DisplayNameEn { get; set; }
    }

    public class BuildingTypeInfo
    {
        public BuildingType Type { get; set; }
        public string NameZh { get; set; }
        public string NameEn { get; set; }
        public string DescriptionZh { get; set; }
        public string DescriptionEn { get; set; }
        public List<string> ExampleDefs { get; set; } = new List<string>();
        public string DefaultCategory { get; set; }
        public List<string> DefaultWorkTypes { get; set; } = new List<string>();
    }

    public class BuildingModWizard
    {
        private readonly ProbeContext _context;
        private readonly DevWizard _innerWizard;
        private readonly BuildingModConfig _config;
        private readonly List<BuildingTypeInfo> _buildingTypes;
        private bool _useChinese = true;

        public BuildingModWizard(ProbeContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _config = new BuildingModConfig();
            _buildingTypes = InitializeBuildingTypes();
            _innerWizard = new DevWizard(context);
            
            SetupWizardSteps();
        }

        private List<BuildingTypeInfo> InitializeBuildingTypes()
        {
            return new List<BuildingTypeInfo>
            {
                new BuildingTypeInfo
                {
                    Type = BuildingType.Workbench,
                    NameZh = "工作台",
                    NameEn = "Workbench",
                    DescriptionZh = "用于生产制作的工作台，如加工台、研究台等",
                    DescriptionEn = "Workbenches for production and crafting, such as machining tables, research benches, etc.",
                    ExampleDefs = new List<string> { "TableMachining", "TableResearch", "TableButcher" },
                    DefaultCategory = "Production",
                    DefaultWorkTypes = new List<string> { "Crafting", "Research", "Cooking" }
                },
                new BuildingTypeInfo
                {
                    Type = BuildingType.Defense,
                    NameZh = "防御建筑",
                    NameEn = "Defense Building",
                    DescriptionZh = "防御性建筑，如炮塔、陷阱、城墙等",
                    DescriptionEn = "Defensive structures such as turrets, traps, walls, etc.",
                    ExampleDefs = new List<string> { "Turret_MiniTurret", "TrapSpike", "Wall" },
                    DefaultCategory = "Security",
                    DefaultWorkTypes = new List<string>()
                },
                new BuildingTypeInfo
                {
                    Type = BuildingType.Furniture,
                    NameZh = "家具",
                    NameEn = "Furniture",
                    DescriptionZh = "家具类建筑，如床、椅子、桌子等",
                    DescriptionEn = "Furniture such as beds, chairs, tables, etc.",
                    ExampleDefs = new List<string> { "Bed", "Chair", "Table1x1c" },
                    DefaultCategory = "Furniture",
                    DefaultWorkTypes = new List<string>()
                },
                new BuildingTypeInfo
                {
                    Type = BuildingType.Power,
                    NameZh = "电力设施",
                    NameEn = "Power Building",
                    DescriptionZh = "电力相关建筑，如发电机、电池、电缆等",
                    DescriptionEn = "Power-related buildings such as generators, batteries, cables, etc.",
                    ExampleDefs = new List<string> { "SolarGenerator", "Battery", "PowerConduit" },
                    DefaultCategory = "Power",
                    DefaultWorkTypes = new List<string>()
                },
                new BuildingTypeInfo
                {
                    Type = BuildingType.Production,
                    NameZh = "生产设施",
                    NameEn = "Production Building",
                    DescriptionZh = "生产类建筑，如种植盆、动物饲养区等",
                    DescriptionEn = "Production buildings such as growing zones, animal pens, etc.",
                    ExampleDefs = new List<string> { "HydroponicsBasin", "AnimalPenMarker" },
                    DefaultCategory = "Production",
                    DefaultWorkTypes = new List<string> { "Growing", "Animals" }
                },
                new BuildingTypeInfo
                {
                    Type = BuildingType.Storage,
                    NameZh = "存储设施",
                    NameEn = "Storage Building",
                    DescriptionZh = "存储类建筑，如货架、储物区等",
                    DescriptionEn = "Storage buildings such as shelves, storage zones, etc.",
                    ExampleDefs = new List<string> { "Shelf", "StorageZone" },
                    DefaultCategory = "Storage",
                    DefaultWorkTypes = new List<string>()
                },
                new BuildingTypeInfo
                {
                    Type = BuildingType.Decoration,
                    NameZh = "装饰建筑",
                    NameEn = "Decoration Building",
                    DescriptionZh = "装饰性建筑，如雕像、花盆等",
                    DescriptionEn = "Decorative buildings such as statues, plant pots, etc.",
                    ExampleDefs = new List<string> { "Statue", "PlantPot" },
                    DefaultCategory = "Structure",
                    DefaultWorkTypes = new List<string>()
                },
                new BuildingTypeInfo
                {
                    Type = BuildingType.Custom,
                    NameZh = "自定义建筑",
                    NameEn = "Custom Building",
                    DescriptionZh = "自定义类型的建筑",
                    DescriptionEn = "Custom type building",
                    ExampleDefs = new List<string>(),
                    DefaultCategory = "Structure",
                    DefaultWorkTypes = new List<string>()
                }
            };
        }

        private void SetupWizardSteps()
        {
            _innerWizard.AddStep(new BuildingWelcomeStep(this));
            _innerWizard.AddStep(new BuildingLanguageSelectStep(this));
            _innerWizard.AddStep(new BuildingTypeSelectStep(this));
            _innerWizard.AddStep(new BuildingBasicInfoStep(this));
            _innerWizard.AddStep(new BuildingFunctionConfigStep(this));
            _innerWizard.AddStep(new BuildingCodeGenerationStep(this));
            _innerWizard.AddStep(new BuildingTestSuggestionStep(this));
        }

        public WizardResult Run()
        {
            return _innerWizard.Run();
        }

        public BuildingModConfig Config => _config;
        public bool UseChinese => _useChinese;
        public List<BuildingTypeInfo> BuildingTypes => _buildingTypes;
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

    public class BuildingWelcomeStep : WizardStepBase
    {
        private readonly BuildingModWizard _wizard;

        public BuildingWelcomeStep(BuildingModWizard wizard) 
            : base(wizard.T("欢迎 - 建筑 Mod 向导", "Welcome - Building Mod Wizard"), 
                   wizard.T("本向导将引导您完成建筑 Mod 的开发流程", "This wizard will guide you through the building mod development process"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();
            
            Console.WriteLine(_wizard.T(
                "欢迎使用建筑 Mod 向导！\n\n" +
                "本向导将帮助您：\n" +
                "  1. 选择建筑类型（工作台/防御/家具等）\n" +
                "  2. 配置建筑基本属性\n" +
                "  3. 设置建筑功能参数\n" +
                "  4. 生成完整的 ThingDef XML 代码\n" +
                "  5. 获取测试和调试建议\n",
                "Welcome to the Building Mod Wizard!\n\n" +
                "This wizard will help you:\n" +
                "  1. Select building type (Workbench/Defense/Furniture etc.)\n" +
                "  2. Configure building basic properties\n" +
                "  3. Set building function parameters\n" +
                "  4. Generate complete ThingDef XML code\n" +
                "  5. Get testing and debugging suggestions\n"));
            
            Pause(_wizard.T("按任意键继续...", "Press any key to continue..."));
        }
    }

    public class BuildingLanguageSelectStep : WizardStepBase
    {
        private readonly BuildingModWizard _wizard;

        public BuildingLanguageSelectStep(BuildingModWizard wizard) 
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

    public class BuildingTypeSelectStep : WizardStepBase
    {
        private readonly BuildingModWizard _wizard;

        public BuildingTypeSelectStep(BuildingModWizard wizard) 
            : base(wizard.T("建筑类型选择", "Building Type Selection"), 
                   wizard.T("选择您要创建的建筑类型", "Select the building type you want to create"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();
            
            Console.WriteLine(_wizard.T("可用建筑类型:", "Available building types:"));
            Console.WriteLine();
            
            var buildingTypes = _wizard.BuildingTypes;
            for (int i = 0; i < buildingTypes.Count; i++)
            {
                var type = buildingTypes[i];
                var name = _wizard.UseChinese ? type.NameZh : type.NameEn;
                var desc = _wizard.UseChinese ? type.DescriptionZh : type.DescriptionEn;
                Console.WriteLine($"  [{i + 1}] {name}");
                Console.WriteLine($"      {desc}");
                Console.WriteLine();
            }
            
            var options = buildingTypes.Select(t => _wizard.UseChinese ? t.NameZh : t.NameEn).ToList();
            var selectedName = ReadChoice(_wizard.T("请选择建筑类型", "Please select building type"), options);
            
            var selectedIndex = options.IndexOf(selectedName);
            var selectedType = buildingTypes[selectedIndex];
            
            _wizard.Config.BuildingType = selectedType.Type;
            context.SetData("SelectedBuildingType", selectedType);
            
            ShowInfo(_wizard.T($"已选择: {selectedName}", $"Selected: {selectedName}"));
            
            Console.WriteLine();
            Console.WriteLine(_wizard.T("选择建筑尺寸:", "Select building size:"));
            var sizes = new List<string>
            {
                _wizard.T("小型 (1x1)", "Small (1x1)"),
                _wizard.T("中型 (2x2)", "Medium (2x2)"),
                _wizard.T("大型 (3x3)", "Large (3x3)"),
                _wizard.T("超大 (自定义)", "Extra Large (Custom)")
            };
            
            for (int i = 0; i < sizes.Count; i++)
            {
                Console.WriteLine($"  [{i + 1}] {sizes[i]}");
            }
            
            var selectedSize = ReadChoice(_wizard.T("请选择建筑尺寸", "Please select building size"), sizes);
            _wizard.Config.BuildingSize = (BuildingSize)sizes.IndexOf(selectedSize);
            
            if (_wizard.Config.BuildingSize == BuildingSize.ExtraLarge)
            {
                _wizard.Config.Width = ReadInt(_wizard.T("请输入宽度", "Enter width"), 4, 1, 20);
                _wizard.Config.Height = ReadInt(_wizard.T("请输入高度", "Enter height"), 4, 1, 20);
            }
            else
            {
                var sizeMap = new Dictionary<BuildingSize, (int w, int h)>
                {
                    { BuildingSize.Small1x1, (1, 1) },
                    { BuildingSize.Medium2x2, (2, 2) },
                    { BuildingSize.Large3x3, (3, 3) }
                };
                var (w, h) = sizeMap[_wizard.Config.BuildingSize];
                _wizard.Config.Width = w;
                _wizard.Config.Height = h;
            }
        }
    }

    public class BuildingBasicInfoStep : WizardStepBase
    {
        private readonly BuildingModWizard _wizard;

        public BuildingBasicInfoStep(BuildingModWizard wizard) 
            : base(wizard.T("基本信息配置", "Basic Information Configuration"), 
                   wizard.T("配置建筑的基本信息", "Configure building basic information"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();
            
            var buildingType = context.GetData<BuildingTypeInfo>("SelectedBuildingType");
            if (buildingType == null)
            {
                ShowError(_wizard.T("未找到建筑类型信息", "Building type information not found"));
                return;
            }
            
            Console.WriteLine(_wizard.T($"正在配置: {(_wizard.UseChinese ? buildingType.NameZh : buildingType.NameEn)}", 
                                       $"Configuring: {buildingType.NameEn}"));
            Console.WriteLine();
            
            _wizard.Config.ModName = ReadInput(_wizard.T("Mod 名称", "Mod name"), "MyBuildingMod");
            _wizard.Config.AuthorName = ReadInput(_wizard.T("作者名称", "Author name"), "YourName");
            _wizard.Config.Description = ReadInput(_wizard.T("Mod 描述", "Mod description"), 
                _wizard.T("自定义建筑 Mod", "Custom building mod"));
            
            Console.WriteLine();
            Console.WriteLine(_wizard.T("=== 建筑定义信息 ===", "=== Building Definition Info ==="));
            Console.WriteLine();
            
            _wizard.Config.BuildingDefName = ReadInput(
                _wizard.T("建筑定义名称 (defName)", "Building defName"), 
                "MyCustomBuilding");
            
            _wizard.Config.BuildingLabelZh = ReadInput(
                _wizard.T("建筑名称 (中文)", "Building label (Chinese)"), 
                "自定义建筑");
            
            _wizard.Config.BuildingLabelEn = ReadInput(
                _wizard.T("建筑名称 (英文)", "Building label (English)"), 
                "Custom Building");
            
            _wizard.Config.BuildingDescriptionZh = ReadInput(
                _wizard.T("建筑描述 (中文)", "Building description (Chinese)"), 
                "一个自定义的建筑。");
            
            _wizard.Config.BuildingDescriptionEn = ReadInput(
                _wizard.T("建筑描述 (英文)", "Building description (English)"), 
                "A custom building.");
            
            Console.WriteLine();
            Console.WriteLine(_wizard.T("=== 建筑属性 ===", "=== Building Properties ==="));
            Console.WriteLine();
            
            _wizard.Config.GraphicPath = ReadInput(
                _wizard.T("贴图路径 (相对路径)", "Graphic path (relative)"), 
                "Things/Building/Production/MyBuilding");
            
            _wizard.Config.GraphicClass = ReadInput(
                _wizard.T("贴图类型", "Graphic class"), 
                "Graphic_Single");
            
            _wizard.Config.MaxHitPoints = ReadInt(_wizard.T("耐久度", "Max hit points"), 200, 1, 10000);
            _wizard.Config.Flammability = float.Parse(ReadInput(_wizard.T("易燃性 (0-1)", "Flammability (0-1)"), "1.0"));
            _wizard.Config.MarketValue = ReadInt(_wizard.T("市场价值", "Market value"), 500, 1, 100000);
            _wizard.Config.WorkToBuild = ReadInt(_wizard.T("建造工作量", "Work to build"), 2000, 1, 100000);
            
            Console.WriteLine();
            Console.WriteLine(_wizard.T("=== 建造需求 ===", "=== Build Requirements ==="));
            Console.WriteLine();
            
            var categories = new List<string>
            {
                _wizard.T("生产 (Production)", "Production"),
                _wizard.T("安全 (Security)", "Security"),
                _wizard.T("家具 (Furniture)", "Furniture"),
                _wizard.T("电力 (Power)", "Power"),
                _wizard.T("建筑 (Structure)", "Structure"),
                _wizard.T("存储 (Storage)", "Storage")
            };
            
            Console.WriteLine(_wizard.T("选择建造分类:", "Select designation category:"));
            for (int i = 0; i < categories.Count; i++)
            {
                Console.WriteLine($"  [{i + 1}] {categories[i]}");
            }
            
            var selectedCategory = ReadChoice(_wizard.T("请选择分类", "Please select category"), categories);
            _wizard.Config.DesignationCategory.Add(selectedCategory.Contains("(") ? 
                selectedCategory.Split('(')[1].TrimEnd(')') : selectedCategory);
            
            Console.WriteLine();
            Console.WriteLine(_wizard.T("建造材料类型 (可多选，输入数字，用逗号分隔):", "Build material types (multiple, comma-separated):"));
            var stuffCategories = new List<string>
            {
                "Metallic (金属)",
                "Woody (木质)",
                "Stony (石材)",
                "Fabric (布料)",
                "Leathery (皮革)"
            };
            
            for (int i = 0; i < stuffCategories.Count; i++)
            {
                Console.WriteLine($"  [{i + 1}] {stuffCategories[i]}");
            }
            
            var stuffInput = ReadInput(_wizard.T("请选择材料类型", "Please select stuff categories"), "1,2,3");
            var stuffIndices = stuffInput.Split(',').Select(s => int.Parse(s.Trim()) - 1).Where(i => i >= 0 && i < stuffCategories.Count);
            foreach (var idx in stuffIndices)
            {
                var stuffName = stuffCategories[idx].Split('(')[1].TrimEnd(')');
                _wizard.Config.StuffCategories.Add(new StuffCategoryDef 
                { 
                    Name = stuffName,
                    DisplayNameZh = stuffCategories[idx].Split('(')[0].Trim(),
                    DisplayNameEn = stuffName
                });
            }
            
            ShowSuccess(_wizard.T("基本信息配置完成", "Basic information configuration completed"));
        }
    }

    public class BuildingFunctionConfigStep : WizardStepBase
    {
        private readonly BuildingModWizard _wizard;

        public BuildingFunctionConfigStep(BuildingModWizard wizard) 
            : base(wizard.T("功能配置", "Function Configuration"), 
                   wizard.T("配置建筑的功能参数", "Configure building function parameters"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();
            
            var buildingType = context.GetData<BuildingTypeInfo>("SelectedBuildingType");
            if (buildingType == null)
            {
                ShowError(_wizard.T("未找到建筑类型信息", "Building type information not found"));
                return;
            }
            
            Console.WriteLine(_wizard.T("=== 功能配置 ===", "=== Function Configuration ==="));
            Console.WriteLine();
            
            switch (buildingType.Type)
            {
                case BuildingType.Workbench:
                    ConfigureWorkbench(context);
                    break;
                case BuildingType.Defense:
                    ConfigureDefense(context);
                    break;
                case BuildingType.Furniture:
                    ConfigureFurniture(context);
                    break;
                case BuildingType.Power:
                    ConfigurePower(context);
                    break;
                case BuildingType.Production:
                    ConfigureProduction(context);
                    break;
                case BuildingType.Storage:
                    ConfigureStorage(context);
                    break;
                case BuildingType.Decoration:
                    ConfigureDecoration(context);
                    break;
                default:
                    ConfigureGeneric(context);
                    break;
            }
            
            Console.WriteLine();
            Console.WriteLine(_wizard.T("=== 音效配置 ===", "=== Sound Configuration ==="));
            Console.WriteLine();
            
            _wizard.Config.SoundBuildDefName = ReadInput(
                _wizard.T("建造音效 (可选)", "Build sound (optional)"), 
                "Building_Complete");
            _wizard.Config.SoundDestroyDefName = ReadInput(
                _wizard.T("摧毁音效 (可选)", "Destroy sound (optional)"), 
                "Building_Destroyed");
            _wizard.Config.SoundInteractDefName = ReadInput(
                _wizard.T("交互音效 (可选)", "Interact sound (optional)"), 
                "");
            
            ShowSuccess(_wizard.T("功能配置完成", "Function configuration completed"));
        }

        private void ConfigureWorkbench(WizardContext context)
        {
            Console.WriteLine(_wizard.T("--- 工作台专属配置 ---", "--- Workbench Specific Config ---"));
            Console.WriteLine();
            
            _wizard.Config.WorkSpeedFactor = ReadInt(_wizard.T("工作速度系数 (%)", "Work speed factor (%)"), 100, 1, 500);
            
            Console.WriteLine();
            Console.WriteLine(_wizard.T("是否需要电力?", "Does it need power?"));
            _wizard.Config.NeedsPower = ReadBool(_wizard.T("需要电力", "Needs power"), true);
            
            if (_wizard.Config.NeedsPower)
            {
                _wizard.Config.PowerConsumption = float.Parse(ReadInput(_wizard.T("电力消耗 (W)", "Power consumption (W)"), "200"));
            }
            
            Console.WriteLine();
            Console.WriteLine(_wizard.T("工作类型 (可多选，输入数字，用逗号分隔):", "Work types (multiple, comma-separated):"));
            var workTypes = new List<string>
            {
                "Crafting (制作)",
                "Cooking (烹饪)",
                "Research (研究)",
                "Smithing (锻造)",
                "Tailoring (裁缝)",
                "Art (艺术)"
            };
            
            for (int i = 0; i < workTypes.Count; i++)
            {
                Console.WriteLine($"  [{i + 1}] {workTypes[i]}");
            }
            
            var workInput = ReadInput(_wizard.T("请选择工作类型", "Please select work types"), "1");
            var workIndices = workInput.Split(',').Select(s => int.Parse(s.Trim()) - 1).Where(i => i >= 0 && i < workTypes.Count);
            foreach (var idx in workIndices)
            {
                var workName = workTypes[idx].Split('(')[1].TrimEnd(')');
                _wizard.Config.WorkTypes.Add(workName);
            }
        }

        private void ConfigureDefense(WizardContext context)
        {
            Console.WriteLine(_wizard.T("--- 防御建筑专属配置 ---", "--- Defense Building Specific Config ---"));
            Console.WriteLine();
            
            Console.WriteLine(_wizard.T("防御建筑类型:", "Defense building type:"));
            var defenseTypes = new List<string>
            {
                _wizard.T("炮塔", "Turret"),
                _wizard.T("陷阱", "Trap"),
                _wizard.T("城墙", "Wall"),
                _wizard.T("其他", "Other")
            };
            
            for (int i = 0; i < defenseTypes.Count; i++)
            {
                Console.WriteLine($"  [{i + 1}] {defenseTypes[i]}");
            }
            
            var selectedDefense = ReadChoice(_wizard.T("请选择类型", "Please select type"), defenseTypes);
            context.SetData("DefenseType", selectedDefense);
            
            if (selectedDefense == _wizard.T("炮塔", "Turret"))
            {
                Console.WriteLine();
                Console.WriteLine(_wizard.T("炮塔需要电力?", "Does turret need power?"));
                _wizard.Config.NeedsPower = ReadBool(_wizard.T("需要电力", "Needs power"), true);
                
                if (_wizard.Config.NeedsPower)
                {
                    _wizard.Config.PowerConsumption = float.Parse(ReadInput(_wizard.T("电力消耗 (W)", "Power consumption (W)"), "100"));
                }
            }
        }

        private void ConfigureFurniture(WizardContext context)
        {
            Console.WriteLine(_wizard.T("--- 家具专属配置 ---", "--- Furniture Specific Config ---"));
            Console.WriteLine();
            
            Console.WriteLine(_wizard.T("家具类型:", "Furniture type:"));
            var furnitureTypes = new List<string>
            {
                _wizard.T("床铺", "Bed"),
                _wizard.T("椅子", "Chair"),
                _wizard.T("桌子", "Table"),
                _wizard.T("其他", "Other")
            };
            
            for (int i = 0; i < furnitureTypes.Count; i++)
            {
                Console.WriteLine($"  [{i + 1}] {furnitureTypes[i]}");
            }
            
            var selectedFurniture = ReadChoice(_wizard.T("请选择类型", "Please select type"), furnitureTypes);
            context.SetData("FurnitureType", selectedFurniture);
            
            if (selectedFurniture == _wizard.T("床铺", "Bed"))
            {
                Console.WriteLine();
                Console.WriteLine(_wizard.T("床铺配置:", "Bed configuration:"));
                var bedCount = ReadInt(_wizard.T("床位数量", "Bed count"), 1, 1, 10);
                context.SetData("BedCount", bedCount);
            }
        }

        private void ConfigurePower(WizardContext context)
        {
            Console.WriteLine(_wizard.T("--- 电力设施专属配置 ---", "--- Power Building Specific Config ---"));
            Console.WriteLine();
            
            Console.WriteLine(_wizard.T("电力设施类型:", "Power building type:"));
            var powerTypes = new List<string>
            {
                _wizard.T("发电机", "Generator"),
                _wizard.T("电池", "Battery"),
                _wizard.T("电缆", "Conduit"),
                _wizard.T("其他", "Other")
            };
            
            for (int i = 0; i < powerTypes.Count; i++)
            {
                Console.WriteLine($"  [{i + 1}] {powerTypes[i]}");
            }
            
            var selectedPower = ReadChoice(_wizard.T("请选择类型", "Please select type"), powerTypes);
            context.SetData("PowerType", selectedPower);
            
            if (selectedPower == _wizard.T("发电机", "Generator"))
            {
                Console.WriteLine();
                _wizard.Config.PowerConsumption = -float.Parse(ReadInput(_wizard.T("发电功率 (W)", "Power generation (W)"), "1000"));
            }
            else if (selectedPower == _wizard.T("电池", "Battery"))
            {
                Console.WriteLine();
                var storageCapacity = ReadInt(_wizard.T("储能容量 (Wd)", "Storage capacity (Wd)"), 1000, 1, 100000);
                context.SetData("StorageCapacity", storageCapacity);
            }
        }

        private void ConfigureProduction(WizardContext context)
        {
            Console.WriteLine(_wizard.T("--- 生产设施专属配置 ---", "--- Production Building Specific Config ---"));
            Console.WriteLine();

            Console.WriteLine(_wizard.T("是否需要电力?", "Does it need power?"));
            _wizard.Config.NeedsPower = ReadBool(_wizard.T("需要电力", "Needs power"), true);

            if (_wizard.Config.NeedsPower)
            {
                _wizard.Config.PowerConsumption = float.Parse(ReadInput(_wizard.T("电力消耗 (W)", "Power consumption (W)"), "200"));
            }
        }

        private void ConfigureStorage(WizardContext context)
        {
            Console.WriteLine(_wizard.T("--- 存储设施专属配置 ---", "--- Storage Building Specific Config ---"));
            Console.WriteLine();
            
            _wizard.Config.IsStorage = true;
            _wizard.Config.StorageCapacity = ReadInt(_wizard.T("存储容量", "Storage capacity"), 100, 1, 10000);
        }

        private void ConfigureDecoration(WizardContext context)
        {
            Console.WriteLine(_wizard.T("--- 装饰建筑专属配置 ---", "--- Decoration Building Specific Config ---"));
            Console.WriteLine();
            
            Console.WriteLine(_wizard.T("装饰建筑通常不需要特殊功能配置。", "Decoration buildings usually don't need special function configuration."));
        }

        private void ConfigureGeneric(WizardContext context)
        {
            Console.WriteLine(_wizard.T("--- 通用建筑配置 ---", "--- Generic Building Config ---"));
            Console.WriteLine();
            
            Console.WriteLine(_wizard.T("是否需要电力?", "Does it need power?"));
            _wizard.Config.NeedsPower = ReadBool(_wizard.T("需要电力", "Needs power"), false);
            
            if (_wizard.Config.NeedsPower)
            {
                _wizard.Config.PowerConsumption = float.Parse(ReadInput(_wizard.T("电力消耗 (W)", "Power consumption (W)"), "100"));
            }
        }
    }

    public class BuildingCodeGenerationStep : WizardStepBase
    {
        private readonly BuildingModWizard _wizard;

        public BuildingCodeGenerationStep(BuildingModWizard wizard) 
            : base(wizard.T("代码生成", "Code Generation"), 
                   wizard.T("生成完整的 ThingDef XML 代码", "Generate complete ThingDef XML code"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();
            
            var buildingType = context.GetData<BuildingTypeInfo>("SelectedBuildingType");
            
            if (buildingType == null)
            {
                ShowError(_wizard.T("未找到建筑类型信息", "Building type information not found"));
                return;
            }
            
            var generatedCode = new StringBuilder();
            
            generatedCode.AppendLine(_wizard.T("=== 生成的代码 ===", "=== Generated Code ==="));
            generatedCode.AppendLine();
            
            generatedCode.AppendLine(GenerateModStructure());
            generatedCode.AppendLine();
            
            generatedCode.AppendLine(GenerateThingDefXml(buildingType, context));
            generatedCode.AppendLine();
            
            generatedCode.AppendLine(GenerateLanguageDefXml());
            
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
            sb.AppendLine("|   |-- ThingDefs_Buildings/");
            sb.AppendLine("|       |-- Buildings_*.xml");
            sb.AppendLine("|-- Languages/");
            sb.AppendLine("|   |-- ChineseSimplified/");
            sb.AppendLine("|   |   |-- DefInjected/");
            sb.AppendLine("|   |       |-- ThingDef/");
            sb.AppendLine("|   |           |-- Buildings_*.xml");
            sb.AppendLine("|   |-- English/");
            sb.AppendLine("|       |-- DefInjected/");
            sb.AppendLine("|           |-- ThingDef/");
            sb.AppendLine("|               |-- Buildings_*.xml");
            sb.AppendLine("|-- Textures/");
            sb.AppendLine("|   |-- Things/");
            sb.AppendLine("|       |-- Building/");
            sb.AppendLine("|           |-- *.png");
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

        private string GenerateThingDefXml(BuildingTypeInfo buildingType, WizardContext context)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine(_wizard.T("=== ThingDef XML ===", "=== ThingDef XML ==="));
            sb.AppendLine();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sb.AppendLine("<Defs>");
            
            GenerateBuildingDef(sb, buildingType, context);
            
            sb.AppendLine("</Defs>");
            
            return sb.ToString();
        }

        private void GenerateBuildingDef(StringBuilder sb, BuildingTypeInfo buildingType, WizardContext context)
        {
            sb.AppendLine("    <ThingDef ParentName=\"BuildingBase\">");
            sb.AppendLine($"        <defName>{_wizard.Config.BuildingDefName}</defName>");
            sb.AppendLine($"        <label>{_wizard.Config.BuildingLabelEn}</label>");
            sb.AppendLine($"        <description>{_wizard.Config.BuildingDescriptionEn}</description>");
            sb.AppendLine($"        <thingClass>Building</thingClass>");
            sb.AppendLine($"        <category>Building</category>");
            sb.AppendLine($"        <graphicData>");
            sb.AppendLine($"            <texPath>{_wizard.Config.GraphicPath}</texPath>");
            sb.AppendLine($"            <graphicClass>{_wizard.Config.GraphicClass}</graphicClass>");
            sb.AppendLine($"            <drawSize>({_wizard.Config.Width},{_wizard.Config.Height})</drawSize>");
            sb.AppendLine($"        </graphicData>");
            sb.AppendLine($"        <size>({_wizard.Config.Width},{_wizard.Config.Height})</size>");
            sb.AppendLine($"        <statBases>");
            sb.AppendLine($"            <MaxHitPoints>{_wizard.Config.MaxHitPoints}</MaxHitPoints>");
            sb.AppendLine($"            <WorkToBuild>{_wizard.Config.WorkToBuild}</WorkToBuild>");
            sb.AppendLine($"            <Flammability>{_wizard.Config.Flammability}</Flammability>");
            sb.AppendLine($"            <MarketValue>{_wizard.Config.MarketValue}</MarketValue>");
            sb.AppendLine($"        </statBases>");
            sb.AppendLine($"        <designationCategory>{_wizard.Config.DesignationCategory.FirstOrDefault() ?? "Structure"}</designationCategory>");
            
            if (_wizard.Config.StuffCategories.Count > 0)
            {
                sb.AppendLine($"        <stuffCategories>");
                foreach (var stuff in _wizard.Config.StuffCategories)
                {
                    sb.AppendLine($"            <li>{stuff.Name}</li>");
                }
                sb.AppendLine($"        </stuffCategories>");
            }
            
            sb.AppendLine($"        <costList>");
            sb.AppendLine($"            <Steel>50</Steel>");
            sb.AppendLine($"        </costList>");
            
            if (_wizard.Config.NeedsPower)
            {
                sb.AppendLine($"        <comps>");
                sb.AppendLine($"            <li Class=\"CompProperties_Power\">");
                sb.AppendLine($"                <compClass>CompPowerTrader</compClass>");
                sb.AppendLine($"                <basePowerConsumption>{_wizard.Config.PowerConsumption}</basePowerConsumption>");
                sb.AppendLine($"            </li>");
                sb.AppendLine($"            <li Class=\"CompProperties_Flickable\"/>");
                sb.AppendLine($"        </comps>");
            }
            
            if (buildingType.Type == BuildingType.Workbench)
            {
                GenerateWorkbenchSpecific(sb, context);
            }
            else if (buildingType.Type == BuildingType.Storage)
            {
                GenerateStorageSpecific(sb);
            }
            
            sb.AppendLine($"        <building>");
            sb.AppendLine($"            <soundBuild>{_wizard.Config.SoundBuildDefName}</soundBuild>");
            sb.AppendLine($"        </building>");
            sb.AppendLine("    </ThingDef>");
        }

        private void GenerateWorkbenchSpecific(StringBuilder sb, WizardContext context)
        {
            if (_wizard.Config.WorkSpeedFactor != 100)
            {
                sb.AppendLine($"        <statBases>");
                sb.AppendLine($"            <WorkTableWorkSpeedFactor>{_wizard.Config.WorkSpeedFactor / 100f:F2}</WorkTableWorkSpeedFactor>");
                sb.AppendLine($"        </statBases>");
            }
            
            sb.AppendLine($"        <building>");
            sb.AppendLine($"            <spawnedConceptLearnOpportunity>BillsTab</spawnedConceptLearnOpportunity>");
            sb.AppendLine($"        </building>");
            sb.AppendLine($"        <inspectorTabs>");
            sb.AppendLine($"            <li>ITab_Bills</li>");
            sb.AppendLine($"        </inspectorTabs>");
        }

        private void GenerateStorageSpecific(StringBuilder sb)
        {
            if (_wizard.Config.IsStorage)
            {
                sb.AppendLine($"        <building>");
                sb.AppendLine($"            <fixedStorageSettings>");
                sb.AppendLine($"                <priority>Important</priority>");
                sb.AppendLine($"            </fixedStorageSettings>");
                sb.AppendLine($"            <defaultStorageSettings>");
                sb.AppendLine($"                <priority>Important</priority>");
                sb.AppendLine($"            </defaultStorageSettings>");
                sb.AppendLine($"        </building>");
                sb.AppendLine($"        <inspectorTabs>");
                sb.AppendLine($"            <li>ITab_Storage</li>");
                sb.AppendLine($"        </inspectorTabs>");
            }
        }

        private string GenerateLanguageDefXml()
        {
            var sb = new StringBuilder();
            
            sb.AppendLine(_wizard.T("=== 中文语言文件 ===", "=== Chinese Language File ==="));
            sb.AppendLine();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sb.AppendLine("<LanguageData>");
            sb.AppendLine($"    <{_wizard.Config.BuildingDefName}.label>{_wizard.Config.BuildingLabelZh}</{_wizard.Config.BuildingDefName}.label>");
            sb.AppendLine($"    <{_wizard.Config.BuildingDefName}.description>{_wizard.Config.BuildingDescriptionZh}</{_wizard.Config.BuildingDefName}.description>");
            sb.AppendLine("</LanguageData>");
            sb.AppendLine();
            
            sb.AppendLine(_wizard.T("=== 英文语言文件 ===", "=== English Language File ==="));
            sb.AppendLine();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sb.AppendLine("<LanguageData>");
            sb.AppendLine($"    <{_wizard.Config.BuildingDefName}.label>{_wizard.Config.BuildingLabelEn}</{_wizard.Config.BuildingDefName}.label>");
            sb.AppendLine($"    <{_wizard.Config.BuildingDefName}.description>{_wizard.Config.BuildingDescriptionEn}</{_wizard.Config.BuildingDefName}.description>");
            sb.AppendLine("</LanguageData>");
            
            return sb.ToString();
        }
    }

    public class BuildingTestSuggestionStep : WizardStepBase
    {
        private readonly BuildingModWizard _wizard;

        public BuildingTestSuggestionStep(BuildingModWizard wizard) 
            : base(wizard.T("测试建议", "Test Suggestions"), 
                   wizard.T("提供测试建议和注意事项", "Provide testing suggestions and notes"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();
            
            var buildingType = context.GetData<BuildingTypeInfo>("SelectedBuildingType");
            
            Console.WriteLine(_wizard.T("=== 测试步骤 ===", "=== Testing Steps ==="));
            Console.WriteLine();
            
            Console.WriteLine(_wizard.T("1. 准备贴图文件", "1. Prepare texture files"));
            Console.WriteLine(_wizard.T("   * 推荐尺寸: 根据建筑大小确定 (1x1=64x64, 2x2=128x128 等)", "   * Recommended size: Based on building size (1x1=64x64, 2x2=128x128 etc.)"));
            Console.WriteLine(_wizard.T("   * 格式: PNG (支持透明)", "   * Format: PNG (supports transparency)"));
            Console.WriteLine(_wizard.T("   * 文件路径需与 XML 中的 texPath 一致", "   * File path must match texPath in XML"));
            Console.WriteLine();
            
            Console.WriteLine(_wizard.T("2. 创建 Mod 文件夹结构", "2. Create mod folder structure"));
            Console.WriteLine(_wizard.T("   * 按照生成的目录结构创建文件夹", "   * Create folders according to generated structure"));
            Console.WriteLine(_wizard.T("   * 将 XML 文件放入正确的 Defs 子文件夹", "   * Place XML files in correct Defs subfolder"));
            Console.WriteLine(_wizard.T("   * 将贴图放入 Textures 文件夹", "   * Place textures in Textures folder"));
            Console.WriteLine();
            
            Console.WriteLine(_wizard.T("3. 游戏内测试", "3. In-game testing"));
            Console.WriteLine(_wizard.T("   * 启动游戏，在 Mod 管理器中启用 Mod", "   * Start game, enable mod in Mod Manager"));
            Console.WriteLine(_wizard.T("   * 检查错误日志 (开发者控制台)", "   * Check error log (Developer Console)"));
            Console.WriteLine(_wizard.T("   * 在建筑菜单中查找新建筑", "   * Find new building in architecture menu"));
            Console.WriteLine(_wizard.T("   * 测试建造、使用、拆除等功能", "   * Test building, using, and deconstructing"));
            Console.WriteLine();
            
            Console.WriteLine(_wizard.T("=== 注意事项 ===", "=== Notes ==="));
            Console.WriteLine();
            
            if (buildingType != null)
            {
                PrintTypeSpecificNotes(buildingType);
            }
            
            Console.WriteLine(_wizard.T("通用注意事项:", "General notes:"));
            Console.WriteLine(_wizard.T("  * defName 必须唯一，避免与其他 Mod 冲突", "  * defName must be unique, avoid conflicts with other mods"));
            Console.WriteLine(_wizard.T("  * 贴图路径区分大小写", "  * Texture paths are case-sensitive"));
            Console.WriteLine(_wizard.T("  * 确保建筑大小与贴图匹配", "  * Ensure building size matches texture"));
            Console.WriteLine(_wizard.T("  * 测试与其他 Mod 的兼容性", "  * Test compatibility with other mods"));
            Console.WriteLine();
            
            Console.WriteLine(_wizard.T("=== 调试技巧 ===", "=== Debug Tips ==="));
            Console.WriteLine();
            Console.WriteLine(_wizard.T("启用开发者模式:", "Enable Developer Mode:"));
            Console.WriteLine(_wizard.T("  设置 -> 开发者模式 -> 启用", "  Settings -> Developer Mode -> Enable"));
            Console.WriteLine();
            Console.WriteLine(_wizard.T("查看错误日志:", "View error log:"));
            Console.WriteLine(_wizard.T("  按 ` 键打开控制台", "  Press ` key to open console"));
            Console.WriteLine(_wizard.T("  或查看 Player.log 文件", "  Or check Player.log file"));
            Console.WriteLine();
            Console.WriteLine(_wizard.T("快速测试建筑:", "Quick building test:"));
            Console.WriteLine(_wizard.T("  开发模式 -> 动作 -> 生成物品 -> 搜索建筑名称", "  Dev Mode -> Actions -> Spawn item -> Search building name"));
            Console.WriteLine();
            
            Pause(_wizard.T("按任意键完成向导...", "Press any key to finish wizard..."));
            
            ShowSuccess(_wizard.T("向导完成！", "Wizard completed!"));
            Console.WriteLine();
            Console.WriteLine(_wizard.T("生成的代码已保存到上下文中，您可以复制使用。", "Generated code is saved in context, you can copy and use it."));
        }

        private void PrintTypeSpecificNotes(BuildingTypeInfo buildingType)
        {
            Console.WriteLine(_wizard.T(buildingType.NameZh + " 特定注意事项:", buildingType.NameEn + " specific notes:"));
            
            switch (buildingType.Type)
            {
                case BuildingType.Workbench:
                    Console.WriteLine(_wizard.T("  - 测试工作速度是否正确", "  - Test if work speed is correct"));
                    Console.WriteLine(_wizard.T("  - 检查配方是否正确加载", "  - Check if recipes load correctly"));
                    Console.WriteLine(_wizard.T("  - 测试电力消耗（如适用）", "  - Test power consumption (if applicable)"));
                    Console.WriteLine(_wizard.T("  - 检查工作类型是否正确", "  - Check if work types are correct"));
                    break;
                case BuildingType.Defense:
                    Console.WriteLine(_wizard.T("  - 测试防御功能是否正常", "  - Test if defense function works"));
                    Console.WriteLine(_wizard.T("  - 检查攻击范围和伤害", "  - Check attack range and damage"));
                    Console.WriteLine(_wizard.T("  - 测试电力需求（如适用）", "  - Test power requirement (if applicable)"));
                    Console.WriteLine(_wizard.T("  - 检查 AI 行为", "  - Check AI behavior"));
                    break;
                case BuildingType.Furniture:
                    Console.WriteLine(_wizard.T("  - 测试舒适度属性", "  - Test comfort attributes"));
                    Console.WriteLine(_wizard.T("  - 检查角色使用动画", "  - Check pawn usage animation"));
                    Console.WriteLine(_wizard.T("  - 测试房间效果", "  - Test room effects"));
                    break;
                case BuildingType.Power:
                    Console.WriteLine(_wizard.T("  - 测试发电/储电功能", "  - Test power generation/storage"));
                    Console.WriteLine(_wizard.T("  - 检查电网连接", "  - Check power grid connection"));
                    Console.WriteLine(_wizard.T("  - 测试电力显示", "  - Test power display"));
                    break;
                case BuildingType.Storage:
                    Console.WriteLine(_wizard.T("  - 测试存储容量", "  - Test storage capacity"));
                    Console.WriteLine(_wizard.T("  - 检查存储设置界面", "  - Check storage settings UI"));
                    Console.WriteLine(_wizard.T("  - 测试物品存取", "  - Test item storage and retrieval"));
                    break;
            }
            
            Console.WriteLine();
        }
    }
}
