using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorldModDevProbe.Core;
using RimWorldModDevProbe.Wizards.Core;

namespace RimWorldModDevProbe
{
    public enum WeaponType
    {
        Melee,
        Ranged,
        Thrown,
        Grenade,
        Custom
    }

    public enum WeaponClass
    {
        Simple,
        Medieval,
        Industrial,
        Spacer,
        Ultratech,
        Archaic
    }

    public class WeaponModConfig
    {
        public WeaponType WeaponType { get; set; }
        public WeaponClass WeaponClass { get; set; }
        public string ModName { get; set; }
        public string AuthorName { get; set; }
        public string Description { get; set; }
        public string WeaponDefName { get; set; }
        public string WeaponLabelZh { get; set; }
        public string WeaponLabelEn { get; set; }
        public string WeaponDescriptionZh { get; set; }
        public string WeaponDescriptionEn { get; set; }
        public float Damage { get; set; }
        public float ArmorPenetration { get; set; }
        public float Range { get; set; }
        public float WarmupTime { get; set; }
        public float CooldownTime { get; set; }
        public int BurstShotCount { get; set; }
        public float BurstShotDelay { get; set; }
        public float AccuracyTouch { get; set; }
        public float AccuracyShort { get; set; }
        public float AccuracyMedium { get; set; }
        public float AccuracyLong { get; set; }
        public int MaxHitPoints { get; set; }
        public float Mass { get; set; }
        public int MarketValue { get; set; }
        public List<string> Tags { get; set; } = new List<string>();
        public string ProjectileDefName { get; set; }
        public float ProjectileSpeed { get; set; }
        public float ProjectileDamage { get; set; }
        public string SoundShootDefName { get; set; }
        public string SoundMeleeHitDefName { get; set; }
        public string SoundMeleeMissDefName { get; set; }
        public string GraphicPath { get; set; }
        public bool UseBilingual { get; set; } = true;
    }

    public class WeaponTypeInfo
    {
        public WeaponType Type { get; set; }
        public string NameZh { get; set; }
        public string NameEn { get; set; }
        public string DescriptionZh { get; set; }
        public string DescriptionEn { get; set; }
        public List<string> ExampleDefs { get; set; } = new List<string>();
        public List<string> DefaultTags { get; set; } = new List<string>();
    }

    public class WeaponModWizard
    {
        private readonly ProbeContext _context;
        private readonly DevWizard _innerWizard;
        private readonly WeaponModConfig _config;
        private readonly List<WeaponTypeInfo> _weaponTypes;
        private bool _useChinese = true;

        public WeaponModWizard(ProbeContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _config = new WeaponModConfig();
            _weaponTypes = InitializeWeaponTypes();
            _innerWizard = new DevWizard(context);
            
            SetupWizardSteps();
        }

        private List<WeaponTypeInfo> InitializeWeaponTypes()
        {
            return new List<WeaponTypeInfo>
            {
                new WeaponTypeInfo
                {
                    Type = WeaponType.Melee,
                    NameZh = "近战武器",
                    NameEn = "Melee Weapon",
                    DescriptionZh = "近战武器，如刀剑、锤、矛等",
                    DescriptionEn = "Melee weapons such as swords, hammers, spears, etc.",
                    ExampleDefs = new List<string> { "MeleeWeapon_Knife", "MeleeWeapon_LongSword", "MeleeWeapon_Mace" },
                    DefaultTags = new List<string> { "Melee" }
                },
                new WeaponTypeInfo
                {
                    Type = WeaponType.Ranged,
                    NameZh = "远程武器",
                    NameEn = "Ranged Weapon",
                    DescriptionZh = "远程射击武器，如步枪、手枪、弓箭等",
                    DescriptionEn = "Ranged shooting weapons such as rifles, pistols, bows, etc.",
                    ExampleDefs = new List<string> { "Gun_AssaultRifle", "Gun_Pistol", "Gun_SniperRifle" },
                    DefaultTags = new List<string> { "Ranged" }
                },
                new WeaponTypeInfo
                {
                    Type = WeaponType.Thrown,
                    NameZh = "投掷武器",
                    NameEn = "Thrown Weapon",
                    DescriptionZh = "投掷类武器，如标枪、飞刀等",
                    DescriptionEn = "Thrown weapons such as javelins, throwing knives, etc.",
                    ExampleDefs = new List<string> { "MeleeWeapon_Spear", "Weapon_GrenadeFrag" },
                    DefaultTags = new List<string> { "Thrown" }
                },
                new WeaponTypeInfo
                {
                    Type = WeaponType.Grenade,
                    NameZh = "手榴弹",
                    NameEn = "Grenade",
                    DescriptionZh = "爆炸类投掷武器",
                    DescriptionEn = "Explosive throwing weapons",
                    ExampleDefs = new List<string> { "Weapon_GrenadeFrag", "Weapon_GrenadeMolotov", "Weapon_GrenadeEMP" },
                    DefaultTags = new List<string> { "Grenade", "Explosive" }
                },
                new WeaponTypeInfo
                {
                    Type = WeaponType.Custom,
                    NameZh = "自定义武器",
                    NameEn = "Custom Weapon",
                    DescriptionZh = "自定义类型的武器",
                    DescriptionEn = "Custom type weapon",
                    ExampleDefs = new List<string>(),
                    DefaultTags = new List<string>()
                }
            };
        }

