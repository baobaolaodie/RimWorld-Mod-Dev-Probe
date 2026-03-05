using System.Collections.Generic;

namespace RimWorldModDevProbe.Examples
{
    public static class IncidentExamples
    {
        public static List<Example> GetExamples()
        {
            var examples = new List<Example>();

            examples.Add(GetCustomIncidentExample());
            examples.Add(GetRaidIncidentExample());
            examples.Add(GetEventIncidentExample());

            return examples;
        }

        private static Example GetCustomIncidentExample()
        {
            var example = new Example
            {
                Title = "自定义事件示例",
                Description = "创建一个全新的随机事件，包括事件定义、工作类和UI显示。",
                Feature = "事件定义",
                Keywords = new List<string> { "事件", "incident", "事件mod", "自定义事件", "magic storm" }
            };

            example.Files.Add(new ExampleFile(
                "CustomIncidentDef.xml",
                "Defs/IncidentDef/CustomIncidentDef.xml",
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Defs>
    <IncidentDef>
        <defName>CustomResourceDrop</defName>
        <label>Resource Pod Crash</label>
        <category>Misc</category>
        <targetTags>
            <li>Map_PlayerHome</li>
        </targetTags>
        <workerClass>CustomIncidentMod.IncidentWorker_ResourcePodCrash</workerClass>
        <baseChance>1.5</baseChance>
        <minRefireDays>5</minRefireDays>
        <maxRefireDays>15</maxRefireDays>
        <minDifficulty>1.0</minDifficulty>
        <pointsScaleable>false</pointsScaleable>
        <letterDef>PositiveEvent</letterDef>
    </IncidentDef>

    <IncidentDef>
        <defName>CustomTraderArrival</defName>
        <label>Exotic Trader</label>
        <category>OrbitalVisitor</category>
        <targetTags>
            <li>Map_PlayerHome</li>
        </targetTags>
        <workerClass>CustomIncidentMod.IncidentWorker_ExoticTrader</workerClass>
        <baseChance>0.8</baseChance>
        <minRefireDays>10</minRefireDays>
        <maxRefireDays>30</maxRefireDays>
        <letterDef>NegativeEvent</letterDef>
    </IncidentDef>

    <IncidentDef>
        <defName>CustomPlague</defName>
        <label>Strange Plague</label>
        <category>Disease</category>
        <targetTags>
            <li>Map_PlayerHome</li>
        </targetTags>
        <workerClass>CustomIncidentMod.IncidentWorker_StrangePlague</workerClass>
        <baseChance>0.5</baseChance>
        <minRefireDays>30</minRefireDays>
        <maxRefireDays>60</maxRefireDays>
        <minDifficulty>2.0</minDifficulty>
        <diseasePartEfficiencyInitial>0.8</diseasePartEfficiencyInitial>
        <letterDef>NegativeEvent</letterDef>
    </IncidentDef>
</Defs>",
                FileType.Xml
            ));

            example.Files.Add(new ExampleFile(
                "IncidentWorker.cs",
                "Source/IncidentWorker.cs",
                @"using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;

namespace CustomIncidentMod
{
    public class IncidentWorker_ResourcePodCrash : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }

            Map map = (Map)parms.target;
            if (map == null)
            {
                return false;
            }

            return TryFindCell(out IntVec3 _, map);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            if (!TryFindCell(out IntVec3 cell, map))
            {
                return false;
            }

            List<Thing> resources = GenerateResources();
            
            DropPodUtility.DropThingsNear(cell, map, resources, 110, false, true, false);

            string text = ""A resource pod has crashed nearby!\n\nIt contains:\n"" + GetResourceDescription(resources);
            
            Find.LetterStack.ReceiveLetter(
                ""Resource Pod Crash"",
                text,
                LetterDefOf.PositiveEvent,
                new TargetInfo(cell, map)
            );

            return true;
        }

        private bool TryFindCell(out IntVec3 cell, Map map)
        {
            return CellFinder.TryFindRandomEdgeCellWith(
                c => c.Standable(map) && !c.Fogged(map),
                map,
                CellFinder.EdgeRoadChance_Neutral,
                out cell
            );
        }

        private List<Thing> GenerateResources()
        {
            List<Thing> resources = new List<Thing>();
            
            float totalValue = Rand.Range(500f, 2000f);
            List<ThingDef> possibleResources = new List<ThingDef>
            {
                ThingDefOf.Steel,
                ThingDefOf.Plasteel,
                ThingDefOf.Gold,
                ThingDefOf.Silver,
                ThingDef.Named(""ComponentIndustrial""),
                ThingDef.Named(""Chemfuel"")
            };

            float remainingValue = totalValue;
            while (remainingValue > 50f)
            {
                ThingDef resourceDef = possibleResources.RandomElement();
                float valuePerItem = resourceDef.BaseMarketValue;
                int count = Mathf.Min(
                    Mathf.FloorToInt(remainingValue / valuePerItem),
                    resourceDef.stackLimit
                );

                if (count > 0)
                {
                    Thing thing = ThingMaker.MakeThing(resourceDef);
                    thing.stackCount = count;
                    resources.Add(thing);
                    remainingValue -= count * valuePerItem;
                }
            }

            return resources;
        }

        private string GetResourceDescription(List<Thing> resources)
        {
            return string.Join(""\n"", resources.Select(t => $""  - {t.stackCount}x {t.LabelNoCount}"").ToArray());
        }
    }

    public class IncidentWorker_ExoticTrader : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            return map != null && map.passingShipManager.passingShips.Count < 5;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            
            TradeShip tradeShip = new TradeShip(DefDatabase<TraderKindDef>.GetNamed(""ExoticTraderKind""));
            tradeShip.generateWithOrbitalTrade = true;
            
            map.passingShipManager.AddShip(tradeShip);

            Find.LetterStack.ReceiveLetter(
                ""Exotic Trader"",
                $""An exotic trader ship has arrived in orbit.\n\nThey will stay for {tradeShip.ticksUntilDeparture.ToStringTicksToPeriod()}."",
                LetterDefOf.NeutralEvent,
                tradeShip
            );

            return true;
        }
    }

