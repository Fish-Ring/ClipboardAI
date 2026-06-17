using System.IO;
using System.Media;
using ClipboardAI.Forms;
using ClipboardAI.Services;
using Clipboard = System.Windows.Clipboard;

namespace ClipboardAI;

public class AppContext
{
    public ConfigManager Config { get; }
    public bool IsEnabled { get; private set; } = true;

    private readonly ClipboardMonitor _monitor;
    private readonly LlmService _llm;
    private readonly HistoryManager _history;
    private readonly TrayIcon _tray;
    private ConfigWindow? _configWindow;
    private CancellationTokenSource? _cts;
    private bool _processing;

    public AppContext(string? tempConfigPath = null)
    {
        Config = new ConfigManager(tempConfigPath);
        _monitor = new ClipboardMonitor();
        _llm = new LlmService(Config);
        _history = new HistoryManager(Config.IsTempMode);
        I18n.Lang = Config.Data.Language;
        _tray = new TrayIcon(this);

        _history.SetMaxEntries(Config.Data.MaxHistory);
        IsEnabled = Config.Data.Enabled;

        _monitor.Start();
        _monitor.TextCopied += OnTextCopied;
        _monitor.HotkeyPressed += OnHotkeyPressed;
        RegisterHotkeyFromConfig();

        Config.ConfigChanged += OnConfigChanged;

        if (Config.IsTempMode)
            _tray.ShowNotification(I18n.Get("Status.TempConfig"));
    }

    private async void OnTextCopied(string text)
    {
        if (!IsEnabled || _processing || !Config.Data.ClipboardMonitoringEnabled) return;
        await ProcessClipboardText(text);
    }

    private async void OnHotkeyPressed()
    {
        if (_processing) return;
        try
        {
            var text = Services.SelectionReader.GetSelectedText();
            if (string.IsNullOrWhiteSpace(text))
                text = await TryGetTextViaClipboard();
            if (!string.IsNullOrWhiteSpace(text))
                await ProcessClipboardText(text);
        }
        catch { }
    }

    private async Task<string?> TryGetTextViaClipboard()
    {
        try
        {
            var savedData = Clipboard.GetDataObject();
            NativeMethods.SendCtrlCWithAltRelease();
            await Task.Delay(200);
            var text = Clipboard.GetText();
            if (!string.IsNullOrWhiteSpace(text))
                return text;
            return null;
        }
        catch
        {
            return null;
        }
    }

    private async Task ProcessClipboardText(string text)
    {
        _processing = true;
        _cts = CancellationTokenSource.CreateLinkedTokenSource(
            new CancellationTokenSource(TimeSpan.FromSeconds(30)).Token);

        var promptName = Config.GetCurrentPromptName() ?? "?";
        var entry = new HistoryEntry
        {
            Timestamp = DateTime.Now,
            OriginalText = text,
            PromptName = promptName,
            Status = EntryStatus.Processing,
        };
        _history.AddEntry(entry);

        try
        {
            if (Config.Data.ShowNotifications)
                _tray.ShowNotification(I18n.Format("Status.ProcessingWith", promptName));
            else
                _tray.SetStatusText(I18n.Format("Status.Processing", promptName));

            var result = await _llm.ProcessTextAsync(text, _cts.Token);

            if (result != null)
            {
                _monitor.SetOwnChange();
                await SetClipboardWithRetry(result);

                entry.Status = EntryStatus.Success;
                entry.ResultText = result;

                if (Config.Data.PlaySoundOnComplete)
                    PlayBeepSound();

                var preview = result.Length > 50 ? result[..50] + "..." : result;
                _tray.SetStatusText(I18n.Format("Status.Done", preview, _history.Count));
                if (Config.Data.ShowNotifications)
                    _tray.ShowNotification(I18n.Format("Status.Done", preview, _history.Count));
            }
            else
            {
                entry.Status = EntryStatus.Failed;
                entry.ErrorMessage = I18n.Get("Status.AIEmptyResult");
                _tray.SetStatusText(I18n.Format("Status.Failed", I18n.Get("Status.AIEmptyResult"), _history.Count));
                if (Config.Data.ShowNotifications)
                    _tray.ShowNotification(I18n.Get("Status.AIEmptyResult"), ToolTipIcon.Error);
            }
        }
        catch (OperationCanceledException)
        {
            entry.Status = EntryStatus.Failed;
            entry.ErrorMessage = I18n.Get("Status.TimeoutCancelled");
            _tray.SetStatusText(I18n.Format("Status.TimeoutCancelCount", _history.Count));
            if (Config.Data.ShowNotifications)
                _tray.ShowNotification(I18n.Get("Status.TimeoutCancelled"), ToolTipIcon.Warning);
        }
        catch (Exception ex)
        {
            entry.Status = EntryStatus.Failed;
            entry.ErrorMessage = ex.Message;
            _tray.SetStatusText(I18n.Format("Status.Failed", ex.Message, _history.Count));
            if (Config.Data.ShowNotifications)
                _tray.ShowNotification(I18n.Format("Status.Failed", ex.Message, _history.Count), ToolTipIcon.Error);
        }
        finally
        {
            _processing = false;
        }
    }

