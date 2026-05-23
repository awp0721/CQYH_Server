<div align="center">

<img src="assets/elaina.png" alt="Elaina · 伊蕾娜" width="520">

# Elaina Engine · 伊蕾娜引擎

> 公开发布的 .NET 8 游戏服务端引擎，包含游戏服务器、账号服务器、客户端登录器三个独立项目。

**游戏服务器代号: Elaina (伊蕾娜)** · 角色形象致敬《魔女之旅 / 魔女の旅々》

</div>

## 📦 项目结构

```
YH_Server_Code/
├── YH_Server_Code.sln        # Visual Studio 解决方案
├── 游戏服务器/                # 游戏主服务器（核心逻辑、A* 寻路、地图、任务、NPC 等）
├── 账号服务器/                # 账号验证服务器（监听 8001 端口，门票转发 6678）
└── 游戏登录器/                # 客户端启动器（图形验证码、登录界面）
```

## 🛠️ 技术栈

- **语言**：C#
- **框架**：.NET Framework 4.8（Windows Forms）
- **IDE**：Visual Studio 2022 (v17.9+)
- **UI 组件**：DevExpress
- **核心模块**：A* 寻路算法、自定义网络通信、INI 配置读写、任务/成就系统

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

本仓库仅供学习与技术研究使用，请勿用于任何商业用途。使用本代码所产生的任何后果由使用者自行承担。

## 🌟 星级历史

[![Star History Chart](https://api.star-history.com/svg?repos=awp0721/cqyh_server&type=Date&_=20260523a)](https://star-history.com/#awp0721/cqyh_server&Date)

## 📄 许可

仅供学习交流，二次开发请保留原始来源信息。
