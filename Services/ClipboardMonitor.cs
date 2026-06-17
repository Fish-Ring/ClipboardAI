using System.Windows.Interop;

namespace ClipboardAI.Services;

public class ClipboardMonitor : IDisposable
{
    private HwndSource? _hwndSource;
    private IntPtr _nextClipboardViewer;
    private bool _ownChange;
    private const int HOTKEY_ID = 9000;
    private bool _hotkeyRegistered;
    private DateTime _startTime;

    public event Action<string>? TextCopied;
    public event Action? HotkeyPressed;

    public void Start()
    {
        _startTime = DateTime.Now;

        _hwndSource = new HwndSource(0, 0, 0, 0, 0, 0, 0, "ClipboardAIMonitor", IntPtr.Zero);
        _hwndSource.AddHook(WndProc);

        _nextClipboardViewer = NativeMethods.SetClipboardViewer(_hwndSource.Handle);
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        switch (msg)
        {
            case NativeMethods.WM_DRAWCLIPBOARD:
                OnClipboardChange();
                NativeMethods.SendMessage(_nextClipboardViewer, msg, wParam, lParam);
                handled = true;
                break;

            case NativeMethods.WM_CHANGECBCHAIN:
                if (wParam == _nextClipboardViewer)
                    _nextClipboardViewer = lParam;
                else
                    NativeMethods.SendMessage(_nextClipboardViewer, msg, wParam, lParam);
                handled = true;
                break;

            case NativeMethods.WM_HOTKEY:
                if ((int)wParam == HOTKEY_ID)
                    HotkeyPressed?.Invoke();
                handled = true;
                break;
        }
        return IntPtr.Zero;
    }

    private void OnClipboardChange()
    {
        if (_ownChange)
        {
            _ownChange = false;
            return;
        }

        if ((DateTime.Now - _startTime).TotalMilliseconds < 1000) return;
        if (!System.Windows.Clipboard.ContainsText()) return;

        try
        {
            var text = System.Windows.Clipboard.GetText();
            if (!string.IsNullOrWhiteSpace(text))
                TextCopied?.Invoke(text);
        }
        catch { }
    }

    public void SetOwnChange()
    {
        _ownChange = true;
    }

    public void RegisterHotkey(bool enabled, string modifiers, string key)
    {
        UnregisterHotkey();
        _hotkeyRegistered = false;
        if (!enabled || _hwndSource == null) return;

        uint mods = ParseModifiers(modifiers);
        uint vk = ParseKey(key);
        if (mods == 0 || vk == 0) return;

        _hotkeyRegistered = NativeMethods.RegisterHotKey(_hwndSource.Handle, HOTKEY_ID, mods | NativeMethods.MOD_NOREPEAT, vk);
    }

    public void UnregisterHotkey()
    {
        if (_hotkeyRegistered && _hwndSource != null)
        {
            NativeMethods.UnregisterHotKey(_hwndSource.Handle, HOTKEY_ID);
            _hotkeyRegistered = false;
        }
    }

    private static uint ParseModifiers(string modStr)
    {
        if (string.IsNullOrWhiteSpace(modStr)) return 0;
        uint result = 0;
        foreach (var part in modStr.Split('+', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            switch (part.ToLower())
            {
                case "alt": result |= NativeMethods.MOD_ALT; break;
                case "ctrl": case "control": result |= NativeMethods.MOD_CONTROL; break;
                case "shift": result |= NativeMethods.MOD_SHIFT; break;
            }
        }
        return result;
    }

    private static uint ParseKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key)) return 0;
        key = key.Trim().ToUpper();
        if (key.Length == 1 && key[0] >= 'A' && key[0] <= 'Z') return key[0];
        if (key.Length == 1 && key[0] >= '0' && key[0] <= '9') return key[0];
        return 0;
    }

    public void Dispose()
    {
        UnregisterHotkey();
        if (_hwndSource != null)
        {
            NativeMethods.ChangeClipboardChain(_hwndSource.Handle, _nextClipboardViewer);
            _hwndSource.Dispose();
        }
    }
}
