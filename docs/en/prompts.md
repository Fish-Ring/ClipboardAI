> [简体中文](../zh-CN/prompts.md) · [Back to Home](../README.md)

# Prompt Presets Guide

## Contents

- [Built-in Presets](#built-in-presets)
- [Creating Custom Presets](#creating-custom-presets)
- [Preset Format](#preset-format)
- [Examples](#examples)

---

## Built-in Presets

| ID | Name | System Prompt |
|----|------|---------------|
| `qa` | Q&A | Answer in Chinese, plain text only — no Markdown formatting |
| `translate` | Translate | Translate to Simplified Chinese, output translation only |
| `polish` | Polish | Improve fluency and professionalism, keep original meaning |
| `summarize` | Summarize | Summarize key points in Chinese |

---

## Creating Custom Presets

1. Double-click tray icon to open Settings
2. Switch to «Prompts» tab
3. Click «Add»
4. Fill in ID, Name, System Prompt
5. Click «Save»

---

## Preset Format

```json
{
  "id": "your-preset-id",
  "name": "Preset Name",
  "systemPrompt": "Your system prompt text"
}
```

- `id`: Unique identifier, use lowercase English with underscores
- `name`: Display name shown in menu
- `systemPrompt`: System prompt sent to the AI

---

## Examples

### Code Review

```json
{
  "id": "code-review",
  "name": "Code Review",
  "systemPrompt": "Review the following code. Point out potential issues, performance optimizations, and security concerns. Answer in Chinese."
}
```

### Email Polish

```json
{
  "id": "email-polish",
  "name": "Email Polish",
  "systemPrompt": "Polish the following email to make it more professional and courteous while keeping the original intent. Output plain text."
}
```

### English to Chinese

```json
{
  "id": "en-translate",
  "name": "EN→ZH Translate",
  "systemPrompt": "Translate the following content into fluent, natural Chinese. Output translation only."
}
```
