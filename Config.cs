using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using ClipboardAI.Models;

namespace ClipboardAI;

public class ConfigData
{
    public string ApiBase { get; set; } = "https://api.openai.com/v1";
    public string ApiKey { get; set; } = "";
    public string Model { get; set; } = "gpt-4o-mini";
    public int MaxTokens { get; set; } = 2048;
    public double Temperature { get; set; } = 0.3;
    public string CurrentPromptId { get; set; } = "";
    public List<PromptPreset> Prompts { get; set; } = new();
    public double PollInterval { get; set; } = 0.5;
    public bool Enabled { get; set; } = true;
    public int MaxHistory { get; set; } = 20;
    public bool HotkeyEnabled { get; set; } = true;
    public string HotkeyModifiers { get; set; } = "Alt";
    public string HotkeyKey { get; set; } = "C";
    public bool ShowNotifications { get; set; } = false;
    public bool ClipboardMonitoringEnabled { get; set; } = false;
    public bool PlaySoundOnComplete { get; set; } = false;

}

public class ConfigManager : IDisposable
{
    private static readonly string AppDataDir = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ClipboardAI");
    private static readonly string ConfigPath = Path.Combine(AppDataDir, "config.json");
    public static string DataFolderPath => AppDataDir;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private ConfigData _data = new();
    private FileSystemWatcher? _watcher;
    private bool _suppressReload;
    private readonly object _lock = new();

    public ConfigData Data
    {
        get { lock (_lock) return _data; }
    }

    public event Action? ConfigChanged;

    public ConfigManager()
    {
        Load();
        StartWatching();
    }

    public void Load()
    {
        try
        {
            Directory.CreateDirectory(AppDataDir);
            if (File.Exists(ConfigPath))
            {
                var json = File.ReadAllText(ConfigPath);
                var data = JsonSerializer.Deserialize<ConfigData>(json, _jsonOptions);
                if (data != null)
                {
                    EnsurePrompts(data, json);
                    lock (_lock) _data = data;
                }
            }
            else
            {
                SaveDefault();
            }
        }
        catch
        {
            SaveDefault();
        }
    }

    private void EnsurePrompts(ConfigData data, string? rawJson = null)
    {
        if (data.Prompts.Count == 0 && rawJson != null)
        {
            try
            {
                using var doc = JsonDocument.Parse(rawJson);
                if (doc.RootElement.TryGetProperty("system_prompt", out var oldPrompt))
                {
                    data.Prompts.Add(new PromptPreset
                    {
                        Id = "default",
                        Name = "默认",
                        SystemPrompt = oldPrompt.GetString() ?? ""
                    });
                    data.CurrentPromptId = "default";
                }
            }
            catch { }
        }

        if (data.Prompts.Count == 0)
        {
            data.Prompts.Add(new PromptPreset
            {
                Id = "default",
                Name = "默认",
                SystemPrompt = "将以下内容翻译为简体中文，只输出翻译结果"
            });
            data.CurrentPromptId = "default";
        }

        var qaPrompt = data.Prompts.FirstOrDefault(p => p.Id == "qa");
        if (qaPrompt != null && qaPrompt.SystemPrompt == "请用中文回答以下问题，简洁明了，只输出纯文本，不要任何格式标记")
            qaPrompt.SystemPrompt = "用中文回答问题，简洁明了。输出必须为纯文本，严禁使用任何Markdown格式符号（包括**加粗**、*斜体*、`代码`、#标题、-列表、>引用、[链接](url)、---分割线等）。只返回纯文字。";

        if (!data.Prompts.Any(p => p.Id == data.CurrentPromptId))
            data.CurrentPromptId = data.Prompts.First().Id;
    }

    public void Save()
    {
        lock (_lock)
        {
            _suppressReload = true;
            var json = JsonSerializer.Serialize(_data, _jsonOptions);
            File.WriteAllText(ConfigPath, json);
            _suppressReload = false;
        }
    }

    private void SaveDefault()
    {
        var d = new ConfigData();
        d.Prompts.Add(new PromptPreset { Id = "qa", Name = "回答问题", SystemPrompt = "用中文回答问题，简洁明了。输出必须为纯文本，严禁使用任何Markdown格式符号（包括**加粗**、*斜体*、`代码`、#标题、-列表、>引用、[链接](url)、---分割线等）。只返回纯文字。" });
        d.Prompts.Add(new PromptPreset { Id = "translate", Name = "翻译中文", SystemPrompt = "将以下内容翻译为简体中文，只输出翻译结果" });
        d.Prompts.Add(new PromptPreset { Id = "polish", Name = "润色文本", SystemPrompt = "润色以下文本，使其更通顺专业，保持原意，只输出润色结果" });
        d.Prompts.Add(new PromptPreset { Id = "summarize", Name = "AI 总结", SystemPrompt = "用中文总结以下内容的核心要点，简洁明了" });
        d.CurrentPromptId = "qa";
        lock (_lock) _data = d;
        Save();
    }

    private void StartWatching()
    {
        var dir = Path.GetDirectoryName(ConfigPath);
        if (dir == null || !Directory.Exists(dir)) return;

        _watcher = new FileSystemWatcher(dir)
        {
            Filter = "config.json",
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime,
            EnableRaisingEvents = true,
        };

        var lastTrigger = DateTime.MinValue;
        _watcher.Changed += (s, e) =>
        {
            if (_suppressReload) return;
            var now = DateTime.Now;
            if ((now - lastTrigger).TotalMilliseconds < 300) return;
            lastTrigger = now;
            Thread.Sleep(50);
            Load();
            ConfigChanged?.Invoke();
        };
    }

    public string? GetCurrentSystemPrompt()
    {
        lock (_lock)
        {
            var p = _data.Prompts.FirstOrDefault(p => p.Id == _data.CurrentPromptId);
            return p?.SystemPrompt;
        }
    }

    public string? GetCurrentPromptName()
    {
        lock (_lock)
        {
            var p = _data.Prompts.FirstOrDefault(p => p.Id == _data.CurrentPromptId);
            return p?.Name;
        }
    }

    public void SetCurrentPrompt(string id)
    {
        lock (_lock)
        {
            if (_data.Prompts.Any(p => p.Id == id))
            {
                _data.CurrentPromptId = id;
                Save();
            }
        }
    }

    public void NotifyChanged()
    {
        ConfigChanged?.Invoke();
    }

    public void Dispose()
    {
        _watcher?.Dispose();
    }
}
