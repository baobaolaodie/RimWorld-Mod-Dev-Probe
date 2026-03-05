using System.Collections.Generic;

namespace RimWorldModDevProbe.Examples
{
    public static class WeaponExamples
    {
        public static List<Example> GetExamples()
        {
            var examples = new List<Example>();

            examples.Add(GetMeleeWeaponExample());
            examples.Add(GetRangedWeaponExample());

            return examples;
        }

        private static Example GetMeleeWeaponExample()
        {
            var example = new Example
            {
                Title = "近战武器定义示例",
                Description = "创建自定义近战武器，包括剑、锤、匕首等类型。包含伤害、冷却时间、材质等完整属性配置。",
                Feature = "武器定义",
                Keywords = new List<string> { "近战武器", "melee weapon", "剑", "近战" }
            };

            example.Files.Add(new ExampleFile(
                "ThingDef_MeleeWeapon.xml",
                "Defs/ThingDefs_Misc/ThingDef_MeleeWeapon.xml",
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Defs>
    <!-- ==================== 基础近战武器 - 铁剑 ==================== -->
    <ThingDef ParentName=""BaseMeleeWeapon_Sharp_Quality"">
        <defName>MeleeWeapon_IronSword</defName>
        <label>iron sword</label>
        <description>A sturdy iron sword. Good for cutting and thrusting.</description>
        <graphicData>
            <texPath>Things/Item/Equipment/WeaponMelee/IronSword</texPath>
            <graphicClass>Graphic_Single</graphicClass>
        </graphicData>
        <statBases>
            <WorkToMake>1000</WorkToMake>
            <Mass>2.0</Mass>
            <MeleeDPS>12</MeleeDPS>
            <ArmorPenetration>0.25</ArmorPenetration>
        </statBases>
        <equippedStatOffsets>
            <MoveSpeed>-0.05</MoveSpeed>
        </equippedStatOffsets>
        <costList>
            <Steel>50</Steel>
        </costList>
        <recipeMaker>
            <workSpeedStat>GeneralLaborSpeed</workSpeedStat>
            <workSkill>Crafting</workSkill>
            <recipeUsers>
                <li>TableMachining</li>
            </recipeUsers>
            <skillRequirements>
                <Crafting>4</Crafting>
            </skillRequirements>
        </recipeMaker>
        <tools>
            <li>
                <label>handle</label>
                <capacities>
                    <li>Poke</li>
                </capacities>
                <power>9</power>
                <cooldownTime>1.8</cooldownTime>
            </li>
            <li>
                <label>point</label>
                <capacities>
                    <li>Stab</li>
                </capacities>
                <power>24</power>
                <cooldownTime>1.9</cooldownTime>
            </li>
            <li>
                <label>edge</label>
                <capacities>
                    <li>Cut</li>
                </capacities>
                <power>24</power>
                <cooldownTime>1.9</cooldownTime>
            </li>
        </tools>
    </ThingDef>

    <!-- ==================== 高级近战武器 - 钢铁战锤 ==================== -->
    <ThingDef ParentName=""BaseMeleeWeapon_Blunt_Quality"">
        <defName>MeleeWeapon_SteelWarHammer</defName>
        <label>steel war hammer</label>
        <description>A heavy steel war hammer designed for crushing armor and bones.</description>
        <graphicData>
            <texPath>Things/Item/Equipment/WeaponMelee/SteelWarHammer</texPath>
            <graphicClass>Graphic_Single</graphicClass>
        </graphicData>
        <statBases>
            <WorkToMake>1500</WorkToMake>
            <Mass>4.5</Mass>
            <MeleeDPS>10</MeleeDPS>
            <ArmorPenetration>0.35</ArmorPenetration>
        </statBases>
        <equippedStatOffsets>
            <MoveSpeed>-0.10</MoveSpeed>
        </equippedStatOffsets>
        <costList>
            <Steel>80</Steel>
        </costList>
        <recipeMaker>
            <workSpeedStat>GeneralLaborSpeed</workSpeedStat>
            <workSkill>Crafting</workSkill>
            <recipeUsers>
                <li>TableMachining</li>
            </recipeUsers>
            <skillRequirements>
                <Crafting>6</Crafting>
            </skillRequirements>
        </recipeMaker>
        <tools>
            <li>
                <label>handle</label>
                <capacities>
                    <li>Poke</li>
                </capacities>
                <power>10</power>
                <cooldownTime>2.0</cooldownTime>
            </li>
            <li>
                <label>head</label>
                <capacities>
                    <li>Blunt</li>
                </capacities>
                <power>32</power>
                <cooldownTime>2.8</cooldownTime>
            </li>
        </tools>
    </ThingDef>

    <!-- ==================== 特殊近战武器 - 能量剑 ==================== -->
    <ThingDef ParentName=""BaseMeleeWeapon_Sharp_Quality"">
        <defName>MeleeWeapon_EnergyBlade</defName>
        <label>energy blade</label>
        <description>A futuristic energy blade that cuts through almost anything.</description>
        <graphicData>
            <texPath>Things/Item/Equipment/WeaponMelee/EnergyBlade</texPath>
            <graphicClass>Graphic_Single</graphicClass>
        </graphicData>
        <statBases>
            <WorkToMake>5000</WorkToMake>
            <Mass>1.5</Mass>
            <MeleeDPS>18</MeleeDPS>
            <ArmorPenetration>0.60</ArmorPenetration>
        </statBases>
        <equippedStatOffsets>
            <MoveSpeed>-0.02</MoveSpeed>
        </equippedStatOffsets>
        <costList>
            <Steel>20</Steel>
            <Plasteel>50</Plasteel>
            <ComponentIndustrial>3</ComponentIndustrial>
        </costList>
        <recipeMaker>
            <workSpeedStat>GeneralLaborSpeed</workSpeedStat>
            <workSkill>Crafting</workSkill>
            <recipeUsers>
                <li>TableMachining</li>
            </recipeUsers>
            <skillRequirements>
                <Crafting>10</Crafting>
            </skillRequirements>
            <researchPrerequisite>ChargedShot</researchPrerequisite>
        </recipeMaker>
        <tools>
            <li>
                <label>handle</label>
                <capacities>
                    <li>Poke</li>
                </capacities>
                <power>12</power>
                <cooldownTime>1.5</cooldownTime>
            </li>
            <li>
                <label>energy edge</label>
                <capacities>
                    <li>Cut</li>
                    <li>Stab</li>
                </capacities>
                <power>36</power>
                <cooldownTime>1.6</cooldownTime>
                <extraMeleeDamages>
                    <li>
                        <def>Flame</def>
                        <amount>5</amount>
                    </li>
                </extraMeleeDamages>
            </li>
        </tools>
    </ThingDef>
</Defs>",
                FileType.Xml
            ));

            example.Files.Add(new ExampleFile(
                "About.xml",
                "About/About.xml",
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<ModMetaData>
    <name>Custom Melee Weapons</name>
    <author>YourName</author>
    <packageId>YourName.CustomMeleeWeapons</packageId>
    <description>添加自定义近战武器，包括铁剑、战锤和能量剑</description>
    <supportedVersions>
        <li>1.5</li>
    </supportedVersions>
</ModMetaData>",
                FileType.Xml
            ));

            example.Steps.Add("在 Defs/ThingDefs_Misc/ 目录下创建 ThingDef_MeleeWeapon.xml 文件");
            example.Steps.Add("继承 BaseMeleeWeapon_Sharp_Quality 或 BaseMeleeWeapon_Blunt_Quality 基类");
            example.Steps.Add("定义 defName（唯一标识符）、label（显示名称）、description（描述）");
            example.Steps.Add("配置 graphicData 指定贴图路径");
            example.Steps.Add("设置 statBases：WorkToMake（制作工时）、Mass（重量）、MeleeDPS（近战DPS）、ArmorPenetration（护甲穿透）");
            example.Steps.Add("配置 tools 列表定义武器的攻击方式（切割、刺击、钝击等）");
            example.Steps.Add("设置 costList 定义制作材料消耗");
            example.Steps.Add("配置 recipeMaker 定义制作配方和工作台要求");
            example.Steps.Add("将贴图文件放置在 Textures/Things/Item/Equipment/WeaponMelee/ 目录下");
            example.Steps.Add("测试：在游戏中检查武器是否正确显示、能否制作和使用");

            return example;
        }

