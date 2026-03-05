using System.Collections.Generic;

namespace RimWorldModDevProbe.Examples
{
    public static class RaceExamples
    {
        public static List<Example> GetExamples()
        {
            var examples = new List<Example>();

            examples.Add(GetCustomRaceExample());
            examples.Add(GetRaceWithSpecialAbilityExample());
            examples.Add(GetAnimalRaceExample());

            return examples;
        }

        private static Example GetCustomRaceExample()
        {
            var example = new Example
            {
                Title = "自定义种族示例",
                Description = "创建一个全新的可玩种族，包括种族定义、身体结构、材质和派系。",
                Feature = "种族定义",
                Keywords = new List<string> { "种族", "race", "种族mod", "自定义种族", "elf" }
            };

            example.Files.Add(new ExampleFile(
                "CustomRaceDef.xml",
                "Defs/ThingDef_Races/CustomRaceDef.xml",
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Defs>
    <ThingDef ParentName=""BasePawn"">
        <defName>Alien_CustomRace</defName>
        <label>Custom Alien</label>
        <description>A custom alien race with unique abilities.</description>
        <thingClass>Pawn</thingClass>
        <category>Pawn</category>
        <selectable>true</selectable>
        <tickerType>Normal</tickerType>
        <altitudeLayer>Pawn</altitudeLayer>
        <useHitPoints>false</useHitPoints>
        <hasTooltip>true</hasTooltip>
        <soundImpactDefault>BulletImpact_Flesh</soundImpactDefault>
        <inspectorTabs>
            <li>ITab_Pawn_Health</li>
            <li>ITab_Pawn_Needs</li>
            <li>ITab_Pawn_Character</li>
            <li>ITab_Pawn_Training</li>
            <li>ITab_Pawn_Gear</li>
            <li>ITab_Pawn_Social</li>
        </inspectorTabs>
        <comps>
            <li Class=""CompProperties_AttachableFuel"">
                <fuelFilter>
                    <thingDefs>
                        <li>Chemfuel</li>
                    </thingDefs>
                </fuelFilter>
            </li>
        </comps>
        <drawGUIOverlay>true</drawGUIOverlay>
        
        <statBases>
            <MarketValue>1500</MarketValue>
            <MoveSpeed>5.0</MoveSpeed>
            <Flammability>1.0</Flammability>
            <ComfyTemperatureMin>-20</ComfyTemperatureMin>
            <ComfyTemperatureMax>45</ComfyTemperatureMax>
            <LeatherAmount>40</LeatherAmount>
            <MeatAmount>120</MeatAmount>
        </statBases>
        
        <tools>
            <li>
                <label>left fist</label>
                <capacities>
                    <li>Blunt</li>
                </capacities>
                <power>6</power>
                <cooldownTime>1.6</cooldownTime>
                <linkedBodyPartsGroup>LeftHand</linkedBodyPartsGroup>
                <surpriseAttack>
                    <extraMeleeDamages>
                        <li>
                            <def>Stun</def>
                            <amount>14</amount>
                        </li>
                    </extraMeleeDamages>
                </surpriseAttack>
            </li>
            <li>
                <label>right fist</label>
                <capacities>
                    <li>Blunt</li>
                </capacities>
                <power>6</power>
                <cooldownTime>1.6</cooldownTime>
                <linkedBodyPartsGroup>RightHand</linkedBodyPartsGroup>
                <surpriseAttack>
                    <extraMeleeDamages>
                        <li>
                            <def>Stun</def>
                            <amount>14</amount>
                        </li>
                    </extraMeleeDamages>
                </surpriseAttack>
            </li>
            <li>
                <label>head</label>
                <capacities>
                    <li>Blunt</li>
                </capacities>
                <power>5</power>
                <cooldownTime>1.8</cooldownTime>
                <linkedBodyPartsGroup>HeadAttackTool</linkedBodyPartsGroup>
                <ensureLinkedBodyPartsGroupAlwaysUsable>true</ensureLinkedBodyPartsGroupAlwaysUsable>
                <chanceFactor>0.2</chanceFactor>
            </li>
        </tools>
        
        <race>
            <thinkTreeMain>Humanlike</thinkTreeMain>
            <thinkTreeConstant>HumanlikeConstant</thinkTreeConstant>
            <intelligence>Humanlike</intelligence>
            <makesFilth>true</makesFilth>
            <meatDef>Meat_Human</meatDef>
            <leatherDef>Leather_Human</leatherDef>
            <nameCategory>HumanStandard</nameCategory>
            <body>Human</body>
            <baseBodySize>1.0</baseBodySize>
            <baseHealthScale>1.0</baseHealthScale>
            <baseHungerRate>1.0</baseHungerRate>
            <foodType>OmnivoreHuman</foodType>
            <gestationPeriodDays>45</gestationPeriodDays>
            <litterSizeCurve>
                <points>
                    <li>(0.5, 0)</li>
                    <li>(1, 1)</li>
                    <li>(1.5, 1)</li>
                    <li>(2, 0)</li>
                </points>
            </litterSizeCurve>
            <lifeExpectancy>80</lifeExpectancy>
            <lifeStageAges>
                <li>
                    <def>HumanlikeBaby</def>
                    <minAge>0</minAge>
                </li>
                <li>
                    <def>HumanlikeToddler</def>
                    <minAge>1.5</minAge>
                </li>
                <li>
                    <def>HumanlikeChild</def>
                    <minAge>4</minAge>
                </li>
                <li>
                    <def>HumanlikeTeenager</def>
                    <minAge>13</minAge>
                </li>
                <li>
                    <def>HumanlikeAdult</def>
                    <minAge>18</minAge>
                    <soundWounded>Pawn_Human_Wounded</soundWounded>
                    <soundDeath>Pawn_Human_Death</soundDeath>
                    <soundCall>Pawn_Human_Call</soundCall>
                </li>
            </lifeStageAges>
            <soundCallIntervalRange>1000~2000</soundCallIntervalRange>
            <soundMeleeHitPawn>Pawn_Melee_Punch_HitPawn</soundMeleeHitPawn>
            <soundMeleeHitBuilding>Pawn_Melee_Punch_HitBuilding</soundMeleeHitBuilding>
            <soundMeleeMiss>Pawn_Melee_Punch_Miss</soundMeleeMiss>
            <specialShadowData>
                <volume>(0.3, 0.8, 0.3)</volume>
                <offset>(0,0,-0.3)</offset>
            </specialShadowData>
            <ageGenerationCurve>
                <points>
                    <li>(14, 0)</li>
                    <li>(16, 100)</li>
                    <li>(50, 100)</li>
                    <li>(60, 30)</li>
                    <li>(70, 18)</li>
                    <li>(80, 10)</li>
                    <li>(90, 3)</li>
                    <li>(100, 0)</li>
                </points>
            </ageGenerationCurve>
            <hediffGiverSets>
                <li>OrganicStandard</li>
            </hediffGiverSets>
            <humanlike>true</humanlike>
        </race>
        
        <recipes>
            <li>ExciseCarcinoma</li>
            <li>Administer_BloodBag</li>
            <li>RemoveBodyPart</li>
            <li>Euthanize</li>
        </recipes>
    </ThingDef>
</Defs>",
                FileType.Xml
            ));

            example.Files.Add(new ExampleFile(
                "CustomRaceFaction.xml",
                "Defs/FactionDef/CustomRaceFaction.xml",
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Defs>
    <FactionDef ParentName=""PlayerFactionBase"">
        <defName>CustomRacePlayerColony</defName>
        <label>Custom Alien Colony</label>
        <description>A colony of custom aliens.</description>
        <isPlayer>true</isPlayer>
        <basicMemberKind>CustomRaceKind</basicMemberKind>
        <pawnGroupMakers>
            <li>
                <kindDef>Normal</kindDef>
                <options>
                    <CustomRaceKind>100</CustomRaceKind>
                </options>
            </li>
            <li>
                <kindDef>Combat</kindDef>
                <options>
                    <CustomRaceKind>100</CustomRaceKind>
                </options>
            </li>
        </pawnGroupMakers>
        <settlementTexturePath>World/WorldObjects/DefaultSettlement</settlementTexturePath>
        <startingResearchTags>
            <li>ClassicStart</li>
        </startingResearchTags>
        <startingTechprintsResearchTags>
            <li>ClassicStart</li>
        </startingTechprintsResearchTags>
        <apparelStuffFilter>
            <thingDefs>
                <li>Cloth</li>
            </thingDefs>
            <categories>
                <li>Fabric</li>
                <li>Leathery</li>
            </categories>
        </apparelStuffFilter>
        <recipePrerequisiteTags>
            <li>ClassicStart</li>
        </recipePrerequisiteTags>
    </FactionDef>

    <PawnKindDef ParentName=""BasePlayerPawnKind"">
        <defName>CustomRaceKind</defName>
        <label>Custom Alien</label>
        <race>Alien_CustomRace</race>
        <defaultFactionType>CustomRacePlayerColony</defaultFactionType>
        <chemicalAddictionChance>0.08</chemicalAddictionChance>
        <backstoryCryptosleepCommonality>0.1</backstoryCryptosleepCommonality>
        <maxGenerationAge>80</maxGenerationAge>
        <combatPower>50</combatPower>
        <itemQuality>Normal</itemQuality>
        <isFighter>true</isFighter>
        <forceNormalGearQuality>true</forceNormalGearQuality>
        <techHediffsChance>0.03</techHediffsChance>
        <techHediffsMaxMoney>1000</techHediffsMaxMoney>
        <techHediffsTags>
            <li>Poor</li>
        </techHediffsTags>
        <techHediffsRequiredTags>
            <li>Advanced</li>
        </techHediffsRequiredTags>
        <initialWillRange>1~2</initialWillRange>
        <initialResistanceRange>7~11</initialResistanceRange>
        <skills>
            <li>
                <skill>Shooting</skill>
                <range>2~6</range>
            </li>
            <li>
                <skill>Melee</skill>
                <range>2~6</range>
            </li>
            <li>
                <skill>Construction</skill>
                <range>2~6</range>
            </li>
            <li>
                <skill>Mining</skill>
                <range>2~6</range>
            </li>
            <li>
                <skill>Cooking</skill>
                <range>2~6</range>
            </li>
            <li>
                <skill>Plants</skill>
                <range>2~6</range>
            </li>
            <li>
                <skill>Animals</skill>
                <range>2~6</range>
            </li>
            <li>
                <skill>Crafting</skill>
                <range>2~6</range>
            </li>
            <li>
                <skill>Artistic</skill>
                <range>2~6</range>
            </li>
            <li>
                <skill>Medicine</skill>
                <range>2~6</range>
            </li>
            <li>
                <skill>Social</skill>
                <range>2~6</range>
            </li>
            <li>
                <skill>Intellectual</skill>
                <range>2~6</range>
            </li>
        </skills>
    </PawnKindDef>
</Defs>",
                FileType.Xml
            ));

            example.Steps.Add("创建 ThingDef 定义种族基本属性");
            example.Steps.Add("设置 race 节点定义种族特性(身体、智能、生命周期等)");
            example.Steps.Add("创建 PawnKindDef 定义角色类型");
            example.Steps.Add("创建 FactionDef 定义派系");
            example.Steps.Add("准备种族材质(身体、头部、衣服等)");
            example.Steps.Add("测试：在游戏中选择新种族开始游戏");

            return example;
        }

