# 更新日志 / Changelog

本仓库遵循 [Keep a Changelog](https://keepachangelog.com/zh-CN/1.1.0/) 规范，日期格式 `YYYY-MM-DD`。

---

## [Unreleased]

- 补齐 0.6.0 漏改的 SMain(新主窗口)品牌:`（传奇永恒）游戏服务端 - 游戏区名称：` → `Elaina Engine - 游戏区名称：`
- 服务器启动时在系统日志输出 banner:`Elaina Engine (伊蕾娜引擎)` + `源码仓库: https://github.com/awp0721/CQYH_Server`
  - 注意:`账号服务器/主窗口.cs` 中默认 Server.txt 的 `/传奇永恒` 分组名暂未修改,因其影响客户端登录时的分组匹配,改名需配合客户端预设同步
- 移除游戏服务器中残留的"软件授权"机制
  - 删除 `游戏服务器/LicenseTool/`(`LicenseInfo.cs` + `LicenseLoader.cs`,含硬编码 RSA 私钥 + Win32_Processor 机器码方案)
  - 清理 `Program.cs` 启动时早已注释的 LicenseLoader 死代码块,简化 Main 入口
  - 移除 `主窗口.加载系统数据()` 里"授权状态" / "本机机器码"系统日志输出
  - 重命名遗留的 `S_软件授权分组` 控件 → `S_充值密钥分组`(Text 在更早提交里已改为"充值平台密钥")
  - 删除 `SMain.cs` / `主窗口.cs` 顶部的 `//using LicenseTool;` 注释残留
- 规范化 `游戏服务器/` 根目录散落文件,统一按"\*\*类 / \*\*窗口"风格归类
  - 新建 `主程类/`:收纳 6 个 `主程.*.cs` 分部类
  - 新建 `启动窗口/`:收纳 `SMain` / `SMainR` / `主窗口` 三套窗体文件(含 `.Designer.cs` / `.resx`)
  - 合并 `AStar/` + `AStarPathing/` → `寻路类/`(10 个寻路相关文件)
  - 保留 `Program.cs` / `Settings.cs` / `GlobalUsings.cs` / `App.config` / `app.ico` / `lua54.dll` / `csproj` 等入口/配置文件在根目录
- 新增 `Database/README.md`:说明引擎基础数据目录结构、部署放置位置、`Settings.游戏数据目录` 配置方式、典型访问路径
  - 顺带消除 GitHub 仓库首页 `Database/System` 单子目录折叠显示

---

## [0.6.0] - 2026-05-27

- 配套 `Database/` 引擎基础数据加入仓库：地图 / 物品 / 技能 / NPC / 触发器 / csv 配置，约 19,000 文件
- 三个窗口标题统一为 **Elaina Engine** 品牌
  - 游戏服务器主窗口: `九八游戏服务器` → `Elaina Engine`
  - 账号服务器: `永恒传奇登登录服务器`（原始错字"登登"） → `Elaina Engine - 账号服务器`
  - 游戏登录器: `永恒传奇登录器` → `Elaina Engine - 登录器`

---

## [0.5.0] - 2026-05-24

- 拆分 `玩家实例.cs`：24,901 → 19,602 行，抽出 4 个 partial 文件（挖矿 / 公会师门 / 交易摆摊 / 自动挂机）
- README 顶部加入 Elaina 立绘 + Elaina Pro 商用引流章节
- 新增 LICENSE（仅供学习/非商用自定义协议）、CONTRIBUTING.md、GitHub Issue / PR 模板
- 新增 `.gitattributes` 规范化行尾，校正 README 技术栈描述
- csproj `NoWarn` 抑制反编译产物噪音：57 warnings → 3
- 文档措辞中性化

---

## [0.4.0] - 2026-05-24

- 引擎命名为 **Elaina (伊蕾娜)**：README 标题 + AssemblyInfo 同步
- 清除 AssemblyInfo 中的原始厂商元数据

---

## [0.3.0] - 2026-05-24

- 扁平化 `游戏服务器/游戏服务器/` 双层目录结构
- `主程.cs` 按功能拆分为 6 个 partial 文件（1315 → 188 行）

---

## [0.2.0] - 2026-05-23

- 60+ 散落 .cs 文件按功能归位（新建 `日志类/` `任务类/`，合并外层 `工具类` `窗口视图`）
- 标识符可读性优化：`_0015_...` / `_0008_0006_...` → `WebApi` / `AutoBattle`
- 删除死代码：`------------/` 文件夹、`Attribute0/1/2.cs`、`Form1.cs`、`lua54.zip`、孤立 sln、重复 enum
- 新增 `GlobalUsings.cs`（.NET 8 全局 using）

---

## [0.1.0] - 2026-05-23

### 安全修复

详细审计报告见 [SECURITY_AUDIT.md](SECURITY_AUDIT.md)。

| ID | 严重 | 修复 |
|----|------|------|
| CRIT-01 | 🔴 | Newtonsoft.Json TypeNameHandling RCE → SerializationBinder 白名单 |
| CRIT-02 | 🔴 | 门票 RNG 可预测 → `RandomNumberGenerator` |
| CRIT-03 | 🔴 | 账号文件路径穿越 → 字符白名单 + 路径规范化 |
| CRIT-04 | 🔴 | 怪物爆率文件路径穿越 → 同上 |
| HIGH-01 | 🟠 | IP 封禁绕过（缺 else 分支） |
| HIGH-02 | 🟠 | 封包字节字段无上界 → 64KB 上限 + 流剩余检查 |
| HIGH-03 | 🟠 | TLS 1.0/1.1 弃用 → 仅 TLS 1.2/1.3 |
| DEEP-01 | 🔴 | 中和 `最优恢复` 的隐蔽外发逻辑（硬件指纹 POST）|
| DEEP-02 | 🔴 | WebApi 弱签名 `MD5(query+"&")` → `HMAC-SHA256(query, Settings.充值签名密钥)` |
| DEEP-03 | 🟡 | 删除 `------------/` 死代码目录 |

PROTO-01 ~ PROTO-08 协议级安全洞未自动修复（需客户端配合升级），详见审计报告。

### 文档
- 新增 README.md（项目说明、构建指南、端口表）
- 新增 SECURITY_AUDIT.md（完整审计报告）

---

## 版本约定

宽松遵循 [语义化版本](https://semver.org/lang/zh-CN/)：

- **MAJOR** — 网络协议或数据格式不兼容（客户端需同步升级）
- **MINOR** — 新功能 / 大规模重构 / 安全加固
- **PATCH** — Bug 修复 / 小幅调整

当前处于 0.x 阶段，所有 0.X.0 版本可能包含破坏性改动。
