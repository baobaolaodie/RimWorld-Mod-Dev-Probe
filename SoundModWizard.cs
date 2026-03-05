using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorldModDevProbe.Core;
using RimWorldModDevProbe.Analysis;
using RimWorldModDevProbe.Wizards.Core;
using RimWorldModDevProbe.Utils;

namespace RimWorldModDevProbe
{
    public enum SoundModType
    {
        Death,
        Damage,
        Attack,
        Movement,
        Ambient,
        Interaction,
        UI,
        Custom
    }

    public enum PatchMethod
    {
        HarmonyPatch,
        XmlPatch
    }

    public class SoundModConfig
    {
        public SoundModType SoundType { get; set; }
        public string TargetTypeName { get; set; }
        public string SoundDefName { get; set; }
        public string CustomSoundDefName { get; set; }
        public PatchMethod PatchMethod { get; set; }
        public string ModName { get; set; }
        public string AuthorName { get; set; }
        public string Description { get; set; }
        public List<string> TargetMethods { get; set; } = new List<string>();
        public List<string> RelatedSoundDefs { get; set; } = new List<string>();
        public bool UseBilingual { get; set; } = true;
    }

    public class SoundTypeInfo
    {
        public SoundModType Type { get; set; }
        public string NameZh { get; set; }
        public string NameEn { get; set; }
        public string DescriptionZh { get; set; }
        public string DescriptionEn { get; set; }
        public List<string> RelatedMethods { get; set; } = new List<string>();
        public List<string> RelatedFields { get; set; } = new List<string>();
        public List<string> ExampleSoundDefs { get; set; } = new List<string>();
    }

    public class SoundModWizard
    {
        private readonly ProbeContext _context;
        private readonly DevWizard _innerWizard;
        private readonly SoundModConfig _config;
        private readonly FeatureKeywordMap _keywordMap;
        private readonly PatchRecommender _patchRecommender;
        private readonly CodeGenerator _codeGenerator;
        private readonly List<SoundTypeInfo> _soundTypes;
        private bool _useChinese = true;

        public SoundModWizard(ProbeContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _config = new SoundModConfig();
            _keywordMap = new FeatureKeywordMap();
            _keywordMap.Initialize(context);
            _patchRecommender = new PatchRecommender(context);
            _codeGenerator = new CodeGenerator();
            _soundTypes = InitializeSoundTypes();
            _innerWizard = new DevWizard(context);
            
            SetupWizardSteps();
        }

        private List<SoundTypeInfo> InitializeSoundTypes()
        {
            return new List<SoundTypeInfo>
            {
                new SoundTypeInfo
                {
                    Type = SoundModType.Death,
                    NameZh = "死亡音效",
                    NameEn = "Death Sound",
                    DescriptionZh = "角色死亡时播放的音效，如惨叫、倒地声等",
                    DescriptionEn = "Sounds played when a pawn dies, such as death cries or falling sounds",
                    RelatedMethods = new List<string> { "Pawn.Die", "Pawn.Kill" },
                    RelatedFields = new List<string> { "Pawn.health" },
                    ExampleSoundDefs = new List<string> { "PawnDeath", "DeathAcid", "DeathInferno" }
                },
                new SoundTypeInfo
                {
                    Type = SoundModType.Damage,
                    NameZh = "受伤音效",
                    NameEn = "Damage Sound",
                    DescriptionZh = "角色受到伤害时播放的音效，如惨叫、撞击声等",
                    DescriptionEn = "Sounds played when a pawn takes damage, such as pain cries or impact sounds",
                    RelatedMethods = new List<string> { "Pawn.TakeDamage", "DamageWorker.Apply" },
                    RelatedFields = new List<string> { "Pawn.health" },
                    ExampleSoundDefs = new List<string> { "PawnHurt", "ImpactBlunt", "ImpactSharp" }
                },
                new SoundTypeInfo
                {
                    Type = SoundModType.Attack,
                    NameZh = "攻击音效",
                    NameEn = "Attack Sound",
                    DescriptionZh = "攻击动作时播放的音效，如武器射击、挥砍声等",
                    DescriptionEn = "Sounds played during attack actions, such as gunshots or weapon swings",
                    RelatedMethods = new List<string> { "Verb.TryStartCastOn", "Verb.LaunchProjectile" },
                    RelatedFields = new List<string> { "Verb.verbProps" },
                    ExampleSoundDefs = new List<string> { "Shot_AssaultRifle", "Shot_SniperRifle", "MeleeHit" }
                },
                new SoundTypeInfo
                {
                    Type = SoundModType.Movement,
                    NameZh = "移动音效",
                    NameEn = "Movement Sound",
                    DescriptionZh = "角色移动时播放的音效，如脚步声、奔跑声等",
                    DescriptionEn = "Sounds played during pawn movement, such as footsteps or running sounds",
                    RelatedMethods = new List<string> { "PawnPather.PatherTick", "PawnPather.StartPath" },
                    RelatedFields = new List<string> { "Pawn.pather", "Pawn.def.soundMove" },
                    ExampleSoundDefs = new List<string> { "Footstep", "Step_Metal", "Step_Water" }
                },
                new SoundTypeInfo
                {
                    Type = SoundModType.Ambient,
                    NameZh = "环境音效",
                    NameEn = "Ambient Sound",
                    DescriptionZh = "环境背景音效，如风声、雨声、机器运转声等",
                    DescriptionEn = "Ambient background sounds, such as wind, rain, or machine operation",
                    RelatedMethods = new List<string> { "AmbientSoundManager" },
                    RelatedFields = new List<string> { "ThingDef.soundAmbient" },
                    ExampleSoundDefs = new List<string> { "Ambient_Wind", "Ambient_Rain", "Ambient_Generator" }
                },
                new SoundTypeInfo
                {
                    Type = SoundModType.Interaction,
                    NameZh = "交互音效",
                    NameEn = "Interaction Sound",
                    DescriptionZh = "交互动作音效，如开门、拾取物品、建造等",
                    DescriptionEn = "Sounds for interaction actions, such as opening doors, picking up items, or building",
                    RelatedMethods = new List<string> { "JobDriver.Work", "Building.Door" },
                    RelatedFields = new List<string> { "ThingDef.soundInteract" },
                    ExampleSoundDefs = new List<string> { "Door_Open", "DropPod_Open", "Building_Complete" }
                },
                new SoundTypeInfo
                {
                    Type = SoundModType.UI,
                    NameZh = "界面音效",
                    NameEn = "UI Sound",
                    DescriptionZh = "用户界面音效，如按钮点击、通知提示等",
                    DescriptionEn = "User interface sounds, such as button clicks or notification alerts",
                    RelatedMethods = new List<string> { "UI.SoundPlay" },
                    RelatedFields = new List<string> { "SoundDefOf" },
                    ExampleSoundDefs = new List<string> { "Click", "Message", "Alert" }
                },
                new SoundTypeInfo
                {
                    Type = SoundModType.Custom,
                    NameZh = "自定义音效",
                    NameEn = "Custom Sound",
                    DescriptionZh = "其他自定义音效修改需求",
                    DescriptionEn = "Other custom sound modification requirements",
                    RelatedMethods = new List<string>(),
                    RelatedFields = new List<string>(),
                    ExampleSoundDefs = new List<string>()
                }
            };
        }