        private static Example GetRaceWithSpecialAbilityExample()
        {
            var example = new Example
            {
                Title = "带特殊能力的种族示例",
                Description = "创建一个拥有特殊能力的种族，包括自定义Hediff、能力系统和视觉效果。",
                Feature = "种族能力",
                Keywords = new List<string> { "种族能力", "race ability", "特殊能力", "hediff" }
            };

            example.Files.Add(new ExampleFile(
                "RaceAbilityDef.xml",
                "Defs/HediffDef/RaceAbilityDef.xml",
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Defs>
    <HediffDef ParentName=""AddedBodyPartBase"">
        <defName>CustomRaceAbility</defName>
        <label>Special Ability</label>
        <description>A special ability unique to this race.</description>
        <hediffClass>HediffWithComps</hediffClass>
        <defaultLabelColor>(0.8, 0.4, 0.8)</defaultLabelColor>
        <scenarioCanAdd>true</scenarioCanAdd>
        <maxSeverity>1.0</maxSeverity>
        <isBad>false</isBad>
        <stages>
            <li>
                <statOffsets>
                    <MoveSpeed>0.5</MoveSpeed>
                    <WorkSpeedGlobal>0.2</WorkSpeedGlobal>
                </statOffsets>
                <capMods>
                    <li>
                        <capacity>Sight</capacity>
                        <offset>0.2</offset>
                    </li>
                    <li>
                        <capacity>Manipulation</capacity>
                        <offset>0.2</offset>
                    </li>
                </capMods>
            </li>
        </stages>
        <comps>
            <li Class=""HediffCompProperties_Disappears"">
                <disappearsAfterTicks>60000~120000</disappearsAfterTicks>
                <showRemainingTime>true</showRemainingTime>
            </li>
            <li Class=""HediffCompProperties_Glower"">
                <glowRadius>5</glowRadius>
                <glowColor>(200, 100, 200, 0)</glowColor>
            </li>
        </comps>
    </HediffDef>

    <AbilityDef>
        <defName>CustomRaceSpecialAbility</defName>
        <label>Racial Power</label>
        <description>Activate a powerful racial ability.</description>
        <iconPath>UI/Abilities/RacialPower</iconPath>
        <cooldownTicksRange>60000</cooldownTicksRange>
        <verbProperties>
            <verbClass>Verb_CastAbility</verbClass>
            <range>10</range>
            <warmupTime>1</warmupTime>
            <targetParams>
                <canTargetPawns>true</canTargetPawns>
                <canTargetBuildings>false</canTargetBuildings>
            </targetParams>
        </verbProperties>
        <comps>
            <li Class=""CompProperties_AbilityEffect"">
                <compClass>CompAbilityEffect_WithDest</compClass>
                <effecterDef>CustomAbilityEffect</effecterDef>
            </li>
        </comps>
    </AbilityDef>
</Defs>",
                FileType.Xml
            ));

            example.Files.Add(new ExampleFile(
                "RaceAbilityComp.cs",
                "Source/RaceAbilityComp.cs",
                @"using Verse;
using RimWorld;
using UnityEngine;

namespace CustomRaceMod
{
    public class HediffComp_RaceAbility : HediffComp
    {
        private int ticksUntilNextActivation;
        private const int CooldownTicks = 60000;

        public override void CompPostMake()
        {
            base.CompPostMake();
            ticksUntilNextActivation = 0;
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);

            if (ticksUntilNextActivation > 0)
            {
                ticksUntilNextActivation--;
            }

            if (Pawn.IsHashIntervalTick(60))
            {
                ApplyPassiveEffects();
            }
        }

        private void ApplyPassiveEffects()
        {
            if (Pawn.Map == null || Pawn.Dead)
            {
                return;
            }

            float radius = 5f;
            foreach (Thing thing in GenRadial.RadialDistinctThingsAround(Pawn.Position, Pawn.Map, radius, true))
            {
                if (thing is Pawn otherPawn && otherPawn.Faction == Pawn.Faction)
                {
                    Hediff hediff = otherPawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named(""RacialBlessing""));
                    if (hediff == null)
                    {
                        otherPawn.health.AddHediff(HediffDef.Named(""RacialBlessing""));
                    }
                }
            }
        }

        public bool CanActivateAbility()
        {
            return ticksUntilNextActivation <= 0 && !Pawn.Dead && !Pawn.Downed;
        }

        public void ActivateAbility(LocalTargetInfo target)
        {
            if (!CanActivateAbility())
            {
                return;
            }

            DoAbilityEffect(target);
            ticksUntilNextActivation = CooldownTicks;

            MoteMaker.MakeStaticMote(Pawn.Position, Pawn.Map, ThingDefOf.Mote_PsycastAreaEffect, 5f);
        }

        private void DoAbilityEffect(LocalTargetInfo target)
        {
            if (target.HasThing && target.Thing is Pawn targetPawn)
            {
                Hediff abilityHediff = HediffMaker.MakeHediff(HediffDef.Named(""CustomRaceAbilityEffect""), targetPawn);
                targetPawn.health.AddHediff(abilityHediff);

                DamageInfo dinfo = new DamageInfo(DamageDefOf.Stun, 0, 0, -1, Pawn);
                targetPawn.stances.stunner.StunFor(120, dinfo);
            }
            else if (target.Cell.IsValid)
            {
                Effecter effecter = EffecterDefOf.PsycastAreaEffect.Spawn(target.Cell, Pawn.Map);
                effecter.Trigger(target.Cell, target.Cell);
                effecter.Cleanup();
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look(ref ticksUntilNextActivation, ""ticksUntilNextActivation"", 0);
        }

        public override string CompDescriptionExtra => $""Cooldown: {ticksUntilNextActivation.ToStringTicksToPeriod()}"";

        public override bool CompAllowStackWith(Hediff other)
        {
            return false;
        }
    }

    public class HediffCompProperties_RaceAbility : HediffCompProperties
    {
        public float passiveRadius = 5f;
        public int cooldownTicks = 60000;

        public HediffCompProperties_RaceAbility()
        {
            compClass = typeof(HediffComp_RaceAbility);
        }
    }
}",
                FileType.CSharp
            ));

