using System;
using System.Collections.Generic;
using System.Linq;
using RimWorldModDevProbe.Core;

namespace RimWorldModDevProbe.Analysis
{
    public enum RecommendationType
    {
        Method,
        Field,
        Class,
        Property
    }

    public class FeatureKeywordEntry
    {
        public List<string> Keywords { get; set; } = new List<string>();
        public RecommendationType RecommendationType { get; set; }
        public string TargetName { get; set; }
        public string Description { get; set; }
        public string Reason { get; set; }
        public string RelatedType { get; set; }
        public int Priority { get; set; } = 0;
    }

    public class FeatureKeywordResult : ProbeResult
    {
        public FeatureKeywordEntry Entry { get; }
        public int MatchScore { get; }
        public string MatchedKeyword { get; }

        public FeatureKeywordResult(FeatureKeywordEntry entry, int matchScore, string matchedKeyword)
        {
            Entry = entry;
            MatchScore = matchScore;
            MatchedKeyword = matchedKeyword;
            Id = $"keyword_{entry.TargetName}_{entry.RecommendationType}";
            Name = entry.TargetName;
            Type = entry.RecommendationType.ToString();
            Source = "FeatureKeywordMap";
            Location = entry.RelatedType ?? "";
        }

        public override void PrintDetails()
        {
            Console.WriteLine($"\n--- {Entry.TargetName} ({Entry.RecommendationType}) ---");
            Console.WriteLine($"Description: {Entry.Description}");
            Console.WriteLine($"Recommendation Reason: {Entry.Reason}");
            if (!string.IsNullOrEmpty(Entry.RelatedType))
            {
                Console.WriteLine($"Related Type: {Entry.RelatedType}");
            }
            Console.WriteLine($"Matched Keyword: {MatchedKeyword}");
            Console.WriteLine($"Relevance Score: {MatchScore}");
            Console.WriteLine($"\nKeywords: {string.Join(", ", Entry.Keywords)}");
        }
    }

    public class FeatureKeywordMap : IProbe
    {
        public string Name => "keyword";
        private ProbeContext _context;
        private List<FeatureKeywordEntry> _entries;

        public void Initialize(ProbeContext context)
        {
            _context = context;
            InitializeEntries();
        }