    public class IncidentWorker_StrangePlague : IncidentWorker_Disease
    {
        protected override IEnumerable<Pawn> PotentialVictimCandidates(IIncidentTarget target)
        {
            Map map = (Map)target;
            return map.mapPawns.FreeColonistsAndPrisoners.Where(p => !p.health.hediffSet.HasHediff(HediffDef.Named(""StrangePlagueHediff"")));
        }

        protected override IEnumerable<Pawn> ActualVictims(IncidentParms parms)
        {
            int num = Mathf.Max(1, Rand.Range(1, 3));
            return PotentialVictimCandidates(parms.target).InRandomOrder().Take(num);
        }

        protected override HediffDef HediffDef => HediffDef.Named(""StrangePlagueHediff"");

        protected override string GetLetterLabel()
        {
            return ""Strange Plague"";
        }

        protected override string GetLetterText(Pawn victim, List<Pawn> totalVictims)
        {
            return $""{victim.LabelShortCap} has contracted a strange plague!\n\nThis disease will progressively weaken the victim if not treated."";
        }

        protected override LetterDef GetLetterDef()
        {
            return LetterDefOf.NegativeEvent;
        }
    }
}",
                FileType.CSharp
            ));

            example.Steps.Add("创建 IncidentDef 定义事件基本属性");
            example.Steps.Add("继承 IncidentWorker 实现事件逻辑");
            example.Steps.Add("重写 CanFireNowSub 检查事件是否可以触发");
            example.Steps.Add("重写 TryExecuteWorker 执行事件逻辑");
            example.Steps.Add("使用 DropPodUtility、Find.LetterStack 等工具类");
            example.Steps.Add("测试：在游戏中等待或强制触发事件");

            return example;
        }

        private static Example GetRaidIncidentExample()
        {
            var example = new Example
            {
                Title = "自定义袭击事件示例",
                Description = "创建一个自定义的袭击事件，包括敌人生成、战术行为和特殊条件。",
                Feature = "袭击事件",
                Keywords = new List<string> { "袭击", "raid", "敌人", "faction", "派系" }
            };

            example.Files.Add(new ExampleFile(
                "CustomRaidDef.xml",
                "Defs/IncidentDef/CustomRaidDef.xml",
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Defs>
    <IncidentDef ParentName=""BaseRaidIncident"">
        <defName>CustomRaid_BanditAmbush</defName>
        <label>Bandit Ambush</label>
        <category>ThreatBig</category>
        <targetTags>
            <li>Map_PlayerHome</li>
        </targetTags>
        <workerClass>CustomIncidentMod.IncidentWorker_BanditAmbush</workerClass>
        <baseChance>2.0</baseChance>
        <minRefireDays>8</minRefireDays>
        <maxRefireDays>20</maxRefireDays>
        <minDifficulty>1.5</minDifficulty>
        <pointsScaleable>true</pointsScaleable>
        <letterDef>ThreatBig</letterDef>
        <tale>Raid</tale>
    </IncidentDef>

    <PawnKindDef ParentName=""BasePlayerPawnKind"">
        <defName>BanditRaider</defName>
        <label>Bandit Raider</label>
        <race>Human</race>
        <defaultFactionType>BanditFaction</defaultFactionType>
        <combatPower>60</combatPower>
        <apparelMoney>200~500</apparelMoney>
        <apparelAllowHeadgearChance>0.5</apparelAllowHeadgearChance>
        <weaponMoney>150~400</weaponMoney>
        <weaponTags>
            <li>Gun</li>
            <li>Melee</li>
        </weaponTags>
        <techHediffsChance>0.05</techHediffsChance>
        <techHediffsMaxMoney>200</techHediffsMaxMoney>
        <initialWillRange>1~3</initialWillRange>
        <initialResistanceRange>5~10</initialResistanceRange>
        <skills>
            <li>
                <skill>Shooting</skill>
                <range>4~10</range>
            </li>
            <li>
                <skill>Melee</skill>
                <range>4~10</range>
            </li>
        </skills>
    </PawnKindDef>

    <FactionDef ParentName=""FactionBase"">
        <defName>BanditFaction</defName>
        <label>Bandit Gang</label>
        <description>A group of bandits who prey on travelers and settlements.</description>
        <colorSpectrum>
            <li>(0.6, 0.4, 0.3)</li>
        </colorSpectrum>
        <startingGoodwill>
            <min>-80</min>
            <max>-40</max>
        </startingGoodwill>
        <naturalColonyGoodwill>
            <min>-80</min>
            <max>-20</max>
        </naturalColonyGoodwill>
        <maxPawnOptionPoints>1000</maxPawnOptionPoints>
        <pawnGroupMakers>
            <li>
                <kindDef>Combat</kindDef>
                <options>
                    <BanditRaider>100</BanditRaider>
                </options>
            </li>
        </pawnGroupMakers>
        <raidLootMaker>BanditRaidLoot</raidLootMaker>
    </FactionDef>
</Defs>",
                FileType.Xml
            ));

            example.Files.Add(new ExampleFile(
                "IncidentWorker_Raid.cs",
                "Source/IncidentWorker_Raid.cs",
                @"using Verse;
using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CustomIncidentMod
{
    public class IncidentWorker_BanditAmbush : IncidentWorker_Raid
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            if (!base.CanFireNowSub(parms))
            {
                return false;
            }

            Faction faction = Find.FactionManager.FirstFactionOfDef(DefDatabase<FactionDef>.GetNamed(""BanditFaction""));
            return faction != null && !faction.defeated;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            if (parms.faction == null)
            {
                parms.faction = Find.FactionManager.FirstFactionOfDef(DefDatabase<FactionDef>.GetNamed(""BanditFaction""));
            }

            if (parms.faction == null)
            {
                return false;
            }

            parms.raidStrategy = DefDatabase<RaidStrategyDef>.GetNamed(""ImmediateAttack"");
            parms.raidArrivalMode = DefDatabase<PawnsArrivalModeDef>.GetNamed(""EdgeWalkIn"");

            if (!parms.raidArrivalMode.Worker.TryResolveRaidSpawnCenter(parms))
            {
                return false;
            }

            parms.points = Mathf.Max(parms.points, 100f);

            PawnGroupMakerParms groupParms = new PawnGroupMakerParms
            {
                faction = parms.faction,
                points = parms.points,
                groupKind = PawnGroupKindDefOf.Combat,
                tile = parms.target.Tile,
                inhabitants = false,
                seed = Rand.Int
            };

            List<Pawn> pawns = PawnGroupMakerUtility.GeneratePawns(groupParms).ToList();
            
            if (pawns.Count == 0)
            {
                return false;
            }

            parms.raidArrivalMode.Worker.Arrive(pawns, parms);

            string letterLabel = GetLetterLabel(parms);
            string letterText = GetLetterText(parms, pawns);

            SendStandardLetter(letterLabel, letterText, LetterDefOf.ThreatBig, parms, pawns.FirstOrDefault());

            Find.TickManager.slower.SignalForceNormalSpeedShort();
            Find.StoryWatcher.statsRecord.numRaids++;

            return true;
        }

        private string GetLetterLabel(IncidentParms parms)
        {
            return ""Bandit Ambush"";
        }

        private string GetLetterText(IncidentParms parms, List<Pawn> pawns)
        {
            int count = pawns.Count;
            string factionName = parms.faction.Name;
            return $""A bandit gang from {factionName} has launched an ambush!\n\nThey have {count} raiders."";
        }
    }

    public class RaidStrategyWorker_BanditAmbush : RaidStrategyWorker
    {
        public override float SelectionChance(Map map, float points)
        {
            return 1.0f;
        }

        public override bool CanUseWith(IncidentParms parms, PawnGroupKindDef groupKind)
        {
            return base.CanUseWith(parms, groupKind) && parms.faction != null && parms.faction.def.defName == ""BanditFaction"";
        }

        public override void MakeLords(IncidentParms parms, List<Pawn> pawns)
        {
            Map map = (Map)parms.target;

            LordJob lordJob = new LordJob_AssaultColony(parms.faction, true, true, false, false, true);
            Lord lord = LordMaker.MakeNewLord(parms.faction, lordJob, map, pawns);

            foreach (Pawn pawn in pawns)
            {
                if (pawn.mindState != null)
                {
                    pawn.mindState.enemyTarget = null;
                }
            }
        }
    }
}",
                FileType.CSharp
            ));

            example.Files.Add(new ExampleFile(
                "RaidLootMaker.xml",
                "Defs/RaidLootMaker.xml",
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Defs>
    <RaidLootMakerDef>
        <defName>BanditRaidLoot</defName>
        <options>
            <li>
                <thingDef>Silver</thingDef>
                <countRange>50~200</countRange>
            </li>
            <li>
                <thingDef>Steel</thingDef>
                <countRange>20~80</countRange>
            </li>
            <li>
                <thingDef>ComponentIndustrial</thingDef>
                <countRange>1~5</countRange>
            </li>
            <li>
                <thingDef>MedicineIndustrial</thingDef>
                <countRange>2~10</countRange>
            </li>
        </options>
    </RaidLootMakerDef>
</Defs>",
                FileType.Xml
            ));

            example.Steps.Add("创建 FactionDef 定义敌对派系");
            example.Steps.Add("创建 PawnKindDef 定义敌人类型");
            example.Steps.Add("继承 IncidentWorker_Raid 实现袭击逻辑");
            example.Steps.Add("使用 PawnGroupMakerUtility 生成敌人");
            example.Steps.Add("设置袭击策略和到达方式");
            example.Steps.Add("创建 RaidLootMakerDef 定义战利品");
            example.Steps.Add("测试：在游戏中触发袭击事件");

            return example;
        }

        private static Example GetEventIncidentExample()
        {
            var example = new Example
            {
                Title = "复杂事件链示例",
                Description = "创建一个多阶段的事件链，包括事件触发、条件判断和连续事件。",
                Feature = "事件链",
                Keywords = new List<string> { "事件链", "event chain", "多阶段", "quest", "任务" }
            };

            example.Files.Add(new ExampleFile(
                "EventChainDef.xml",
                "Defs/IncidentDef/EventChainDef.xml",
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Defs>
    <IncidentDef>
        <defName>MysteriousSignal</defName>
        <label>Mysterious Signal</label>
        <category>Misc</category>
        <targetTags>
            <li>Map_PlayerHome</li>
        </targetTags>
        <workerClass>CustomIncidentMod.IncidentWorker_MysteriousSignal</workerClass>
        <baseChance>0.5</baseChance>
        <minRefireDays>60</minRefireDays>
        <maxRefireDays>120</maxRefireDays>
        <letterDef>NegativeEvent</letterDef>
    </IncidentDef>

    <IncidentDef>
        <defName>SignalInvestigation</defName>
        <label>Signal Investigation</label>
        <category>Misc</category>
        <targetTags>
            <li>Map_PlayerHome</li>
        </targetTags>
        <workerClass>CustomIncidentMod.IncidentWorker_SignalInvestigation</workerClass>
        <baseChance>0</baseChance>
        <letterDef>PositiveEvent</letterDef>
    </IncidentDef>

    <WorldObjectDef>
        <defName>SignalSource</defName>
        <label>Signal Source</label>
        <worldObjectClass>CustomIncidentMod.WorldObject_SignalSource</worldObjectClass>
        <canSpawnPlayerBase>true</canSpawnPlayerBase>
        <canHaveFaction>false</canHaveFaction>
        <expandingIcon>true</expandingIcon>
        <expandingIconPriority>100</expandingIconPriority>
        <comps>
            <li Class=""WorldObjectCompProperties_TimeLimit"">
                <timeoutDays>15</timeoutDays>
            </li>
        </comps>
    </WorldObjectDef>

    <QuestScriptDef>
        <defName>SignalQuest</defName>
        <rootSelectionWeight>0</rootSelectionWeight>
        <canGiveRoyalFavor>false</canGiveRoyalFavor>
    </QuestScriptDef>
</Defs>",
                FileType.Xml
            ));

            example.Files.Add(new ExampleFile(
                "EventChainWorker.cs",
                "Source/EventChainWorker.cs",
                @"using Verse;
using RimWorld;
using System.Collections.Generic;
using UnityEngine;

namespace CustomIncidentMod
{
    public class IncidentWorker_MysteriousSignal : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            return map != null && Find.WorldObjects.AllWorldObjects.Any(o => o.def.defName == ""SignalSource"") == false;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;

            int tile;
            if (!TileFinder.TryFindNewSiteTile(out tile, 10, 50, true, false, -1, (int t) => t != map.Tile))
            {
                return false;
            }

            WorldObject_SignalSource signalSource = (WorldObject_SignalSource)WorldObjectMaker.MakeWorldObject(DefDatabase<WorldObjectDef>.GetNamed(""SignalSource""));
            signalSource.Tile = tile;
            signalSource.SetFaction(Faction.OfPlayer);
            Find.WorldObjects.Add(signalSource);

            ChoiceLetter choiceLetter = LetterMaker.MakeLetter(
                ""Mysterious Signal"",
                ""A mysterious signal has been detected from a nearby location. Investigating it could reveal valuable resources... or dangers.\n\nThe signal source will remain detectable for 15 days."",
                LetterDefOf.NegativeEvent,
                signalSource
            );

            choiceLetter.title = ""Mysterious Signal Detected"";
            choiceLetter.startTicks = Find.TickManager.TicksGame;
            choiceLetter.disappearAtTick = Find.TickManager.TicksGame + 60000;

            Find.LetterStack.ReceiveLetter(choiceLetter, null);

            return true;
        }
    }

    public class WorldObject_SignalSource : WorldObject
    {
        private int ticksUntilDisappear;
        private bool investigated = false;

        public override void SpawnSetup()
        {
            base.SpawnSetup();
            ticksUntilDisappear = 15 * 60000;
        }

        public override void Tick()
        {
            base.Tick();

            ticksUntilDisappear--;

            if (ticksUntilDisappear <= 0)
            {
                Find.WorldObjects.Remove(this);
                Messages.Message(""The mysterious signal has faded away."", MessageTypeDefOf.NegativeEvent);
            }
        }

        public override IEnumerable<Gizmo> GetCaravanGizmos(Caravan caravan)
        {
            foreach (Gizmo g in base.GetCaravanGizmos(caravan))
            {
                yield return g;
            }

            if (!investigated)
            {
                yield return new Command_Action
                {
                    defaultLabel = ""Investigate Signal"",
                    defaultDesc = ""Search the area for the source of the mysterious signal."",
                    icon = TexCommand.Attack,
                    action = () =>
                    {
                        Investigate(caravan);
                    }
                };
            }
        }

        private void Investigate(Caravan caravan)
        {
            investigated = true;

            float randomValue = Rand.Value;
            
            if (randomValue < 0.3f)
            {
                TriggerAmbush(caravan);
            }
            else if (randomValue < 0.6f)
            {
                GiveRewards(caravan);
            }
            else
            {
                FindTreasure(caravan);
            }

            Find.WorldObjects.Remove(this);
        }

        private void TriggerAmbush(Caravan caravan)
        {
            IncidentParms parms = new IncidentParms
            {
                target = caravan,
                points = StorytellerUtility.DefaultThreatPointsNow(Find.World)
            };

            Find.Storyteller.incidentQueue.Add(IncidentDefOf.CaravanAmbush, Find.TickManager.TicksGame + 60, parms);

            Messages.Message(""The signal was a trap! Enemies are approaching!"", MessageTypeDefOf.ThreatBig);
        }

        private void GiveRewards(Caravan caravan)
        {
            List<Thing> rewards = new List<Thing>();
            
            Thing gold = ThingMaker.MakeThing(ThingDefOf.Gold);
            gold.stackCount = Rand.Range(50, 200);
            rewards.Add(gold);

            Thing component = ThingMaker.MakeThing(ThingDef.Named(""ComponentIndustrial""));
            component.stackCount = Rand.Range(5, 15);
            rewards.Add(component);

            foreach (Thing thing in rewards)
            {
                CaravanInventoryUtility.GiveThing(caravan, thing);
            }

            Messages.Message(""You found valuable resources at the signal source!"", MessageTypeDefOf.PositiveEvent);
        }

        private void FindTreasure(Caravan caravan)
        {
            Thing treasure = ThingMaker.MakeThing(ThingDef.Named(""AIPersonaCore""));
            CaravanInventoryUtility.GiveThing(caravan, treasure);

            Messages.Message(""You discovered an AI Persona Core at the signal source!"", MessageTypeDefOf.PositiveEvent);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticksUntilDisappear, ""ticksUntilDisappear"");
            Scribe_Values.Look(ref investigated, ""investigated"");
        }

        public override string GetInspectString()
        {
            string baseString = base.GetInspectString();
            string timeLeft = ""Time remaining: "" + ticksUntilDisappear.ToStringTicksToPeriod();
            return string.IsNullOrEmpty(baseString) ? timeLeft : baseString + ""\n"" + timeLeft;
        }
    }

    public class IncidentWorker_SignalInvestigation : IncidentWorker
    {
        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            return false;
        }
    }
}",
                FileType.CSharp
            ));

            example.Files.Add(new ExampleFile(
                "EventConditionManager.cs",
                "Source/EventConditionManager.cs",
                @"using Verse;
using RimWorld;
using System.Collections.Generic;

namespace CustomIncidentMod
{
    public class GameComponent_EventChainManager : GameComponent
    {
        private Dictionary<string, int> eventCooldowns = new Dictionary<string, int>();
        private Dictionary<string, int> eventProgress = new Dictionary<string, int>();
        private List<QueuedEvent> queuedEvents = new List<QueuedEvent>();

        public GameComponent_EventChainManager(Game game)
        {
        }

        public override void GameComponentTick()
        {
            base.GameComponentTick();

            if (Find.TickManager.TicksGame % 60 == 0)
            {
                ProcessCooldowns();
                ProcessQueuedEvents();
            }
        }

        private void ProcessCooldowns()
        {
            List<string> toRemove = new List<string>();
            
            foreach (var kvp in eventCooldowns)
            {
                if (kvp.Value <= Find.TickManager.TicksGame)
                {
                    toRemove.Add(kvp.Key);
                }
            }

            foreach (string key in toRemove)
            {
                eventCooldowns.Remove(key);
            }
        }

        private void ProcessQueuedEvents()
        {
            List<QueuedEvent> toExecute = new List<QueuedEvent>();
            
            foreach (var queued in queuedEvents)
            {
                if (queued.executeTick <= Find.TickManager.TicksGame)
                {
                    toExecute.Add(queued);
                }
            }

            foreach (var queued in toExecute)
            {
                queued.Execute();
                queuedEvents.Remove(queued);
            }
        }

        public bool IsOnCooldown(string eventDefName)
        {
            return eventCooldowns.ContainsKey(eventDefName);
        }

        public void SetCooldown(string eventDefName, int ticks)
        {
            eventCooldowns[eventDefName] = Find.TickManager.TicksGame + ticks;
        }

        public int GetProgress(string eventDefName)
        {
            return eventProgress.TryGetValue(eventDefName, 0);
        }

        public void SetProgress(string eventDefName, int progress)
        {
            eventProgress[eventDefName] = progress;
        }

        public void QueueEvent(string eventDefName, int delayTicks, IncidentParms parms = null)
        {
            QueuedEvent queued = new QueuedEvent
            {
                incidentDef = DefDatabase<IncidentDef>.GetNamed(eventDefName),
                executeTick = Find.TickManager.TicksGame + delayTicks,
                parms = parms ?? new IncidentParms()
            };
            queuedEvents.Add(queued);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Collections.Look(ref eventCooldowns, ""eventCooldowns"", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref eventProgress, ""eventProgress"", LookMode.Value, LookMode.Value);
            Scribe_Collections.Look(ref queuedEvents, ""queuedEvents"", LookMode.Deep);
        }
    }

    public class QueuedEvent : IExposable
    {
        public IncidentDef incidentDef;
        public int executeTick;
        public IncidentParms parms;

        public void Execute()
        {
            if (incidentDef != null)
            {
                incidentDef.Worker.TryExecute(parms);
            }
        }

        public void ExposeData()
        {
            Scribe_Defs.Look(ref incidentDef, ""incidentDef"");
            Scribe_Values.Look(ref executeTick, ""executeTick"");
            Scribe_Deep.Look(ref parms, ""parms"");
        }
    }
}",
                FileType.CSharp
            ));

            example.Steps.Add("创建 WorldObjectDef 定义事件地点");
            example.Steps.Add("实现 WorldObject 子类处理交互逻辑");
            example.Steps.Add("创建 GameComponent 管理事件状态和冷却");
            example.Steps.Add("实现事件队列系统处理延迟事件");
            example.Steps.Add("添加多种事件结果(奖励、战斗、发现)");
            example.Steps.Add("使用 ExposeData 保存事件状态");
            example.Steps.Add("测试：触发事件链并验证各阶段逻辑");

            return example;
        }
    }
}
