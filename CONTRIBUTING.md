# 贡献指南 / Contributing Guide

感谢对 **Elaina Engine** 的兴趣！本项目欢迎以下形式的贡献：

- 🐛 Bug 报告
- 🔒 安全漏洞披露
- ✨ 功能改进 / 重构 PR
- 📝 文档完善
- 💡 架构讨论 / 设计建议

> 在提交贡献之前，请先阅读 [LICENSE](LICENSE) 和 [README 的免责声明](README.md#%EF%B8%8F-免责声明)。提交即视为同意将贡献内容以相同许可发布。

---

## 提交前请确认

| ✅ | 检查项 |
|---|--------|
| ☐ | 已搜索现有 Issue / PR，确认没有重复 |
| ☐ | 改动经过 `dotnet build` 编译通过（0 errors）|
| ☐ | 没有引入新的 `_0001_0002_...` 风格的乱码命名 |
| ☐ | 涉及网络协议 / 数据格式的改动已在 commit message 中说明兼容性影响 |
| ☐ | 安全相关改动同步更新了 [SECURITY_AUDIT.md](SECURITY_AUDIT.md) |
| ☐ | 公开可见的改动同步更新了 [CHANGELOG.md](CHANGELOG.md) |

---

## 提交 Issue

请使用对应模板提交：

| 类型 | 模板 | 说明 |
|------|------|------|
| 🐛 Bug 报告 | [Bug Report](https://github.com/awp0721/CQYH_Server/issues/new?template=bug_report.yml) | 包含复现步骤、预期行为、实际行为、运行环境 |
| 🔒 安全漏洞 | 见下方 [安全披露流程](#安全披露流程) | **不要直接公开提 Issue** |
| ✨ 功能建议 | [Feature Request](https://github.com/awp0721/CQYH_Server/issues/new?template=feature_request.yml) | 说明使用场景和预期效果 |
| ❓ 使用问题 | [Question](https://github.com/awp0721/CQYH_Server/issues/new?template=question.yml) | 先看 README / SECURITY_AUDIT，再提问 |

---

## 提交 PR

### 分支策略

- 主分支：`main`（保护分支，不接受直接 push）
- 从 main fork → 在你的 fork 上建分支 → PR 回 main
- 建议分支命名：`fix/<简短描述>`、`feat/<功能名>`、`refactor/<重构目标>`、`docs/<文档名>`

### Commit Message 规范

借鉴 [Conventional Commits](https://www.conventionalcommits.org/zh-hans/v1.0.0/) 但不强制：

```
<type>: <简短描述>

[可选的详细描述]

[可选的关联 Issue: Closes #123]
```

常用 type：
- `feat`：新功能
- `fix`：修复 bug
- `security`：安全修复（CRITICAL/HIGH 建议先走安全披露流程）
- `refactor`：重构（无行为变化）
- `perf`：性能优化
- `docs`：文档
- `chore`：杂项（构建配置、依赖更新等）
- `test`：测试相关

示例（参考本项目的 git 历史）：
```
security: neutralize dormant backdoor, fix weak signing
refactor: flatten nested project layout + split 主程.cs by domain
fix: update FQN reference to match new namespace
```

### PR 流程

1. 提 PR 前先在本地跑 `dotnet build`，确保 0 errors
2. PR 标题使用上述格式
3. 填写 PR 模板的所有相关字段
4. 如果改动较大，建议先开 Discussion 或 Issue 讨论方案
5. 等待 review，按反馈修改
6. 维护者合并后会同步更新 CHANGELOG.md（或请你在 PR 中一起更新）

---

## 安全披露流程

如果你发现了**安全漏洞**，**请不要直接在公开 Issue 中讨论**。

### 推荐渠道（按优先级）

1. **GitHub Private Vulnerability Reporting**（推荐）
   - 访问 [Security 页面](https://github.com/awp0721/CQYH_Server/security/advisories/new)
   - 提交私密漏洞报告

2. **Issue 模板（Security Report）**
   - 使用安全模板，仓库会将其标记为限制可见
   - 描述漏洞时**不要附带能直接复现的攻击载荷**

### 漏洞报告应包含

- 漏洞类型（参考 OWASP / CWE 分类）
- 影响范围（哪个文件、哪个方法、哪个端口）
- 严重程度（CRITICAL / HIGH / MEDIUM / LOW）
- 复现步骤（高层描述，避免直接给可武器化的 PoC）
- 修复建议（如果有）

### 我们的承诺

- 72 小时内首次响应
- 修复后会在 [SECURITY_AUDIT.md](SECURITY_AUDIT.md) 中记录并致谢报告者（如愿意）
- 不会追究 善意安全研究行为 的责任

---

## 代码风格

本项目代码风格因历史原因混合了多种习惯，**不强制统一**，但新写的代码建议：

- **命名**：中文标识符可以接受（项目主流），但避免 `_0001_0002` 这种 hex 风格
- **缩进**：4 空格（参考 `.editorconfig` 如有）
- **行尾**：跟随 `.gitattributes` 设置（Windows 项目文件 CRLF，其他文件 LF/auto）
- **命名空间**：跟随子文件夹结构，使用 `游戏服务器.<子目录>` 形式
- **新增 partial class**：文件名格式 `<原文件名>.<功能>.cs`（参考 `主程.日志.cs`）

---

## 开发环境

| 项 | 版本 / 工具 |
|----|------------|
| .NET SDK | .NET 8 |
| IDE | Visual Studio 2022 (17.9+) / JetBrains Rider |
| OS | Windows 10/11（项目使用 Windows Forms）|
| 依赖 | DevExpress、Newtonsoft.Json、SharpZipLib、NLua、CsvHelper |

构建命令：
```bash
dotnet build YH_Server_Code.sln -c Debug
```

---

## 行为准则

- 尊重他人的时间和工作
- 讨论中保持专业，避免人身攻击
- 不传播任何引擎用户的隐私数据
- 不在 Issue / PR 中讨论任何未经授权的部署或盈利行为

违反将关闭对应 Issue / PR，严重者直接 ban。

---

## 致谢

感谢每一位贡献者。如果你的 PR 被合并，会被自动添加到 GitHub 的 Contributors 列表。

主要贡献者会同时出现在 README 末尾的 Contributors 章节（待建立）。

---

*Last updated: 2026-05-24*
