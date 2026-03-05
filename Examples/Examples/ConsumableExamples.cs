using System.Collections.Generic;

namespace RimWorldModDevProbe.Examples
{
    public static class ConsumableExamples
    {
        public static List<Example> GetExamples()
        {
            var examples = new List<Example>();

            examples.Add(GetDrugExample());
            examples.Add(GetFoodExample());

            return examples;
        }

        private static Example GetDrugExample()
        {
            var example = new Example
            {
                Title = "消耗品定义示例 - 药物",
                Description = "创建自定义消耗品，包括药物、食物、饮料等类型。包含效果、成瘾、营养等完整属性配置。",
                Feature = "物品定义",
                Keywords = new List<string> { "消耗品", "consumable", "药物", "食物" }
            };

            example.Files.Add(new ExampleFile(
                "ThingDef_Consumable.xml",
                "Defs/ThingDefs_Misc/ThingDef_Consumable.xml",
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Defs>
    <!-- ==================== 药物 - 强效止痛药 ==================== -->
    <ThingDef ParentName=""DrugBase"">
        <defName>Drug_PowerfulPainkiller</defName>
        <label>powerful painkiller</label>
        <description>A powerful painkiller that provides immediate pain relief. Can be addictive.</description>
        <graphicData>
            <texPath>Things/Item/Drug/PowerfulPainkiller</texPath>
            <graphicClass>Graphic_Single</graphicClass>
        </graphicData>
        <rotatable>false</rotatable>
        <statBases>
            <WorkToMake>600</WorkToMake>
            <MarketValue>50</MarketValue>
            <Mass>0.02</Mass>
            <Flammability>0.5</Flammability>
            <DeteriorationRate>6</DeteriorationRate>
        </statBases>
        <costList>
            <MedicineIndustrial>1</MedicineIndustrial>
            <Neutroamine>2</Neutroamine>
        </costList>
        <recipeMaker>
            <workSpeedStat>DrugSynthesisSpeed</workSpeedStat>
            <workSkill>Intellectual</workSkill>
            <recipeUsers>
                <li>DrugLab</li>
            </recipeUsers>
            <skillRequirements>
                <Intellectual>6</Intellectual>
            </skillRequirements>
            <researchPrerequisite>MedicineProduction</researchPrerequisite>
        </recipeMaker>
        <ingestible>
            <drugCategory>Medical</drugCategory>
            <foodType>Processed</foodType>
            <baseIngestTicks>60</baseIngestTicks>
            <nurseable>true</nurseable>
            <joy>0.10</joy>
            <joyKind>Chemical</joyKind>
            <outcomeDoers>
                <li Class=""IngestionOutcomeDoer_GiveHediff"">
                    <hediffDef>PainkillerHigh</hediffDef>
                    <severity>0.8</severity>
                </li>
                <li Class=""IngestionOutcomeDoer_OffsetNeed"">
                    <need>Pain</need>
                    <offset>-1.0</offset>
                </li>
            </outcomeDoers>
        </ingestible>
        <comps>
            <li Class=""CompProperties_Drug"">
                <chemical>Painkiller</chemical>
                <addictiveness>0.15</addictiveness>
                <minToleranceToAddict>0.3</minToleranceToAddict>
                <existingAddictionSeverityOffset>0.1</existingAddictionSeverityOffset>
                <needLevelOffset>1</needLevelOffset>
                <listOrder>1010</listOrder>
            </li>
        </comps>
    </ThingDef>

    <!-- ==================== 药物效果 - 止痛药增益 ==================== -->
    <HediffDef>
        <defName>PainkillerHigh</defName>
        <label>painkiller high</label>
        <description>A state of reduced pain sensitivity from painkiller use.</description>
        <hediffClass>Hediff_High</hediffClass>
        <defaultLabelColor>(1,0,0.5)</defaultLabelColor>
        <scenarioCanAdd>true</scenarioCanAdd>
        <maxSeverity>1.0</maxSeverity>
        <isBad>false</isBad>
        <comps>
            <li Class=""HediffCompProperties_SeverityPerDay"">
                <severityPerDay>-0.5</severityPerDay>
            </li>
        </comps>
        <stages>
            <li>
                <painFactor>0.5</painFactor>
                <capMods>
                    <li>
                        <capacity>Consciousness</capacity>
                        <offset>-0.1</offset>
                    </li>
                </capMods>
            </li>
        </stages>
    </HediffDef>

    <!-- ==================== 食物 - 能量棒 ==================== -->
    <ThingDef ParentName=""MealBase"">
        <defName>Meal_EnergyBar</defName>
        <label>energy bar</label>
        <description>A compact energy bar packed with nutrients. Quick to eat and filling.</description>
        <graphicData>
            <texPath>Things/Item/Meal/EnergyBar</texPath>
            <graphicClass>Graphic_Single</graphicClass>
        </graphicData>
        <statBases>
            <WorkToMake>300</WorkToMake>
            <MarketValue>15</MarketValue>
            <Mass>0.1</Mass>
            <DeteriorationRate>10</DeteriorationRate>
        </statBases>
        <costList>
            <NutrientPaste>1</NutrientPaste>
        </costList>
        <recipeMaker>
            <workSpeedStat>CookSpeed</workSpeedStat>
            <workSkill>Cooking</workSkill>
            <recipeUsers>
                <li>TableStove</li>
            </recipeUsers>
            <skillRequirements>
                <Cooking>3</Cooking>
            </skillRequirements>
        </recipeMaker>
        <ingestible>
            <foodType>Processed</foodType>
            <preferability>MealSimple</preferability>
            <nutrition>0.9</nutrition>
            <joy>0.05</joy>
            <joyKind>Gluttonous</joyKind>
            <optimalityOffsetHumanlikes>6</optimalityOffsetHumanlikes>
            <optimalityOffsetFeedingAnimals>-10</optimalityOffsetFeedingAnimals>
            <ingestedDirectThought>AteFineMeal</ingestedDirectThought>
        </ingestible>
        <comps>
            <li Class=""CompProperties_Ingredients""/>
            <li Class=""CompProperties_FoodPoisonable""/>
        </comps>
    </ThingDef>

    <!-- ==================== 饮料 - 咖啡 ==================== -->
    <ThingDef ParentName=""DrugBase"">
        <defName>Drink_Coffee</defName>
        <label>coffee</label>
        <description>A hot cup of coffee. Provides an energy boost and improves focus.</description>
        <graphicData>
            <texPath>Things/Item/Drug/Coffee</texPath>
            <graphicClass>Graphic_Single</graphicClass>
        </graphicData>
        <statBases>
            <WorkToMake>400</WorkToMake>
            <MarketValue>8</MarketValue>
            <Mass>0.2</Mass>
            <DeteriorationRate>2</DeteriorationRate>
        </statBases>
        <costList>
            <RawCoffeeBeans>5</RawCoffeeBeans>
        </costList>
        <recipeMaker>
            <workSpeedStat>CookSpeed</workSpeedStat>
            <workSkill>Cooking</workSkill>
            <recipeUsers>
                <li>TableStove</li>
            </recipeUsers>
        </recipeMaker>
        <ingestible>
            <drugCategory>None</drugCategory>
            <foodType>Fluid, Processed</foodType>
            <nutrition>0.1</nutrition>
            <joy>0.15</joy>
            <joyKind>Gluttonous</joyKind>
            <baseIngestTicks>120</baseIngestTicks>
            <nurseable>true</nurseable>
            <outcomeDoers>
                <li Class=""IngestionOutcomeDoer_GiveHediff"">
                    <hediffDef>CoffeeHigh</hediffDef>
                    <severity>0.5</severity>
                </li>
            </outcomeDoers>
        </ingestible>
    </ThingDef>

    <!-- ==================== 咖啡效果 ==================== -->
    <HediffDef>
        <defName>CoffeeHigh</defName>
        <label>caffeinated</label>
        <description>A boost of energy from caffeine.</description>
        <hediffClass>Hediff_High</hediffClass>
        <defaultLabelColor>(0.6,0.4,0.2)</defaultLabelColor>
        <scenarioCanAdd>true</scenarioCanAdd>
        <maxSeverity>1.0</maxSeverity>
        <isBad>false</isBad>
        <comps>
            <li Class=""HediffCompProperties_SeverityPerDay"">
                <severityPerDay>-0.25</severityPerDay>
            </li>
        </comps>
        <stages>
            <li>
                <capMods>
                    <li>
                        <capacity>Consciousness</capacity>
                        <offset>0.05</offset>
                    </li>
                    <li>
                        <capacity>Moving</capacity>
                        <offset>0.05</offset>
                    </li>
                </capMods>
                <statOffsets>
                    <WorkSpeedGlobal>0.1</WorkSpeedGlobal>
                </statOffsets>
            </li>
        </stages>
    </HediffDef>
    <!-- ==================== 食物效果 ==================== -->
    <HediffDef>
        <defName>AteFineMeal</defName>
        <label>ate fine meal</label>
        <description>Thoughts from eating a fine meal.</description>
        <thoughtClass>Thought_Memory</thoughtClass>
        <durationDays>1</durationDays>
        <stackLimit>1</stackLimit>
        <stages>
            <li>
                <label>tasty</label>
                <baseMoodEffect>5</baseMoodEffect>
            </li>
        </stages>
    </HediffDef>
</Defs>",
                FileType.Xml
            ));

            example.Files.Add(new ExampleFile(
                "About.xml",
                "About/About.xml",
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<ModMetaData>
    <name>Custom Consumables</name>
    <author>YourName</author>
    <packageId>YourName.CustomConsumables</packageId>
    <description>添加自定义消耗品，包括止痛药、能量棒和咖啡</description>
    <supportedVersions>
        <li>1.5</li>
    </supportedVersions>
</ModMetaData>",
                FileType.Xml
            ));

            example.Steps.Add("在 Defs/ThingDefs_Misc/ 目录下创建 ThingDef_Consumable.xml 文件");
            example.Steps.Add("药物：继承 DrugBase，配置 ingestible 节点中的 outcomeDoers 定义使用效果");
            example.Steps.Add("创建 HediffDef 定义药物效果（增益/减益状态）");
            example.Steps.Add("添加 CompProperties_Drug 组件配置成瘾属性");
            example.Steps.Add("食物：继承 MealBase，设置 nutrition 和 joy");
            example.Steps.Add("饮料：继承 DrugBase，设置 outcomeDoers 定义效果");
            example.Steps.Add("创建 HediffDef 定义饮料效果");
            example.Steps.Add("测试：在游戏中检查物品是否正确显示、能否制作和使用");

            return example;
        }