        private void SetupWizardSteps()
        {
            _innerWizard.AddStep(new WeaponWelcomeStep(this));
            _innerWizard.AddStep(new WeaponLanguageSelectStep(this));
            _innerWizard.AddStep(new WeaponTypeSelectStep(this));
            _innerWizard.AddStep(new WeaponBasicInfoStep(this));
            _innerWizard.AddStep(new WeaponAttributeConfigStep(this));
            _innerWizard.AddStep(new WeaponCodeGenerationStep(this));
            _innerWizard.AddStep(new WeaponTestSuggestionStep(this));
        }

        public WizardResult Run()
        {
            return _innerWizard.Run();
        }

        public WeaponModConfig Config => _config;
        public bool UseChinese => _useChinese;
        public List<WeaponTypeInfo> WeaponTypes => _weaponTypes;
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

    public class WeaponWelcomeStep : WizardStepBase
    {
        private readonly WeaponModWizard _wizard;

        public WeaponWelcomeStep(WeaponModWizard wizard) 
            : base(wizard.T("欢迎 - 武器 Mod 向导", "Welcome - Weapon Mod Wizard"), 
                   wizard.T("本向导将引导您完成武器 Mod 的开发流程", "This wizard will guide you through the weapon mod development process"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();
            
            Console.WriteLine(_wizard.T(
                "欢迎使用武器 Mod 向导！\n\n" +
                "本向导将帮助您：\n" +
                "  1. 选择武器类型（近战/远程/投掷等）\n" +
                "  2. 配置武器基本属性\n" +
                "  3. 设置武器伤害、射程、冷却等参数\n" +
                "  4. 生成完整的 ThingDef XML 代码\n" +
                "  5. 获取测试和调试建议\n",
                "Welcome to the Weapon Mod Wizard!\n\n" +
                "This wizard will help you:\n" +
                "  1. Select weapon type (Melee/Ranged/Thrown etc.)\n" +
                "  2. Configure weapon basic properties\n" +
                "  3. Set weapon damage, range, cooldown parameters\n" +
                "  4. Generate complete ThingDef XML code\n" +
                "  5. Get testing and debugging suggestions\n"));
            
            Pause(_wizard.T("按任意键继续...", "Press any key to continue..."));
        }
    }

    public class WeaponLanguageSelectStep : WizardStepBase
    {
        private readonly WeaponModWizard _wizard;

        public WeaponLanguageSelectStep(WeaponModWizard wizard) 
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

    public class WeaponTypeSelectStep : WizardStepBase
    {
        private readonly WeaponModWizard _wizard;

        public WeaponTypeSelectStep(WeaponModWizard wizard) 
            : base(wizard.T("武器类型选择", "Weapon Type Selection"), 
                   wizard.T("选择您要创建的武器类型", "Select the weapon type you want to create"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();
            
            Console.WriteLine(_wizard.T("可用武器类型:", "Available weapon types:"));
            Console.WriteLine();
            
            var weaponTypes = _wizard.WeaponTypes;
            for (int i = 0; i < weaponTypes.Count; i++)
            {
                var type = weaponTypes[i];
                var name = _wizard.UseChinese ? type.NameZh : type.NameEn;
                var desc = _wizard.UseChinese ? type.DescriptionZh : type.DescriptionEn;
                Console.WriteLine($"  [{i + 1}] {name}");
                Console.WriteLine($"      {desc}");
                Console.WriteLine();
            }
            
            var options = weaponTypes.Select(t => _wizard.UseChinese ? t.NameZh : t.NameEn).ToList();
            var selectedName = ReadChoice(_wizard.T("请选择武器类型", "Please select weapon type"), options);
            
            var selectedIndex = options.IndexOf(selectedName);
            var selectedType = weaponTypes[selectedIndex];
            
            _wizard.Config.WeaponType = selectedType.Type;
            context.SetData("SelectedWeaponType", selectedType);
            
            ShowInfo(_wizard.T($"已选择: {selectedName}", $"Selected: {selectedName}"));
            
            Console.WriteLine();
            Console.WriteLine(_wizard.T("选择武器科技等级:", "Select weapon tech level:"));
            var techLevels = new List<string>
            {
                _wizard.T("原始 (Neolithic)", "Neolithic"),
                _wizard.T("中世纪 (Medieval)", "Medieval"),
                _wizard.T("工业 (Industrial)", "Industrial"),
                _wizard.T("太空 (Spacer)", "Spacer"),
                _wizard.T("超科技 (Ultratech)", "Ultratech"),
                _wizard.T("古代 (Archaic)", "Archaic")
            };
            
            for (int i = 0; i < techLevels.Count; i++)
            {
                Console.WriteLine($"  [{i + 1}] {techLevels[i]}");
            }
            
            var selectedTechLevel = ReadChoice(_wizard.T("请选择科技等级", "Please select tech level"), techLevels);
            _wizard.Config.WeaponClass = (WeaponClass)techLevels.IndexOf(selectedTechLevel);
        }
    }

    public class WeaponBasicInfoStep : WizardStepBase
    {
        private readonly WeaponModWizard _wizard;

