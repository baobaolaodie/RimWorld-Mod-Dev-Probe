using System.Collections.Generic;

namespace RimWorldModDevProbe.Examples
{
    public static class PatchExamples
    {
        public static List<Example> GetExamples()
        {
            var examples = new List<Example>();

            examples.Add(GetDefPatchExample());
            examples.Add(GetResearchPatchExample());
            examples.Add(GetWeaponPatchExample());

            return examples;
        }

        private static Example GetDefPatchExample()
        {
            var example = new Example
            {
                Title = "基础Def修改Patch示例",
                Description = "使用PatchOperation修改游戏中的现有Def定义，包括修改属性值、添加新元素等。",
                Feature = "XML Patch",
                Keywords = new List<string> { "武器属性", "weapon patch", "修改武器", "PatchOperationReplace" }
            };

            example.Files.Add(new ExampleFile(
                "Patch_Beds.xml",
                "Defs/Patch_Beds.xml",
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Patch>
    <!-- 修改现有床的属性 -->
    <Operation Class=""PatchOperationReplace"">
        <xpath>/Defs/ThingDef[defName=""Bed""]/statBases/MaxHitPoints</xpath>
        <value>
            <MaxHitPoints>200</MaxHitPoints>
        </value>
    </Operation>

    <!-- 添加新的属性 -->
    <Operation Class=""PatchOperationAdd"">
        <xpath>/Defs/ThingDef[defName=""Bed""]/statBases</xpath>
        <value>
            <Beauty>5</Beauty>
        </value>
    </Operation>

    <!-- 条件性修改：只在特定Mod存在时执行 -->
    <Operation Class=""PatchOperationFindMod"">
        <mods>
            <li>Royalty</li>
        </mods>
        <match Class=""PatchOperationAdd"">
            <xpath>/Defs/ThingDef[defName=""Bed""]/comps</xpath>
            <value>
                <li Class=""CompProperties_AffectedByFacilities"">
                    <linkableFacilities>
                        <li>PodBed</li>
                    </linkableFacilities>
                </li>
            </value>
        </match>
    </Operation>
</Patch>",
                FileType.Xml
            ));

            example.Files.Add(new ExampleFile(
                "Patch_Apparel.xml",
                "Defs/Patch_Apparel.xml",
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Patch>
    <!-- 批量修改多个装备 -->
    <Operation Class=""PatchOperationReplace"">
        <xpath>/Defs/ThingDef[defName=""Apparel_Parka"" or defName=""Apparel_Jacket""]/statBases/Insulation_Cold</xpath>
        <value>
            <Insulation_Cold>-50</Insulation_Cold>
        </value>
    </Operation>

    <!-- 使用通配符匹配 -->
    <Operation Class=""PatchOperationAdd"">
        <xpath>/Defs/ThingDef[starts-with(defName, ""Apparel_"")]/statBases</xpath>
        <value>
            <MarketValue>100</MarketValue>
        </value>
    </Operation>

    <!-- 移除元素 -->
    <Operation Class=""PatchOperationRemove"">
        <xpath>/Defs/ThingDef[defName=""Apparel_CowboyHat""]/equippedStatOffsets</xpath>
    </Operation>
</Patch>",
                FileType.Xml
            ));

            example.Steps.Add("创建 Patch 文件，文件名以 Patch_ 开头，放置在 Defs/ 目录下");
            example.Steps.Add("使用 PatchOperationReplace 替换现有属性的值");
            example.Steps.Add("使用 PatchOperationAdd 添加新的属性或元素");
            example.Steps.Add("使用 PatchOperationRemove 移除不需要的元素");
            example.Steps.Add("使用 PatchOperationFindMod 实现条件性 Patch");
            example.Steps.Add("使用 xpath 定位要修改的 Def 元素");
            example.Steps.Add("测试：启动游戏，验证修改是否生效");

            return example;
        }