        private void SetupWizardSteps()
        {
            _innerWizard.AddStep(new SoundWelcomeStep(this));
            _innerWizard.AddStep(new SoundLanguageSelectStep(this));
            _innerWizard.AddStep(new SoundTypeSelectStep(this));
            _innerWizard.AddStep(new SoundTargetSelectStep(this));
            _innerWizard.AddStep(new SoundResourceDisplayStep(this));
            _innerWizard.AddStep(new SoundPatchMethodSelectStep(this));
            _innerWizard.AddStep(new SoundCodeGenerationStep(this));
            _innerWizard.AddStep(new SoundTestSuggestionStep(this));
        }

        public WizardResult Run()
        {
            return _innerWizard.Run();
        }

        public SoundModConfig Config => _config;
        public bool UseChinese => _useChinese;
        public List<SoundTypeInfo> SoundTypes => _soundTypes;
        public ProbeContext Context => _context;
        public FeatureKeywordMap KeywordMap => _keywordMap;
        public PatchRecommender PatchRecommender => _patchRecommender;
        public CodeGenerator CodeGenerator => _codeGenerator;

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

    public class SoundWelcomeStep : WizardStepBase
    {
        private readonly SoundModWizard _wizard;

        public SoundWelcomeStep(SoundModWizard wizard) 
            : base(wizard.T("欢迎 - 音效修改向导", "Welcome - Sound Mod Wizard"), 
                   wizard.T("本向导将引导您完成音效修改 Mod 的开发流程", "This wizard will guide you through the sound mod development process"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();
            
            Console.WriteLine(_wizard.T(
                "欢迎使用音效修改向导！\n\n" +
                "本向导将帮助您：\n" +
                "  1. 选择要修改的音效类型\n" +
                "  2. 确定修改目标（Pawn 类型、武器等）\n" +
                "  3. 查看相关的 SoundDef 和代码位置\n" +
                "  4. 选择修改方式（XML Patch 或 Harmony Patch）\n" +
                "  5. 生成可直接使用的代码\n" +
                "  6. 获取测试建议\n",
                "Welcome to the Sound Mod Wizard!\n\n" +
                "This wizard will help you:\n" +
                "  1. Select the sound type to modify\n" +
                "  2. Determine the target (Pawn type, weapon, etc.)\n" +
                "  3. View related SoundDefs and code locations\n" +
                "  4. Choose modification method (XML Patch or Harmony Patch)\n" +
                "  5. Generate ready-to-use code\n" +
                "  6. Get testing suggestions\n"));
            
            Pause(_wizard.T("按任意键继续...", "Press any key to continue..."));
        }
    }

    public class SoundLanguageSelectStep : WizardStepBase
    {
        private readonly SoundModWizard _wizard;

        public SoundLanguageSelectStep(SoundModWizard wizard) 
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

    public class SoundTypeSelectStep : WizardStepBase
    {
        private readonly SoundModWizard _wizard;