        public WeaponBasicInfoStep(WeaponModWizard wizard) 
            : base(wizard.T("基本信息配置", "Basic Information Configuration"), 
                   wizard.T("配置武器的基本信息", "Configure weapon basic information"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();
            
            var weaponType = context.GetData<WeaponTypeInfo>("SelectedWeaponType");
            if (weaponType == null)
            {
                ShowError(_wizard.T("未找到武器类型信息", "Weapon type information not found"));
                return;
            }
            
            Console.WriteLine(_wizard.T($"正在配置: {(_wizard.UseChinese ? weaponType.NameZh : weaponType.NameEn)}", 
                                       $"Configuring: {weaponType.NameEn}"));
            Console.WriteLine();
            
            _wizard.Config.ModName = ReadInput(_wizard.T("Mod 名称", "Mod name"), "MyWeaponMod");
            _wizard.Config.AuthorName = ReadInput(_wizard.T("作者名称", "Author name"), "YourName");
            _wizard.Config.Description = ReadInput(_wizard.T("Mod 描述", "Mod description"), 
                _wizard.T("自定义武器 Mod", "Custom weapon mod"));
            
            Console.WriteLine();
            Console.WriteLine(_wizard.T("=== 武器定义信息 ===", "=== Weapon Definition Info ==="));
            Console.WriteLine();
            
            _wizard.Config.WeaponDefName = ReadInput(
                _wizard.T("武器定义名称 (defName)", "Weapon defName"), 
                "MyCustomWeapon");
            
            _wizard.Config.WeaponLabelZh = ReadInput(
                _wizard.T("武器名称 (中文)", "Weapon label (Chinese)"), 
                "自定义武器");
            
            _wizard.Config.WeaponLabelEn = ReadInput(
                _wizard.T("武器名称 (英文)", "Weapon label (English)"), 
                "Custom Weapon");
            
            _wizard.Config.WeaponDescriptionZh = ReadInput(
                _wizard.T("武器描述 (中文)", "Weapon description (Chinese)"), 
                "一把自定义的武器。");
            
            _wizard.Config.WeaponDescriptionEn = ReadInput(
                _wizard.T("武器描述 (英文)", "Weapon description (English)"), 
                "A custom weapon.");
            
            Console.WriteLine();
            _wizard.Config.GraphicPath = ReadInput(
                _wizard.T("贴图路径 (相对路径)", "Graphic path (relative)"), 
                "Things/Item/Equipment/WeaponRanged/MyWeapon");
            
            _wizard.Config.MaxHitPoints = ReadInt(_wizard.T("耐久度", "Max hit points"), 100, 1, 10000);
            _wizard.Config.Mass = float.Parse(ReadInput(_wizard.T("重量 (kg)", "Mass (kg)"), "1.5"));
            _wizard.Config.MarketValue = ReadInt(_wizard.T("市场价值", "Market value"), 500, 1, 100000);
            
            ShowSuccess(_wizard.T("基本信息配置完成", "Basic information configuration completed"));
        }
    }

    public class WeaponAttributeConfigStep : WizardStepBase
    {
        private readonly WeaponModWizard _wizard;