        private void InitializeEntries()
        {
            _entries = new List<FeatureKeywordEntry>
            {
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "死亡音效", "death sound", "死亡声音", "die sound", "death audio" },
                    RecommendationType = RecommendationType.Method,
                    TargetName = "Pawn.Die",
                    Description = "Pawn.Die() 方法处理角色死亡逻辑，包括触发死亡音效",
                    Reason = "死亡时播放音效的核心方法，可通过 Harmony 前缀/后缀修改或添加自定义音效",
                    RelatedType = "Verse.Pawn",
                    Priority = 10
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "死亡音效", "death sound", "死亡声音", "die sound" },
                    RecommendationType = RecommendationType.Field,
                    TargetName = "Pawn.health",
                    Description = "Pawn.health 属性包含角色的健康状态信息",
                    Reason = "通过 healthTracker 可以检测死亡状态，在死亡前后执行自定义逻辑",
                    RelatedType = "Verse.Pawn",
                    Priority = 8
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "死亡音效", "death sound", "音效定义", "sound def", "声音定义" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "SoundDef",
                    Description = "SoundDef 用于定义游戏中的音效资源",
                    Reason = "创建自定义 SoundDef 来定义新的死亡音效，然后在 Pawn.Die 中播放",
                    RelatedType = "Verse.SoundDef",
                    Priority = 9
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "受伤音效", "damage sound", "受伤声音", "hurt sound", "伤害音效", "hit sound" },
                    RecommendationType = RecommendationType.Method,
                    TargetName = "Pawn.TakeDamage",
                    Description = "Pawn.TakeDamage() 方法处理角色受到伤害的逻辑",
                    Reason = "受伤时播放音效的核心入口点，可在此添加或修改受伤音效",
                    RelatedType = "Verse.Pawn",
                    Priority = 10
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "受伤音效", "damage sound", "伤害处理", "damage worker" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "DamageWorker",
                    Description = "DamageWorker 类处理不同类型伤害的具体逻辑",
                    Reason = "每种伤害类型都有对应的 DamageWorker，可在其中自定义受伤音效",
                    RelatedType = "Verse.DamageWorker",
                    Priority = 9
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "攻击音效", "attack sound", "攻击声音", "weapon sound", "射击音效", "shoot sound" },
                    RecommendationType = RecommendationType.Method,
                    TargetName = "Verb.TryStartCastOn",
                    Description = "Verb.TryStartCastOn() 方法处理攻击动作的启动",
                    Reason = "攻击动作的核心方法，可在此添加攻击音效播放逻辑",
                    RelatedType = "Verse.Verb",
                    Priority = 10
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "攻击音效", "attack sound", "射击音效", "shoot sound", "枪声" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "Verb",
                    Description = "Verb 类是所有动作（攻击、使用物品等）的基类",
                    Reason = "不同类型的 Verb（如 Verb_Shoot）处理不同的攻击方式，可在其中自定义音效",
                    RelatedType = "Verse.Verb",
                    Priority = 8
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "移动音效", "movement sound", "脚步声", "footstep sound", "走路声音", "walk sound" },
                    RecommendationType = RecommendationType.Field,
                    TargetName = "Pawn.pather",
                    Description = "Pawn.pather 属性管理角色的寻路和移动",
                    Reason = "通过 pather 可以检测移动状态，在移动时播放脚步声等音效",
                    RelatedType = "Verse.Pawn",
                    Priority = 10
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "移动音效", "movement sound", "脚步声", "footstep sound" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "PawnPather",
                    Description = "PawnPather 类处理角色的寻路和移动逻辑",
                    Reason = "可在 Pather 的移动方法中添加脚步声音效播放",
                    RelatedType = "Verse.PawnPather",
                    Priority = 9
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "建造", "building", "建筑", "construct", "放置建筑", "place building" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "Frame",
                    Description = "Frame 类表示建造中的建筑框架",
                    Reason = "建造过程中使用 Frame 对象，可在建造完成/取消时添加自定义逻辑",
                    RelatedType = "Verse.Frame",
                    Priority = 10
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "建造", "building", "建筑", "construct", "建筑定义" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "Building",
                    Description = "Building 类是所有建筑的基类",
                    Reason = "继承 Building 创建自定义建筑，或通过 Harmony 修改建筑行为",
                    RelatedType = "Verse.Building",
                    Priority = 9
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "建造", "building", "建筑", "construct", "搬运建筑", "minified" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "MinifiedThing",
                    Description = "MinifiedThing 类表示可搬运的打包建筑",
                    Reason = "用于创建可搬运的建筑物品，如家具等",
                    RelatedType = "Verse.MinifiedThing",
                    Priority = 8
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "研究", "research", "科技", "technology", "解锁", "unlock" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "ResearchManager",
                    Description = "ResearchManager 类管理游戏的研究系统",
                    Reason = "通过 ResearchManager 可以检测研究完成事件，添加自定义奖励等",
                    RelatedType = "Verse.ResearchManager",
                    Priority = 10
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "研究", "research", "科技", "technology", "研究项目" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "ResearchProjectDef",
                    Description = "ResearchProjectDef 定义研究项目的属性和需求",
                    Reason = "创建自定义 ResearchProjectDef 来添加新的研究项目",
                    RelatedType = "Verse.ResearchProjectDef",
                    Priority = 9
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "贸易", "trade", "交易", "商人", "merchant", "买卖" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "TradeShip",
                    Description = "TradeShip 类表示轨道贸易飞船",
                    Reason = "通过 TradeShip 可以检测贸易飞船到达/离开事件",
                    RelatedType = "RimWorld.TradeShip",
                    Priority = 10
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "贸易", "trade", "交易", "买卖", "deal" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "TradeDeal",
                    Description = "TradeDeal 类处理具体的贸易交易逻辑",
                    Reason = "在 TradeDeal 中可以修改贸易价格、添加贸易限制等",
                    RelatedType = "RimWorld.TradeDeal",
                    Priority = 9
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "音效", "sound", "声音", "audio", "播放音效", "play sound" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "SoundDef",
                    Description = "SoundDef 定义游戏中的音效资源",
                    Reason = "创建 SoundDef 定义音效，使用 SoundDefOf 快速访问常用音效",
                    RelatedType = "Verse.SoundDef",
                    Priority = 10
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "音效", "sound", "播放音效", "play sound" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "SoundInfo",
                    Description = "SoundInfo 包含音效播放的参数信息",
                    Reason = "使用 SoundInfo 配置音效播放的位置、音量等参数",
                    RelatedType = "Verse.SoundInfo",
                    Priority = 8
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "角色", "pawn", "生物", "creature", "单位", "unit" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "Pawn",
                    Description = "Pawn 类是游戏中所有角色的基类",
                    Reason = "Pawn 包含角色的所有状态和行为，是 Modding 的核心类之一",
                    RelatedType = "Verse.Pawn",
                    Priority = 10
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "物品", "item", "东西", "thing", "装备", "equipment" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "Thing",
                    Description = "Thing 类是游戏中所有物品和实体的基类",
                    Reason = "Thing 是最基础的类，Pawn、Building 等都继承自它",
                    RelatedType = "Verse.Thing",
                    Priority = 10
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "事件", "incident", "随机事件", "random event", "突发事件" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "IncidentDef",
                    Description = "IncidentDef 定义游戏中的随机事件",
                    Reason = "创建自定义 IncidentDef 添加新的事件类型",
                    RelatedType = "RimWorld.IncidentDef",
                    Priority = 9
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "事件", "incident", "事件工作器", "incident worker" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "IncidentWorker",
                    Description = "IncidentWorker 处理事件的具体执行逻辑",
                    Reason = "继承 IncidentWorker 实现自定义事件的执行逻辑",
                    RelatedType = "RimWorld.IncidentWorker",
                    Priority = 10
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "任务", "quest", "委托", "mission" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "Quest",
                    Description = "Quest 类管理游戏中的任务系统",
                    Reason = "通过 Quest 可以创建自定义任务和奖励",
                    RelatedType = "RimWorld.Quest",
                    Priority = 10
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "技能", "skill", "能力", "ability" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "SkillDef",
                    Description = "SkillDef 定义角色的技能类型",
                    Reason = "创建自定义 SkillDef 添加新的技能类型",
                    RelatedType = "RimWorld.SkillDef",
                    Priority = 9
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "技能", "skill", "技能记录", "skill record" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "SkillRecord",
                    Description = "SkillRecord 记录角色的具体技能等级和经验",
                    Reason = "通过 SkillRecord 可以修改角色的技能等级和经验",
                    RelatedType = "RimWorld.SkillRecord",
                    Priority = 10
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "武器", "weapon", "枪械", "gun", "近战武器", "melee" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "ThingDef",
                    Description = "ThingDef 定义物品的属性，包括武器",
                    Reason = "创建自定义 ThingDef 定义新的武器类型",
                    RelatedType = "Verse.ThingDef",
                    Priority = 9
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "武器", "weapon", "武器属性", "weapon stats" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "VerbProperties",
                    Description = "VerbProperties 定义武器的攻击属性",
                    Reason = "通过 VerbProperties 配置武器的射程、伤害、冷却等属性",
                    RelatedType = "Verse.VerbProperties",
                    Priority = 8
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "工作", "work", "任务", "job", "工作类型", "work type" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "WorkTypeDef",
                    Description = "WorkTypeDef 定义角色可执行的工作类型",
                    Reason = "创建自定义 WorkTypeDef 添加新的工作类型",
                    RelatedType = "RimWorld.WorkTypeDef",
                    Priority = 9
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "工作", "work", "工作给予者", "work giver" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "WorkGiver",
                    Description = "WorkGiver 决定角色如何寻找和执行工作",
                    Reason = "继承 WorkGiver 实现自定义的工作分配逻辑",
                    RelatedType = "RimWorld.WorkGiver",
                    Priority = 10
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "UI", "界面", "界面", "interface", "窗口", "window", "菜单", "menu" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "Window",
                    Description = "Window 类是所有游戏界面的基类",
                    Reason = "继承 Window 创建自定义界面窗口",
                    RelatedType = "Verse.Window",
                    Priority = 10
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "UI", "界面", "窗口", "window", "调试", "debug" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "DebugWindowsOpener",
                    Description = "DebugWindowsOpener 提供调试窗口的打开方法",
                    Reason = "用于开发调试时打开各种调试窗口",
                    RelatedType = "RimWorld.DebugWindowsOpener",
                    Priority = 7
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "存档", "save", "加载", "load", "保存", "saving" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "SaveLoader",
                    Description = "SaveLoader 处理游戏存档的加载",
                    Reason = "通过 SaveLoader 可以自定义存档加载逻辑",
                    RelatedType = "Verse.SaveLoader",
                    Priority = 9
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "存档", "save", "保存数据", "save data", "序列化", "serialize" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "Scribe",
                    Description = "Scribe 类提供游戏数据的序列化和反序列化功能",
                    Reason = "使用 Scribe.Look 方法保存和加载自定义数据",
                    RelatedType = "Verse.Scribe",
                    Priority = 10
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "地图", "map", "世界", "world", "地图生成", "map generation" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "Map",
                    Description = "Map 类表示游戏中的一个地图",
                    Reason = "通过 Map 可以访问地图上的所有物品、角色和地形",
                    RelatedType = "Verse.Map",
                    Priority = 10
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "地图", "map", "地图生成", "map generation", "地形生成" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "MapGenerator",
                    Description = "MapGenerator 处理地图的生成逻辑",
                    Reason = "通过 MapGenerator 可以自定义地图生成过程",
                    RelatedType = "Verse.MapGenerator",
                    Priority = 9
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "派系", "faction", "势力", "force", "阵营" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "Faction",
                    Description = "Faction 类表示游戏中的一个派系",
                    Reason = "通过 Faction 可以管理派系关系、资源和成员",
                    RelatedType = "RimWorld.Faction",
                    Priority = 10
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "派系", "faction", "派系定义", "faction def" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "FactionDef",
                    Description = "FactionDef 定义派系的属性",
                    Reason = "创建自定义 FactionDef 添加新的派系类型",
                    RelatedType = "RimWorld.FactionDef",
                    Priority = 9
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "Harmony", "补丁", "patch", "注入", "inject", "修改方法" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "HarmonyPatch",
                    Description = "HarmonyPatch 特性用于标记 Harmony 补丁方法",
                    Reason = "使用 HarmonyPatch 特性修改游戏方法的执行逻辑",
                    RelatedType = "HarmonyLib.HarmonyPatch",
                    Priority = 10
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "Harmony", "前缀", "prefix", "方法前", "before method" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "HarmonyPrefix",
                    Description = "HarmonyPrefix 特性标记在原方法执行前运行的代码",
                    Reason = "使用 HarmonyPrefix 在原方法执行前添加自定义逻辑或跳过原方法",
                    RelatedType = "HarmonyLib.HarmonyPrefix",
                    Priority = 10
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "Harmony", "后缀", "postfix", "方法后", "after method" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "HarmonyPostfix",
                    Description = "HarmonyPostfix 特性标记在原方法执行后运行的代码",
                    Reason = "使用 HarmonyPostfix 在原方法执行后添加自定义逻辑",
                    RelatedType = "HarmonyLib.HarmonyPostfix",
                    Priority = 10
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "Def", "定义", "definition", "数据定义", "xml定义" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "Def",
                    Description = "Def 类是所有数据定义的基类",
                    Reason = "所有 ThingDef、SoundDef 等都继承自 Def，可通过 DefDatabase 访问",
                    RelatedType = "Verse.Def",
                    Priority = 10
                },
                new FeatureKeywordEntry
                {
                    Keywords = new List<string> { "Def", "定义", "definition", "Def数据库", "def database" },
                    RecommendationType = RecommendationType.Class,
                    TargetName = "DefDatabase",
                    Description = "DefDatabase 管理所有 Def 的数据库",
                    Reason = "使用 DefDatabase<T>.GetNamed 或 AllDefs 访问游戏定义数据",
                    RelatedType = "Verse.DefDatabase",
                    Priority = 10
                }
            };
        }

        public IEnumerable<ProbeResult> Search(string query, SearchOptions options)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<ProbeResult>();

            var results = new List<FeatureKeywordResult>();
            var normalizedQuery = query.ToLowerInvariant().Trim();

            foreach (var entry in _entries)
            {
                var bestMatch = FindBestMatch(normalizedQuery, entry, options);
                if (bestMatch != null)
                {
                    results.Add(bestMatch);
                }
            }

            return results
                .OrderByDescending(r => r.MatchScore)
                .ThenByDescending(r => r.Entry.Priority)
                .Take(options.MaxResults);
        }

        private FeatureKeywordResult FindBestMatch(string query, FeatureKeywordEntry entry, SearchOptions options)
        {
            int bestScore = 0;
            string bestKeyword = null;

            foreach (var keyword in entry.Keywords)
            {
                var normalizedKeyword = keyword.ToLowerInvariant();
                int score = CalculateMatchScore(query, normalizedKeyword, options);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestKeyword = keyword;
                }
            }

            if (bestScore > 0)
            {
                return new FeatureKeywordResult(entry, bestScore, bestKeyword);
            }

            return null;
        }

        private int CalculateMatchScore(string query, string keyword, SearchOptions options)
        {
            if (options.ExactMatch)
            {
                var comparison = options.CaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                return query.Equals(keyword, comparison) ? 100 : 0;
            }

            int score = 0;

            if (keyword == query)
            {
                score = 100;
            }
            else if (keyword.StartsWith(query))
            {
                score = 90 + (query.Length * 10 / Math.Max(keyword.Length, 1));
            }
            else if (keyword.Contains(query))
            {
                score = 70 + (query.Length * 20 / Math.Max(keyword.Length, 1));
            }
            else if (CalculateFuzzyMatch(query, keyword))
            {
                score = 50 + (query.Length * 10 / Math.Max(keyword.Length, 1));
            }

            return score;
        }

        private bool CalculateFuzzyMatch(string query, string keyword)
        {
            if (string.IsNullOrEmpty(query) || string.IsNullOrEmpty(keyword))
                return false;

            int queryIndex = 0;
            int keywordIndex = 0;

            while (queryIndex < query.Length && keywordIndex < keyword.Length)
            {
                if (query[queryIndex] == keyword[keywordIndex])
                {
                    queryIndex++;
                }
                keywordIndex++;
            }

            return queryIndex == query.Length;
        }

        public ProbeResult GetDetails(string id)
        {
            var parts = id.Split('_');
            if (parts.Length < 3) return null;

            var targetName = parts[1];
            var typeStr = parts[2];

            if (!Enum.TryParse<RecommendationType>(typeStr, out var recType))
                return null;

            var entry = _entries.FirstOrDefault(e =>
                e.TargetName == targetName && e.RecommendationType == recType);

            if (entry != null)
            {
                return new FeatureKeywordResult(entry, 100, entry.Keywords.FirstOrDefault() ?? "");
            }

            return null;
        }

        public void ClearCache()
        {
        }

        public List<FeatureKeywordEntry> GetAllEntries()
        {
            return _entries.ToList();
        }

        public List<FeatureKeywordEntry> GetEntriesByType(RecommendationType type)
        {
            return _entries.Where(e => e.RecommendationType == type).ToList();
        }

        public void AddEntry(FeatureKeywordEntry entry)
        {
            _entries.Add(entry);
        }

        public void AddEntries(IEnumerable<FeatureKeywordEntry> entries)
        {
            _entries.AddRange(entries);
        }
    }
}