    private void OnConfigChanged()
    {
        _history.SetMaxEntries(Config.Data.MaxHistory);
        IsEnabled = Config.Data.Enabled;
        I18n.Lang = Config.Data.Language;
        RegisterHotkeyFromConfig();
        _tray.RecreateMenu();
        _tray.UpdateEnableState();
    }

    private void RegisterHotkeyFromConfig()
    {
        _monitor.RegisterHotkey(
            Config.Data.HotkeyEnabled,
            Config.Data.HotkeyModifiers,
            Config.Data.HotkeyKey);
    }

    public void ToggleEnabled()
    {
        IsEnabled = !IsEnabled;
        Config.Data.Enabled = IsEnabled;
        Config.Save();
        _tray.UpdateEnableState();
        _tray.ShowNotification(IsEnabled ? I18n.Get("Status.Enabled") : I18n.Get("Status.Disabled"));
    }

    public void SelectPrompt(string id)
    {
        Config.SetCurrentPrompt(id);
        OnConfigChanged();
        _tray.ShowNotification(I18n.Format("Status.SwitchPrompt", Config.GetCurrentPromptName()));
    }

    public void ShowAboutWindow()
    {
        var win = new Forms.AboutWindow();
        win.ShowDialog();
    }

    public void ShowConfigForm()
    {
        if (_configWindow != null && _configWindow.IsLoaded)
        {
            _configWindow.Activate();
            return;
        }

        _configWindow = new ConfigWindow(Config);
        _configWindow.Closed += (s, e) => _configWindow = null;
        _configWindow.Show();
    }

    public IEnumerable<HistoryEntry> GetHistory() => _history.GetEntries();

    public void ClearHistory() => _history.Clear();

    public void ShowHistoryDetail(HistoryEntry entry)
    {
        var win = new Forms.HistoryDetailWindow(entry);
        win.ShowDialog();
    }

    private static async Task SetClipboardWithRetry(string text, int maxRetries = 15)
    {
        for (int i = 0; i < maxRetries; i++)
        {
            try
            {
                Clipboard.SetText(text);
                return;
            }
            catch
            {
                if (i >= maxRetries - 1) throw;
                await Task.Delay(200 + i * 50);
            }
        }
    }

    private static void PlayBeepSound()
    {
        try
        {
            var sampleRate = 8000;
            var freq = 880;
            var durationMs = 120;
            var samples = sampleRate * durationMs / 1000;

            using var ms = new MemoryStream();
            using var bw = new BinaryWriter(ms);
            var dataSize = samples;
            bw.Write(0x46464952); // "RIFF"
            bw.Write(36 + dataSize);
            bw.Write(0x45564157); // "WAVE"
            bw.Write(0x20746D66); // "fmt "
            bw.Write(16);
            bw.Write((short)1);
            bw.Write((short)1);
            bw.Write(sampleRate);
            bw.Write(sampleRate);
            bw.Write((short)1);
            bw.Write((short)8);
            bw.Write(0x61746164); // "data"
            bw.Write(dataSize);

            for (int i = 0; i < samples; i++)
            {
                var sample = (byte)(128 + 127 * Math.Sin(2 * Math.PI * freq * i / sampleRate));
                bw.Write(sample);
            }

            bw.Flush();
            ms.Position = 0;
            using var player = new SoundPlayer(ms);
            player.PlaySync();
        }
        catch { }
    }

    public void Exit()
    {
        _cts?.Cancel();
        _monitor.TextCopied -= OnTextCopied;
        _monitor.Dispose();
        Config.Dispose();
        _tray.Dispose();
        System.Windows.Application.Current.Shutdown();
    }
}