        public SoundTypeSelectStep(SoundModWizard wizard) 
            : base(wizard.T("音效类型选择", "Sound Type Selection"), 
                   wizard.T("选择您要修改的音效类型", "Select the sound type you want to modify"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();
            
            Console.WriteLine(_wizard.T("可用音效类型:", "Available sound types:"));
            Console.WriteLine();
            
            var soundTypes = _wizard.SoundTypes;
            for (int i = 0; i < soundTypes.Count; i++)
            {
                var type = soundTypes[i];
                var name = _wizard.UseChinese ? type.NameZh : type.NameEn;
                var desc = _wizard.UseChinese ? type.DescriptionZh : type.DescriptionEn;
                Console.WriteLine($"  [{i + 1}] {name}");
                Console.WriteLine($"      {desc}");
                Console.WriteLine();
            }
            
            var options = soundTypes.Select(t => _wizard.UseChinese ? t.NameZh : t.NameEn).ToList();
            var selectedName = ReadChoice(_wizard.T("请选择音效类型", "Please select sound type"), options);
            
            var selectedIndex = options.IndexOf(selectedName);
            var selectedType = soundTypes[selectedIndex];
            
            _wizard.Config.SoundType = selectedType.Type;
            context.SetData("SelectedSoundType", selectedType);
            
            ShowInfo(_wizard.T($"已选择: {selectedName}", $"Selected: {selectedName}"));
        }
    }

    public class SoundTargetSelectStep : WizardStepBase
    {
        private readonly SoundModWizard _wizard;

        public SoundTargetSelectStep(SoundModWizard wizard) 
            : base(wizard.T("目标选择", "Target Selection"), 
                   wizard.T("选择具体要修改的目标", "Select the specific target to modify"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();
            
            var soundType = context.GetData<SoundTypeInfo>("SelectedSoundType");
            if (soundType == null)
            {
                ShowError(_wizard.T("未找到音效类型信息", "Sound type information not found"));
                return;
            }
            
            Console.WriteLine(_wizard.T($"正在配置: {(_wizard.UseChinese ? soundType.NameZh : soundType.NameEn)}", 
                                       $"Configuring: {soundType.NameEn}"));
            Console.WriteLine();
            
            _wizard.Config.ModName = ReadInput(_wizard.T("Mod 名称", "Mod name"), "MySoundMod");
            _wizard.Config.AuthorName = ReadInput(_wizard.T("作者名称", "Author name"), "YourName");
            _wizard.Config.Description = ReadInput(_wizard.T("Mod 描述", "Mod description"), 
                _wizard.T("自定义音效修改 Mod", "Custom sound modification mod"));
            
            Console.WriteLine();
            Console.WriteLine(_wizard.T("目标类型选择:", "Target type selection:"));
            
            var targetOptions = GetTargetOptions(soundType.Type);
            for (int i = 0; i < targetOptions.Count; i++)
            {
                Console.WriteLine($"  [{i + 1}] {targetOptions[i]}");
            }
            
            var selectedTarget = ReadChoice(_wizard.T("请选择目标类型", "Please select target type"), targetOptions);
            _wizard.Config.TargetTypeName = selectedTarget;
            
            Console.WriteLine();
            _wizard.Config.CustomSoundDefName = ReadInput(
                _wizard.T("自定义音效定义名称 (可选)", "Custom sound def name (optional)"), 
                $"Custom_{soundType.Type}_Sound");
            
            context.SetData("TargetOptions", targetOptions);
            ShowSuccess(_wizard.T("目标配置完成", "Target configuration completed"));
        }

        private List<string> GetTargetOptions(SoundModType soundType)
        {
            var options = new List<string>();
            
            switch (soundType)
            {
                case SoundModType.Death:
                case SoundModType.Damage:
                case SoundModType.Movement:
                    options.AddRange(new[] { "Pawn (所有角色)", "Humanlike (类人生物)", "Animal (动物)", 
                        "Mechanoid (机械族)", "Custom (自定义类型)" });
                    break;
                case SoundModType.Attack:
                    options.AddRange(new[] { "Verb (所有动作)", "Verb_Shoot (射击)", "Verb_Melee (近战)", 
                        "Specific Weapon (特定武器)", "Custom (自定义)" });
                    break;
                case SoundModType.Ambient:
                    options.AddRange(new[] { "Building (建筑)", "Weather (天气)", "Terrain (地形)", 
                        "Custom (自定义)" });
                    break;
                case SoundModType.Interaction:
                    options.AddRange(new[] { "Door (门)", "Item (物品)", "Building (建筑)", 
                        "Custom (自定义)" });
                    break;
                case SoundModType.UI:
                    options.AddRange(new[] { "Button (按钮)", "Message (消息)", "Alert (警报)", 
                        "Custom (自定义)" });
                    break;
                default:
                    options.Add("Custom (自定义)");
                    break;
            }
            
            return options;
        }
    }

    public class SoundResourceDisplayStep : WizardStepBase
    {
        private readonly SoundModWizard _wizard;

