> [English](docs/en/README.md)

# ClipboardAI

剪贴板 AI 助手 — 选中文本，一键发送给 OpenAI 兼容大模型，自动写回剪贴板。

[下载最新版](https://github.com/Fish-Ring/ClipboardAI/releases/latest)

---

## 特性

- **剪贴板监控** — 自动检测剪贴板变化，无需手动粘贴
- **全局快捷键** — `Alt+C`（可自定义）快速触发处理
- **多 Prompt 预设** — 翻译、润色、问答、总结等，自由添加
- **AI 后端自由** — 兼容 OpenAI /chat/completions 接口，支持自定义 API 地址和模型
- **处理历史** — 自动记录每次处理，方便回溯
- **拖放临时配置** — 拖入 config.json 加载临时配置，不影响永久设置
- **提示音反馈** — AI 返回结果后播放简短提示音
- **Windows 托盘** — 常驻托盘，双击打开设置，右键菜单操作
- **多语言** — 简体中文 / English 自动切换
- **零配置启动** — 首次运行自动生成默认配置

---

## 快速安装

```powershell
# 从源码构建（需要 .NET 8 SDK）
git clone https://github.com/Fish-Ring/ClipboardAI.git
cd ClipboardAI/ClipboardAI
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o dist
```

或直接下载 [Releases 页面](https://github.com/Fish-Ring/ClipboardAI/releases/latest) 的 `ClipboardAI.exe`。

---

## 快速开始

1. 运行 `ClipboardAI.exe`，程序常驻托盘
2. 复制任意文本
3. 按 `Alt+C`（或右键托盘图标选择 Prompt）
4. AI 处理完成后自动写回剪贴板

首次运行会自动在 `%APPDATA%\ClipboardAI\` 生成 `config.json`，可编辑或使用设置窗口配置。

---

## 配置

| 配置项 | 说明 |
|--------|------|
| `apiBase` | API 地址，默认 `https://api.openai.com/v1` |
| `apiKey` | 你的 API Key |
| `model` | 模型名称，默认 `gpt-4o-mini` |
| `temperature` | 创造性，0.0–2.0，默认 0.3 |
| `maxTokens` | 最大输出 token，默认 2048 |
| `hotkeyEnabled` | 是否启用全局快捷键 |
| `hotkeyModifiers` | 修饰键（Alt/Ctrl/Shift） |
| `hotkeyKey` | 触发键 |
| `showNotifications` | 是否显示 Windows 托盘通知 |
| `clipboardMonitoringEnabled` | 是否监控剪贴板变化 |
| `playSoundOnComplete` | 完成时播放提示音 |
| `language` | 界面语言（zh-CN / en） |

---

## 拖放临时配置

将 `config.json` 拖到 `ClipboardAI.exe` 上即可加载临时配置：

- 仅当前会话有效，不影响永久配置
- 配置窗口修改不会保存
- 历史记录仅在内存中保留
- 下次启动自动回到永久配置

托盘显示 `ClipboardAI [临时]` 标识。

---

## 文档

- [配置详情](docs/zh-CN/config.md)
- [Prompt 预设指南](docs/zh-CN/prompts.md)

---

## 许可证

MIT