        private static Example GetFoodExample()
        {
            var example = new Example
            {
                Title = "消耗品定义示例 - 食物",
                Description = "创建自定义食物，包括营养值、心情效果和制作配方。",
                Feature = "物品定义",
                Keywords = new List<string> { "食物", "food", "消耗品", "营养" }
            };

            example.Files.Add(new ExampleFile(
                "ThingDef_Food.xml",
                "Defs/ThingDefs_Misc/ThingDef_Food.xml",
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Defs>
    <ThingDef ParentName=""MealBase"">
        <defName>Meal_GourmetDinner</defName>
        <label>gourmet dinner</label>
        <description>A carefully prepared gourmet meal that provides excellent nutrition and a mood boost.</description>
        <graphicData>
            <texPath>Things/Item/Meal/GourmetDinner</texPath>
            <graphicClass>Graphic_Single</graphicClass>
        </graphicData>
        <statBases>
            <WorkToMake>800</WorkToMake>
            <MarketValue>40</MarketValue>
            <Mass>0.5</Mass>
            <DeteriorationRate>6</DeteriorationRate>
        </statBases>
        <costList>
            <Meat_Raw>10</Meat_Raw>
            <RawVegetables>10</RawVegetables>
        </costList>
        <recipeMaker>
            <workSpeedStat>CookSpeed</workSpeedStat>
            <workSkill>Cooking</workSkill>
            <recipeUsers>
                <li>TableStove</li>
            </recipeUsers>
            <skillRequirements>
                <Cooking>8</Cooking>
            </skillRequirements>
        </recipeMaker>
        <ingestible>
            <foodType>Meal</foodType>
            <preferability>MealFine</preferability>
            <nutrition>1.0</nutrition>
            <joy>0.25</joy>
            <joyKind>Gluttonous</joyKind>
            <optimalityOffsetHumanlikes>12</optimalityOffsetHumanlikes>
            <ingestedDirectThought>AteGourmetMeal</ingestedDirectThought>
        </ingestible>
        <comps>
            <li Class=""CompProperties_Ingredients""/>
            <li Class=""CompProperties_FoodPoisonable""/>
        </comps>
    </ThingDef>

    <ThoughtDef>
        <defName>AteGourmetMeal</defName>
        <durationDays>1</durationDays>
        <stackLimit>1</stackLimit>
        <stages>
            <li>
                <label>ate gourmet meal</label>
                <description>That was an amazing meal! The flavors were exquisite.</description>
                <baseMoodEffect>12</baseMoodEffect>
            </li>
        </stages>
    </ThoughtDef>
</Defs>",
                FileType.Xml
            ));

            example.Steps.Add("创建 ThingDef 继承 MealBase");
            example.Steps.Add("设置 nutrition 定义营养值");
            example.Steps.Add("设置 joy 和 joyKind 定义心情效果");
            example.Steps.Add("创建 ThoughtDef 定义进食后的心情变化");
            example.Steps.Add("配置 recipeMaker 定义制作配方");
            example.Steps.Add("测试：在游戏中制作并食用食物");

            return example;
        }
    }
}