        public SoundResourceDisplayStep(SoundModWizard wizard) 
            : base(wizard.T("资源展示", "Resource Display"), 
                   wizard.T("显示相关的 SoundDef 和代码位置", "Display related SoundDefs and code locations"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();
            
            var soundType = context.GetData<SoundTypeInfo>("SelectedSoundType");
            if (soundType == null)
            {
                ShowError(_wizard.T("未找到音效类型信息", "Sound type information not found"));
                return;
            }
            
            Console.WriteLine(_wizard.T("=== 相关方法 ===", "=== Related Methods ==="));
            Console.WriteLine();
            
            foreach (var method in soundType.RelatedMethods)
            {
                Console.WriteLine($"  * {method}");
                var recommendations = _wizard.PatchRecommender.Recommend(method);
                if (recommendations.Any())
                {
                    var topRec = recommendations.First();
                    Console.WriteLine($"    {_wizard.T("推荐 Patch 类型", "Recommended patch type")}: {topRec.GetPatchTypeName()}");
                    Console.WriteLine($"    {_wizard.T("置信度", "Confidence")}: {topRec.ConfidenceScore}/100");
                }
            }
            
            Console.WriteLine();
            Console.WriteLine(_wizard.T("=== 相关字段 ===", "=== Related Fields ==="));
            Console.WriteLine();
            
            foreach (var field in soundType.RelatedFields)
            {
                Console.WriteLine($"  * {field}");
            }
            
            Console.WriteLine();
            Console.WriteLine(_wizard.T("=== 示例 SoundDef ===", "=== Example SoundDefs ==="));
            Console.WriteLine();
            
            foreach (var soundDef in soundType.ExampleSoundDefs)
            {
                Console.WriteLine($"  * {soundDef}");
            }
            
            Console.WriteLine();
            Console.WriteLine(_wizard.T("=== FeatureKeywordMap 搜索结果 ===", "=== FeatureKeywordMap Search Results ==="));
            Console.WriteLine();
            
            string keyword;
            switch (soundType.Type)
            {
                case SoundModType.Death:
                    keyword = "死亡音效";
                    break;
                case SoundModType.Damage:
                    keyword = "受伤音效";
                    break;
                case SoundModType.Attack:
                    keyword = "攻击音效";
                    break;
                case SoundModType.Movement:
                    keyword = "移动音效";
                    break;
                default:
                    keyword = "音效";
                    break;
            }
            
            var searchResults = _wizard.KeywordMap.Search(keyword, new SearchOptions { MaxResults = 10 });
            foreach (var result in searchResults.Take(5))
            {
                var keywordResult = result as FeatureKeywordResult;
                Console.WriteLine($"  * {result.Name} ({result.Type})");
                if (keywordResult != null)
                {
                    Console.WriteLine($"    {keywordResult.Entry.Description}");
                }
            }
            
            context.SetData("KeywordSearchResults", searchResults.ToList());
            
            Pause(_wizard.T("按任意键继续...", "Press any key to continue..."));
        }
    }

    public class SoundPatchMethodSelectStep : WizardStepBase
    {
        private readonly SoundModWizard _wizard;

        public SoundPatchMethodSelectStep(SoundModWizard wizard) 
            : base(wizard.T("修改方式选择", "Patch Method Selection"), 
                   wizard.T("选择修改方式：XML Patch 或 Harmony Patch", "Select modification method: XML Patch or Harmony Patch"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();
            
            Console.WriteLine(_wizard.T("修改方式对比:", "Patch method comparison:"));
            Console.WriteLine();
            
            Console.WriteLine(_wizard.T("[XML Patch]", "[XML Patch]"));
            Console.WriteLine(_wizard.T("  优点:", "  Pros:"));
            Console.WriteLine(_wizard.T("    * 简单易用，无需编程", "    * Simple to use, no programming required"));
            Console.WriteLine(_wizard.T("    * 适合修改现有 Def 定义", "    * Suitable for modifying existing Def definitions"));
            Console.WriteLine(_wizard.T("    * 兼容性好，不易冲突", "    * Good compatibility, less prone to conflicts"));
            Console.WriteLine(_wizard.T("  缺点:", "  Cons:"));
            Console.WriteLine(_wizard.T("    * 只能修改 Def 数据", "    * Can only modify Def data"));
            Console.WriteLine(_wizard.T("    * 无法添加复杂逻辑", "    * Cannot add complex logic"));
            Console.WriteLine();
            
            Console.WriteLine(_wizard.T("[Harmony Patch]", "[Harmony Patch]"));
            Console.WriteLine(_wizard.T("  优点:", "  Pros:"));
            Console.WriteLine(_wizard.T("    * 可以修改任何方法的行为", "    * Can modify behavior of any method"));
            Console.WriteLine(_wizard.T("    * 支持复杂逻辑和条件判断", "    * Supports complex logic and conditional checks"));
            Console.WriteLine(_wizard.T("    * 可以添加全新的功能", "    * Can add entirely new features"));
            Console.WriteLine(_wizard.T("  缺点:", "  Cons:"));
            Console.WriteLine(_wizard.T("    * 需要编程知识", "    * Requires programming knowledge"));
            Console.WriteLine(_wizard.T("    * 可能与其他 Mod 冲突", "    * May conflict with other mods"));
            Console.WriteLine();
            
            var options = new[] { "XML Patch", "Harmony Patch", _wizard.T("两者都用", "Both") };
            var selectedMethod = ReadChoice(_wizard.T("请选择修改方式", "Please select patch method"), options);
            
            switch (selectedMethod)
            {
                case "XML Patch":
                    _wizard.Config.PatchMethod = PatchMethod.XmlPatch;
                    break;
                case "Harmony Patch":
                    _wizard.Config.PatchMethod = PatchMethod.HarmonyPatch;
                    break;
                default:
                    _wizard.Config.PatchMethod = PatchMethod.HarmonyPatch;
                    break;
            }
            
            context.SetData("PatchMethod", selectedMethod);
            ShowSuccess(_wizard.T($"已选择: {selectedMethod}", $"Selected: {selectedMethod}"));
        }
    }

    public class SoundCodeGenerationStep : WizardStepBase
    {
        private readonly SoundModWizard _wizard;