        private static Example GetRangedWeaponExample()
        {
            var example = new Example
            {
                Title = "远程武器定义示例",
                Description = "创建自定义远程武器，包括枪械、弓箭等类型。包含射程、伤害、射速、弹药等完整属性配置。",
                Feature = "武器定义",
                Keywords = new List<string> { "远程武器", "ranged weapon", "枪械", "弓箭" }
            };

            example.Files.Add(new ExampleFile(
                "ThingDef_RangedWeapon.xml",
                "Defs/ThingDefs_Misc/ThingDef_RangedWeapon.xml",
                @"<?xml version=""1.0"" encoding=""utf-8"" ?>
<Defs>
    <!-- ==================== 基础枪械 - 自制手枪 ==================== -->
    <ThingDef ParentName=""BaseGun"">
        <defName>Gun_CustomPistol</defName>
        <label>custom pistol</label>
        <description>A simple homemade pistol. Not very accurate but gets the job done.</description>
        <graphicData>
            <texPath>Things/Item/Equipment/WeaponRanged/CustomPistol</texPath>
            <graphicClass>Graphic_Single</graphicClass>
        </graphicData>
        <statBases>
            <WorkToMake>2000</WorkToMake>
            <Mass>1.2</Mass>
            <AccuracyTouch>0.70</AccuracyTouch>
            <AccuracyShort>0.55</AccuracyShort>
            <AccuracyMedium>0.35</AccuracyMedium>
            <AccuracyLong>0.20</AccuracyLong>
            <RangedWeapon_Cooldown>1.5</RangedWeapon_Cooldown>
        </statBases>
        <costList>
            <Steel>30</Steel>
            <ComponentIndustrial>2</ComponentIndustrial>
        </costList>
        <recipeMaker>
            <workSpeedStat>GeneralLaborSpeed</workSpeedStat>
            <workSkill>Crafting</workSkill>
            <recipeUsers>
                <li>TableMachining</li>
            </recipeUsers>
            <skillRequirements>
                <Crafting>4</Crafting>
            </skillRequirements>
        </recipeMaker>
        <verbs>
            <li>
                <verbClass>Verb_Shoot</verbClass>
                <hasStandardCommand>true</hasStandardCommand>
                <defaultProjectile>Bullet_Pistol</defaultProjectile>
                <warmupTime>0.5</warmupTime>
                <range>20</range>
                <soundCast>GunShotA</soundCast>
                <soundCastTail>GunTail_Light</soundCastTail>
                <muzzleFlashScale>9</muzzleFlashScale>
            </li>
        </verbs>
        <tools>
            <li>
                <label>grip</label>
                <capacities>
                    <li>Poke</li>
                </capacities>
                <power>9</power>
                <cooldownTime>1.8</cooldownTime>
            </li>
            <li>
                <label>barrel</label>
                <capacities>
                    <li>Blunt</li>
                </capacities>
                <power>10</power>
                <cooldownTime>1.9</cooldownTime>
            </li>
        </tools>
    </ThingDef>

    <!-- ==================== 高级枪械 - 突击步枪 ==================== -->
    <ThingDef ParentName=""BaseGun"">
        <defName>Gun_CustomAssaultRifle</defName>
        <label>custom assault rifle</label>
        <description>A reliable assault rifle with good range and accuracy.</description>
        <graphicData>
            <texPath>Things/Item/Equipment/WeaponRanged/CustomAssaultRifle</texPath>
            <graphicClass>Graphic_Single</graphicClass>
        </graphicData>
        <statBases>
            <WorkToMake>4500</WorkToMake>
            <Mass>3.5</Mass>
            <AccuracyTouch>0.65</AccuracyTouch>
            <AccuracyShort>0.75</AccuracyShort>
            <AccuracyMedium>0.65</AccuracyMedium>
            <AccuracyLong>0.45</AccuracyLong>
            <RangedWeapon_Cooldown>1.0</RangedWeapon_Cooldown>
        </statBases>
        <costList>
            <Steel>60</Steel>
            <ComponentIndustrial>5</ComponentIndustrial>
            <Chemfuel>10</Chemfuel>
        </costList>
        <recipeMaker>
            <workSpeedStat>GeneralLaborSpeed</workSpeedStat>
            <workSkill>Crafting</workSkill>
            <recipeUsers>
                <li>TableMachining</li>
            </recipeUsers>
            <skillRequirements>
                <Crafting>8</Crafting>
            </skillRequirements>
            <researchPrerequisite>PrecisionRifling</researchPrerequisite>
        </recipeMaker>
        <verbs>
            <li>
                <verbClass>Verb_Shoot</verbClass>
                <hasStandardCommand>true</hasStandardCommand>
                <defaultProjectile>Bullet_AssaultRifle</defaultProjectile>
                <warmupTime>1.0</warmupTime>
                <range>35</range>
                <burstShotCount>3</burstShotCount>
                <ticksBetweenBurstShots>6</ticksBetweenBurstShots>
                <soundCast>GunShotA</soundCast>
                <soundCastTail>GunTail_Heavy</soundCastTail>
                <muzzleFlashScale>11</muzzleFlashScale>
            </li>
        </verbs>
        <tools>
            <li>
                <label>stock</label>
                <capacities>
                    <li>Blunt</li>
                </capacities>
                <power>10</power>
                <cooldownTime>2.0</cooldownTime>
            </li>
            <li>
                <label>barrel</label>
                <capacities>
                    <li>Blunt</li>
                </capacities>
                <power>11</power>
                <cooldownTime>2.1</cooldownTime>
            </li>
        </tools>
    </ThingDef>

    <!-- ==================== 弓箭武器 - 复合弓 ==================== -->
    <ThingDef ParentName=""BaseProjectileNeolithic"">
        <defName>Gun_CustomCompoundBow</defName>
        <label>compound bow</label>
        <description>A modern compound bow with pulleys and high tension. Very accurate and powerful.</description>
        <graphicData>
            <texPath>Things/Item/Equipment/WeaponRanged/CompoundBow</texPath>
            <graphicClass>Graphic_Single</graphicClass>
        </graphicData>
        <statBases>
            <WorkToMake>3000</WorkToMake>
            <Mass>2.0</Mass>
            <AccuracyTouch>0.60</AccuracyTouch>
            <AccuracyShort>0.80</AccuracyShort>
            <AccuracyMedium>0.70</AccuracyMedium>
            <AccuracyLong>0.50</AccuracyLong>
            <RangedWeapon_Cooldown>1.8</RangedWeapon_Cooldown>
        </statBases>
        <costList>
            <WoodLog>30</WoodLog>
            <Steel>20</Steel>
        </costList>
        <recipeMaker>
            <workSpeedStat>GeneralLaborSpeed</workSpeedStat>
            <workSkill>Crafting</workSkill>
            <recipeUsers>
                <li>CraftingSpot</li>
                <li>TableMachining</li>
            </recipeUsers>
            <skillRequirements>
                <Crafting>6</Crafting>
            </skillRequirements>
        </recipeMaker>
        <verbs>
            <li>
                <verbClass>Verb_Shoot</verbClass>
                <hasStandardCommand>true</hasStandardCommand>
                <defaultProjectile>Arrow_CustomCompound</defaultProjectile>
                <warmupTime>1.5</warmupTime>
                <range>30</range>
                <soundCast>Bow_Large</soundCast>
            </li>
        </verbs>
        <tools>
            <li>
                <label>limb</label>
                <capacities>
                    <li>Blunt</li>
                </capacities>
                <power>8</power>
                <cooldownTime>1.6</cooldownTime>
            </li>
        </tools>
    </ThingDef>

    <!-- ==================== 自定义弹药定义 ==================== -->
    <ThingDef ParentName=""BaseBullet"">
        <defName>Arrow_CustomCompound</defName>
        <label>compound arrow</label>
        <graphicData>
            <texPath>Things/Projectile/Arrow_CustomCompound</texPath>
            <graphicClass>Graphic_Single</graphicClass>
        </graphicData>
        <projectile>
            <damageDef>Arrow</damageDef>
            <damageAmountBase>18</damageAmountBase>
            <armorPenetrationBase>0.20</armorPenetrationBase>
            <speed>45</speed>
        </projectile>
    </ThingDef>
</Defs>",
                FileType.Xml
            ));

            example.Files.Add(new ExampleFile(
                "About.xml",
                "About/About.xml",
                @"<?xml version=""1.0"" encoding=""utf-8""?>
<ModMetaData>
    <name>Custom Ranged Weapons</name>
    <author>YourName</author>
    <packageId>YourName.CustomRangedWeapons</packageId>
    <description>添加自定义远程武器，包括手枪、突击步枪和复合弓</description>
    <supportedVersions>
        <li>1.5</li>
    </supportedVersions>
</ModMetaData>",
                FileType.Xml
            ));

            example.Steps.Add("在 Defs/ThingDefs_Misc/ 目录下创建 ThingDef_RangedWeapon.xml 文件");
            example.Steps.Add("继承 BaseGun（枪械）或 BaseProjectileNeolithic（弓箭）基类");
            example.Steps.Add("定义 defName、label、description 和 graphicData");
            example.Steps.Add("设置 statBases：AccuracyTouch/Short/Medium/Long（各距离精度）、RangedWeapon_Cooldown（射击冷却）");
            example.Steps.Add("配置 verbs 列表定义射击属性：defaultProjectile（弹药）、range（射程）、warmupTime（瞄准时间）、burstShotCount（连发数）");
            example.Steps.Add("如需自定义弹药，创建 BaseBullet 子类定义弹药属性");
            example.Steps.Add("设置 costList 定义制作材料消耗");
            example.Steps.Add("配置 recipeMaker 定义制作配方和研究需求");
            example.Steps.Add("将贴图文件放置在 Textures/Things/Item/Equipment/WeaponRanged/ 目录下");
            example.Steps.Add("测试：在游戏中检查武器是否正确显示、能否制作和射击");

            return example;
        }
    }
}