        public WeaponAttributeConfigStep(WeaponModWizard wizard) 
            : base(wizard.T("属性配置", "Attribute Configuration"), 
                   wizard.T("配置武器的详细属性", "Configure weapon detailed attributes"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();
            
            var weaponType = context.GetData<WeaponTypeInfo>("SelectedWeaponType");
            if (weaponType == null)
            {
                ShowError(_wizard.T("未找到武器类型信息", "Weapon type information not found"));
                return;
            }
            
            Console.WriteLine(_wizard.T("=== 武器属性配置 ===", "=== Weapon Attribute Configuration ==="));
            Console.WriteLine();
            
            _wizard.Config.Damage = float.Parse(ReadInput(_wizard.T("基础伤害", "Base damage"), "10"));
            _wizard.Config.ArmorPenetration = float.Parse(ReadInput(_wizard.T("穿甲值", "Armor penetration"), "0.2"));
            
            if (weaponType.Type == WeaponType.Ranged)
            {
                ConfigureRangedWeapon();
            }
            else if (weaponType.Type == WeaponType.Melee)
            {
                ConfigureMeleeWeapon();
            }
            else if (weaponType.Type == WeaponType.Grenade)
            {
                ConfigureGrenadeWeapon();
            }
            else
            {
                ConfigureGenericWeapon();
            }
            
            Console.WriteLine();
            Console.WriteLine(_wizard.T("=== 音效配置 ===", "=== Sound Configuration ==="));
            Console.WriteLine();
            
            _wizard.Config.SoundShootDefName = ReadInput(
                _wizard.T("射击/挥击音效 (可选)", "Shoot/Swing sound (optional)"), 
                "");
            _wizard.Config.SoundMeleeHitDefName = ReadInput(
                _wizard.T("命中音效 (可选)", "Melee hit sound (optional)"), 
                "");
            _wizard.Config.SoundMeleeMissDefName = ReadInput(
                _wizard.T("未命中音效 (可选)", "Melee miss sound (optional)"), 
                "");
            
            ShowSuccess(_wizard.T("属性配置完成", "Attribute configuration completed"));
        }

        private void ConfigureRangedWeapon()
        {
            Console.WriteLine(_wizard.T("--- 远程武器专属配置 ---", "--- Ranged Weapon Specific Config ---"));
            Console.WriteLine();
            
            _wizard.Config.Range = float.Parse(ReadInput(_wizard.T("射程", "Range"), "30"));
            _wizard.Config.WarmupTime = float.Parse(ReadInput(_wizard.T("预热时间 (秒)", "Warmup time (seconds)"), "1.0"));
            _wizard.Config.CooldownTime = float.Parse(ReadInput(_wizard.T("冷却时间 (秒)", "Cooldown time (seconds)"), "1.5"));
            _wizard.Config.BurstShotCount = ReadInt(_wizard.T("连发次数", "Burst shot count"), 1, 1, 100);
            
            if (_wizard.Config.BurstShotCount > 1)
            {
                _wizard.Config.BurstShotDelay = float.Parse(ReadInput(_wizard.T("连发间隔 (秒)", "Burst shot delay (seconds)"), "0.1"));
            }
            
            Console.WriteLine();
            Console.WriteLine(_wizard.T("--- 精度配置 ---", "--- Accuracy Configuration ---"));
            Console.WriteLine();
            
            _wizard.Config.AccuracyTouch = float.Parse(ReadInput(_wizard.T("贴身精度 (0-1)", "Touch accuracy (0-1)"), "0.8"));
            _wizard.Config.AccuracyShort = float.Parse(ReadInput(_wizard.T("短距离精度 (0-1)", "Short accuracy (0-1)"), "0.7"));
            _wizard.Config.AccuracyMedium = float.Parse(ReadInput(_wizard.T("中距离精度 (0-1)", "Medium accuracy (0-1)"), "0.6"));
            _wizard.Config.AccuracyLong = float.Parse(ReadInput(_wizard.T("长距离精度 (0-1)", "Long accuracy (0-1)"), "0.5"));
            
            Console.WriteLine();
            Console.WriteLine(_wizard.T("--- 弹药配置 ---", "--- Projectile Configuration ---"));
            Console.WriteLine();
            
            _wizard.Config.ProjectileDefName = ReadInput(
                _wizard.T("弹药定义名称 (可选，留空使用默认)", "Projectile defName (optional, leave empty for default)"), 
                "");
            
            if (!string.IsNullOrEmpty(_wizard.Config.ProjectileDefName))
            {
                _wizard.Config.ProjectileSpeed = float.Parse(ReadInput(_wizard.T("弹药速度", "Projectile speed"), "100"));
                _wizard.Config.ProjectileDamage = float.Parse(ReadInput(_wizard.T("弹药伤害", "Projectile damage"), "10"));
            }
        }

        private void ConfigureMeleeWeapon()
        {
            Console.WriteLine(_wizard.T("--- 近战武器专属配置 ---", "--- Melee Weapon Specific Config ---"));
            Console.WriteLine();
            
            _wizard.Config.WarmupTime = float.Parse(ReadInput(_wizard.T("预热时间 (秒)", "Warmup time (seconds)"), "0.5"));
            _wizard.Config.CooldownTime = float.Parse(ReadInput(_wizard.T("冷却时间 (秒)", "Cooldown time (seconds)"), "1.5"));
        }

        private void ConfigureGrenadeWeapon()
        {
            Console.WriteLine(_wizard.T("--- 手榴弹专属配置 ---", "--- Grenade Specific Config ---"));
            Console.WriteLine();
            
            _wizard.Config.Range = float.Parse(ReadInput(_wizard.T("投掷距离", "Throw range"), "12"));
            _wizard.Config.WarmupTime = float.Parse(ReadInput(_wizard.T("预热时间 (秒)", "Warmup time (seconds)"), "1.5"));
            _wizard.Config.CooldownTime = float.Parse(ReadInput(_wizard.T("冷却时间 (秒)", "Cooldown time (seconds)"), "2.0"));
        }

        private void ConfigureGenericWeapon()
        {
            Console.WriteLine(_wizard.T("--- 通用武器配置 ---", "--- Generic Weapon Config ---"));
            Console.WriteLine();
            
            _wizard.Config.Range = float.Parse(ReadInput(_wizard.T("射程", "Range"), "10"));
            _wizard.Config.WarmupTime = float.Parse(ReadInput(_wizard.T("预热时间 (秒)", "Warmup time (seconds)"), "1.0"));
            _wizard.Config.CooldownTime = float.Parse(ReadInput(_wizard.T("冷却时间 (秒)", "Cooldown time (seconds)"), "1.5"));
        }
    }

    public class WeaponCodeGenerationStep : WizardStepBase
    {
        private readonly WeaponModWizard _wizard;