        public SoundCodeGenerationStep(SoundModWizard wizard) 
            : base(wizard.T("代码生成", "Code Generation"), 
                   wizard.T("生成完整的可编译代码", "Generate complete compilable code"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();
            
            var soundType = context.GetData<SoundTypeInfo>("SelectedSoundType");
            var patchMethod = context.GetData<string>("PatchMethod");
            
            if (soundType == null)
            {
                ShowError(_wizard.T("未找到音效类型信息", "Sound type information not found"));
                return;
            }
            
            var generatedCode = new StringBuilder();
            
            generatedCode.AppendLine(_wizard.T("=== 生成的代码 ===", "=== Generated Code ==="));
            generatedCode.AppendLine();
            
            generatedCode.AppendLine(GenerateModStructure());
            generatedCode.AppendLine();
            
            if (_wizard.Config.PatchMethod == PatchMethod.XmlPatch || patchMethod == _wizard.T("两者都用", "Both"))
            {
                generatedCode.AppendLine(GenerateXmlPatchCode(soundType));
                generatedCode.AppendLine();
            }
            
            if (_wizard.Config.PatchMethod == PatchMethod.HarmonyPatch || patchMethod == _wizard.T("两者都用", "Both"))
            {
                generatedCode.AppendLine(GenerateHarmonyPatchCode(soundType));
                generatedCode.AppendLine();
            }
            
            generatedCode.AppendLine(GenerateSoundDefCode(soundType));
            
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
            sb.AppendLine("|-- Assemblies/");
            sb.AppendLine("|   |-- (编译后的 DLL)");
            sb.AppendLine("|-- Defs/");
            sb.AppendLine("|   |-- SoundDefs/");
            sb.AppendLine("|       |-- Sounds_*.xml");
            sb.AppendLine("|-- Patches/");
            sb.AppendLine("|   |-- Patch_*.xml");
            sb.AppendLine("|-- Sounds/");
            sb.AppendLine("|   |-- (音频文件)");
            sb.AppendLine("|-- Source/");
            sb.AppendLine("    |-- *.cs");
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

        private string GenerateXmlPatchCode(SoundTypeInfo soundType)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine(_wizard.T("=== XML Patch 代码 ===", "=== XML Patch Code ==="));
            sb.AppendLine();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sb.AppendLine("<Patch>");
            sb.AppendLine($"    <!-- {_wizard.T("修改音效定义", "Modify sound definition")} -->");
            sb.AppendLine("    <Operation Class=\"PatchOperationReplace\">");
            
            string defType;
            switch (soundType.Type)
            {
                case SoundModType.Attack:
                case SoundModType.Ambient:
                case SoundModType.Interaction:
                    defType = "ThingDef";
                    break;
                default:
                    defType = "SoundDef";
                    break;
            }
            
            var targetDef = soundType.ExampleSoundDefs.FirstOrDefault() ?? "TargetSoundDef";
            
            sb.AppendLine($"        <xpath>/Defs/{defType}[defName=\"{targetDef}\"]/soundPath</xpath>");
            sb.AppendLine("        <value>");
            sb.AppendLine("            <soundPath>MyMod/Sounds/CustomSound</soundPath>");
            sb.AppendLine("        </value>");
            sb.AppendLine("    </Operation>");
            sb.AppendLine();
            sb.AppendLine($"    <!-- {_wizard.T("添加条件修改", "Add conditional modification")} -->");
            sb.AppendLine("    <Operation Class=\"PatchOperationConditional\">");
            sb.AppendLine($"        <xpath>/Defs/{defType}[defName=\"{targetDef}\"]</xpath>");
            sb.AppendLine("        <match Class=\"PatchOperationReplace\">");
            sb.AppendLine("            <xpath>soundPath</xpath>");
            sb.AppendLine("            <value>");
            sb.AppendLine("                <soundPath>MyMod/Sounds/CustomSound</soundPath>");
            sb.AppendLine("            </value>");
            sb.AppendLine("        </match>");
            sb.AppendLine("    </Operation>");
            sb.AppendLine("</Patch>");
            
            return sb.ToString();
        }

        private string GenerateHarmonyPatchCode(SoundTypeInfo soundType)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine(_wizard.T("=== Harmony Patch 代码 ===", "=== Harmony Patch Code ==="));
            sb.AppendLine();
            
            var targetMethod = soundType.RelatedMethods.FirstOrDefault() ?? "Pawn.Die";
            var parts = targetMethod.Split('.');
            var typeName = parts.Length > 1 ? parts[0] : "Pawn";
            var methodName = parts.Length > 1 ? parts[1] : targetMethod;
            
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Collections.Generic;");
            sb.AppendLine("using System.Reflection;");
            sb.AppendLine("using System.Reflection.Emit;");
            sb.AppendLine("using HarmonyLib;");
            sb.AppendLine("using RimWorld;");
            sb.AppendLine("using Verse;");
            sb.AppendLine("using Verse.Sound;");
            sb.AppendLine();
            sb.AppendLine($"namespace {_wizard.Config.ModName}");
            sb.AppendLine("{");
            sb.AppendLine("    /// <summary>");
            sb.AppendLine($"    /// {_wizard.Config.Description}");
            sb.AppendLine($"    /// {soundType.NameEn} modification patch");
            sb.AppendLine("    /// </summary>");
            sb.AppendLine($"    [HarmonyPatch(typeof({typeName}))]");
            sb.AppendLine($"    public static class {typeName}_{methodName}_SoundPatch");
            sb.AppendLine("    {");
            
            if (soundType.Type == SoundModType.Death)
            {
                GenerateDeathPatchCode(sb, typeName, methodName);
            }
            else if (soundType.Type == SoundModType.Damage)
            {
                GenerateDamagePatchCode(sb, typeName, methodName);
            }
            else if (soundType.Type == SoundModType.Attack)
            {
                GenerateAttackPatchCode(sb, typeName, methodName);
            }
            else if (soundType.Type == SoundModType.Movement)
            {
                GenerateMovementPatchCode(sb, typeName, methodName);
            }
            else
            {
                GenerateGenericPatchCode(sb, typeName, methodName, soundType);
            }
            
            sb.AppendLine("    }");
            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine("/// <summary>");
            sb.AppendLine($"/// {_wizard.T("SoundDef 引用类", "SoundDef reference class")}");
            sb.AppendLine("/// </summary>");
            sb.AppendLine("[DefOf]");
            sb.AppendLine("public static class CustomSoundDefOf");
            sb.AppendLine("{");
            sb.AppendLine($"    public static SoundDef {_wizard.Config.CustomSoundDefName};");
            sb.AppendLine("}");
            
            return sb.ToString();
        }

