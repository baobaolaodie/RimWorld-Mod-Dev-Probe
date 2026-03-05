using System.Collections.Generic;

namespace RimWorldModDevProbe.Examples
{
    public static class BuildingExamples
    {
        public static List<Example> GetExamples()
        {
            var examples = new List<Example>();

            examples.Add(GetWorkTableExample());
            examples.Add(GetDefenseBuildingExample());

            return examples;
        }

        private static Example GetWorkTableExample()
        {
            return new Example
            {
                Title = "工作台示例",
                Description = "创建一个自定义工作台，可用于制作物品",
                Feature = "建筑",
                Keywords = new List<string> { "工作台", "work table", "建筑", "制作台" },
                Files = new List<ExampleFile>
                {
                    new ExampleFile(
                        "Buildings_Work.xml",
                        "Defs/ThingDef_MyBuilding/Buildings_Work.xml",
                        @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Defs>
  <ThingDef ParentName=""BenchBase"">
    <defName>MyWorkBench</defName>
    <label>my work bench</label>
    <description>A custom work bench for crafting.</description>
    <thingClass>Building_WorkTable</thingClass>
    <graphicData>
      <texPath>Things/Building/Production/MyWorkBench</texPath>
      <graphicClass>Graphic_Multi</graphicClass>
      <drawSize>(3,1)</drawSize>
    </graphicData>
    <statBases>
      <WorkToBuild>2000</WorkToBuild>
      <MaxHitPoints>100</MaxHitPoints>
      <Flammability>1.0</Flammability>
    </statBases>
    <size>(3,1)</size>
    <costList>
      <Steel>150</Steel>
      <ComponentIndustrial>2</ComponentIndustrial>
    </costList>
    <recipes>
      <li>MakeSimpleMeal</li>
    </recipes>
    <inspectorTabs>
      <li>ITab_Bills</li>
    </inspectorTabs>
    <building>
      <spawnedConceptLearnOpportunity>BillsTab</spawnedConceptLearnOpportunity>
    </building>
    <comps>
      <li Class=""CompProperties_Power"">
        <compClass>CompPowerTrader</compClass>
        <basePowerConsumption>200</basePowerConsumption>
      </li>
      <li Class=""CompProperties_Flickable""/>
      <li Class=""CompProperties_AffectedByFacilities"">
        <linkableFacilities>
          <li>ToolCabinet</li>
        </linkableFacilities>
      </li>
    </comps>
    <researchPrerequisites>
      <li>Machining</li>
    </researchPrerequisites>
    <placeWorkers>
      <li>PlaceWorker_ShowFacilitiesConnections</li>
    </placeWorkers>
  </ThingDef>
</Defs>",
                        FileType.Xml
                    )
                },
                Steps = new List<string>
                {
                    "创建 ThingDef 定义工作台",
                    "设置图形数据和尺寸",
                    "定义建造成本和属性",
                    "添加配方和交互标签",
                    "配置电力和设施组件",
                    "设置研究前置条件"
                }
            };
        }

        private static Example GetDefenseBuildingExample()
        {
            return new Example
            {
                Title = "防御建筑示例",
                Description = "创建一个自动防御炮塔",
                Feature = "建筑",
                Keywords = new List<string> { "防御建筑", "defense building", "炮塔", "墙", "陷阱" },
                Files = new List<ExampleFile>
                {
                    new ExampleFile(
                        "Buildings_Defense.xml",
                        "Defs/ThingDef_MyBuilding/Buildings_Defense.xml",
                        @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Defs>
  <ThingDef ParentName=""BuildingBase"">
    <defName>MyTurret</defName>
    <label>my turret</label>
    <description>An automated defense turret.</description>
    <thingClass>Building_TurretGun</thingClass>
    <graphicData>
      <texPath>Things/Building/Security/MyTurret</texPath>
      <graphicClass>Graphic_Single</graphicClass>
      <drawSize>(2,2)</drawSize>
    </graphicData>
    <statBases>
      <WorkToBuild>3000</WorkToBuild>
      <MaxHitPoints>200</MaxHitPoints>
      <Flammability>0.5</Flammability>
      <ShootingAccuracyTurret>0.8</ShootingAccuracyTurret>
    </statBases>
    <size>(2,2)</size>
    <costList>
      <Steel>200</Steel>
      <ComponentIndustrial>4</ComponentIndustrial>
    </costList>
    <comps>
      <li Class=""CompProperties_Power"">
        <compClass>CompPowerTrader</compClass>
        <basePowerConsumption>500</basePowerConsumption>
      </li>
      <li Class=""CompProperties_Flickable""/>
      <li Class=""CompProperties_Forbiddable""/>
    </comps>
    <building>
      <turretGunDef>Gun_MyTurret</turretGunDef>
      <turretTopDrawSize>2</turretTopDrawSize>
    </building>
    <researchPrerequisites>
      <li>GunTurrets</li>
    </researchPrerequisites>
  </ThingDef>
  
  <ThingDef ParentName=""BaseGun"">
    <defName>Gun_MyTurret</defName>
    <label>turret gun</label>
    <graphicData>
      <texPath>Things/Item/Equipment/WeaponRanged/Gun_MyTurret</texPath>
      <graphicClass>Graphic_Single</graphicClass>
    </graphicData>
    <statBases>
      <AccuracyTouch>0.9</AccuracyTouch>
      <AccuracyShort>0.8</AccuracyShort>
      <AccuracyMedium>0.6</AccuracyMedium>
      <AccuracyLong>0.4</AccuracyLong>
      <RangedWeapon_Cooldown>1.0</RangedWeapon_Cooldown>
    </statBases>
    <verbs>
      <li>
        <verbClass>Verb_Shoot</verbClass>
        <hasStandardCommand>true</hasStandardCommand>
        <defaultProjectile>Bullet_Fast</defaultProjectile>
        <warmupTime>1.0</warmupTime>
        <range>30</range>
        <ticksBetweenBurstShots>5</ticksBetweenBurstShots>
        <burstShotCount>3</burstShotCount>
        <soundCast>GunShotA</soundCast>
        <soundCastTail>GunTail_Light</soundCastTail>
        <muzzleFlashScale>9</muzzleFlashScale>
      </li>
    </verbs>
  </ThingDef>
</Defs>",
                        FileType.Xml
                    )
                },
                Steps = new List<string>
                {
                    "创建炮塔 ThingDef",
                    "设置 Building_TurretGun 类",
                    "定义炮塔属性和成本",
                    "创建炮塔武器定义",
                    "配置射击动词和弹道",
                    "设置研究前置条件"
                }
            };
        }
    }
}