        public WeaponCodeGenerationStep(WeaponModWizard wizard) 
            : base(wizard.T("代码生成", "Code Generation"), 
                   wizard.T("生成完整的 ThingDef XML 代码", "Generate complete ThingDef XML code"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();
            
            var weaponType = context.GetData<WeaponTypeInfo>("SelectedWeaponType");
            
            if (weaponType == null)
            {
                ShowError(_wizard.T("未找到武器类型信息", "Weapon type information not found"));
                return;
            }
            
            var generatedCode = new StringBuilder();
            
            generatedCode.AppendLine(_wizard.T("=== 生成的代码 ===", "=== Generated Code ==="));
            generatedCode.AppendLine();
            
            generatedCode.AppendLine(GenerateModStructure());
            generatedCode.AppendLine();
            
            generatedCode.AppendLine(GenerateThingDefXml(weaponType));
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
            sb.AppendLine("|   |-- ThingDefs_Misc/");
            sb.AppendLine("|       |-- Weapons_*.xml");
            sb.AppendLine("|-- Languages/");
            sb.AppendLine("|   |-- ChineseSimplified/");
            sb.AppendLine("|   |   |-- DefInjected/");
            sb.AppendLine("|   |       |-- ThingDef/");
            sb.AppendLine("|   |           |-- Weapons_*.xml");
            sb.AppendLine("|   |-- English/");
            sb.AppendLine("|       |-- DefInjected/");
            sb.AppendLine("|           |-- ThingDef/");
            sb.AppendLine("|               |-- Weapons_*.xml");
            sb.AppendLine("|-- Textures/");
            sb.AppendLine("|   |-- Things/");
            sb.AppendLine("|       |-- Item/");
            sb.AppendLine("|           |-- Equipment/");
            sb.AppendLine("|               |-- WeaponRanged/");
            sb.AppendLine("|                   |-- *.png");
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

        private string GenerateThingDefXml(WeaponTypeInfo weaponType)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine(_wizard.T("=== ThingDef XML ===", "=== ThingDef XML ==="));
            sb.AppendLine();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sb.AppendLine("<Defs>");
            
            if (weaponType.Type == WeaponType.Ranged)
            {
                GenerateRangedWeaponDef(sb);
            }
            else if (weaponType.Type == WeaponType.Melee)
            {
                GenerateMeleeWeaponDef(sb);
            }
            else if (weaponType.Type == WeaponType.Grenade)
            {
                GenerateGrenadeWeaponDef(sb);
            }
            else
            {
                GenerateGenericWeaponDef(sb);
            }
            
            sb.AppendLine("</Defs>");
            
            return sb.ToString();
        }

        private void GenerateRangedWeaponDef(StringBuilder sb)
        {
            sb.AppendLine("    <ThingDef ParentName=\"BaseGun\">");
            sb.AppendLine($"        <defName>{_wizard.Config.WeaponDefName}</defName>");
            sb.AppendLine($"        <label>{_wizard.Config.WeaponLabelEn}</label>");
            sb.AppendLine($"        <description>{_wizard.Config.WeaponDescriptionEn}</description>");
            sb.AppendLine($"        <graphicData>");
            sb.AppendLine($"            <texPath>{_wizard.Config.GraphicPath}</texPath>");
            sb.AppendLine($"            <graphicClass>Graphic_Single</graphicClass>");
            sb.AppendLine($"        </graphicData>");
            sb.AppendLine($"        <statBases>");
            sb.AppendLine($"            <WorkToMake>1000</WorkToMake>");
            sb.AppendLine($"            <Mass>{_wizard.Config.Mass}</Mass>");
            sb.AppendLine($"            <AccuracyTouch>{_wizard.Config.AccuracyTouch}</AccuracyTouch>");
            sb.AppendLine($"            <AccuracyShort>{_wizard.Config.AccuracyShort}</AccuracyShort>");
            sb.AppendLine($"            <AccuracyMedium>{_wizard.Config.AccuracyMedium}</AccuracyMedium>");
            sb.AppendLine($"            <AccuracyLong>{_wizard.Config.AccuracyLong}</AccuracyLong>");
            sb.AppendLine($"            <RangedWeapon_Cooldown>{_wizard.Config.CooldownTime}</RangedWeapon_Cooldown>");
            sb.AppendLine($"        </statBases>");
            sb.AppendLine($"        <techLevel>{GetTechLevelString()}</techLevel>");
            sb.AppendLine($"        <equippedStatOffsets>");
            sb.AppendLine($"            <MoveSpeed>-0.1</MoveSpeed>");
            sb.AppendLine($"        </equippedStatOffsets>");
            sb.AppendLine($"        <verbs>");
            sb.AppendLine($"            <li>");
            sb.AppendLine($"                <verbClass>Verb_Shoot</verbClass>");
            sb.AppendLine($"                <hasStandardCommand>true</hasStandardCommand>");
            sb.AppendLine($"                <defaultProjectile>{GetProjectileDef()}</defaultProjectile>");
            sb.AppendLine($"                <warmupTime>{_wizard.Config.WarmupTime}</warmupTime>");
            sb.AppendLine($"                <range>{_wizard.Config.Range}</range>");
            sb.AppendLine($"                <soundCast>{_wizard.Config.SoundShootDefName}</soundCast>");
            sb.AppendLine($"                <burstShotCount>{_wizard.Config.BurstShotCount}</burstShotCount>");
            if (_wizard.Config.BurstShotCount > 1)
            {
                sb.AppendLine($"                <ticksBetweenBurstShots>{(int)(_wizard.Config.BurstShotDelay * 60)}</ticksBetweenBurstShots>");
            }
            sb.AppendLine($"            </li>");
            sb.AppendLine($"        </verbs>");
            sb.AppendLine($"        <tools>");
            sb.AppendLine($"            <li>");
            sb.AppendLine($"                <label>stock</label>");
            sb.AppendLine($"                <capacities>");
            sb.AppendLine($"                    <li>Poke</li>");
            sb.AppendLine($"                </capacities>");
            sb.AppendLine($"                <power>9</power>");
            sb.AppendLine($"                <cooldownTime>1.8</cooldownTime>");
            sb.AppendLine($"            </li>");
            sb.AppendLine($"            <li>");
            sb.AppendLine($"                <label>barrel</label>");
            sb.AppendLine($"                <capacities>");
            sb.AppendLine($"                    <li>Blunt</li>");
            sb.AppendLine($"                </capacities>");
            sb.AppendLine($"                <power>10</power>");
            sb.AppendLine($"                <cooldownTime>1.9</cooldownTime>");
            sb.AppendLine($"            </li>");
            sb.AppendLine($"        </tools>");
            sb.AppendLine("    </ThingDef>");
        }

        private void GenerateMeleeWeaponDef(StringBuilder sb)
        {
            sb.AppendLine("    <ThingDef ParentName=\"BaseMeleeWeapon_Sharp_Quality\">");
            sb.AppendLine($"        <defName>{_wizard.Config.WeaponDefName}</defName>");
            sb.AppendLine($"        <label>{_wizard.Config.WeaponLabelEn}</label>");
            sb.AppendLine($"        <description>{_wizard.Config.WeaponDescriptionEn}</description>");
            sb.AppendLine($"        <graphicData>");
            sb.AppendLine($"            <texPath>{_wizard.Config.GraphicPath}</texPath>");
            sb.AppendLine($"            <graphicClass>Graphic_Single</graphicClass>");
            sb.AppendLine($"        </graphicData>");
            sb.AppendLine($"        <statBases>");
            sb.AppendLine($"            <WorkToMake>1000</WorkToMake>");
            sb.AppendLine($"            <Mass>{_wizard.Config.Mass}</Mass>");
            sb.AppendLine($"            <MeleeDPS>{_wizard.Config.Damage / _wizard.Config.CooldownTime:F1}</MeleeDPS>");
            sb.AppendLine($"        </statBases>");
            sb.AppendLine($"        <techLevel>{GetTechLevelString()}</techLevel>");
            sb.AppendLine($"        <equippedStatOffsets>");
            sb.AppendLine($"            <MoveSpeed>0</MoveSpeed>");
            sb.AppendLine($"        </equippedStatOffsets>");
            sb.AppendLine($"        <tools>");
            sb.AppendLine($"            <li>");
            sb.AppendLine($"                <label>handle</label>");
            sb.AppendLine($"                <capacities>");
            sb.AppendLine($"                    <li>Poke</li>");
            sb.AppendLine($"                </capacities>");
            sb.AppendLine($"                <power>9</power>");
            sb.AppendLine($"                <cooldownTime>1.8</cooldownTime>");
            sb.AppendLine($"            </li>");
            sb.AppendLine($"            <li>");
            sb.AppendLine($"                <label>blade</label>");
            sb.AppendLine($"                <capacities>");
            sb.AppendLine($"                    <li>Cut</li>");
            sb.AppendLine($"                </capacities>");
            sb.AppendLine($"                <power>{_wizard.Config.Damage}</power>");
            sb.AppendLine($"                <cooldownTime>{_wizard.Config.CooldownTime}</cooldownTime>");
            sb.AppendLine($"            </li>");
            sb.AppendLine($"            <li>");
            sb.AppendLine($"                <label>point</label>");
            sb.AppendLine($"                <capacities>");
            sb.AppendLine($"                    <li>Stab</li>");
            sb.AppendLine($"                </capacities>");
            sb.AppendLine($"                <power>{_wizard.Config.Damage * 0.8f:F1}</power>");
            sb.AppendLine($"                <cooldownTime>{_wizard.Config.CooldownTime * 1.1f:F2}</cooldownTime>");
            sb.AppendLine($"            </li>");
            sb.AppendLine($"        </tools>");
            sb.AppendLine("    </ThingDef>");
        }

        private void GenerateGrenadeWeaponDef(StringBuilder sb)
        {
            sb.AppendLine("    <ThingDef ParentName=\"BaseEquipment\">");
            sb.AppendLine($"        <defName>{_wizard.Config.WeaponDefName}</defName>");
            sb.AppendLine($"        <label>{_wizard.Config.WeaponLabelEn}</label>");
            sb.AppendLine($"        <description>{_wizard.Config.WeaponDescriptionEn}</description>");
            sb.AppendLine($"        <graphicData>");
            sb.AppendLine($"            <texPath>{_wizard.Config.GraphicPath}</texPath>");
            sb.AppendLine($"            <graphicClass>Graphic_Single</graphicClass>");
            sb.AppendLine($"        </graphicData>");
            sb.AppendLine($"        <thingClass>ThingWithComps</thingClass>");
            sb.AppendLine($"        <category>Item</category>");
            sb.AppendLine($"        <useHitPoints>true</useHitPoints>");
            sb.AppendLine($"        <statBases>");
            sb.AppendLine($"            <MaxHitPoints>{_wizard.Config.MaxHitPoints}</MaxHitPoints>");
            sb.AppendLine($"            <Mass>{_wizard.Config.Mass}</Mass>");
            sb.AppendLine($"            <Flammability>1.0</Flammability>");
            sb.AppendLine($"            <MarketValue>{_wizard.Config.MarketValue}</MarketValue>");
            sb.AppendLine($"        </statBases>");
            sb.AppendLine($"        <techLevel>{GetTechLevelString()}</techLevel>");
            sb.AppendLine($"        <comps>");
            sb.AppendLine($"            <li Class=\"CompProperties_Explosive\">");
            sb.AppendLine($"                <explosiveRadius>3</explosiveRadius>");
            sb.AppendLine($"                <explosiveDamageType>Bomb</explosiveDamageType>");
            sb.AppendLine($"            </li>");
            sb.AppendLine($"        </comps>");
            sb.AppendLine($"        <verbs>");
            sb.AppendLine($"            <li>");
            sb.AppendLine($"                <verbClass>Verb_LaunchProjectile</verbClass>");
            sb.AppendLine($"                <hasStandardCommand>true</hasStandardCommand>");
            sb.AppendLine($"                <defaultProjectile>Proj_GrenadeFrag</defaultProjectile>");
            sb.AppendLine($"                <warmupTime>{_wizard.Config.WarmupTime}</warmupTime>");
            sb.AppendLine($"                <range>{_wizard.Config.Range}</range>");
            sb.AppendLine($"                <burstShotCount>1</burstShotCount>");
            sb.AppendLine($"            </li>");
            sb.AppendLine($"        </verbs>");
            sb.AppendLine("    </ThingDef>");
        }

        private void GenerateGenericWeaponDef(StringBuilder sb)
        {
            sb.AppendLine("    <ThingDef ParentName=\"BaseGun\">");
            sb.AppendLine($"        <defName>{_wizard.Config.WeaponDefName}</defName>");
            sb.AppendLine($"        <label>{_wizard.Config.WeaponLabelEn}</label>");
            sb.AppendLine($"        <description>{_wizard.Config.WeaponDescriptionEn}</description>");
            sb.AppendLine($"        <graphicData>");
            sb.AppendLine($"            <texPath>{_wizard.Config.GraphicPath}</texPath>");
            sb.AppendLine($"            <graphicClass>Graphic_Single</graphicClass>");
            sb.AppendLine($"        </graphicData>");
            sb.AppendLine($"        <statBases>");
            sb.AppendLine($"            <WorkToMake>1000</WorkToMake>");
            sb.AppendLine($"            <Mass>{_wizard.Config.Mass}</Mass>");
            sb.AppendLine($"            <RangedWeapon_Cooldown>{_wizard.Config.CooldownTime}</RangedWeapon_Cooldown>");
            sb.AppendLine($"        </statBases>");
            sb.AppendLine($"        <techLevel>{GetTechLevelString()}</techLevel>");
            sb.AppendLine($"        <verbs>");
            sb.AppendLine($"            <li>");
            sb.AppendLine($"                <verbClass>Verb_Shoot</verbClass>");
            sb.AppendLine($"                <hasStandardCommand>true</hasStandardCommand>");
            sb.AppendLine($"                <defaultProjectile>Bullet_Pistol</defaultProjectile>");
            sb.AppendLine($"                <warmupTime>{_wizard.Config.WarmupTime}</warmupTime>");
            sb.AppendLine($"                <range>{_wizard.Config.Range}</range>");
            sb.AppendLine($"                <burstShotCount>1</burstShotCount>");
            sb.AppendLine($"            </li>");
            sb.AppendLine($"        </verbs>");
            sb.AppendLine("    </ThingDef>");
        }

        private string GenerateLanguageDefXml()
        {
            var sb = new StringBuilder();
            
            sb.AppendLine(_wizard.T("=== 中文语言文件 ===", "=== Chinese Language File ==="));
            sb.AppendLine();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sb.AppendLine("<LanguageData>");
            sb.AppendLine($"    <{_wizard.Config.WeaponDefName}.label>{_wizard.Config.WeaponLabelZh}</{_wizard.Config.WeaponDefName}.label>");
            sb.AppendLine($"    <{_wizard.Config.WeaponDefName}.description>{_wizard.Config.WeaponDescriptionZh}</{_wizard.Config.WeaponDefName}.description>");
            sb.AppendLine("</LanguageData>");
            sb.AppendLine();
            
            sb.AppendLine(_wizard.T("=== 英文语言文件 ===", "=== English Language File ==="));
            sb.AppendLine();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sb.AppendLine("<LanguageData>");
            sb.AppendLine($"    <{_wizard.Config.WeaponDefName}.label>{_wizard.Config.WeaponLabelEn}</{_wizard.Config.WeaponDefName}.label>");
            sb.AppendLine($"    <{_wizard.Config.WeaponDefName}.description>{_wizard.Config.WeaponDescriptionEn}</{_wizard.Config.WeaponDefName}.description>");
            sb.AppendLine("</LanguageData>");
            
            return sb.ToString();
        }

        private string GetTechLevelString()
        {
            switch (_wizard.Config.WeaponClass)
            {
                case WeaponClass.Simple:
                    return "Neolithic";
                case WeaponClass.Medieval:
                    return "Medieval";
                case WeaponClass.Industrial:
                    return "Industrial";
                case WeaponClass.Spacer:
                    return "Spacer";
                case WeaponClass.Ultratech:
                    return "Ultra";
                case WeaponClass.Archaic:
                    return "Archaic";
                default:
                    return "Industrial";
            }
        }

        private string GetProjectileDef()
        {
            if (!string.IsNullOrEmpty(_wizard.Config.ProjectileDefName))
            {
                return _wizard.Config.ProjectileDefName;
            }
            return "Bullet_Pistol";
        }
    }

