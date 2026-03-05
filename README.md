# RimWorld Mod Dev Probe

![License](https://img.shields.io/badge/license-MIT-blue.svg)

轻量级 C# 控制台工具，用于探测和分析 RimWorld 游戏资源，辅助 Mod 开发。

**版本**: v3.0.0  
**作者**: LongYinHaHa 
**许可证**: MIT

---

## 功能概述

RimWorld Mod Dev Probe 提供多种探测模式和开发辅助功能，帮助开发者快速了解游戏结构和完成 Mod 开发：

### 基础探测模式

| 模式 | 命令 | 功能 |
|------|------|------|
| **DLL** | `dll` | 探测游戏和 Mod 的类型、方法、字段 |
| **Def** | `def` | 探测 XML Defs 定义 |
| **Patch** | `patch` | 探测 XML Patch Operations |
| **Harmony** | `harmony` | 探测 Harmony Patch 注入点 |
| **Mod** | `mod` | 探测 Mod 资源信息 |

### 高级分析命令

| 命令 | 功能 |
|------|------|
| `calls <method>` | 分析方法调用链（谁调用了它、它调用了谁） |
| `usage <field>` | 分析字段使用位置（读/写位置） |
| `xml <type>` | 显示类型的 XML 结构示例 |

### 智能推荐命令

| 命令 | 功能 |
|------|------|
| `feature <keyword>` | 功能关键词搜索（如"死亡音效"） |
| `recommend <feature>` | 获取 Patch 点推荐 |
| `relate <type>` | 显示相关资源推荐 |

### 开发向导命令

| 命令 | 功能 |
|------|------|
| `wizard` | 启动交互式开发向导 |
| `example [key]` | 查看代码示例 |
| `example category <type>` | 按分类查看示例 |

**示例分类类型：**
- `weapon` - 武器相关示例（近战/远程）
- `building` - 建筑相关示例（工作台/防御）
- `item` - 物品相关示例（消耗品/资源）
- `apparel` - 服装相关示例
- `animal` - 动物相关示例
- `patch` - XML Patch 示例
- `harmony` - Harmony Patch 示例
- `sound` - 音效相关示例
- `race` - 种族相关示例
- `incident` - 事件相关示例

---

## 快速开始

### 环境要求
- .NET SDK 4.7.2 或兼容版本

### 安装配置

1. 将 RimWorld 的 DLL 复制到 `GameDll` 目录：
   - `Assembly-CSharp.dll` (必需)
   - `0Harmony.dll` (可选，Harmony 探测需要)
   - 可以再根据需要放置DLL

2. 首次运行会自动生成 `config.json`：
   ```bash
   dotnet run
   ```

3. 编辑 `config.json` 设置游戏路径：
   ```json
   {
     "gamePath": "D:/SteamLibrary/steamapps/common/RimWorld",
     "modsPath": ""
   }
   ```
  tips: 建议 `modsPath` 设置不要设置创意工坊的地址，以节省探测成本；比如: 
  ```bash
  "modsPath": "D:/SteamLibrary/steamapps/common/RimWorld/Mods"
  ```

### 运行
```bash
dotnet run
```

---

## 使用指南

### 命令总览

#### 模式切换命令

| 命令 | 说明 |
|------|------|
| `mode <type>` | 切换搜索模式（dll/def/patch/harmony/mod） |
| `dll [query]` | 切换到 DLL 模式 / 搜索类型 |
| `def [query]` | 切换到 Def 模式 / 搜索 Defs |
| `patch [query]` | 切换到 Patch 模式 / 搜索 XML Patches |
| `harmony [query]` | 切换到 Harmony 模式 / 搜索 Harmony Patches |
| `mod [query]` | 切换到 Mod 模式 / 搜索 Mods |

#### 搜索命令

| 命令 | 说明 |
|------|------|
| `type <query>` | 搜索类型名称 |
| `method <query>` | 搜索方法名称 |
| `field <query>` | 搜索字段名称 |
| `inherit <type>` | 显示类型继承链 |

#### 分析命令

| 命令 | 说明 |
|------|------|
| `calls <method>` | 分析方法调用链（调用者/被调用者） |
| `usage <field>` | 分析字段使用位置（读/写位置） |
| `xml <type>` | 显示类型的 XML 结构示例 |

#### 推荐命令

| 命令 | 说明 |
|------|------|
| `feature <keyword>` | 功能关键词搜索（如"死亡音效"） |
| `recommend <feature>` | 获取 Patch 点推荐 |
| `relate <type>` | 显示相关资源推荐 |

#### 向导命令

| 命令 | 说明 |
|------|------|
| `wizard` | 启动交互式开发向导 |
| `example [key]` | 查看代码示例 |
| `example category <type>` | 按分类查看示例 |

#### 工具命令

| 命令 | 说明 |
|------|------|
| `types` | 列出所有 DefTypes |
| `mods` | 列出所有 Mods |
| `info` | 显示系统信息 |
| `clear` | 清除缓存 |
| `help` | 显示帮助信息 |
| `exit` | 退出程序 |

### 基础命令示例

#### 类型搜索
```
> dll Pawn

Switched to dll mode.

Found 100 results:
  [0] PawnTags (Class) - Assembly-CSharp
  [1] PawnStagePosition (Class) - Assembly-CSharp
  [2] PawnRitualReference (Class) - Assembly-CSharp
  [3] PawnRenderTreeDef (Class) - Assembly-CSharp
  [4] PawnWaterRippleMaker (Class) - Assembly-CSharp
  [5] PawnPathPool (Class) - Assembly-CSharp
  ...
  [19] PawnKindDef (Class) - Assembly-CSharp
  ... and 80 more.

Enter number to view details (or press Enter to skip):
```

#### 方法搜索
```
> method Die

Found 100 methods:
  [0] AddColorGradientPreset() - UnityEngine.TextCoreTextEngineModule
  [1] TryGetColorGradientPreset() - UnityEngine.TextCoreTextEngineModule
  ...
  [17] PawnDied() - Assembly-CSharp
  [18] PawnDied() - Assembly-CSharp
  [19] PawnDied() - Assembly-CSharp
  ... and 80 more.

Enter number to view details (or press Enter to skip):
```

#### 字段搜索
```
> field health

Found 56 fields:
  [0] m_HealthyColor - UnityEngine.TerrainModule
  [1] DefaultHealthColor - UnityEngine.TerrainModule
  [2] fleeHealthThresholdRange - Assembly-CSharp
  [3] gearHealthRange - Assembly-CSharp
  [4] harmsHealth - Assembly-CSharp
  [5] baseHealthScale - Assembly-CSharp
  ...
  [15] health - Assembly-CSharp
  [16] healthState - Assembly-CSharp
  [17] summaryHealth - Assembly-CSharp
  ... and 36 more.

Enter number to view details (or press Enter to skip):
```

#### 继承链查询
```
> inherit Pawn

Inheritance chain for Pawn:
Pawn →
  ThingWithComps →
    Thing →
      Entity →
        Object
```

#### XML 结构显示
```
> xml SoundDef

=== XML Structure for SoundDef ===
C# Type: Verse.SoundDef
Def Type: SoundDef

--- XML Template ---
<SoundDef>
  <defName>ExampleDefName</defName>
  <label>An Example Label</label>
  <description>A description of this def.</description>
  <sustain>true</sustain>
  <context>Any</context>
  <eventNames>
    <li>SomeText</li>
  </eventNames>
  <maxVoices>0</maxVoices>
  <maxSimultaneous>0</maxSimultaneous>
  ...
</SoundDef>

--- Field Mappings ---
  label -> label (String)
  description -> description (String)
  sustain -> sustain (Boolean)
  context -> context (SoundContext)
  eventNames -> eventNames (List<String>)
  ...
```

#### 功能搜索
```
> feature 死亡音效

Found 3 recommendations:

  [0] Pawn.Die (Method)
      处理 Pawn 死亡逻辑的主要方法
      Matched: 死亡音效 (Score: 100)
      
  [1] SoundDef (Class)
      SoundDef 用于定义游戏中的音效资源
      Matched: 死亡音效 (Score: 100)
      
  [2] Pawn.health (Field)
      Pawn.health 属性包含角色的健康状态信息
      Matched: 死亡音效 (Score: 100)
```

#### 相关资源推荐
```
> relate Pawn

=== Related Resources for Pawn ===

--- DirectlyRelated (8) ---

  [SoundDef] (Priority: 10)
    Reason: Pawn-related sound effects

  [JobDef] (Priority: 10)
    Reason: Jobs that pawns can perform

  [HediffDef] (Priority: 9)
    Reason: Health conditions and injuries

  [NeedDef] (Priority: 9)
    Reason: Pawn needs and motivations

  [SkillDef] (Priority: 8)
    Reason: Pawn skills and abilities

  ...

--- CommonlyUsedTogether (4) ---

  [ThoughtDef] (Priority: 8)
    Reason: Pawn thoughts and moods

  [TraitDef] (Priority: 8)
    Reason: Pawn traits and personalities

  ...
```

### 开发向导

#### 查看示例
```
> example

= 示例库 (21 个示例) =

--- XML Def 示例 ---
[1] 近战武器定义示例
    功能: 武器定义
    文件数: 2
    步骤数: 5

[2] 远程武器定义示例
    功能: 武器定义
    文件数: 3
    步骤数: 6

[3] 工作台建筑定义示例
    功能: 建筑定义
    文件数: 2
    步骤数: 7

[4] 防御建筑定义示例
    功能: 建筑定义
    文件数: 2
    步骤数: 6

[5] 消耗品定义示例
    功能: 物品定义
    文件数: 2
    步骤数: 5

[6] 资源物品定义示例
    功能: 物品定义
    文件数: 2
    步骤数: 4

[7] 服装定义示例
    功能: 服装定义
    文件数: 2
    步骤数: 6

[8] 动物定义示例
    功能: 动物定义
    文件数: 3
    步骤数: 8

--- XML Patch 示例 ---
[9] 修改属性示例
    功能: XML Patch
    文件数: 1
    步骤数: 3

[10] 修改成本示例
    功能: XML Patch
    文件数: 1
    步骤数: 3

[11] 添加功能示例
    功能: XML Patch
    文件数: 1
    步骤数: 4

[12] 条件 Patch 示例
    功能: XML Patch
    文件数: 1
    步骤数: 5

--- Harmony Patch 示例 ---
[13] Prefix Patch 示例
    功能: Harmony Patch
    文件数: 2
    步骤数: 6

[14] Postfix Patch 示例
    功能: Harmony Patch
    文件数: 2
    步骤数: 5

[15] Transpiler Patch 示例
    功能: Harmony Patch
    文件数: 2
    步骤数: 8

--- 种族与事件示例 ---
[16] 种族定义示例
    功能: 种族定义
    文件数: 4
    步骤数: 10

[17] 种族能力示例
    功能: 种族能力
    文件数: 3
    步骤数: 8

[18] 事件定义示例
    功能: 事件定义
    文件数: 2
    步骤数: 6

--- 音效示例 ---
[19] 死亡音效修改示例
    功能: 音效修改
    文件数: 2
    步骤数: 5

[20] 受伤音效修改示例
    功能: 音效修改
    文件数: 2
    步骤数: 5

[21] 自定义音效示例
    功能: 音效定义
    文件数: 3
    步骤数: 7

使用 'example <名称>' 查看详细示例
可用关键词: weapon, building, item, apparel, animal, patch, harmony, sound, race, incident, 武器, 建筑, 物品, 服装, 动物, 补丁, 音效, 种族, 事件
```

#### 启动向导
```
> wizard

=== RimWorld Mod Development Wizard ===

Select wizard type:
  [0] Weapon Mod Wizard - 武器 Mod 向导
  [1] Building Mod Wizard - 建筑 Mod 向导
  [2] Race Mod Wizard - 种族 Mod 向导
  [3] XML Patch Wizard - XML Patch 向导
  [4] Sound Mod Wizard - 音效修改向导
  [5] Harmony Patch Wizard - Harmony Patch 向导
  [6] Exit

Enter selection: 
```

**向导类型说明：**

| 向导 | 功能 | 适用场景 |
|------|------|----------|
| **Weapon Mod Wizard** | 创建武器 Mod | 近战武器、远程武器、特殊武器 |
| **Building Mod Wizard** | 创建建筑 Mod | 工作台、防御建筑、装饰建筑 |
| **Race Mod Wizard** | 创建种族 Mod | 自定义种族、特殊能力种族 |
| **XML Patch Wizard** | 创建 XML 补丁 | 修改原版内容、兼容性补丁 |
| **Sound Mod Wizard** | 修改音效 | 死亡音效、受伤音效、环境音效 |
| **Harmony Patch Wizard** | 创建 Harmony Patch | 修改游戏逻辑、复杂功能 |

---

## 目录结构

```
RimWorldModDevProbe/
├── config.json                  # 配置文件 (首次运行自动生成)
├── GameDll/                     # 游戏 DLL 目录
│   ├── Assembly-CSharp.dll      # 游戏核心程序集
│   ├── UnityEngine.dll          # Unity 引擎
│   └── 0Harmony.dll             # Harmony 库 (可选)
│
├── Core/                        # 核心模块
│   ├── IProbe.cs                # 探测器接口
│   ├── ProbeContext.cs          # 探测上下文
│   ├── ProbeConfig.cs           # 配置管理
│   ├── ProbeResult.cs           # 探测结果基类
│   ├── SearchOptions.cs         # 搜索选项
│   ├── CurrentMode.cs           # 当前模式
│   └── ServiceContainer.cs      # 服务容器
│
├── Commands/                    # 命令系统
│   ├── ICommand.cs              # 命令接口
│   ├── CommandBase.cs           # 命令基类
│   ├── CommandRegistry.cs       # 命令注册表
│   └── Commands/                # 具体命令实现 (19个命令类)
│       ├── ModeCommand.cs       # 模式切换命令 (dll/def/patch/harmony/mod)
│       ├── TypeCommand.cs       # 类型搜索
│       ├── MethodCommand.cs     # 方法搜索
│       ├── FieldCommand.cs      # 字段搜索
│       ├── InheritCommand.cs    # 继承链查询
│       ├── CallsCommand.cs      # 调用链分析
│       ├── UsageCommand.cs      # 字段使用分析
│       ├── XmlCommand.cs        # XML 结构显示
│       ├── FeatureCommand.cs    # 功能关键词搜索
│       ├── RecommendCommand.cs  # Patch 推荐
│       ├── RelateCommand.cs     # 相关资源推荐
│       ├── WizardCommand.cs     # 开发向导
│       ├── ExampleCommand.cs    # 示例查看
│       ├── TypesCommand.cs      # 列出 DefTypes
│       ├── ModsCommand.cs       # 列出 Mods
│       ├── InfoCommand.cs       # 系统信息
│       ├── ClearCommand.cs      # 清除缓存
│       ├── HelpCommand.cs       # 帮助信息
│       └── SearchCommand.cs     # 搜索命令
│
├── Probes/                      # 探测器模块
│   ├── DllProbe.cs              # DLL 探测器
│   ├── DefsProbe.cs             # Defs 探测器
│   ├── PatchProbe.cs            # XML Patch 探测器
│   ├── HarmonyProbe.cs          # Harmony Patch 探测器
│   └── ModProbe.cs              # Mod 探测器
│
├── Analysis/                    # 分析器模块
│   ├── CallChainAnalyzer.cs     # 调用链分析器
│   ├── CallChainResult.cs       # 调用链结果
│   ├── FieldUsageAnalyzer.cs    # 字段使用分析器
│   ├── FieldUsageResult.cs      # 字段使用结果
│   ├── TypeDefMapper.cs         # 类型-Def 映射器
│   ├── FeatureKeywordMap.cs     # 功能关键词映射
│   ├── PatchRecommender.cs      # Patch 点推荐器
│   └── ResourceRecommender.cs   # 资源推荐器
│
├── Wizards/                     # 向导系统
│   └── Core/                    # 向导核心
│       ├── IWizardStep.cs       # 向导步骤接口
│       ├── DevWizard.cs         # 向导基类
│       ├── WizardContext.cs     # 向导上下文
│       ├── WizardResult.cs      # 向导结果
│       ├── WizardStepBase.cs    # 向导步骤基类
│       ├── WizardExceptions.cs  # 向导异常
│       └── ConsoleHelper.cs     # 控制台辅助
│
├── Examples/                    # 示例库
│   ├── Example.cs               # 示例基类
│   ├── ExampleFile.cs           # 示例文件类
│   └── Examples/                # 分类示例 (8个分类)
│       ├── WeaponExamples.cs    # 武器示例
│       ├── BuildingExamples.cs  # 建筑示例
│       ├── ConsumableExamples.cs # 消耗品示例
│       ├── RaceExamples.cs      # 种族示例
│       ├── SoundExamples.cs     # 音效示例
│       ├── PatchExamples.cs     # XML Patch 示例
│       ├── HarmonyExamples.cs   # Harmony Patch 示例
│       └── IncidentExamples.cs  # 事件示例
│
├── Utils/                       # 工具类
│   ├── CodeGenerator.cs         # 代码生成器
│   ├── CodeValidator.cs         # 代码验证器
│   ├── ConsoleHelper.cs         # 控制台辅助
│   └── IlHelper.cs              # IL 辅助工具
│
├── Program.cs                   # 主程序入口
├── CommandRouter.cs             # 命令路由器
├── ExampleLibrary.cs            # 示例库管理
├── StringExtensions.cs          # 字符串扩展
├── HarmonyPatchWizard.cs        # Harmony Patch 向导
├── SoundModWizard.cs            # 音效修改向导
├── WeaponModWizard.cs           # 武器 Mod 向导
├── BuildingModWizard.cs         # 建筑 Mod 向导
├── RaceModWizard.cs             # 种族 Mod 向导
├── XmlPatchWizard.cs            # XML Patch 向导
└── RimWorldModDevProbe.csproj   # 项目文件
```

---

## 技术特点

- **模块化架构**: 每种探测器实现统一接口，易于扩展
- **懒加载**: 延迟解析详细数据，快速启动
- **缓存机制**: 避免重复解析，提升响应速度
- **并行处理**: 多核并行扫描，加速大数据处理
- **配置灵活**: 通过 config.json 索引外部目录，无需复制大文件
- **版本号支持**: 自动识别 Mod 版本号文件夹（如 `1.6/Assemblies/`）
- **智能推荐**: 基于功能描述推荐 Patch 点
- **交互向导**: 引导完成常见 Mod 开发任务
- **代码验证**: 自动检查生成的代码是否符合规范

---

## 架构说明

### 整体架构

```
┌─────────────────────────────────────────────────────────────────┐
│                        Program.cs                                │
│                      (程序入口点)                                 │
└───────────────────────────┬─────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────────┐
│                    CommandRouter                                 │
│                    (命令路由器)                                   │
└───────────────────────────┬─────────────────────────────────────┘
                            │
            ┌───────────────┼───────────────┐
            │               │               │
            ▼               ▼               ▼
┌───────────────┐  ┌───────────────┐  ┌───────────────┐
│ CommandRegistry│  │ ProbeContext  │  │ServiceContainer│
│  (命令注册表)  │  │ (探测上下文)   │  │  (服务容器)    │
└───────┬───────┘  └───────┬───────┘  └───────┬───────┘
        │                  │                  │
        ▼                  ▼                  ▼
┌───────────────┐  ┌───────────────┐  ┌───────────────┐
│   Commands    │  │    Probes     │  │   Analysis    │
│  (命令系统)   │  │  (探测器模块)  │  │  (分析器模块)  │
└───────────────┘  └───────────────┘  └───────────────┘
```

### 核心模块说明

#### 1. Core 模块

| 组件 | 职责 |
|------|------|
| `ProbeContext` | 管理探测上下文，包括路径配置、程序集加载、缓存管理 |
| `ServiceContainer` | 依赖注入容器，管理服务生命周期 |
| `IProbe` | 探测器接口，定义统一的探测行为 |
| `ProbeResult` | 探测结果基类，提供结果格式化 |

#### 2. Commands 模块

采用命令模式，每个命令实现 `ICommand` 接口：

```csharp
public interface ICommand
{
    string Name { get; }
    string Description { get; }
    void Execute(string[] args);
}
```

命令分类：
- **模式命令**: `mode`, `dll`, `def`, `patch`, `harmony`, `mod`
- **搜索命令**: `search`, `type`, `method`, `field`, `inherit`
- **分析命令**: `calls`, `usage`, `xml`
- **推荐命令**: `feature`, `recommend`, `relate`
- **向导命令**: `wizard`, `example`
- **工具命令**: `types`, `mods`, `info`, `clear`, `help`

#### 3. Probes 模块

每种探测器负责特定类型的数据探测：

| 探测器 | 数据源 | 功能 |
|--------|--------|------|
| `DllProbe` | 游戏程序集 | 类型、方法、字段搜索 |
| `DefsProbe` | XML Defs 文件 | Def 定义搜索 |
| `PatchProbe` | XML Patch 文件 | Patch 操作搜索 |
| `HarmonyProbe` | Mod 程序集 | Harmony Patch 搜索 |
| `ModProbe` | Mod 目录 | Mod 信息搜索 |

#### 4. Analysis 模块

提供高级分析功能：

| 分析器 | 功能 |
|--------|------|
| `CallChainAnalyzer` | 分析方法调用链（调用者/被调用者） |
| `FieldUsageAnalyzer` | 分析字段读写位置 |
| `TypeDefMapper` | 映射 C# 类型到 XML Def 类型 |
| `FeatureKeywordMap` | 功能关键词到代码位置的映射 |
| `PatchRecommender` | 基于 Harmony 最佳实践推荐 Patch 点 |
| `ResourceRecommender` | 推荐相关资源 |

#### 5. Wizards 模块

交互式开发向导系统：

```
向导流程:
WelcomeStep → FeatureDescriptionStep → PatchTypeSelectionStep
    → TargetRecommendationStep → TargetConfirmationStep
    → CodeGenerationStep → RegistrationGuideStep
```

### 数据流

```
用户输入 → CommandRouter → CommandRegistry → ICommand.Execute()
                                              ↓
                              ProbeContext (加载程序集/缓存)
                                              ↓
                              Probe (执行探测逻辑)
                                              ↓
                              Analysis (分析/推荐)
                                              ↓
                              输出结果
```

### 扩展指南

**添加新命令：**

1. 在 `Commands/Commands/` 创建新命令类
2. 继承 `CommandBase` 或实现 `ICommand`
3. 命令会自动被 `CommandRegistry` 注册

**添加新探测器：**

1. 在 `Probes/` 创建新探测器类
2. 实现 `IProbe` 接口
3. 在 `Program.cs` 中注册探测器

**添加新向导：**

1. 在根目录创建向导类
2. 在 `WizardCommand.cs` 中注册

---

## 更新日志

### v3.0.0 (2026-03-05)
- **新增**: `calls` 命令 - 方法调用链分析
- **新增**: `usage` 命令 - 字段使用位置分析
- **新增**: `xml` 命令 - 类型 XML 结构显示
- **新增**: `feature` 命令 - 功能关键词搜索
- **新增**: `recommend` 命令 - Patch 点推荐
- **新增**: `relate` 命令 - 相关资源推荐
- **新增**: `wizard` 命令 - 交互式开发向导
- **新增**: `example` 命令 - 代码示例查看
- **新增**: 音效修改向导 (SoundModWizard)
- **新增**: Harmony Patch 向导 (HarmonyPatchWizard)
- **新增**: 代码验证系统 (CodeValidator)
- **新增**: 示例库 (ExampleLibrary)
- **增强**: 支持中英文双语关键词搜索
- **增强**: 生成的代码包含完整 using 语句

### v2.2.0 (2026-03-04)
- **新增**: `method` 命令 - 搜索方法名
- **新增**: `field` 命令 - 搜索字段名
- **新增**: `inherit` 命令 - 显示类型继承链
- **新增**: `type` 命令 - 类型搜索（同 dll）
- **增强**: 类型详情显示基类和接口信息

### v2.1.1 (2026-03-04)
- **新增**: 版本号文件夹支持（如 `ModName/1.6/Assemblies/`）
- **文档**: 添加 Harmony 探测示例输出
- **文档**: 说明 0Harmony.dll 依赖

### v2.1.0 (2026-03-04)
- **重命名**: 项目名称从 RimWorldApiProbe 改为 RimWorldModDevProbe
- **新增**: 配置文件支持 (config.json)
- **新增**: Mod DLL 加载，Harmony 探测支持 Mod Patch
- **优化**: README 重构

### v2.0.0 (2026-03-04)
- **重大更新**: 模块化架构重构
- **新增**: Defs 探测功能
- **新增**: XML Patch 探测功能
- **新增**: Harmony Patch 探测功能
- **新增**: Mod 探测功能
- **优化**: 懒加载、缓存、并行处理

### v1.0.1 (2026-02-14)
- 支持探测 NonPublic 成员
- 辅助 Harmony 补丁开发

### v1.0.0 (2026-02-09)
- 初始发布
- 基础类型搜索和成员探测

---

## 许可证

MIT License - 详见 [LICENSE](LICENSE) 文件
