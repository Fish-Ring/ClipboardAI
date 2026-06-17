> [English](../en/config.md) · [返回首页](https://github.com/Fish-Ring/ClipboardAI)

# 配置详情

## 目录

- [文件位置](#文件位置)
- [配置项说明](#配置项说明)
- [Prompt 预设](#prompt-预设)
- [快捷键配置](#快捷键配置)
- [临时配置](#临时配置)

---

## 文件位置

```
%APPDATA%\ClipboardAI\
├── config.json      # 主配置
└── history.json     # 处理历史
```

---

## 配置项说明

### API 设置

| 配置项 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `apiBase` | string | `https://api.openai.com/v1` | API 端点，支持任何兼容 OpenAI 的服务 |
| `apiKey` | string | `""` | API 密钥 |
| `model` | string | `gpt-4o-mini` | 模型名称 |
| `maxTokens` | int | `2048` | 最大输出 token 数 |
| `temperature` | double | `0.3` | 创造性，值越高输出越多样 |

### 快捷键

| 配置项 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `hotkeyEnabled` | bool | `true` | 启用全局快捷键 |
| `hotkeyModifiers` | string | `"Alt"` | 修饰键：`Alt`、`Ctrl`、`Shift` |
| `hotkeyKey` | string | `"C"` | 触发键 |

### 其他

| 配置项 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `enabled` | bool | `true` | 全局启用/禁用 |
| `maxHistory` | int | `20` | 历史记录上限 |
| `showNotifications` | bool | `false` | 显示 Windows 托盘通知 |
| `clipboardMonitoringEnabled` | bool | `false` | 监控剪贴板变化 |
| `playSoundOnComplete` | bool | `false` | 完成时播放提示音 |
| `language` | string | `"zh-CN"` | 界面语言：`zh-CN` 或 `en` |

---

## Prompt 预设

默认包含 4 个预设：

| ID | 名称 | 说明 |
|----|------|------|
| `qa` | 回答问题 | 用中文回答，纯文本输出 |
| `translate` | 翻译中文 | 翻译为简体中文 |
| `polish` | 润色文本 | 使文本更通顺专业 |
| `summarize` | AI 总结 | 用中文总结核心要点 |

### 添加自定义预设

在设置窗口 → Prompt 预设 → 添加，填写：
- **ID**：唯一标识符（如 `code-review`）
- **名称**：显示名称
- **System Prompt**：发送给 AI 的系统提示词

---

## 快捷键配置

1. 打开设置 → 快捷键
2. 勾选「启用全局快捷键」
3. 选择修饰键和触发键
4. 保存

快捷键触发流程：
1. 选中任意文本
2. 按下快捷键
3. 程序模拟 Ctrl+C 复制
4. 发送当前选中的 Prompt 处理
5. 结果写回剪贴板

---

## 临时配置

将 `config.json` 拖到 `ClipboardAI.exe` 上即可加载临时配置。

### 行为

- 临时配置完全替换当前会话的内存配置
- **不写入**永久配置文件
- 配置窗口的修改**不会保存**
- 历史记录仅在内存中保留
- 下次启动自动恢复为永久配置

### 使用场景

- 临时切换 API Key 或模型
- 测试不同 Prompt 配置
- 在多台机器间携带配置（配合 USB 使用）

### 识别

临时模式启动后：
- 托盘图标文字变为 `ClipboardAI [临时]`
- 首次加载时弹出通知提示
