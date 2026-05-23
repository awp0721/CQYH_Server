<div align="center">

<img src="assets/elaina.png" alt="Elaina · 伊蕾娜" width="520">

# Elaina Engine · 伊蕾娜引擎

> 公开发布的 .NET 8 游戏服务端引擎，包含游戏服务器、账号服务器、客户端登录器三个独立项目。

**游戏服务器代号: Elaina (伊蕾娜)** · 角色形象致敬《魔女之旅 / 魔女の旅々》

</div>

> 💼 **私人引擎咨询请联系：** awp227hcx@gmail.com

## 📦 项目结构

```
YH_Server_Code/
├── YH_Server_Code.sln        # Visual Studio 解决方案
├── 游戏服务器/                # 游戏主服务器（核心逻辑、A* 寻路、地图、任务、NPC 等）
├── 账号服务器/                # 账号验证服务器（监听 8001 端口，门票转发 6678）
└── 游戏登录器/                # 客户端启动器（图形验证码、登录界面）
```

## 🛠️ 技术栈

- **语言**：C# 10.0
- **框架**：.NET 8 (`net8.0-windows`, Windows Forms)
- **IDE**：Visual Studio 2022 (v17.9+)
- **UI 组件**：DevExpress
- **依赖**：Newtonsoft.Json、SharpZipLib、NLua、CsvHelper
- **核心模块**：A* 寻路算法、自定义二进制网络协议、INI 配置读写、Lua 脚本系统、任务/成就/副本系统

## 🚀 构建说明

1. 使用 Visual Studio 2022 打开 `YH_Server_Code.sln`
2. 还原 NuGet 依赖并安装 DevExpress 组件
3. 选择 `Debug` 或 `Release` 配置，编译解决方案
4. 启动顺序建议：**账号服务器 → 游戏服务器 → 游戏登录器**

### 端口说明

| 服务 | 默认端口 | 用途 |
|------|---------|------|
| 账号服务器 | 8001 | 客户端账号验证 |
| 门票转发 | 6678 | 账号 ↔ 游戏服务器通信 |

## 📋 模块清单

### 游戏服务器
- A* 寻路（`AStar/`）
- 任务系统（QuestType / QuestReward / QuestMission 等）
- 日志体系（消费日志、商人日志、登录登出日志、命令日志）
- 角色创建与属性管理
- NPC 与代理数据上行

### 账号服务器
- 网络通信层
- 账号数据序列化
- 多账号并发管理

### 游戏登录器
- 图形验证码
- 登录界面
- 网络通信封装

## 📖 相关文档

- [CHANGELOG.md](CHANGELOG.md) —— 版本更新日志
- [SECURITY_AUDIT.md](SECURITY_AUDIT.md) —— 安全审计报告

## ⚠️ 免责声明

> 阅读本仓库前请认真阅读本节。继续浏览、克隆、Fork 或基于本仓库二次开发即视为已知悉并同意以下条款。完整法律条款见 [LICENSE](LICENSE)，本节为面向用户的通俗摘要。

### 1. 允许的用途

- ✅ **个人学习**：研究 .NET 8 / C# / Windows Forms / 游戏服务端架构 / 网络协议设计
- ✅ **学术研究**：发表关于本项目的安全分析、架构论文、漏洞披露报告
- ✅ **二次开发**：用于个人项目、毕业设计、技术演示，需保留 LICENSE、SECURITY_AUDIT.md、CHANGELOG.md
- ✅ **安全审计**：基于 SECURITY_AUDIT.md 继续挖掘漏洞或验证已修复项

### 2. 禁止的用途

- ❌ **商业运营**：直接部署本代码运营游戏服务器、收取玩家充值或会员费
- ❌ **盈利分发**：以付费形式分发本仓库内容或衍生作品
- ❌ **去除声明**：重新分发时移除 LICENSE / SECURITY_AUDIT.md / CHANGELOG.md / 本节免责声明
- ❌ **违法使用**：任何违反所在司法管辖区相关法律的使用方式

### 3. 安全提示

本代码经过审计但**仍存在协议级安全洞**（明文密码、未鉴权门票转发等，详见 SECURITY_AUDIT.md 中 PROTO-01 ~ PROTO-08）。即使仅用于学习，也**不建议在生产网络中暴露监听端口**。如需测试，请使用本地回环 (127.0.0.1) 或受控的隔离环境。

### 4. 第三方权利

- **角色形象**：README 顶部使用的 Elaina (伊蕾娜) 形象**致敬**《魔女之旅 / 魔女の旅々》（原作者：白石定規，イラスト：あずーる，出版：SB Creative）。该形象著作权归原作者所有。本仓库使用的图片由 AI 生成，仅用于装饰
- **DevExpress / Newtonsoft.Json / NLua 等依赖**：各自版权归原作者所有，使用须遵守其各自的 License

### 5. 担保免除

本仓库**按"现状"提供，不附带任何明示或暗示的担保**，包括但不限于：
- 不保证代码的正确性、完整性、可用性
- 不保证安全审计已覆盖所有漏洞
- 不保证编译产物可在任何特定环境下正常运行

使用者使用本仓库内容所产生的**任何法律责任、经济损失、数据损坏、商誉损失**等后果，均由使用者自行承担。仓库维护者不为任何直接或间接损失负责。

### 6. 联系方式

如对本仓库内容有疑问、建议或需要沟通的事项，请通过 [GitHub Issues](https://github.com/awp0721/CQYH_Server/issues) 提交。

---

## 🌟 星级历史

<a href="https://www.star-history.com/?type=date&repos=awp0721%2Fcqyh_server">
 <picture>
   <source media="(prefers-color-scheme: dark)" srcset="https://api.star-history.com/chart?repos=awp0721/cqyh_server&type=date&theme=dark&legend=top-left" />
   <source media="(prefers-color-scheme: light)" srcset="https://api.star-history.com/chart?repos=awp0721/cqyh_server&type=date&legend=top-left" />
   <img alt="Star History Chart" src="https://api.star-history.com/chart?repos=awp0721/cqyh_server&type=date&legend=top-left" />
 </picture>
</a>

## 📄 许可

完整许可条款见 [LICENSE](LICENSE) 文件。简要概括：**仅供学习交流，禁止商业用途，二次分发须保留来源声明**。
