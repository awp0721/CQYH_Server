# 📦 Database · 引擎基础数据目录

游戏服务器运行时所需的**全部静态数据**：地图、物品、技能、NPC、任务、触发器、CSV 配置等,约 19,000 个文件,导入自 0.6.0 版本配套的「引擎基础版」。

> 此目录是游戏服务器**正常启动的硬性依赖**。缺失会导致大量模板加载失败、玩家无法登录。

---

## 📁 目录结构

```
Database/
└── System/                  ← 所有数据集中在这一层
    ├── Achievements/        # 成就模板 (.json / .txt)
    ├── Envir/               # 地图环境数据 (地图 .map、刷怪点、传送点、Mon*.txt)
    ├── lua/                 # 服务器侧 Lua 脚本 (NLua 加载)
    ├── Npc数据/              # NPC 对话与触发脚本
    ├── Quests/              # 任务定义
    ├── 技能数据/              # 技能模板
    ├── 游戏地图/              # 地图属性表
    ├── 游戏坐骑/              # 坐骑配置
    ├── 物品数据/              # 物品模板
    ├── 龙卫数据/              # 龙卫宠物数据
    └── *.csv / *.txt / *.json   # 各类全局配置表
                              # (掉落分组、装备升级、月卡奖励、玩家升级经验 等)
```

---

## 📍 放置位置(部署/运行时)

### 默认约定:仓库根目录

[游戏服务器/Settings.cs:49](../游戏服务器/Settings.cs#L49) 默认配置:

```csharp
public static string 游戏数据目录 = ".\\Database";
```

这是一个**相对于游戏服务器进程当前工作目录**的路径,所以:

| 场景 | 工作目录应在哪 | `Database/` 应放在哪 |
|------|--------------|--------------------|
| Visual Studio 调试 | 默认是 `游戏服务器\bin\Debug\net8.0-windows\` | **复制 / 软链 `Database\` 到该输出目录**,或改工作目录为仓库根 |
| `dotnet run` 启动 | 仓库根 `YH_Server_Code\` | 仓库根(本仓库默认布局,直接可用) |
| 发布部署 | `游戏服务器.exe` 同级 | 与 `.exe` 同级,即 `<部署目录>\Database\` |

### 改路径:三种方式

1. **修改 `Settings.cs:49`**:把 `".\\Database"` 改成绝对路径或其他相对路径,重新编译。
2. **运行时 UI 修改**:游戏服务器主窗口里有「游戏数据目录」文本框([主窗口.cs:3359](../游戏服务器/主窗口.cs#L3359)),修改后保存配置。
3. **符号链接**(推荐用于多实例共享):

   ```powershell
   # 以管理员 PowerShell 执行
   New-Item -ItemType SymbolicLink -Path "C:\Servers\Inst1\Database" -Target "E:\YH_Server_Code\Database"
   ```

---

## 🔗 代码访问模式

服务器代码统一通过 `Settings.游戏数据目录` 拼接子路径,几个典型例子:

| 模块 | 访问路径 | 源文件 |
|------|---------|--------|
| 地图环境 | `Database\System\Envir\` | [Settings.cs:353](../游戏服务器/Settings.cs#L353) |
| 成就模板 | `Database\System\Achievements\` | [模板类/GameAchievements.cs:37](../游戏服务器/模板类/GameAchievements.cs#L37) |
| 任务模板 | `Database\System\Quests\` | [模板类/GameQuests.cs:75](../游戏服务器/模板类/GameQuests.cs#L75) |
| 杀怪成就 | `Database\System\杀怪成就.csv` | [数据类/杀怪成就.cs:38](../游戏服务器/数据类/杀怪成就.cs#L38) |
| 紧急任务 | `Database\System\紧急任务.txt` | [模板类/GameQuests.cs:93](../游戏服务器/模板类/GameQuests.cs#L93) |

---

## ⚠️ 注意事项

- **路径分隔符**:代码使用 Windows 风格 `\\`,跨平台运行需自行替换为 `Path.Combine`。
- **编码**:CSV / TXT 普遍为 **GBK / GB2312**,直接用 UTF-8 编辑器打开会乱码;推荐 VS Code + `GBK Encoding Support` 插件,或 Notepad++。
- **不建议直接 Git 跟踪二进制大文件**:`Envir/*.map` 体积较大,如需精简仓库可参考 [.gitattributes](../.gitattributes) 配合 LFS。
- **数据归属**:本目录数据来自公开流传的「引擎基础版」,仅供学习研究,**不得用于商业运营**,详见根目录 [LICENSE](../LICENSE) 与 [README.md](../README.md#️-免责声明)。

---

## 📜 相关变更

- **v0.6.0 (2026-05-27)**:首次将完整 `Database/` 引入仓库(此前仓库仅含代码,数据需自备),详见 [CHANGELOG.md](../CHANGELOG.md)。