            example.Files.Add(new ExampleFile(
                "RaceAbilityGiver.cs",
                "Source/RaceAbilityGiver.cs",
                @"using Verse;
using RimWorld;

namespace CustomRaceMod
{
    public class HediffGiver_RaceAbility : HediffGiver
    {
        public override bool OnHediffAdded(Pawn pawn, Hediff hediff)
        {
            if (pawn.def.defName == ""Alien_CustomRace"")
            {
                if (!pawn.health.hediffSet.HasHediff(hediff.def))
                {
                    Hediff abilityHediff = HediffMaker.MakeHediff(def, pawn);
                    pawn.health.AddHediff(abilityHediff);
                    return true;
                }
            }
            return false;
        }
    }

    public class GameCondition_RaceAbilityBoost : GameCondition
    {
        public override void GameConditionTick()
        {
            base.GameConditionTick();

            if (Find.TickManager.TicksGame % 60 == 0)
            {
                foreach (Map map in AffectedMaps)
                {
                    foreach (Pawn pawn in map.mapPawns.AllPawnsSpawned)
                    {
                        if (pawn.def.defName == ""Alien_CustomRace"" && !pawn.Dead)
                        {
                            Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(HediffDef.Named(""CustomRaceAbility""));
                            if (hediff == null)
                            {
                                pawn.health.AddHediff(HediffDef.Named(""CustomRaceAbility""));
                            }
                        }
                    }
                }
            }
        }
    }
}",
                FileType.CSharp
            ));