        private void GenerateDeathPatchCode(StringBuilder sb, string typeName, string methodName)
        {
            sb.AppendLine("        /// <summary>");
            sb.AppendLine($"        /// {_wizard.T("死亡音效 Postfix 补丁", "Death sound Postfix patch")}");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine($"        [HarmonyPatch(\"{methodName}\")]");
            sb.AppendLine($"        public static class {methodName}_Patch");
            sb.AppendLine("        {");
            sb.AppendLine($"            public static void Postfix({typeName} __instance)");
            sb.AppendLine("            {");
            sb.AppendLine($"                // {_wizard.T("检查是否为特定类型的角色", "Check if pawn is of specific type")}");
            sb.AppendLine("                if (__instance.def.defName == \"YourTargetPawnDef\")");
            sb.AppendLine("                {");
            sb.AppendLine($"                    // {_wizard.T("播放自定义死亡音效", "Play custom death sound")}");
            sb.AppendLine($"                    CustomSoundDefOf.{_wizard.Config.CustomSoundDefName}?.PlayOneShot(SoundInfo.InMap(__instance));");
            sb.AppendLine("                }");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
        }

        private void GenerateDamagePatchCode(StringBuilder sb, string typeName, string methodName)
        {
            sb.AppendLine("        /// <summary>");
            sb.AppendLine($"        /// {_wizard.T("受伤音效 Prefix 补丁", "Damage sound Prefix patch")}");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine($"        [HarmonyPatch(\"{methodName}\")]");
            sb.AppendLine($"        public static class {methodName}_Patch");
            sb.AppendLine("        {");
            sb.AppendLine($"            public static void Postfix({typeName} __instance, DamageInfo dinfo)");
            sb.AppendLine("            {");
            sb.AppendLine($"                // {_wizard.T("根据伤害类型播放不同音效", "Play different sounds based on damage type")}");
            sb.AppendLine("                if (__instance.def.defName == \"YourTargetPawnDef\")");
            sb.AppendLine("                {");
            sb.AppendLine("                    if (dinfo.Def.defName == \"Bullet\")");
            sb.AppendLine("                    {");
            sb.AppendLine($"                        // {_wizard.T("子弹伤害音效", "Bullet damage sound")}");
            sb.AppendLine($"                        CustomSoundDefOf.{_wizard.Config.CustomSoundDefName}?.PlayOneShot(SoundInfo.InMap(__instance));");
            sb.AppendLine("                    }");
            sb.AppendLine("                }");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
        }

        private void GenerateAttackPatchCode(StringBuilder sb, string typeName, string methodName)
        {
            sb.AppendLine("        /// <summary>");
            sb.AppendLine($"        /// {_wizard.T("攻击音效 Prefix 补丁", "Attack sound Prefix patch")}");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine($"        [HarmonyPatch(\"{methodName}\")]");
            sb.AppendLine($"        public static class {methodName}_Patch");
            sb.AppendLine("        {");
            sb.AppendLine($"            public static void Prefix({typeName} __instance)");
            sb.AppendLine("            {");
            sb.AppendLine($"                // {_wizard.T("获取攻击者", "Get attacker")}");
            sb.AppendLine("                var caster = __instance.CasterPawn;");
            sb.AppendLine("                if (caster?.def.defName == \"YourTargetPawnDef\")");
            sb.AppendLine("                {");
            sb.AppendLine($"                    // {_wizard.T("播放自定义攻击音效", "Play custom attack sound")}");
            sb.AppendLine($"                    CustomSoundDefOf.{_wizard.Config.CustomSoundDefName}?.PlayOneShot(SoundInfo.InMap(caster));");
            sb.AppendLine("                }");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
        }