        private static Example GetResearchPatchExample()
        {
            var example = new Example
            {
                Title = "科技树修改Patch示例",
                Description = "修改科技研究项目，包括调整研究成本、修改前置科技、添加新的研究项目等。",
                Feature = "XML Patch",
                Keywords = new List<string> { "建筑成本", "building cost", "修改成本", "costList" }
            };

            example.Files.Add(new ExampleFile(
                "Patch_Research.xml",
                "Defs/Patch_Research.xml",
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Patch>
    <!-- 修改研究成本 -->
    <Operation Class=""PatchOperationReplace"">
        <xpath>/Defs/ResearchProjectDef[defName=""Machining""]/baseCost</xpath>
        <value>
            <baseCost>1500</baseCost>
        </value>
    </Operation>

    <!-- 添加前置科技 -->
    <Operation Class=""PatchOperationAdd"">
        <xpath>/Defs/ResearchProjectDef[defName=""Machining""]</xpath>
        <value>
            <prerequisites>
                <li>Smithing</li>
            </prerequisites>
        </value>
    </Operation>

    <!-- 修改已有前置科技列表 -->
    <Operation Class=""PatchOperationReplace"">
        <xpath>/Defs/ResearchProjectDef[defName=""MicroelectronicsBasics""]/prerequisites</xpath>
        <value>
            <prerequisites>
                <li>Machining</li>
                <li>Electricity</li>
            </prerequisites>
        </value>
    </Operation>

    <!-- 添加科技标签 -->
    <Operation Class=""PatchOperationAdd"">
        <xpath>/Defs/ResearchProjectDef[defName=""GunTurrets""]</xpath>
        <value>
            <tags>
                <li>ClassicStart</li>
            </tags>
        </value>
    </Operation>

    <!-- 条件性修改：需要DLC时 -->
    <Operation Class=""PatchOperationFindMod"">
        <mods>
            <li>Biotech</li>
        </mods>
        <match Class=""PatchOperationAdd"">
            <xpath>/Defs/ResearchProjectDef[defName=""Machining""]/requiredResearchFacilities</xpath>
            <value>
                <li>SimpleResearchBench</li>
            </value>
        </match>
    </Operation>
</Patch>",
                FileType.Xml
            ));

            example.Files.Add(new ExampleFile(
                "NewResearchProject.xml",
                "Defs/ResearchProjectDefs/NewResearch.xml",
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Defs>
    <!-- 添加新的研究项目 -->
    <ResearchProjectDef>
        <defName>AdvancedMachining</defName>
        <label>高级机械加工</label>
        <description>解锁更高级的机械加工技术，可以制造更复杂的机器部件。</description>
        <baseCost>2500</baseCost>
        <techLevel>Industrial</techLevel>
        <prerequisites>
            <li>Machining</li>
            <li>MicroelectronicsBasics</li>
        </prerequisites>
        <researchViewX>6</researchViewX>
        <researchViewY>3</researchViewY>
        <tags>
            <li>ClassicStart</li>
        </tags>
    </ResearchProjectDef>
</Defs>",
                FileType.Xml
            ));

            example.Steps.Add("确定要修改的 ResearchProjectDef 的 defName");
            example.Steps.Add("使用 PatchOperationReplace 修改研究成本等属性");
            example.Steps.Add("使用 PatchOperationAdd 添加前置科技或标签");
            example.Steps.Add("注意研究视图坐标(researchViewX/Y)的设置");
            example.Steps.Add("使用 PatchOperationFindMod 实现DLC兼容性");
            example.Steps.Add("测试：在游戏研究界面验证修改是否生效");

            return example;
        }

