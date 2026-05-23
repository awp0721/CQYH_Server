# 更新日志 / Changelog

本仓库遵循 [Keep a Changelog](https://keepachangelog.com/zh-CN/1.1.0/) 规范，日期格式 `YYYY-MM-DD`。

---

## [Unreleased]

暂无未发布改动。

---

## [0.5.0] - 2026-05-24

### 新增 (Added)
- **Elaina 关键视觉**：README 顶部加入 AI 生成的伊蕾娜立绘 (`assets/elaina.png`)，居中布局 + 标语 + 致敬声明。([`5dda863`](https://github.com/awp0721/CQYH_Server/commit/5dda863))
- **LICENSE 文件**：自定义"仅供学习与技术研究"许可。明确允许 (个人学习/学术研究/二次开发/安全审计) 与禁止 (商业运营/盈利分发/去除声明/违法使用) 用途，列出 Elaina 角色形象的第三方权利归属，附担保免除与安全提示。([`4fc96fa`](https://github.com/awp0721/CQYH_Server/commit/4fc96fa))
- **`.gitattributes`**：跨平台行尾规范化，Windows-only 项目文件强制 CRLF，二进制扩展名白名单。
- **`CONTRIBUTING.md`**：贡献指南。包含提交前 checklist、Issue 类型分流、PR 流程、Commit message 规范 (Conventional Commits 风格)、安全披露流程、代码风格。([`4e4b496`](https://github.com/awp0721/CQYH_Server/commit/4e4b496))
- **GitHub Issue / PR 模板** (`.github/`)：
  - `ISSUE_TEMPLATE/config.yml` — 禁用空白 Issue，链向 Private Vulnerability Reporting
  - `ISSUE_TEMPLATE/bug_report.yml` — Bug 报告表单
  - `ISSUE_TEMPLATE/feature_request.yml` — 功能建议表单
  - `ISSUE_TEMPLATE/question.yml` — 提问表单
  - `PULL_REQUEST_TEMPLATE.md` — PR 模板，含改动类型、关联 Issue、兼容性影响、提交前 checklist
- **README "Elaina Pro" 章节**：维护者新增的商用进阶版引流入口（公开版与 Pro 版授权完全独立）。([`298876f`](https://github.com/awp0721/CQYH_Server/commit/298876f))

### 变更 (Changed)
- **README 免责声明大改**：从 1 句话扩成 7 节结构化条款 (代码用途、安全提示、第三方权利、担保免除、联系方式等)。开头加入"继续浏览即视为同意"的接受条款。后续移除"代码来源 (反编译)"段，6 节版本保留。([`922fcd6`](https://github.com/awp0721/CQYH_Server/commit/922fcd6) + [`e53de1d`](https://github.com/awp0721/CQYH_Server/commit/e53de1d))
- **技术栈描述校正**：README 之前错写 ".NET Framework 4.8"，校正为 ".NET 8 (`net8.0-windows`)"，补充缺失的依赖 (Newtonsoft.Json / SharpZipLib / NLua / CsvHelper) 和 Lua 脚本系统模块。
- **文档措辞中性化**：移除所有 "反编译 / 反混淆 / VMProtect / decompiled / 商业引擎 kill-switch" 等措辞，统一改为中性描述 (hex 风格命名空间、远程控制信道、辅助文件等)。涉及 LICENSE / README / CHANGELOG / SECURITY_AUDIT 与 `玩家实例.cs` 注释。([`e53de1d`](https://github.com/awp0721/CQYH_Server/commit/e53de1d))
- **编译警告抑制**：`游戏服务器.csproj` 的 `NoWarn` 添加 `CS0162;CS0169;CS0219;CS0414;CS0649` —— 抑制 "字段从不使用 / 未读赋值 / 不可达代码" 等噪音。Build 输出从 57 warnings 降至 3 (剩余 3 个是真实代码质量信号，保留作信息)。

### 重构 (Refactor)
- **拆分 `玩家实例.cs` (24,901 → 19,602 行)**：从 24k+ 行的巨型 class 中提取 4 个 partial class 文件，按"方法连续聚集的语义块"切分：
  - `玩家实例.挖矿.cs` (690 行) — `#region 挖矿` 整块
  - `玩家实例.公会师门.cs` (2,822 行) — 公会 + 师门系统
  - `玩家实例.交易摆摊.cs` (840 行) — 玩家间交易 + 摆摊
  - `玩家实例.自动挂机.cs` (1,063 行) — 自动战斗 / 内挂逻辑
  
  类声明改为 `public sealed partial class 玩家实例`。**行为完全不变**——C# `partial class` 在编译时合并。([`38724e3`](https://github.com/awp0721/CQYH_Server/commit/38724e3))

### 验证
- 三个项目 `dotnet build -c Debug` 全部 0 errors。
- 玩家实例.cs 拆分中间出过两次边界 off-by-one 错误 (17126 应该是 17125；18280 应该是 17935)，已修正。

---

## [0.4.0] - 2026-05-24

### 变更 (Changed)
- **引擎命名为 Elaina (伊蕾娜)**：项目从无名"游戏服务器"正式定名为 **Elaina Engine**。
  - `README.md` 标题更新为 `Elaina Engine · 伊蕾娜引擎`
  - `游戏服务器/Properties/AssemblyInfo.cs`:
    - `AssemblyTitle` / `AssemblyProduct` → `Elaina Engine`
    - `AssemblyDescription` → `Elaina (伊蕾娜) - Game Server Engine`
    - `AssemblyVersion` → `0.4.0.0`
- **清除原始厂商信息**：
  - `AssemblyCompany` 从 "福建腾柏网络有限公司" 改为空（与该公司的 `pay.tengcanol.com` 是同一主体，参见 SECURITY_AUDIT.md DEEP-01 / DEEP-02 中关于该域名的发现）
  - `AssemblyCopyright` 从 "Copyright © 福建腾柏网络有限公司 2023" 改为空（无法证明所有权传递，留空更合规）

### 备注
- 底层 namespace `游戏服务器` 与 csproj 中的 `AssemblyName` **保持不变**，避免破坏现有代码引用和已部署实例的产物名称。

---

## [0.3.0] - 2026-05-24

### 重构 (Refactor)
- **扁平化项目目录**：把 `游戏服务器/游戏服务器/` 双层结构合并为单层 `游戏服务器/`，全部 12 个根文件 + 10 个子目录上移一级，删除冗余内层文件夹。([`4d11aa9`](https://github.com/awp0721/CQYH_Server/commit/4d11aa9))
- **`主程.cs` 按功能拆分**：原 1315 行单文件拆成 6 个 partial class 文件（行为完全一致）：
  - `主程.cs` (188 行) — 类声明、嵌套类型、静态字段、静态构造函数
  - `主程.日志.cs` (254 行) — 系统/聊天/命令/物品/货币/重铸日志
  - `主程.创角.cs` (152 行) — 角色创建请求流程
  - `主程.循环.cs` (225 行) — 主服务循环 / 日志写线程 / 重载任务
  - `主程.服务.cs` (98 行) — 启停/保存/重载 NPC
  - `主程.网页.cs` (231 行) — HTTP 事件处理（修改角色/执行命令/充值回调）

### 验证
- 三个项目 `dotnet build` 全部 0 errors。

---

## [0.2.0] - 2026-05-23

### 重构 (Refactor)
- **目录结构整理**：将 60+ 个散落在根目录的 .cs 文件按功能归位到 10 个子文件夹（任务类/副本类/地图类/工具类/数据类/日志类/模板类/窗口视图/管理命令/网络类）。
- **新建子文件夹**：`日志类/` 和 `任务类/`。
- **合并外层独立文件夹**：`游戏服务器/工具类/` 和 `游戏服务器/窗口视图/` 这两个独立于内层结构的文件夹被合并到对应内层目录，namespace 归一为 `游戏服务器.工具类` / `游戏服务器.窗口视图`。
- **标识符可读性优化**：
  - 命名空间 `_0015_0003_0007_000E_000D_000D` → `WebApi`
  - 命名空间 `_0008_0006_0005_0007_000F_000E_0004_0003` → `AutoBattle`
  - 类 `_0001_0018_000E_0012_0007_0006` → `WebApiService`
  - 类 `_000B_0018_0019_0016_0004_0018` → `AutoBattleManager`
- **删除死代码与冗余**：
  - 整个 `游戏服务器/------------/` 文件夹（10 个未引用的辅助文件，~500KB，csproj 已通过 `<Compile Remove>` 排除编译）
  - `Attribute0.cs` / `Attribute1.cs` / `Attribute2.cs`（互相自引用的空 marker 属性）
  - `Form1.cs` / `Form1.Designer.cs` / `Form1.resx`（VS 默认模板空 Form）
  - `lua54.zip`（`lua54.dll` 的冗余压缩备份）
  - `游戏服务器/游戏服务器.sln`（孤立的单项目 sln，被根目录的 `YH_Server_Code.sln` 取代）
  - `游戏服务器/地图类/ActFieldType.cs`（与 `工具类/ActFieldType.cs` 同义重复）
- **新增 `GlobalUsings.cs`**：利用 .NET 8 global using 机制，让所有子命名空间在全工程自动可见，避免改 namespace 后要修改几百个 using 语句。

### 提交
- [`b4242a6`](https://github.com/awp0721/CQYH_Server/commit/b4242a6) refactor: rename hex-style identifiers for readability
- [`5a01604`](https://github.com/awp0721/CQYH_Server/commit/5a01604) refactor: reorganize file structure
- [`1560064`](https://github.com/awp0721/CQYH_Server/commit/1560064) fix: update FQN reference
- [`002c3e3`](https://github.com/awp0721/CQYH_Server/commit/002c3e3) refactor: cleanup structure

---

## [0.1.0] - 2026-05-23

### 安全 (Security)

详细审计报告见 [SECURITY_AUDIT.md](SECURITY_AUDIT.md)。

#### CRITICAL / HIGH 级别修复（已自动应用）

- **[CRIT-01] Newtonsoft.Json TypeNameHandling RCE**：游戏服务器和账号服务器的 `JsonSerializerSettings` 均开启 `TypeNameHandling.Auto`，允许 JSON `$type` 字段指定任意类型反序列化，可经磁盘文件触发 RCE。修复：引入 `ISerializationBinder` 白名单，仅允许 `游戏服务器` / `账号服务器` / `Assembly-CSharp` 三个程序集的类型。
- **[CRIT-02] 门票生成可预测**：账号服务器用 `System.Random` 生成 32 字符会话门票，攻击者可脱机暴力枚举。修复：替换为 `System.Security.Cryptography.RandomNumberGenerator`。
- **[CRIT-03] 账号文件写入路径穿越**：`File.WriteAllText(数据目录 + "\\" + 账号名字 + ".txt", ...)` 直接拼接客户端传入的账号名，可越权写任意文件。修复：白名单字符校验 + `Path.GetFullPath` 规范化 + 前缀检查。
- **[CRIT-04] 怪物爆率文件路径穿越**：同模式问题在 `游戏怪物.cs` 中重现。修复：同上方法加固。
- **[HIGH-01] IP 封禁绕过**：TCP 监听回调中被封 IP 被 `Close()` 后**仍然 `Enqueue` 加入连接池**（缺少 `else`），导致封禁完全失效。修复：改为 `if/else` 结构。
- **[HIGH-02] 封包字节字段无上界**：当 `描述符.长度 == 0` 时长度从客户端流读取 `UInt16`，可造成 OOM/DoS。修复：增加 64KB 上限 + 流剩余字节数校验。
- **[HIGH-03] 弱 TLS 协议**：启用了 TLS 1.0（BEAST）和 TLS 1.1（POODLE）。修复：仅启用 TLS 1.2 / 1.3。

#### 深度审计发现（已修复）

- **[DEEP-01] 隐蔽外发逻辑**：`玩家实例.cs::最优恢复` 方法通过 byte 数组拼接字符串的方式构造请求，向 `主程.LogWebSite + base/Uniqueverify` 提交 Win32_Processor 硬件指纹，远端可控制游戏机制（`玩家实例.恢复量`）+ 界面状态栏文本。默认通过 `LogWebSite = null` 关闭，但只要设置一次就激活。修复：方法体置空，原始实现保留为 `最优恢复_已禁用_原始实现` 供审计参考。
- **[DEEP-02] HTTP 签名形同虚设**：`WebApi.WebApiService.Sign()` 使用 `MD5(query + "&")`，`"&"` 是硬编码常量，等于无密钥。该签名保护着 `/useCmd`、`/paymentcallback`、`/modifyRole`、**`/getRoleList`**（live、可被立即用于批量导出全服角色数据）等端点。修复：改为 `HMAC-SHA256(query, Settings.充值签名密钥)`，密钥空时返回固定哨兵导致比对必然失败；签名比对改用 `CryptographicOperations.FixedTimeEquals` 防时序攻击。
- **[DEEP-03] 死代码清理**：删除整个 `------------/` 目录（10 个未引用的辅助文件）和 `_000A_0007_...cs`（孤立 ResourceManager）。

#### 未自动修复（协议级问题）

以下问题需要客户端配合升级，本次未自动应用，详见 [SECURITY_AUDIT.md](SECURITY_AUDIT.md)：
- PROTO-01：明文密码存储与比对
- PROTO-02：密码明文经 UDP 传输
- PROTO-03：账号服务器→游戏服务器之间无身份认证（注释掉的 IP 白名单）
- PROTO-04：多个未鉴权的封包处理器
- PROTO-05：GM 权限基于角色数据中可篡改的标志位
- PROTO-06：License 模块自研 XOR 加密
- PROTO-07：MD5 用于设备指纹和 HTTP 请求签名
- PROTO-08：客户端发来的 IP 被无验证地接受
- MISC-01~05：无限流、无密码复杂度、节点编辑器剪贴板反序列化等

#### 未发现的攻击面
- SQL 注入（项目使用自定义二进制存储，无 SQL）
- XML / XXE
- 命令注入
- 硬编码凭据

### 文档 (Documentation)
- 新增 `README.md`，包含项目结构说明、技术栈、构建指南、端口表、star history 图表。
- 新增 `SECURITY_AUDIT.md`，完整记录所有审计发现。

### 提交
- [`fd5d87e`](https://github.com/awp0721/CQYH_Server/commit/fd5d87e) Initial commit
- [`a6338c1`](https://github.com/awp0721/CQYH_Server/commit/a6338c1) docs: add README
- [`240ffe2`](https://github.com/awp0721/CQYH_Server/commit/240ffe2) security: fix critical and high-severity vulnerabilities
- [`84cfa63`](https://github.com/awp0721/CQYH_Server/commit/84cfa63) security: neutralize dormant backdoor

---

## 版本约定

本项目使用宽松的 [语义化版本](https://semver.org/lang/zh-CN/) 节奏：

- **MAJOR (1.0.0)** — 网络协议或数据格式不兼容（客户端需同步升级）
- **MINOR (0.X.0)** — 新功能 / 大规模重构 / 安全加固
- **PATCH (0.0.X)** — Bug 修复 / 小幅调整

当前处于 0.x 阶段，所有 0.X.0 版本可能包含破坏性改动。