        private void GenerateMovementPatchCode(StringBuilder sb, string typeName, string methodName)
        {
            sb.AppendLine("        /// <summary>");
            sb.AppendLine($"        /// {_wizard.T("移动音效 Postfix 补丁", "Movement sound Postfix patch")}");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine($"        [HarmonyPatch(typeof(PawnPather), \"{methodName}\")]");
            sb.AppendLine($"        public static class Pather_{methodName}_Patch");
            sb.AppendLine("        {");
            sb.AppendLine("            private static int _lastSoundTick;");
            sb.AppendLine();
            sb.AppendLine("            public static void Postfix(Pawn ___pawn)");
            sb.AppendLine("            {");
            sb.AppendLine($"                // {_wizard.T("限制音效播放频率", "Limit sound playback rate")}");
            sb.AppendLine("                if (Find.TickManager.TicksGame - _lastSoundTick < 60)");
            sb.AppendLine("                    return;");
            sb.AppendLine();
            sb.AppendLine("                if (___pawn?.def.defName == \"YourTargetPawnDef\" && ___pawn.pather.Moving)");
            sb.AppendLine("                {");
            sb.AppendLine("                    _lastSoundTick = Find.TickManager.TicksGame;");
            sb.AppendLine($"                    CustomSoundDefOf.{_wizard.Config.CustomSoundDefName}?.PlayOneShot(SoundInfo.InMap(___pawn));");
            sb.AppendLine("                }");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
        }

        private void GenerateGenericPatchCode(StringBuilder sb, string typeName, string methodName, SoundTypeInfo soundType)
        {
            sb.AppendLine("        /// <summary>");
            sb.AppendLine($"        /// {_wizard.T("通用音效补丁", "Generic sound patch")}");
            sb.AppendLine("        /// </summary>");
            sb.AppendLine($"        [HarmonyPatch(\"{methodName}\")]");
            sb.AppendLine($"        public static class {methodName}_Patch");
            sb.AppendLine("        {");
            sb.AppendLine($"            public static void Postfix({typeName} __instance)");
            sb.AppendLine("            {");
            sb.AppendLine($"                // {_wizard.T("添加自定义音效逻辑", "Add custom sound logic")}");
            sb.AppendLine($"                // {_wizard.T("检查条件", "Check conditions")}");
            sb.AppendLine("                if (ShouldPlayCustomSound(__instance))");
            sb.AppendLine("                {");
            sb.AppendLine($"                    CustomSoundDefOf.{_wizard.Config.CustomSoundDefName}?.PlayOneShot(SoundInfo.InMap(__instance));");
            sb.AppendLine("                }");
            sb.AppendLine("            }");
            sb.AppendLine();
            sb.AppendLine($"            private static bool ShouldPlayCustomSound({typeName} instance)");
            sb.AppendLine("            {");
            sb.AppendLine($"                // {_wizard.T("添加您的条件判断逻辑", "Add your condition logic")}");
            sb.AppendLine("                return true;");
            sb.AppendLine("            }");
            sb.AppendLine("        }");
        }

        private string GenerateSoundDefCode(SoundTypeInfo soundType)
        {
            var sb = new StringBuilder();
            
            sb.AppendLine(_wizard.T("=== SoundDef 定义 ===", "=== SoundDef Definition ==="));
            sb.AppendLine();
            sb.AppendLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sb.AppendLine("<Defs>");
            sb.AppendLine("    <SoundDef>");
            sb.AppendLine($"        <defName>{_wizard.Config.CustomSoundDefName}</defName>");
            sb.AppendLine($"        <!-- {_wizard.T("音效名称", "Sound name")} -->");
            sb.AppendLine($"        <label>{soundType.NameEn} Custom Sound</label>");
            sb.AppendLine("        <subSounds>");
            sb.AppendLine("            <li>");
            sb.AppendLine("                <grains>");
            sb.AppendLine("                    <li Class=\"AudioGrain_Clip\">");
            sb.AppendLine($"                        <clipPath>MyMod/Sounds/{_wizard.Config.CustomSoundDefName}</clipPath>");
            sb.AppendLine("                    </li>");
            sb.AppendLine("                </grains>");
            sb.AppendLine("                <volumeRange>");
            sb.AppendLine("                    <min>50</min>");
            sb.AppendLine("                    <max>100</max>");
            sb.AppendLine("                </volumeRange>");
            sb.AppendLine("                <pitchRange>");
            sb.AppendLine("                    <min>0.9</min>");
            sb.AppendLine("                    <max>1.1</max>");
            sb.AppendLine("                </pitchRange>");
            sb.AppendLine("            </li>");
            sb.AppendLine("        </subSounds>");
            sb.AppendLine("    </SoundDef>");
            sb.AppendLine("</Defs>");
            
            return sb.ToString();
        }
    }

    public class SoundTestSuggestionStep : WizardStepBase
    {
        private readonly SoundModWizard _wizard;

        public SoundTestSuggestionStep(SoundModWizard wizard) 
            : base(wizard.T("测试建议", "Test Suggestions"), 
                   wizard.T("提供测试建议和注意事项", "Provide testing suggestions and notes"))
        {
            _wizard = wizard;
        }