        private static Example GetWeaponPatchExample()
        {
            var example = new Example
            {
                Title = "武器修改Patch示例",
                Description = "修改武器属性，包括伤害、射程、射速等，以及添加新的武器标签和分类。",
                Feature = "XML Patch",
                Keywords = new List<string> { "添加功能", "add feature", "添加配方", "PatchOperationAdd" }
            };

            example.Files.Add(new ExampleFile(
                "Patch_Weapons.xml",
                "Defs/Patch_Weapons.xml",
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Patch>
    <!-- 修改武器伤害 -->
    <Operation Class=""PatchOperationReplace"">
        <xpath>/Defs/ThingDef[defName=""Gun_AssaultRifle""]/tools/li[@Class=""Bullet""]/damageAmountBase</xpath>
        <value>
            <damageAmountBase>15</damageAmountBase>
        </value>
    </Operation>

    <!-- 修改武器射程 -->
    <Operation Class=""PatchOperationReplace"">
        <xpath>/Defs/ThingDef[defName=""Gun_SniperRifle""]/statBases/Range</xpath>
        <value>
            <Range>60</Range>
        </value>
    </Operation>

    <!-- 批量修改同类武器 -->
    <Operation Class=""PatchOperationReplace"">
        <xpath>/Defs/ThingDef[defName=""Gun_Pistol"" or defName=""Gun_Revolver""]/statBases/AccuracyTouch</xpath>
        <value>
            <AccuracyTouch>0.90</AccuracyTouch>
        </value>
    </Operation>

    <!-- 添加武器标签 -->
    <Operation Class=""PatchOperationAdd"">
        <xpath>/Defs/ThingDef[defName=""Gun_AssaultRifle""]/weaponTags</xpath>
        <value>
            <li>EliteGun</li>
        </value>
    </Operation>

    <!-- 修改子弹属性 -->
    <Operation Class=""PatchOperationReplace"">
        <xpath>/Defs/ThingDef[defName=""Bullet_AssaultRifle""]/projectile/damageAmountBase</xpath>
        <value>
            <damageAmountBase>12</damageAmountBase>
        </value>
    </Operation>

    <!-- 添加武器装备属性 -->
    <Operation Class=""PatchOperationAdd"">
        <xpath>/Defs/ThingDef[defName=""Gun_SniperRifle""]/equippedStatOffsets</xpath>
        <value>
            <AimingAccuracy>0.2</AimingAccuracy>
        </value>
    </Operation>
</Patch>",
                FileType.Xml
            ));

            example.Files.Add(new ExampleFile(
                "Patch_WeaponRecipes.xml",
                "Defs/Patch_WeaponRecipes.xml",
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Patch>
    <!-- 修改武器制作成本 -->
    <Operation Class=""PatchOperationReplace"">
        <xpath>/Defs/RecipeDef[defName=""Make_AssaultRifle""]/costList</xpath>
        <value>
            <costList>
                <Steel>80</Steel>
                <ComponentIndustrial>5</ComponentIndustrial>
                <Chemfuel>10</Chemfuel>
            </costList>
        </value>
    </Operation>

    <!-- 修改制作时间 -->
    <Operation Class=""PatchOperationReplace"">
        <xpath>/Defs/RecipeDef[defName=""Make_AssaultRifle""]/workAmount</xpath>
        <value>
            <workAmount>3000</workAmount>
        </value>
    </Operation>

    <!-- 添加制作技能要求 -->
    <Operation Class=""PatchOperationAdd"">
        <xpath>/Defs/RecipeDef[defName=""Make_AssaultRifle""]</xpath>
        <value>
            <skillRequirements>
                <Crafting>8</Crafting>
            </skillRequirements>
        </value>
    </Operation>

    <!-- 添加到工作台 -->
    <Operation Class=""PatchOperationAdd"">
        <xpath>/Defs/ThingDef[defName=""Table_Machining""]/recipes</xpath>
        <value>
            <li>Make_AssaultRifle</li>
        </value>
    </Operation>
</Patch>",
                FileType.Xml
            ));

            example.Steps.Add("找到要修改的武器 ThingDef 的 defName");
            example.Steps.Add("使用正确的 xpath 定位武器属性");
            example.Steps.Add("注意武器伤害可能在 tools 或 projectile 中定义");
            example.Steps.Add("修改 RecipeDef 可以调整制作成本和时间");
            example.Steps.Add("使用 PatchOperationAdd 添加新的标签或配方");
            example.Steps.Add("测试：在游戏中检查武器属性和制作配方");

            return example;
        }
    }
}
