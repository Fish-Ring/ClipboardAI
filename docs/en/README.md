> [简体中文](https://github.com/Fish-Ring/ClipboardAI/blob/master/README.md)

# ClipboardAI

Clipboard AI Assistant — Select text, send to OpenAI-compatible LLM, auto-write result back to clipboard.

[Download latest](https://github.com/Fish-Ring/ClipboardAI/releases/latest)

---

## Features

- **Clipboard monitoring** — Auto-detect clipboard changes, no manual paste needed
- **Global hotkey** — `Alt+C` (customizable) to trigger processing
- **Multi-prompt presets** — Translate, polish, Q&A, summarize — add your own
- **Flexible AI backend** — Compatible with OpenAI /chat/completions, custom API endpoints
- **Processing history** — Auto-log every process for easy review
- **Drag-and-drop temp config** — Drop config.json onto exe for session-only config
- **Sound feedback** — Beep on AI completion
- **Windows tray** — Resident tray icon, double-click for settings, right-click menu
- **Multilingual** — 简体中文 / English auto-switch
- **Zero-config startup** — Auto-generate default config on first run

---

## Quick Install

```powershell
# Build from source (requires .NET 8 SDK)
git clone https://github.com/Fish-Ring/ClipboardAI.git
cd ClipboardAI/ClipboardAI
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o dist
```

Or download `ClipboardAI.exe` directly from the [Releases page](https://github.com/Fish-Ring/ClipboardAI/releases/latest).

---

## Quick Start

1. Run `ClipboardAI.exe` — app stays in system tray
2. Copy any text
3. Press `Alt+C` (or right-click tray icon to select a prompt)
4. AI-processed result is auto-written back to clipboard

First run auto-generates `config.json` in `%APPDATA%\ClipboardAI\`. Edit directly or use the settings window.

---

## Configuration

| Setting | Description |
|---------|-------------|
| `apiBase` | API endpoint, default `https://api.openai.com/v1` |
| `apiKey` | Your API Key |
| `model` | Model name, default `gpt-4o-mini` |
| `temperature` | Creativity, 0.0–2.0, default 0.3 |
| `maxTokens` | Max output tokens, default 2048 |
| `hotkeyEnabled` | Enable global hotkey |
| `hotkeyModifiers` | Modifier key (Alt/Ctrl/Shift) |
| `hotkeyKey` | Trigger key |
| `showNotifications` | Show Windows tray notifications |
| `clipboardMonitoringEnabled` | Monitor clipboard changes |
| `playSoundOnComplete` | Play sound on completion |
| `language` | UI language (zh-CN / en) |

---

## Drag-and-Drop Temp Config

Drop a `config.json` onto `ClipboardAI.exe` to load a temporary config:

- Session-only — does not affect permanent config
- Changes in settings window are not saved
- History kept in memory only
- Next launch returns to permanent config

Tray displays `ClipboardAI [Temporary]` indicator.

---

## Documentation

- [Configuration Details](config.md)
- [Prompt Presets Guide](prompts.md)

---

## License

MIT