        public override void Execute(WizardContext context)
        {
            ShowHeader();
            
            var soundType = context.GetData<SoundTypeInfo>("SelectedSoundType");
            
            Console.WriteLine(_wizard.T("=== 测试步骤 ===", "=== Testing Steps ==="));
            Console.WriteLine();
            
            Console.WriteLine(_wizard.T("1. 准备音频文件", "1. Prepare audio files"));
            Console.WriteLine(_wizard.T("   * 支持格式: .wav, .mp3, .ogg", "   * Supported formats: .wav, .mp3, .ogg"));
            Console.WriteLine(_wizard.T("   * 建议使用 .ogg 格式以获得最佳兼容性", "   * Recommend .ogg format for best compatibility"));
            Console.WriteLine(_wizard.T("   * 音频时长建议控制在 2-5 秒", "   * Recommended duration: 2-5 seconds"));
            Console.WriteLine();
            
            Console.WriteLine(_wizard.T("2. 编译代码", "2. Compile code"));
            Console.WriteLine(_wizard.T("   * 确保引用了正确的程序集:", "   * Ensure correct assembly references:"));
            Console.WriteLine("     - Assembly-CSharp.dll");
            Console.WriteLine("     - UnityEngine.dll");
            Console.WriteLine("     - 0Harmony.dll");
            Console.WriteLine();
            
            Console.WriteLine(_wizard.T("3. 部署 Mod", "3. Deploy mod"));
            Console.WriteLine(_wizard.T("   * 将编译后的 DLL 放入 Assemblies 文件夹", "   * Place compiled DLL in Assemblies folder"));
            Console.WriteLine(_wizard.T("   * 将音频文件放入 Sounds 文件夹", "   * Place audio files in Sounds folder"));
            Console.WriteLine(_wizard.T("   * 确保 XML 文件路径正确", "   * Ensure XML file paths are correct"));
            Console.WriteLine();
            
            Console.WriteLine(_wizard.T("4. 游戏内测试", "4. In-game testing"));
            Console.WriteLine(_wizard.T("   * 启用开发模式以便调试", "   * Enable development mode for debugging"));
            Console.WriteLine(_wizard.T("   * 检查音效是否正确加载", "   * Check if sound is loaded correctly"));
            Console.WriteLine(_wizard.T("   * 测试各种触发条件", "   * Test various trigger conditions"));
            Console.WriteLine();
            
            Console.WriteLine(_wizard.T("=== 注意事项 ===", "=== Notes ==="));
            Console.WriteLine();
            
            if (soundType != null)
            {
                PrintTypeSpecificNotes(soundType);
            }
            
            Console.WriteLine(_wizard.T("通用注意事项:", "General notes:"));
            Console.WriteLine(_wizard.T("  * 音效文件路径区分大小写", "  * Sound file paths are case-sensitive"));
            Console.WriteLine(_wizard.T("  * 确保 SoundDef 的 defName 唯一", "  * Ensure SoundDef defName is unique"));
            Console.WriteLine(_wizard.T("  * 测试与其他音效 Mod 的兼容性", "  * Test compatibility with other sound mods"));
            Console.WriteLine(_wizard.T("  * 使用日志输出调试信息", "  * Use logging for debugging"));
            Console.WriteLine();
            
            Console.WriteLine(_wizard.T("=== 调试技巧 ===", "=== Debug Tips ==="));
            Console.WriteLine();
            Console.WriteLine(_wizard.T("添加日志输出:", "Add logging:"));
            Console.WriteLine("  Log.Message(\"[MyMod] Sound triggered\");");
            Console.WriteLine();
            Console.WriteLine(_wizard.T("检查 SoundDef 是否加载:", "Check if SoundDef is loaded:"));
            Console.WriteLine($"  if (CustomSoundDefOf.{_wizard.Config.CustomSoundDefName} == null)");
            Console.WriteLine("  {");
            Console.WriteLine("      Log.Error(\"[MyMod] SoundDef not found!\");");
            Console.WriteLine("  }");
            Console.WriteLine();
            
            Pause(_wizard.T("按任意键完成向导...", "Press any key to finish wizard..."));
            
            ShowSuccess(_wizard.T("向导完成！", "Wizard completed!"));
            Console.WriteLine();
            Console.WriteLine(_wizard.T("生成的代码已保存到上下文中，您可以复制使用。", "Generated code is saved in context, you can copy and use it."));
        }

        private void PrintTypeSpecificNotes(SoundTypeInfo soundType)
        {
            Console.WriteLine(_wizard.T(soundType.NameZh + " 特定注意事项:", soundType.NameEn + " specific notes:"));
            
            switch (soundType.Type)
            {
                case SoundModType.Death:
                    Console.WriteLine(_wizard.T("  - 注意处理死亡原因的不同情况", "  - Handle different death causes"));
                    Console.WriteLine(_wizard.T("  - 考虑是否需要阻止原始音效", "  - Consider whether to block original sound"));
                    Console.WriteLine(_wizard.T("  - 测试各种死亡方式(战斗、饥饿等)", "  - Test various death types (combat, starvation, etc.)"));
                    break;
                case SoundModType.Damage:
                    Console.WriteLine(_wizard.T("  - 处理不同伤害类型", "  - Handle different damage types"));
                    Console.WriteLine(_wizard.T("  - 注意伤害量阈值", "  - Note damage amount thresholds"));
                    Console.WriteLine(_wizard.T("  - 避免音效过于频繁播放", "  - Avoid too frequent sound playback"));
                    break;
                case SoundModType.Attack:
                    Console.WriteLine(_wizard.T("  - 区分近战和远程攻击", "  - Distinguish melee and ranged attacks"));
                    Console.WriteLine(_wizard.T("  - 考虑武器类型", "  - Consider weapon types"));
                    Console.WriteLine(_wizard.T("  - 注意攻击冷却时间", "  - Note attack cooldown"));
                    break;
                case SoundModType.Movement:
                    Console.WriteLine(_wizard.T("  - 控制播放频率避免嘈杂", "  - Control playback rate to avoid noise"));
                    Console.WriteLine(_wizard.T("  - 考虑地形类型影响", "  - Consider terrain type effects"));
                    Console.WriteLine(_wizard.T("  - 处理不同移动速度", "  - Handle different movement speeds"));
                    break;
            }
            
            Console.WriteLine();
        }
    }
}
