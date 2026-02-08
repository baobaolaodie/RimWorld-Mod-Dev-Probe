# RimWorld API Probe

![License](https://img.shields.io/badge/license-MIT-blue.svg)

**RimWorld API Probe** 是一个轻量级的 C# 控制台工具，用于通过反射（Reflection）技术动态探测 RimWorld 游戏 DLL（`Assembly-CSharp.dll` 等）的内部结构。

## 🎯 背景与痛点

随着 RimWorld 版本的更新（如从 1.4 升级到 1.5，或引入新的 DLC），许多核心类的 API（类名、方法签名、属性）会发生变化。Mod 开发者经常面临以下问题：
-   某个类还在吗？
-   `MakeDropPodAt` 方法的参数变了吗？
-   这个属性现在是公有的还是私有的？

如果没有反编译工具（如 dnSpy）或最新的源代码，开发者很难知晓这些变更。本工具允许你直接“询问”DLL 当前的 API 结构，无需打开重量级的反编译器。

## ✨ 功能特点

-   **🔍 交互式查询**：支持 CLI 交互，随时输入类名进行查询。
-   **📂 自动加载**：自动在 `../GameDll/` 或 `../../GameDll/` 等相对路径下寻找并加载 `Assembly-CSharp.dll` 和 `UnityEngine.dll`。
-   **🔎 模糊搜索**：支持通过类名的一部分（如 "DropPod"）查找所有相关类型。
-   **📑 详细报告**：展示目标类型的所有字段（Fields）、属性（Properties）和方法（Methods），包括参数类型和返回值。
-   **🚀 轻量级**：无需安装复杂环境，基于 .NET Framework (兼容 Mono) 运行。

## 📁 目录结构

本工具假设您的工作区结构如下（推荐）：

```text
Workspace/
├── GameDll/                 <-- 存放从游戏目录复制来的 DLL (Assembly-CSharp.dll, UnityEngine.dll 等)
└── RimWorldApiProbe/        <-- 本工具目录
    ├── Program.cs
    ├── RimWorldApiProbe.csproj
    └── ...
```

> **注意**：程序会自动向上级目录搜索 `GameDll` 文件夹。请确保你已经将游戏 `RimWorldWin64_Data/Managed` 目录下的核心 DLL 复制到了 `GameDll` 文件夹中。

## 🛠️ 构建与运行

### 前置要求
-   [.NET SDK](https://dotnet.microsoft.com/download) (推荐安装 .NET Desktop Runtime 或 .NET Developer Pack)

### 运行步骤

1.  **克隆或下载本项目**。
2.  **准备游戏 DLL**：
    将 RimWorld 游戏目录（通常在 Steam 库的 `RimWorld/RimWorldWin64_Data/Managed`）下的所有 DLL 复制到与项目同级的 `GameDll` 目录中。
3.  **运行**：
    在终端中进入项目目录并运行：
    ```bash
    dotnet run
    ```

## 📖 使用示例

```text
Initializing RimWorld API Probe...
Loading assemblies from: D:\Workspace\GameDll
Ready. Loaded 15 assemblies.

RimWorld API Probe Ready.
Type 'exit' or 'quit' to close.
Enter a class name (e.g., 'Pawn', 'Tile') to search.

> DropPodUtility

Found 1 types:
[0] RimWorld.DropPodUtility (Assembly-CSharp)

--- Inspecting: RimWorld.DropPodUtility ---
Assembly: Assembly-CSharp
BaseType: Object

[Methods (Public Static/Instance)]
  Void MakeDropPodAt(IntVec3 c, Map map, ActiveTransporterInfo info, Faction faction)
  ...
```

## 📄 许可证

本项目基于 MIT 许可证开源。详情请参阅 [LICENSE](LICENSE) 文件。
