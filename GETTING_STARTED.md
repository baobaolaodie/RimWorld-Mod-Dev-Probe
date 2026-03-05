# RimWorld Mod 开发新手指南

> 本指南面向完全没有 RimWorld Mod 开发经验的开发者，帮助你快速入门。

---

## 目录

1. [什么是 RimWorld Mod](#1-什么是-rimworld-mod)
2. [核心概念](#2-核心概念)
3. [开发环境搭建](#3-开发环境搭建)
4. [使用 Probe 辅助开发](#4-使用-probe-辅助开发)
5. [实战：创建第一个 Mod](#5-实战创建第一个-mod)
6. [常见问题解答](#6-常见问题解答)

---

## 1. 什么是 RimWorld Mod

### 1.1 Mod 是什么？

Mod（模组）是游戏的扩展内容，可以：
- 添加新物品、武器、建筑
- 修改游戏机制
- 添加新种族、派系
- 改变游戏界面和音效

### 1.2 RimWorld Mod 的组成

一个典型的 RimWorld Mod 包含：

```
MyMod/
├── About/
│   ├── About.xml          # Mod 元数据（名称、版本、描述）
│   └── Preview.png        # Mod 预览图（可选）
├── Defs/                  # XML 定义文件
│   └── ThingDef_MyItem.xml
├── Patches/               # XML 补丁文件（修改原版内容）
│   └── Patch_Vanilla.xml
├── Assemblies/            # C# 编译后的 DLL（可选）
│   └── MyMod.dll
├── Sounds/                # 音效文件（可选）
├── Textures/              # 贴图文件（可选）
└── Languages/             # 翻译文件（可选）
```

---

## 2. 核心概念

### 2.1 Defs（定义）

**Defs** 是 RimWorld 的核心数据结构，用 XML 定义游戏中的所有内容。

**示例：定义一个简单的物品**
```xml
<!-- Defs/ThingDef_MySword.xml -->
<?xml version="1.0" encoding="utf-8"?>
<Defs>
  <ThingDef ParentName="BaseMeleeWeapon_Sharp_Quality">
    <defName>MyMod_MySword</defName>
    <label>my sword</label>
    <description>A custom sword from my mod.</description>
    <graphicData>
      <texPath>Things/Item/Equipment/WeaponMelee/MySword</texPath>
      <graphicClass>Graphic_Single</graphicClass>
    </graphicData>
    <statBases>
      <WorkToMake>1000</WorkToMake>
      <Mass>2.0</Mass>
      <MeleeDPS>10</MeleeDPS>
    </statBases>
    <equippedStatOffsets>
      <MoveSpeed>0.1</MoveSpeed>
    </equippedStatOffsets>
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
        <label>blade</label>
        <capacities>
          <li>Cut</li>
        </capacities>
        <power>26</power>
        <cooldownTime>1.2</cooldownTime>
      </li>
    </tools>
  </ThingDef>
</Defs>
```

**常见 DefType：**
| DefType | 用途 | 示例 |
|---------|------|------|
| `ThingDef` | 物品、武器、建筑、角色 | 武器、工作台、服装 |
| `SoundDef` | 音效定义 | 死亡音效、受伤音效 |
| `JobDef` | 工作任务 | 种植、建造、研究 |
| `RecipeDef` | 制作配方 | 武器制作、药物合成 |
| `ResearchProjectDef` | 研究项目 | 新技术研究 |
| `HediffDef` | 健康状态、疾病、伤势 | 疾病、植入体 |
| `PawnKindDef` | 角色类型定义 | 敌人类型、动物 |
| `FactionDef` | 派系定义 | 敌对派系、友好派系 |
| `IncidentDef` | 事件定义 | 袭击、贸易商来访 |
| `TraderKindDef` | 贸易商类型 | 武器商、奴隶商 |
| `TraitDef` | 特质定义 | 乐观、悲观、快速学习者 |
| `ThoughtDef` | 思维定义 | 开心、悲伤、愤怒 |
| `NeedDef` | 需求定义 | 饥饿、休息、娱乐 |

### 2.1.1 Mod 类型分类

根据复杂程度，RimWorld Mod 可分为以下类型：

**基础 Mod（仅 XML）：**
| 类型 | 说明 | 难度 |
|------|------|------|
| 武器 Mod | 添加新武器（近战/远程） | ★☆☆ |
| 建筑 Mod | 添加新建筑（工作台/防御） | ★☆☆ |
| 物品 Mod | 添加新物品（消耗品/资源） | ★☆☆ |
| 服装 Mod | 添加新服装和护甲 | ★☆☆ |
| 动物 Mod | 添加新动物类型 | ★★☆ |

**中级 Mod（XML + Patch）：**
| 类型 | 说明 | 难度 |
|------|------|------|
| 修改 Mod | 修改原版内容属性 | ★★☆ |
| 兼容性 Mod | 让多个 Mod 协同工作 | ★★☆ |
| 翻译 Mod | 添加语言支持 | ★☆☆ |

**高级 Mod（XML + C#）：**
| 类型 | 说明 | 难度 |
|------|------|------|
| 种族 Mod | 添加新种族（含特殊能力） | ★★★ |
| 事件 Mod | 添加新游戏事件 | ★★★ |
| 机制 Mod | 修改游戏核心机制 | ★★★ |
| 研究 Mod | 添加复杂研究系统 | ★★☆ |

### 2.2 Patches（补丁）

**Patches** 用于修改已有的 Defs，而不是创建新的。

**示例：修改原版武器伤害**
```xml
<!-- Patches/Patch_VanillaSword.xml -->
<?xml version="1.0" encoding="utf-8"?>
<Patch>
  <!-- 找到原版的长剑 -->
  <Operation Class="PatchOperationFindMod">
    <mods>
      <li>Core</li>
    </mods>
    <match Class="PatchOperationSequence">
      <operations>
        <!-- 修改伤害 -->
        <li Class="PatchOperationReplace">
          <xpath>/Defs/ThingDef[defName="MeleeWeapon_LongSword"]/tools/li[label="blade"]/power</xpath>
          <value>
            <power>30</power>
          </value>
        </li>
      </operations>
    </match>
  </Operation>
</Patch>
```

**常见 PatchOperation：**
| 操作 | 用途 |
|------|------|
| `PatchOperationAdd` | 添加新元素 |
| `PatchOperationReplace` | 替换元素 |
| `PatchOperationRemove` | 删除元素 |
| `PatchOperationInsert` | 插入元素 |

### 2.3 Harmony（代码注入）

**Harmony** 是一个 C# 库，用于在运行时修改游戏代码，实现 XML 无法做到的功能。

**Harmony Patch 类型：**
| 类型 | 用途 |
|------|------|
| **Prefix** | 在原方法执行前运行，可以阻止原方法执行 |
| **Postfix** | 在原方法执行后运行，可以修改返回值 |
| **Transpiler** | 直接修改方法的 IL 代码 |

**示例：修改角色死亡时的音效**
```csharp
using HarmonyLib;
using RimWorld;
using Verse;

namespace MyMod
{
    // 标记要 Patch 的类和方法
    [HarmonyPatch(typeof(Pawn), "GetDeathSound")]
    public static class Pawn_GetDeathSound_Patch
    {
        // Postfix: 在原方法执行后运行
        // __result 是原方法的返回值
        // __instance 是当前 Pawn 实例
        public static void Postfix(Pawn __instance, ref SoundDef __result)
        {
            // 如果返回了音效，替换为自定义音效
            if (__result != null)
            {
                __result = DefDatabase<SoundDef>.GetNamed("MyCustomDeathSound");
            }
        }
    }
}
```

---

## 3. 开发环境搭建

### 3.1 必需工具

1. **文本编辑器**：Visual Studio Code 或 Visual Studio
2. **.NET SDK**：4.7.2 或兼容版本
3. **RimWorld 游戏**：Steam 版或 DRM-free 版

### 3.2 推荐工具

1. **RimWorldModDevProbe**：本工具，用于探测游戏 API 和辅助 Mod 开发
2. **dnSpy**：查看游戏 DLL 的反编译代码

### 3.3 项目结构

创建一个新 Mod 的推荐结构：

```
MyMod/
├── About/
│   └── About.xml
├── Defs/
├── Patches/
├── Source/                # C# 源代码（可选）
│   ├── MyMod.csproj
│   └── MyMod.cs
└── README.md
```

### 3.4 About.xml 模板

```xml
<?xml version="1.0" encoding="utf-8"?>
<ModMetaData>
  <name>My Mod Name</name>
  <author>Your Name</author>
  <packageId>YourName.MyMod</packageId>
  <supportedVersions>
    <li>1.6</li>
  </supportedVersions>
  <description>A brief description of your mod.</description>
</ModMetaData>
```

---

## 4. 使用 Probe 辅助开发

### 4.1 Probe 是什么？

**RimWorldModDevProbe** 是一个命令行工具，帮助你：
- 探索游戏的类型、方法、字段
- 查找 XML Defs 的结构
- 分析代码调用关系
- 获取 Harmony Patch 推荐点
- 通过向导快速生成 Mod 代码

**功能亮点：**
- **19 个命令**：覆盖搜索、分析、推荐、向导等功能
- **6 个向导**：武器、建筑、种族、XML Patch、音效、Harmony Patch
- **21 个示例**：覆盖各类 Mod 开发场景
- **中文支持**：支持中文关键词搜索

### 4.2 安装 Probe

1. 配置 `config.json` 文件，设置 RimWorld 游戏路径
2. 将 RimWorld 的 DLL 复制到 `GameDll` 目录（或配置自动加载）
3. 运行 `dotnet run` 或直接运行编译后的 `RimWorldModDevProbe.exe`

### 4.3 常用命令

#### 模式切换
```
> mode dll       # 切换到 DLL 模式 - 搜索类型
> mode def       # 切换到 Def 模式 - 搜索 XML Defs
> mode patch     # 切换到 Patch 模式 - 搜索 XML Patches
> mode harmony   # 切换到 Harmony 模式 - 搜索 Harmony Patches
> mode mod       # 切换到 Mod 模式 - 搜索 Mods
```

**快捷模式切换（带搜索）：**
```
> dll Pawn       # 切换到 DLL 模式并搜索 Pawn
> def Gun        # 切换到 Def 模式并搜索 Gun
> patch Weapon   # 切换到 Patch 模式并搜索 Weapon
> harmony Die    # 切换到 Harmony 模式并搜索 Die
> mod Core       # 切换到 Mod 模式并搜索 Core
```

#### 快捷搜索（无需切换模式）
```
> type Pawn      # 搜索类型
> method Die     # 搜索方法
> field health   # 搜索字段
```

#### 查看继承链
```
> inherit Pawn
```
显示 Pawn 类的继承关系。

#### 搜索方法
```
> method Die
```
搜索所有包含 "Die" 的方法。

#### 搜索字段
```
> field health
```
搜索所有包含 "health" 的字段。

#### 查看 XML 结构
```
> xml SoundDef
```
显示 SoundDef 的 XML 模板。

#### 分析调用链
```
> calls Die
```
分析方法的调用链（谁调用了它、它调用了谁）。

#### 分析字段使用
```
> usage health
```
分析字段的使用位置（读/写位置）。

#### 功能搜索
```
> feature 死亡音效
```
搜索与"死亡音效"相关的代码位置。

#### 获取 Patch 推荐
```
> recommend 修改死亡音效
```
获取修改死亡音效的最佳 Patch 点推荐。

#### 查看相关资源
```
> relate Pawn
```
显示与 Pawn 相关的资源推荐。

#### 查看示例
```
> example
```
查看所有可用的代码示例（21个示例）。

```
> example 近战武器
```
查看特定示例的详细内容。

#### 启动开发向导
```
> wizard
```
启动交互式开发向导，引导完成 Mod 开发。

#### 其他实用命令
```
> types          # 列出所有 DefTypes
> mods           # 列出所有已加载的 Mods
> info           # 显示系统信息
> clear          # 清除缓存
> help           # 显示帮助信息
```

### 4.4 实际使用流程

**场景：我想修改角色死亡时的音效**

1. **搜索相关代码**
   ```
   > feature 死亡音效
   
   Found 3 recommendations:
     [0] Pawn.Die (Method)
         处理 Pawn 死亡逻辑的主要方法
     [1] SoundDef (Class)
         SoundDef 用于定义游戏中的音效资源
   ```

2. **查看方法详情**
   ```
   > method Die
   ```
   选择 `Pawn.Die` 方法查看详情。

3. **获取 Patch 推荐**
   ```
   > recommend 修改死亡音效
   
   Patch Recommendations:
     [0] Target: Pawn.GetDeathSound()
         Type: Prefix
         Reason: 替换返回的音效
   ```

4. **查看示例代码**
   ```
   > example 死亡音效
   ```
   获取完整的实现示例。

---

## 5. 实战：创建第一个 Mod

### 5.1 目标

创建一个简单的 Mod：添加一把自定义武器。

### 5.2 步骤

#### 步骤 1：创建 Mod 目录

```
MyFirstMod/
├── About/
│   └── About.xml
└── Defs/
    └── ThingDef_MySword.xml
```

#### 步骤 2：创建 About.xml

```xml
<?xml version="1.0" encoding="utf-8"?>
<ModMetaData>
  <name>My First Mod</name>
  <author>Your Name</author>
  <packageId>YourName.MyFirstMod</packageId>
  <supportedVersions>
    <li>1.6</li>
  </supportedVersions>
  <description>My first RimWorld mod - adds a custom sword.</description>
</ModMetaData>
```

#### 步骤 3：使用 Probe 查找武器定义模板

```
> xml ThingDef
```

查看 ThingDef 的 XML 结构，或者搜索原版武器：

```
> def LongSword
```

#### 步骤 4：创建武器定义

```xml
<!-- Defs/ThingDef_MySword.xml -->
<?xml version="1.0" encoding="utf-8"?>
<Defs>
  <ThingDef ParentName="BaseMeleeWeapon_Sharp_Quality">
    <defName>MyFirstMod_CustomSword</defName>
    <label>custom sword</label>
    <description>A custom sword created for my first mod.</description>
    <graphicData>
      <texPath>Things/Item/Equipment/WeaponMelee/LongSword</texPath>
      <graphicClass>Graphic_Single</graphicClass>
    </graphicData>
    <statBases>
      <WorkToMake>1000</WorkToMake>
      <Mass>2.0</Mass>
      <MeleeDPS>15</MeleeDPS>
    </statBases>
    <equippedStatOffsets>
      <MoveSpeed>0.05</MoveSpeed>
    </equippedStatOffsets>
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
        <label>blade</label>
        <capacities>
          <li>Cut</li>
        </capacities>
        <power>30</power>
        <cooldownTime>1.0</cooldownTime>
      </li>
    </tools>
  </ThingDef>
</Defs>
```

#### 步骤 5：测试 Mod

1. 将 `MyFirstMod` 文件夹复制到 RimWorld 的 `Mods` 目录
2. 启动游戏，在 Mod 管理器中启用 Mod
3. 进入游戏，使用开发者模式或贸易商购买测试武器

### 5.3 更多实战示例

#### 示例 1：创建远程武器

使用 Probe 查找远程武器模板：
```
> def Gun
```

创建远程武器定义：
```xml
<!-- Defs/ThingDef_MyGun.xml -->
<?xml version="1.0" encoding="utf-8"?>
<Defs>
  <ThingDef ParentName="BaseGun">
    <defName>MyMod_CustomRifle</defName>
    <label>custom rifle</label>
    <description>A custom rifle with balanced stats.</description>
    <graphicData>
      <texPath>Things/Item/Equipment/WeaponRanged/AssaultRifle</texPath>
      <graphicClass>Graphic_Single</graphicClass>
    </graphicData>
    <statBases>
      <WorkToMake>2500</WorkToMake>
      <Mass>3.5</Mass>
      <AccuracyTouch>0.6</AccuracyTouch>
      <AccuracyShort>0.8</AccuracyShort>
      <AccuracyMedium>0.7</AccuracyMedium>
      <AccuracyLong>0.5</AccuracyLong>
      <RangedWeapon_Cooldown>1.2</RangedWeapon_Cooldown>
    </statBases>
    <verbs>
      <li>
        <verbClass>Verb_Shoot</verbClass>
        <hasStandardCommand>true</hasStandardCommand>
        <defaultProjectile>Bullet_AssaultRifle</defaultProjectile>
        <warmupTime>1.0</warmupTime>
        <range>32</range>
        <burstShotCount>3</burstShotCount>
        <ticksBetweenBurstShots>8</ticksBetweenBurstShots>
        <soundCast>GunShot_AssaultRifle</soundCast>
        <soundCastTail>GunTail_Medium</soundCastTail>
      </li>
    </verbs>
  </ThingDef>
</Defs>
```

#### 示例 2：创建工作台建筑

使用 Probe 查找工作台模板：
```
> def WorkTable
```

创建工作台定义：
```xml
<!-- Defs/ThingDef_MyWorkTable.xml -->
<?xml version="1.0" encoding="utf-8"?>
<Defs>
  <ThingDef ParentName="BenchBase">
    <defName>MyMod_CustomWorkTable</defName>
    <label>custom work table</label>
    <description>A custom work table for crafting.</description>
    <thingClass>Building_WorkTable</thingClass>
    <graphicData>
      <texPath>Things/Building/Production/TableButcher</texPath>
      <graphicClass>Graphic_Multi</graphicClass>
      <drawSize>(3,1)</drawSize>
    </graphicData>
    <statBases>
      <WorkToBuild>1500</WorkToBuild>
      <MaxHitPoints>180</MaxHitPoints>
      <Flammability>1.0</Flammability>
    </statBases>
    <size>(3,1)</size>
    <costList>
      <Steel>100</Steel>
      <WoodLog>50</WoodLog>
    </costList>
    <recipes>
      <li>ButcherCorpseFlesh</li>
    </recipes>
    <inspectorTabs>
      <li>ITab_Bills</li>
    </inspectorTabs>
    <building>
      <spawnedConceptLearnOpportunity>BillsTab</spawnedConceptLearnOpportunity>
    </building>
  </ThingDef>
</Defs>
```

#### 示例 3：创建消耗品

使用 Probe 查找消耗品模板：
```
> def Medicine
```

创建消耗品定义：
```xml
<!-- Defs/ThingDef_MyItem.xml -->
<?xml version="1.0" encoding="utf-8"?>
<Defs>
  <ThingDef ParentName="ResourceBase">
    <defName>MyMod_HealingPotion</defName>
    <label>healing potion</label>
    <description>A magical potion that heals wounds.</description>
    <graphicData>
      <texPath>Things/Item/Resource/Medicine</texPath>
      <graphicClass>Graphic_Single</graphicClass>
    </graphicData>
    <statBases>
      <MaxHitPoints>50</MaxHitPoints>
      <MarketValue>50</MarketValue>
      <Mass>0.2</Mass>
    </statBases>
    <thingCategories>
      <li>Medicine</li>
    </thingCategories>
    <comps>
      <li Class="CompProperties_Usable">
        <useJob>UseItem</useJob>
        <useLabel>Drink {0_label}</useLabel>
      </li>
      <li Class="CompProperties_UseEffect">
        <effectHealing>20</effectHealing>
      </li>
    </comps>
  </ThingDef>
</Defs>
```

#### 示例 4：使用 XML Patch 修改原版内容

使用 Probe 查找要修改的内容：
```
> def LongSword
```

创建 Patch 文件：
```xml
<!-- Patches/Patch_VanillaWeapons.xml -->
<?xml version="1.0" encoding="utf-8"?>
<Patch>
  <!-- 修改原版长剑的伤害 -->
  <Operation Class="PatchOperationReplace">
    <xpath>/Defs/ThingDef[defName="MeleeWeapon_LongSword"]/tools/li[label="blade"]/power</xpath>
    <value>
      <power>35</power>
    </value>
  </Operation>
  
  <!-- 修改制作成本 -->
  <Operation Class="PatchOperationReplace">
    <xpath>/Defs/ThingDef[defName="MeleeWeapon_LongSword"]/costList/Steel</xpath>
    <value>
      <Steel>100</Steel>
    </value>
  </Operation>
</Patch>
```

#### 示例 5：创建 Harmony Patch

使用 Probe 查找 Patch 点：
```
> feature 死亡音效
> recommend 修改死亡音效
```

创建 Harmony Patch：
```csharp
// Source/Patches/PawnDeathPatch.cs
using HarmonyLib;
using RimWorld;
using Verse;

namespace MyMod
{
    [HarmonyPatch(typeof(Pawn), "GetDeathSound")]
    public static class Pawn_GetDeathSound_Patch
    {
        public static void Postfix(Pawn __instance, ref SoundDef __result)
        {
            if (__result != null)
            {
                __result = DefDatabase<SoundDef>.GetNamed("MyCustomDeathSound");
            }
        }
    }
}
```

#### 示例 6：创建条件 Patch（兼容性）

```xml
<!-- Patches/Patch_Compatibility.xml -->
<?xml version="1.0" encoding="utf-8"?>
<Patch>
  <!-- 只有当某个 Mod 存在时才执行 -->
  <Operation Class="PatchOperationFindMod">
    <mods>
      <li>Expanded Prosthetics and Body Engineering</li>
    </mods>
    <match Class="PatchOperationAdd">
      <xpath>/Defs/ThingDef[defName="MyMod_CustomItem"]/comps</xpath>
      <value>
        <li Class="MyMod.CompProperties_ProstheticInteraction"/>
      </value>
    </match>
  </Operation>
</Patch>
```

---

## 6. 常见问题解答

### Q1: 如何找到某个功能的代码位置？

**使用 Probe 的 `feature` 命令：**
```
> feature 死亡音效
```
这会返回与"死亡音效"相关的所有代码位置。

### Q2: 如何知道 XML 标签对应什么字段？

**使用 Probe 的 `xml` 命令：**
```
> xml ThingDef
```
这会显示 ThingDef 的完整 XML 结构和字段映射。

### Q3: Harmony Patch 应该用 Prefix 还是 Postfix？

- **Prefix**：当你需要阻止原方法执行，或在执行前做检查
- **Postfix**：当你需要修改返回值，或在执行后做额外处理

**使用 Probe 获取推荐：**
```
> recommend 修改xxx
```

### Q4: 如何调试 Mod？

1. 使用 `Log.Message()` 输出调试信息到游戏日志
2. 日志文件位于：`RimWorld/Player.log`（Windows）
3. 使用开发者模式快速测试功能

### Q5: Mod 不生效怎么办？

1. 检查 `About.xml` 中的 `supportedVersions` 是否正确
2. 检查 XML 语法是否正确
3. 查看 `Player.log` 中的错误信息
4. 确保 `defName` 唯一且没有重复

### Q6: 如何让 Mod 兼容其他 Mod？

1. 使用 `PatchOperationFindMod` 检测其他 Mod 是否存在
2. 使用 `PatchOperationConditional` 做条件判断
3. 避免硬编码覆盖其他 Mod 的内容

```xml
<Operation Class="PatchOperationFindMod">
  <mods>
    <li>OtherModName</li>
  </mods>
  <match Class="PatchOperationAdd">
    <!-- 只有当 OtherModName 存在时才执行 -->
  </match>
</Operation>
```

### Q7: 如何创建自定义武器？

1. 使用 Probe 查找武器模板：
   ```
   > def LongSword
   > def Gun
   ```
2. 使用向导快速生成：
   ```
   > wizard
   选择 "Weapon Mod Wizard"
   ```
3. 参考示例：
   ```
   > example weapon
   ```

### Q8: 如何创建自定义建筑？

1. 使用 Probe 查找建筑模板：
   ```
   > def WorkTable
   > def Turret
   ```
2. 使用向导快速生成：
   ```
   > wizard
   选择 "Building Mod Wizard"
   ```
3. 参考示例：
   ```
   > example building
   ```

### Q9: 如何创建自定义种族？

创建种族 Mod 比较复杂，需要：
1. 定义 PawnKindDef（角色类型）
2. 定义 FactionDef（派系）
3. 可能需要 C# 代码实现特殊能力

使用向导辅助：
```
> wizard
选择 "Race Mod Wizard"
```

### Q10: 如何创建 XML Patch 修改原版内容？

1. 使用 Probe 查找要修改的内容：
   ```
   > def <名称>
   ```
2. 使用向导生成 Patch：
   ```
   > wizard
   选择 "XML Patch Wizard"
   ```
3. 参考示例：
   ```
   > example patch
   ```

### Q11: Transpiler 和 Prefix/Postfix 有什么区别？

| 类型 | 用途 | 难度 |
|------|------|------|
| **Prefix** | 方法执行前运行，可阻止原方法 | ★★☆ |
| **Postfix** | 方法执行后运行，可修改返回值 | ★★☆ |
| **Transpiler** | 直接修改 IL 代码 | ★★★ |

优先使用 Prefix/Postfix，只有在无法实现时才使用 Transpiler。

### Q12: 如何调试 Harmony Patch？

1. 使用 `Log.Message()` 输出调试信息：
   ```csharp
   Log.Message($"[MyMod] Patch executed for: {__instance.Name}");
   ```
2. 检查 `Player.log` 中的错误信息
3. 使用 Probe 的 `calls` 命令分析方法调用链：
   ```
   > calls <方法名>
   ```

---

## 进阶学习

- **RimWorld Wiki**: https://rimworldwiki.com/wiki/Modding
- **Harmony 文档**: https://harmony.pardeike.net/

---

## 获取帮助

- **RimWorld Mod 制作 Discord**: https://discord.gg/rimworld
- **Steam 创意工坊讨论区**: 在 RimWorld 创意工坊页面提问
- **Probe 工具**: 使用 `help` 命令查看所有可用命令

---

*本指南由 RimWorldModDevProbe v3.0.0 生成*

*最后更新：2026-03-05*