    public class WeaponTestSuggestionStep : WizardStepBase
    {
        private readonly WeaponModWizard _wizard;

        public WeaponTestSuggestionStep(WeaponModWizard wizard) 
            : base(wizard.T("测试建议", "Test Suggestions"), 
                   wizard.T("提供测试建议和注意事项", "Provide testing suggestions and notes"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();
            
            var weaponType = context.GetData<WeaponTypeInfo>("SelectedWeaponType");
            
            Console.WriteLine(_wizard.T("=== 测试步骤 ===", "=== Testing Steps ==="));
            Console.WriteLine();
            
            Console.WriteLine(_wizard.T("1. 准备贴图文件", "1. Prepare texture files"));
            Console.WriteLine(_wizard.T("   * 推荐尺寸: 128x128 或 256x256 像素", "   * Recommended size: 128x128 or 256x256 pixels"));
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
            Console.WriteLine(_wizard.T("   * 使用开发模式菜单生成武器", "   * Use Dev Mode menu to spawn weapon"));
            Console.WriteLine(_wizard.T("   * 测试武器各项属性是否正确", "   * Test if weapon attributes are correct"));
            Console.WriteLine();
            
            Console.WriteLine(_wizard.T("=== 注意事项 ===", "=== Notes ==="));
            Console.WriteLine();
            
            if (weaponType != null)
            {
                PrintTypeSpecificNotes(weaponType);
            }
            
            Console.WriteLine(_wizard.T("通用注意事项:", "General notes:"));
            Console.WriteLine(_wizard.T("  * defName 必须唯一，避免与其他 Mod 冲突", "  * defName must be unique, avoid conflicts with other mods"));
            Console.WriteLine(_wizard.T("  * 贴图路径区分大小写", "  * Texture paths are case-sensitive"));
            Console.WriteLine(_wizard.T("  * 数值平衡很重要，参考原版武器", "  * Value balance is important, reference vanilla weapons"));
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
            Console.WriteLine(_wizard.T("生成武器测试:", "Spawn weapon for testing:"));
            Console.WriteLine(_wizard.T("  开发模式 -> 动作 -> 生成物品 -> 搜索武器名称", "  Dev Mode -> Actions -> Spawn item -> Search weapon name"));
            Console.WriteLine();
            
            Pause(_wizard.T("按任意键完成向导...", "Press any key to finish wizard..."));
            
            ShowSuccess(_wizard.T("向导完成！", "Wizard completed!"));
            Console.WriteLine();
            Console.WriteLine(_wizard.T("生成的代码已保存到上下文中，您可以复制使用。", "Generated code is saved in context, you can copy and use it."));
        }

        private void PrintTypeSpecificNotes(WeaponTypeInfo weaponType)
        {
            Console.WriteLine(_wizard.T(weaponType.NameZh + " 特定注意事项:", weaponType.NameEn + " specific notes:"));
            
            switch (weaponType.Type)
            {
                case WeaponType.Ranged:
                    Console.WriteLine(_wizard.T("  - 确保弹药定义存在或创建自定义弹药", "  - Ensure projectile def exists or create custom projectile"));
                    Console.WriteLine(_wizard.T("  - 测试不同距离的精度表现", "  - Test accuracy at different ranges"));
                    Console.WriteLine(_wizard.T("  - 检查射击音效是否正常播放", "  - Check if shooting sound plays correctly"));
                    Console.WriteLine(_wizard.T("  - 测试连发功能（如适用）", "  - Test burst fire (if applicable)"));
                    break;
                case WeaponType.Melee:
                    Console.WriteLine(_wizard.T("  - 测试不同攻击动作的伤害", "  - Test damage for different attack types"));
                    Console.WriteLine(_wizard.T("  - 检查冷却时间是否合理", "  - Check if cooldown time is reasonable"));
                    Console.WriteLine(_wizard.T("  - 测试穿甲效果", "  - Test armor penetration effect"));
                    Console.WriteLine(_wizard.T("  - 检查近战音效", "  - Check melee sounds"));
                    break;
                case WeaponType.Grenade:
                    Console.WriteLine(_wizard.T("  - 测试爆炸范围和伤害", "  - Test explosion radius and damage"));
                    Console.WriteLine(_wizard.T("  - 检查投掷距离", "  - Check throw range"));
                    Console.WriteLine(_wizard.T("  - 测试对友军伤害", "  - Test friendly fire damage"));
                    Console.WriteLine(_wizard.T("  - 检查爆炸特效", "  - Check explosion effects"));
                    break;
            }
            
            Console.WriteLine();
        }
    }
}
