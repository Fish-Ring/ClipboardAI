using ClipboardAI.Models;

namespace ClipboardAI;

public static class I18n
{
    private static readonly Dictionary<string, Dictionary<string, string>> Data = new()
    {
        ["zh-CN"] = new()
        {
            // Menu
            ["Menu.Enable"] = "已启用",
            ["Menu.Prompts"] = "Prompt 预设",
            ["Menu.Recent"] = "最近处理",
            ["Menu.Settings"] = "设置",
            ["Menu.EditSettings"] = "编辑设置",
            ["Menu.OpenConfig"] = "打开配置文件",
            ["Menu.OpenConfigFolder"] = "打开配置文件夹",
            ["Menu.About"] = "关于 ClipboardAI",
            ["Menu.Exit"] = "退出",
            ["Menu.Empty"] = "(空)",
            ["Menu.ClearHistory"] = "清空历史",

            // Status
            ["Status.ProcessingWith"] = "正在使用「{0}」处理...",
            ["Status.Processing"] = "正在处理 [{0}]...",
            ["Status.Done"] = "完成: {0} ({1}条)",
            ["Status.Failed"] = "处理失败: {0} ({1}条)",
            ["Status.TimeoutCancelled"] = "处理超时已取消",
            ["Status.AIEmptyResult"] = "AI 返回空结果",
            ["Status.TimeoutCancelCount"] = "处理超时已取消 ({0}条)",
            ["Status.Enabled"] = "已启用",
            ["Status.Disabled"] = "已禁用",
            ["Status.SwitchPrompt"] = "已切换至「{0}」",

            // Config
            ["Config.Title"] = "ClipboardAI 配置",
            ["Config.TabApi"] = "API 设置",
            ["Config.ApiUrl"] = "API 地址:",
            ["Config.ApiKey"] = "API Key:",
            ["Config.Model"] = "模型:",
            ["Config.MaxTokens"] = "Max Tokens:",
            ["Config.Temperature"] = "Temperature:",
            ["Config.TabPrompts"] = "Prompt 预设",
            ["Config.AddPrompt"] = "添加",
            ["Config.DeletePrompt"] = "删除",
            ["Config.Id"] = "ID:",
            ["Config.Name"] = "名称:",
            ["Config.SystemPrompt"] = "System Prompt:",
            ["Config.TabHotkeys"] = "快捷键",
            ["Config.EnableHotkey"] = "启用快捷键:",
            ["Config.EnableHotkeyCheck"] = "启用全局快捷键",
            ["Config.Modifiers"] = "修饰键:",
            ["Config.Alt"] = "Alt",
            ["Config.Ctrl"] = "Ctrl",
            ["Config.Shift"] = "Shift",
            ["Config.Key"] = "按键:",
            ["Config.TabOther"] = "其他",
            ["Config.MaxHistory"] = "历史记录上限:",
            ["Config.Notifications"] = "显示通知:",
            ["Config.NotificationsCheck"] = "显示Windows托盘通知",
            ["Config.ClipboardMonitor"] = "剪贴板监控:",
            ["Config.ClipboardMonitorCheck"] = "监控剪贴板变化并自动处理",
            ["Config.PlaySound"] = "完成提示音:",
            ["Config.PlaySoundCheck"] = "AI返回结果后播放提示音",
            ["Config.Save"] = "保存",
            ["Config.Cancel"] = "取消",
            ["Config.Language"] = "语言:",
            ["Config.LangZh"] = "中文",
            ["Config.LangEn"] = "English",

            // History
            ["History.Original"] = "原文",
            ["History.Response"] = "回答",
            ["History.Error"] = "错误",
            ["History.CopyOriginal"] = "复制原文",
            ["History.CopyResponse"] = "复制回答",
            ["History.CopyError"] = "复制错误",
            ["History.Close"] = "关闭",
            ["History.Success"] = "✓ 成功",
            ["History.Failed"] = "✗ 失败",
            ["History.Processing"] = "⋯ 处理中",
            ["History.NoResult"] = "(无结果)",
            ["History.NoResponse"] = "(无返回)",
            ["History.UnknownError"] = "未知错误",
            ["History.ProcessingStatus"] = "(处理中...)",
            ["History.DetailTitle"] = "处理详情",

            // About
            ["About.Title"] = "关于 ClipboardAI",
            ["About.Version"] = "版本 {0}",
            ["About.GitHub"] = "访问 GitHub",
            ["About.CheckUpdate"] = "检查更新",
            ["About.UpToDate"] = "已是最新版本",
            ["About.NewVersion"] = "发现新版本 {0}",
            ["About.Download"] = "前往下载",
            ["About.Checking"] = "检查中...",
            ["About.CheckFailed"] = "检查更新失败",
            ["About.Close"] = "关闭",
        },
        ["en"] = new()
        {
            ["Menu.Enable"] = "Enabled",
            ["Menu.Prompts"] = "Prompts",
            ["Menu.Recent"] = "Recent",
            ["Menu.Settings"] = "Settings",
            ["Menu.EditSettings"] = "Edit Settings",
            ["Menu.OpenConfig"] = "Open Config File",
            ["Menu.OpenConfigFolder"] = "Open Config Folder",
            ["Menu.About"] = "About ClipboardAI",
            ["Menu.Exit"] = "Exit",
            ["Menu.Empty"] = "(empty)",
            ["Menu.ClearHistory"] = "Clear History",

            ["Status.ProcessingWith"] = "Processing with 「{0}」...",
            ["Status.Processing"] = "Processing [{0}]...",
            ["Status.Done"] = "Done: {0} ({1})",
            ["Status.Failed"] = "Failed: {0} ({1})",
            ["Status.TimeoutCancelled"] = "Timeout cancelled",
            ["Status.AIEmptyResult"] = "AI returned empty result",
            ["Status.TimeoutCancelCount"] = "Timeout cancelled ({0})",
            ["Status.Enabled"] = "Enabled",
            ["Status.Disabled"] = "Disabled",
            ["Status.SwitchPrompt"] = "Switched to 「{0}」",

            ["Config.Title"] = "ClipboardAI Settings",
            ["Config.TabApi"] = "API Settings",
            ["Config.ApiUrl"] = "API Base URL:",
            ["Config.ApiKey"] = "API Key:",
            ["Config.Model"] = "Model:",
            ["Config.MaxTokens"] = "Max Tokens:",
            ["Config.Temperature"] = "Temperature:",
            ["Config.TabPrompts"] = "Prompts",
            ["Config.AddPrompt"] = "Add",
            ["Config.DeletePrompt"] = "Delete",
            ["Config.Id"] = "ID:",
            ["Config.Name"] = "Name:",
            ["Config.SystemPrompt"] = "System Prompt:",
            ["Config.TabHotkeys"] = "Hotkeys",
            ["Config.EnableHotkey"] = "Enable Hotkey:",
            ["Config.EnableHotkeyCheck"] = "Enable global hotkey",
            ["Config.Modifiers"] = "Modifiers:",
            ["Config.Alt"] = "Alt",
            ["Config.Ctrl"] = "Ctrl",
            ["Config.Shift"] = "Shift",
            ["Config.Key"] = "Key:",
            ["Config.TabOther"] = "Other",
            ["Config.MaxHistory"] = "Max History:",
            ["Config.Notifications"] = "Notifications:",
            ["Config.NotificationsCheck"] = "Show tray notifications",
            ["Config.ClipboardMonitor"] = "Clipboard Monitor:",
            ["Config.ClipboardMonitorCheck"] = "Auto-process clipboard changes",
            ["Config.PlaySound"] = "Completion Sound:",
            ["Config.PlaySoundCheck"] = "Play sound on completion",
            ["Config.Save"] = "Save",
            ["Config.Cancel"] = "Cancel",
            ["Config.Language"] = "Language:",
            ["Config.LangZh"] = "中文",
            ["Config.LangEn"] = "English",

            ["History.Original"] = "Original",
            ["History.Response"] = "Response",
            ["History.Error"] = "Error",
            ["History.CopyOriginal"] = "Copy Original",
            ["History.CopyResponse"] = "Copy Response",
            ["History.CopyError"] = "Copy Error",
            ["History.Close"] = "Close",
            ["History.Success"] = "✓ Success",
            ["History.Failed"] = "✗ Failed",
            ["History.Processing"] = "⋯ Processing",
            ["History.NoResult"] = "(no result)",
            ["History.NoResponse"] = "(no response)",
            ["History.UnknownError"] = "Unknown error",
            ["History.ProcessingStatus"] = "(processing...)",
            ["History.DetailTitle"] = "Processing Detail",

            ["About.Title"] = "About ClipboardAI",
            ["About.Version"] = "Version {0}",
            ["About.GitHub"] = "Visit GitHub",
            ["About.CheckUpdate"] = "Check for Updates",
            ["About.UpToDate"] = "Already up to date",
            ["About.NewVersion"] = "New version {0} available",
            ["About.Download"] = "Download",
            ["About.Checking"] = "Checking...",
            ["About.CheckFailed"] = "Update check failed",
            ["About.Close"] = "Close",
        },
    };

    public static string Lang { get; set; } = "zh-CN";

    private static Dictionary<string, string> Strings =>
        Data.GetValueOrDefault(Lang) ?? Data["en"]!;

    public static string Get(string key)
    {
        var s = Strings;
        return s.GetValueOrDefault(key, key);
    }

    public static string Format(string key, params object?[] args)
    {
        var fmt = Get(key);
        try { return string.Format(fmt, args); }
        catch { return fmt; }
    }
}
