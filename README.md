# RimWorld API Probe

![License](https://img.shields.io/badge/license-MIT-blue.svg)

**RimWorld API Probe** 是一个轻量级的 C# 控制台工具，用于通过反射（Reflection）技术动态探测 RimWorld 游戏 DLL（`Assembly-CSharp.dll` 等）的内部结构。

## 开发背景

随着 RimWorld 版本的更新（如从 1.5 升级到 1.6 或引入新的 DLC），许多核心类的 API（类名、方法签名、属性）会发生变化。例如：
-   `ActiveDropPodInfo` 更名为 `ActiveTransporterInfo`。
-   `DropPodUtility.MakeDropPodAt` 增加了 `Faction` 参数。
-   `Tile.biome` 属性变更为 `Tile.PrimaryBiome`。

如果没有反编译工具或最新的源代码，开发者很难知晓这些具体的变更。本工具旨在解决这一问题，允许开发者直接“询问”DLL 当前的 API 结构。

## 功能特点

-   **自动加载**：自动加载 `GameDll` 目录下的 `Assembly-CSharp.dll` 和 `UnityEngine.dll`。
-   **自动化探测**：支持在代码中硬编码探测逻辑（如 `SearchTypes("Vacuum")`），适合批量检查。
-   **模糊搜索**：支持通过类名的一部分（如 "DropPod"）查找所有相关类型。
-   **详细探查**：展示目标类型的所有字段（Fields）、属性（Properties）和方法（Methods），包括其参数类型和返回值。
-   **无需反编译**：直接运行即可查看结果，无需等待漫长的反编译过程。

## 目录结构与依赖

本工具假设您的工作区结构如下：

```text
Workspace/
├── GameDll/                 <-- 存放游戏 DLL (Assembly-CSharp.dll 等)
└── RimWorldApiProbe/        <-- 本工具目录
    ├── Program.cs
    ├── RimWorldApiProbe.csproj
    └── ...
```

> **注意**：程序会自动向上级目录搜索 `GameDll` 文件夹。请确保你已经将游戏 `RimWorldWin64_Data/Managed` 目录下的核心 DLL 复制到了 `GameDll` 文件夹中。

## 使用方法

1.  **环境准备**：确保安装了 .NET SDK（建议 .NET 4.7.2 或兼容版本）。
2.  **运行工具**：
    打开终端，进入项目目录并运行：
    ```bash
    cd RimWorldApiProbe
    dotnet run
    ```
3.  **交互指令**：
    -   **输入类名**：输入您想查询的类名（例如 `Tile` 或 `Pawn`）。
    -   **选择类型**：如果搜索结果有多个，输入对应的数字 ID 进行选择。
    -   **查看结果**：工具将打印该类的所有成员信息。
    -   **退出**：输入 `exit` 退出程序。

## 示例输出

```text
Enter class name to search: DropPodUtility

Found 1 types:
[0] RimWorld.DropPodUtility (Assembly-CSharp)

--- Inspecting: RimWorld.DropPodUtility ---
Methods (Public Static/Instance):
  Void MakeDropPodAt(IntVec3 c, Map map, ActiveTransporterInfo info, Faction faction)
  ...
```

通过上述输出，我们可以清楚地看到 `MakeDropPodAt` 方法现在需要 4 个参数，从而指导我们正确编写 Mod 代码。

## 许可证
本项目基于 MIT 许可证开源。详情请参阅 [LICENSE](LICENSE) 文件。
