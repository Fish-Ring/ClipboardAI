> [简体中文](../zh-CN/config.md) · [Back to Home](./README.md)

# Configuration Details

## Contents

- [File Locations](#file-locations)
- [Config Reference](#config-reference)
- [Prompt Presets](#prompt-presets)
- [Hotkey Configuration](#hotkey-configuration)
- [Temporary Config](#temporary-config)

---

## File Locations

```
%APPDATA%\ClipboardAI\
├── config.json      # Main config
└── history.json     # Processing history
```

---

## Config Reference

### API Settings

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| `apiBase` | string | `https://api.openai.com/v1` | API endpoint, supports any OpenAI-compatible service |
| `apiKey` | string | `""` | API key |
| `model` | string | `gpt-4o-mini` | Model name |
| `maxTokens` | int | `2048` | Maximum output tokens |
| `temperature` | double | `0.3` | Creativity, higher values produce more varied output |

### Hotkeys

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| `hotkeyEnabled` | bool | `true` | Enable global hotkey |
| `hotkeyModifiers` | string | `"Alt"` | Modifier: `Alt`, `Ctrl`, or `Shift` |
| `hotkeyKey` | string | `"C"` | Trigger key |

### Other

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| `enabled` | bool | `true` | Global enable/disable |
| `maxHistory` | int | `20` | History entry limit |
| `showNotifications` | bool | `false` | Show Windows tray notifications |
| `clipboardMonitoringEnabled` | bool | `false` | Monitor clipboard changes |
| `playSoundOnComplete` | bool | `false` | Play sound on completion |
| `language` | string | `"zh-CN"` | UI language: `zh-CN` or `en` |

---

## Prompt Presets

4 presets included by default:

| ID | Name | Description |
|----|------|-------------|
| `qa` | Q&A | Answer in Chinese, plain text only |
| `translate` | Translate | Translate to Simplified Chinese |
| `polish` | Polish | Improve text fluency and professionalism |
| `summarize` | Summarize | Chinese-language summary of key points |

### Adding Custom Presets

In Settings → Prompts → Add, fill in:
- **ID**: Unique identifier (e.g. `code-review`)
- **Name**: Display name
- **System Prompt**: System prompt sent to the AI

---

## Hotkey Configuration

1. Open Settings → Hotkeys
2. Check «Enable global hotkey»
3. Select modifier and trigger key
4. Save

Hotkey flow:
1. Select any text
2. Press hotkey
3. Program simulates Ctrl+C
4. Sends to AI using current preset
5. Result written back to clipboard

---

## Temporary Config

Drop a `config.json` onto `ClipboardAI.exe` to load a temporary config.

### Behavior

- Temporary config replaces in-memory config for the session
- Does **not** write to permanent config file
- Changes in settings window are **not saved**
- History kept in memory only
- Next launch restores permanent config automatically

### Use Cases

- Temporarily switch API key or model
- Test different prompt configurations
- Carry config across machines (with USB)

### Identification

When temp mode is active:
- Tray text becomes `ClipboardAI [Temporary]`
- Notification shown on first load