            example.Steps.Add("创建 HediffDef 定义种族能力效果");
            example.Steps.Add("创建 AbilityDef 定义可激活的能力");
            example.Steps.Add("实现 HediffComp 处理能力逻辑");
            example.Steps.Add("添加视觉效果(Glower, Mote, Effecter)");
            example.Steps.Add("实现冷却机制和激活条件");
            example.Steps.Add("测试：在游戏中验证能力是否正确触发");

            return example;
        }

        private static Example GetAnimalRaceExample()
        {
            var example = new Example
            {
                Title = "自定义动物种族示例",
                Description = "创建一个全新的动物种族，包括驯服、繁殖、产物和战斗行为。",
                Feature = "动物种族",
                Keywords = new List<string> { "动物", "animal", "宠物", "生物", "驯服" }
            };

            example.Files.Add(new ExampleFile(
                "CustomAnimalDef.xml",
                "Defs/ThingDef_Races/CustomAnimalDef.xml",
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Defs>
    <ThingDef ParentName=""BaseAnimal"">
        <defName>CustomMountAnimal</defName>
        <label>Swift Strider</label>
        <description>A fast and loyal mount animal that can carry heavy loads.</description>
        <statBases>
            <MoveSpeed>7.0</MoveSpeed>
            <MarketValue>800</MarketValue>
            <ComfyTemperatureMin>-30</ComfyTemperatureMin>
            <ComfyTemperatureMax>50</ComfyTemperatureMax>
            <FilthRate>6</FilthRate>
        </statBases>
        <tools>
            <li>
                <label>left claw</label>
                <capacities>
                    <li>Scratch</li>
                </capacities>
                <power>12</power>
                <cooldownTime>1.5</cooldownTime>
                <linkedBodyPartsGroup>FrontLeftClaws</linkedBodyPartsGroup>
            </li>
            <li>
                <label>right claw</label>
                <capacities>
                    <li>Scratch</li>
                </capacities>
                <power>12</power>
                <cooldownTime>1.5</cooldownTime>
                <linkedBodyPartsGroup>FrontRightClaws</linkedBodyPartsGroup>
            </li>
            <li>
                <capacities>
                    <li>Bite</li>
                </capacities>
                <power>18</power>
                <cooldownTime>2.0</cooldownTime>
                <linkedBodyPartsGroup>Teeth</linkedBodyPartsGroup>
                <chanceFactor>0.7</chanceFactor>
            </li>
            <li>
                <label>head</label>
                <capacities>
                    <li>Blunt</li>
                </capacities>
                <power>8</power>
                <cooldownTime>2.0</cooldownTime>
                <linkedBodyPartsGroup>HeadAttackTool</linkedBodyPartsGroup>
                <chanceFactor>0.2</chanceFactor>
            </li>
        </tools>
        <race>
            <body>QuadrupedAnimalWithHoovesAndHump</body>
            <herdAnimal>true</herdAnimal>
            <baseBodySize>2.0</baseBodySize>
            <baseHungerRate>0.8</baseHungerRate>
            <baseHealthScale>1.5</baseHealthScale>
            <foodType>VegetarianRoughAnimal</foodType>
            <gestationPeriodDays>25</gestationPeriodDays>
            <nameOnTameChance>1</nameOnTameChance>
            <trainability>Advanced</trainability>
            <wildness>0.50</wildness>
            <nuzzleMtbHours>120</nuzzleMtbHours>
            <mateMtbHours>8</mateMtbHours>
            <canBePredatorPrey>false</canBePredatorPrey>
            <lifeExpectancy>30</lifeExpectancy>
            <lifeStageAges>
                <li>
                    <def>AnimalBaby</def>
                    <minAge>0</minAge>
                </li>
                <li>
                    <def>AnimalJuvenile</def>
                    <minAge>0.3</minAge>
                </li>
                <li>
                    <def>AnimalAdult</def>
                    <minAge>1.0</minAge>
                    <soundCall>Pawn_Dog_Call</soundCall>
                    <soundAngry>Pawn_Dog_Angry</soundAngry>
                    <soundWounded>Pawn_Dog_Wounded</soundWounded>
                    <soundDeath>Pawn_Dog_Death</soundDeath>
                </li>
            </lifeStageAges>
            <soundMeleeHitPawn>Pawn_Melee_BigDog_HitPawn</soundMeleeHitPawn>
            <soundMeleeHitBuilding>Pawn_Melee_BigDog_HitBuilding</soundMeleeHitBuilding>
            <soundMeleeMiss>Pawn_Melee_BigDog_Miss</soundMeleeMiss>
            <useMeatFrom>Deer</useMeatFrom>
            <useLeatherFrom>Deer</useLeatherFrom>
            <genderProbability>0.5</genderProbability>
            <manhunterOnTameFailChance>0.05</manhunterOnTameFailChance>
            <manhunterOnDamageChance>0.15</manhunterOnDamageChance>
        </race>
        <tradeTags>
            <li>AnimalUncommon</li>
            <li>AnimalMount</li>
        </tradeTags>
        <modExtensions>
            <li Class=""CustomAnimalMod.RaceExtension"">
                <canBeRidden>true</canBeRidden>
                <carryCapacity>200</carryCapacity>
                <ridingSpeedFactor>1.5</ridingSpeedFactor>
            </li>
        </modExtensions>
    </ThingDef>
</Defs>",
                FileType.Xml
            ));

            example.Files.Add(new ExampleFile(
                "CustomAnimalProduct.xml",
                "Defs/ThingDef_Items/CustomAnimalProduct.xml",
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Defs>
    <ThingDef ParentName=""ResourceBase"">
        <defName>CustomAnimalWool</defName>
        <label>Strider Wool</label>
        <description>Soft and warm wool sheared from a Swift Strider.</description>
        <graphicData>
            <texPath>Things/Item/Resource/Wool</texPath>
            <graphicClass>Graphic_Single</graphicClass>
        </graphicData>
        <statBases>
            <MaxHitPoints>100</MaxHitPoints>
            <MarketValue>8</MarketValue>
            <Mass>0.028</Mass>
            <Flammability>1.0</Flammability>
            <Beauty>2</Beauty>
        </statBases>
        <thingCategories>
            <li>Textiles</li>
        </thingCategories>
        <stuffProps>
            <categories>
                <li>Fabric</li>
            </categories>
            <color>(230, 220, 200)</color>
            <commonality>0.5</commonality>
            <statFactors>
                <ArmorRating_Blunt>0.8</ArmorRating_Blunt>
                <ArmorRating_Heat>1.3</ArmorRating_Heat>
                <Insulation_Cold>1.5</Insulation_Cold>
                <Insulation_Heat>0.8</Insulation_Heat>
            </statFactors>
        </stuffProps>
    </ThingDef>

    <RecipeDef>
        <defName>ShearCustomAnimal</defName>
        <label>shear strider</label>
        <description>Shear wool from a Swift Strider.</description>
        <jobString>Shearing.</jobString>
        <workAmount>800</workAmount>
        <workSpeedStat>AnimalsSpeed</workSpeedStat>
        <effectWorking>Shear</effectWorking>
        <soundWorking>Recipe_Shear</soundWorking>
        <targetCountAdjustment>1</targetCountAdjustment>
        <workSkill>Animals</workSkill>
        <recipeUsers>
            <li>CustomMountAnimal</li>
        </recipeUsers>
        <products>
            <CustomAnimalWool>50</CustomAnimalWool>
        </products>
    </RecipeDef>
</Defs>",
                FileType.Xml
            ));

            example.Files.Add(new ExampleFile(
                "CustomAnimalComp.cs",
                "Source/CustomAnimalComp.cs",
                @"using Verse;
using RimWorld;
using UnityEngine;

namespace CustomAnimalMod
{
    public class RaceExtension : DefModExtension
    {
        public bool canBeRidden = false;
        public float carryCapacity = 100f;
        public float ridingSpeedFactor = 1.0f;
    }

    public class CompProperties_Mountable : CompProperties
    {
        public float speedFactor = 1.0f;
        public int maxRiders = 1;

        public CompProperties_Mountable()
        {
            compClass = typeof(CompMountable);
        }
    }

    public class CompMountable : ThingComp
    {
        private Pawn Rider => parent as Pawn;
        private Pawn CurrentRider;

        public bool IsRidden => CurrentRider != null;

        public override void CompTick()
        {
            base.CompTick();

            if (IsRidden && Rider != null)
            {
                if (CurrentRider.Dead || CurrentRider.Downed || CurrentRider.Map != Rider.Map)
                {
                    Dismount();
                }
                else
                {
                    CurrentRider.Position = Rider.Position;
                    CurrentRider.Rotation = Rider.Rotation;
                }
            }
        }

        public bool CanMount(Pawn pawn)
        {
            if (IsRidden)
            {
                return false;
            }

            if (Rider == null || Rider.Dead || Rider.Downed)
            {
                return false;
            }

            if (!Rider.training.IsCompleted(TrainableDefOf.Obedience))
            {
                return false;
            }

            return pawn.CanReach(Rider, Verse.AI.PathEndMode.Touch, Danger.Deadly);
        }

        public void Mount(Pawn pawn)
        {
            if (!CanMount(pawn))
            {
                return;
            }

            CurrentRider = pawn;
            pawn.pather.StopDead();

            MoteMaker.MakeStaticMote(Rider.Position, Rider.Map, ThingDefOf.Mote_CastPuff, 1f);
            Messages.Message($""{pawn.LabelShort} mounted {Rider.LabelShort}."", Rider, MessageTypeDefOf.PositiveEvent);
        }

        public void Dismount()
        {
            if (CurrentRider != null)
            {
                Messages.Message($""{CurrentRider.LabelShort} dismounted from {Rider.LabelShort}."", Rider, MessageTypeDefOf.SilentInput);
                CurrentRider = null;
            }
        }

        public override void PostDeSpawn(Map map)
        {
            base.PostDeSpawn(map);
            Dismount();
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_References.Look(ref CurrentRider, ""currentRider"");
        }
    }

    public class JobDriver_Mount : Verse.AI.JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.CanReserve(job.targetA, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Verse.AI.Toil> MakeNewToils()
        {
            yield return Toils_Goto.GotoThing(TargetIndex.A, Verse.AI.PathEndMode.Touch);
            
            yield return Toils_General.Wait(30);

            yield return new Verse.AI.Toil
            {
                initAction = () =>
                {
                    Pawn animal = job.targetA.Thing as Pawn;
                    if (animal != null)
                    {
                        CompMountable comp = animal.TryGetComp<CompMountable>();
                        comp?.Mount(pawn);
                    }
                }
            };
        }
    }

    public class WorkGiver_Mount : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.Pawn);

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            Pawn animal = t as Pawn;
            if (animal == null || animal == pawn)
            {
                return false;
            }

            CompMountable comp = animal.TryGetComp<CompMountable>();
            return comp != null && comp.CanMount(pawn);
        }

        public override Verse.AI.Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return Verse.AI.JobMaker.MakeJob(JobDefOf.Mount, t);
        }
    }
}",
                FileType.CSharp
            ));

            example.Steps.Add("创建 ThingDef 定义动物基本属性");
            example.Steps.Add("设置 race 节点定义动物特性(驯服度、繁殖、食物类型等)");
            example.Steps.Add("创建产物 ThingDef 和 RecipeDef");
            example.Steps.Add("实现 CompMountable 支持骑乘功能");
            example.Steps.Add("创建 WorkGiver 和 JobDriver 处理骑乘行为");
            example.Steps.Add("准备动物材质和图标");
            example.Steps.Add("测试：在游戏中驯服并使用该动物");

            return example;
        }
    }
}
