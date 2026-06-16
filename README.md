# ClipboardAI

A Windows system tray tool that monitors clipboard text, sends it to an OpenAI-compatible LLM API, and writes the result back to clipboard automatically.

## Features

- **Hotkey trigger** — Press `Alt+C` (configurable) to process selected text via LLM
- **Clipboard monitoring** — Auto-process any copied text (toggle on/off)
- **Multi-prompt presets** — Switch between different system prompts (Translate, Summarize, Q&A, Polish, etc.)
- **History** — Recent processing history with status icons (✓/✗/⋯), persisted across restarts
- **Detail viewer** — Click any history entry to see full original text and AI response
- **Config GUI** — Full settings UI: API endpoint, key, model, prompts, hotkey, notifications, sound
- **Customizable** — Edit `config.json` directly for advanced configuration
- **Portable** — Self-contained single-file executable, no dependencies

## Screenshots

<!-- TODO: Add screenshots -->

## Prerequisites

- Windows 10/11 (64-bit)
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) (only if building from source)

## Download

Download the latest build from [Releases](../../releases).

Or build from source:

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o dist/
```

The executable will be at `dist/ClipboardAI.exe`.

## Configuration

Config is stored at `%APPDATA%\ClipboardAI\config.json` after first launch. You can edit it manually or use the built-in config GUI.

### Default Prompts

| Name | ID | System Prompt |
|---|---|---|
| 回答问题 | qa | Chinese Q&A, plain text output (no markdown) |
| 翻译中文 | translate | Translate to Simplified Chinese |
| 润色文本 | polish | Polish text professionally |
| AI 总结 | summarize | Summarize in Chinese |

## License

MIT
