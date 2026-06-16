using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClipboardAI;

public enum EntryStatus { Processing, Success, Failed }

public class HistoryEntry
{
    public DateTime Timestamp { get; set; }
    public string OriginalText { get; set; } = "";
    public string ResultText { get; set; } = "";
    public string PromptName { get; set; } = "";
    public EntryStatus Status { get; set; } = EntryStatus.Processing;
    public string? ErrorMessage { get; set; }
}

public class HistoryManager
{
    private static readonly string HistoryPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "ClipboardAI", "history.json");

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
    };

    private readonly LinkedList<HistoryEntry> _entries = new();
    private int _maxEntries = 20;

    public int Count => _entries.Count;

    public HistoryManager()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(HistoryPath)!);
        Load();
    }

    public void AddEntry(HistoryEntry entry)
    {
        _entries.AddFirst(entry);
        Trim();
        Save();
    }

    public void SetMaxEntries(int max)
    {
        _maxEntries = Math.Max(1, max);
        Trim();
    }

    private void Trim()
    {
        while (_entries.Count > _maxEntries)
            _entries.RemoveLast();
    }

    public IEnumerable<HistoryEntry> GetEntries() => _entries;

    public void Clear()
    {
        _entries.Clear();
        Save();
    }

    private void Save()
    {
        try
        {
            var list = _entries.ToList();
            var json = JsonSerializer.Serialize(list, JsonOpts);
            File.WriteAllText(HistoryPath, json);
        }
        catch { }
    }

    private void Load()
    {
        try
        {
            if (!File.Exists(HistoryPath)) return;
            var json = File.ReadAllText(HistoryPath);
            var list = JsonSerializer.Deserialize<List<HistoryEntry>>(json, JsonOpts);
            if (list == null || list.Count == 0) return;
            foreach (var entry in list)
                _entries.AddLast(entry);
            Trim();
        }
        catch
        {
            _entries.Clear();
        }
    }
}
