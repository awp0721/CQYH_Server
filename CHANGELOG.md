# 更新日志 / Changelog

本仓库遵循 [Keep a Changelog](https://keepachangelog.com/zh-CN/1.1.0/) 规范，日期格式 `YYYY-MM-DD`。

---

## [Unreleased]

### 变更

- **游戏登录器单文件发布**：登录器编译产物不再附带一堆 DLL。`游戏登录器.csproj` 启用 `PublishSingleFile`（框架依赖、`win-x86`、不裁剪、`DebugType=none`），`dotnet publish -c Release -r win-x86 --self-contained false` 后发布目录仅剩单个 `游戏登录器.exe`（约 4.9 MB）。玩家端需预装 .NET 8 桌面运行时

### 安全修复（第二轮深度审计 2026-05-27）

| ID | 严重 | 修复 |
|----|------|------|
| PROTO-04 续 | 🟠 | 7 处封包处理器（鉴定无相钥石 / 内挂物品过滤 / 领取七天奖励 / 领取七天大奖 / 验证动态密码 / 组队拍卖竞价 / 玩家放弃任务）补加阶段守卫 |
| DEEP-04 | 🟠 | 自定义扩展封包：鉴权 + 字段 1KB 上限 + 日志注入控制字符过滤 |
| DEEP-05 | 🟠 | 聊天日志注入：`主程.添加聊天日志` 与旧版 `主窗口.添加聊天日志` 都加入文本净化 |
| DEEP-06 | 🟠 | 聊天封包带宽放大：发送聊天信息 / 发送好友聊天 / 发送社交消息 单条上限收紧到 2KB |
| DEEP-07 | 🟡 | WebApi：HTTP body 64KB 上限 + 异常信息不再回显客户端 + null 反序列化防御 |
| HIGH-04 | 🟠 | 商人发货命令: `元宝数量*100` 整数溢出 + ulong 累加防 uint 回绕, 防对头利用商人对受害者元宝执行归零攻击 (同步加固赠送元宝) |
| MISC-06 | 🟡 | 玩家合成物品: 原 `_ = this.交易状态;` 死代码守卫替换为真实的死亡/摆摊/交易状态检查, 防止异常状态下合成产生不一致 |
| HIGH-05 | 🟠 | 队伍队长强制踢人未清理被踢者的组队竞拍, 导致竞价金币锁死无法通过邮件退回; 补齐 `放弃所有拍卖` 调用 |
| LOW-A | 🟡 | 行会公告/宣言/摊位名字 新增控制字符与 Unicode 双向覆写过滤 (`净化展示文本`), 防止钓鱼伪装系统通知 |

详见 [SECURITY_AUDIT.md](SECURITY_AUDIT.md#七第二轮深度审计补充2026-05-27) 与第八节第三轮针对性审计。三个项目 `dotnet build` 仍为 0 errors。

### 其他

- README 新增 QQ 技术交流群 `1105229144`,明确标注**仅限单机技术交流**,禁止商业运营/私服推广/外挂相关话题;同时在「联系方式」节列出 Issues / 邮箱 / QQ 群三种渠道
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
