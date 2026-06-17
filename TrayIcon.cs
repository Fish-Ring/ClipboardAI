using System.Drawing.Imaging;
using System.IO;

namespace ClipboardAI;

public class TrayIcon : IDisposable
{
    private readonly AppContext _context;
    private readonly NotifyIcon _notifyIcon;
    private readonly ContextMenuStrip _menu;
    private readonly Icon _idleIcon;
    private readonly Icon _doneIcon;
    private ToolStripMenuItem _enableItem = null!;

    private static readonly Color IdleColor = Color.FromArgb(0, 120, 215);
    private static readonly Color DoneColor = Color.FromArgb(0, 180, 80);

    public TrayIcon(AppContext context)
    {
        _context = context;
        _notifyIcon = new NotifyIcon();
        _menu = new ContextMenuStrip();

        _idleIcon = CreateIcon(IdleColor);
        _doneIcon = CreateIcon(DoneColor);
        _notifyIcon.Text = "ClipboardAI";
        _notifyIcon.Icon = _idleIcon;
        _notifyIcon.Visible = true;
        _notifyIcon.ContextMenuStrip = _menu;
        _notifyIcon.BalloonTipTitle = "ClipboardAI";
        if (_context.Config.IsTempMode)
            _notifyIcon.Text = "ClipboardAI [临时]";
        _notifyIcon.DoubleClick += (s, e) => _context.ShowConfigForm();

        BuildMenu();
    }

    private static Icon CreateIcon(Color bgColor)
    {
        var sizes = new[] { 16, 32, 48, 64 };
        var pngList = new List<byte[]>();

        foreach (var size in sizes)
        {
            using var bmp = new Bitmap(size, size);
            using var g = Graphics.FromImage(bmp);
            g.Clear(Color.Transparent);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var brush = new SolidBrush(bgColor);
            g.FillRectangle(brush, 0, 0, size, size);
            float fontSize = size * 9f / 16f;
            using var font = new Font("Segoe UI", fontSize, FontStyle.Bold, GraphicsUnit.Pixel);
            using var textBrush = new SolidBrush(Color.White);
            var sf = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center,
            };
            g.DrawString("CA", font, textBrush, new RectangleF(0, 0, size, size), sf);
            using var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            pngList.Add(ms.ToArray());
        }

        using var outStream = new MemoryStream();
        using var bw = new BinaryWriter(outStream);
        bw.Write((short)0);
        bw.Write((short)1);
        bw.Write((short)sizes.Length);

        int offset = 6 + sizes.Length * 16;
        for (int i = 0; i < sizes.Length; i++)
        {
            int w = sizes[i] >= 256 ? 0 : sizes[i];
            int h = sizes[i] >= 256 ? 0 : sizes[i];
            bw.Write((byte)w);
            bw.Write((byte)h);
            bw.Write((byte)0);
            bw.Write((byte)0);
            bw.Write((short)1);
            bw.Write((short)32);
            bw.Write(pngList[i].Length);
            bw.Write(offset);
            offset += pngList[i].Length;
        }

        foreach (var png in pngList)
            bw.Write(png);

        bw.Flush();
        outStream.Position = 0;
        return new Icon(outStream);
    }

    private void BuildMenu()
    {
        _menu.Items.Clear();

        _enableItem = new ToolStripMenuItem(I18n.Get("Menu.Enable")) { Checked = _context.IsEnabled };
        _enableItem.Click += (s, e) => _context.ToggleEnabled();
        _menu.Items.Add(_enableItem);

        _menu.Items.Add(new ToolStripSeparator());

        var promptItem = new ToolStripMenuItem(I18n.Get("Menu.Prompts"));
        promptItem.DropDownOpening += (s, e) => RebuildPromptSubmenu(promptItem);
        _menu.Items.Add(promptItem);

        var historyItem = new ToolStripMenuItem(I18n.Get("Menu.Recent"));
        historyItem.DropDownOpening += (s, e) => RebuildHistorySubmenu(historyItem);
        _menu.Items.Add(historyItem);

        _menu.Items.Add(new ToolStripSeparator());

        var settingsItem = new ToolStripMenuItem(I18n.Get("Menu.Settings"));
        settingsItem.DropDownItems.Add(new ToolStripMenuItem(I18n.Get("Menu.EditSettings"), null, (s, e) => _context.ShowConfigForm()));
        settingsItem.DropDownItems.Add(new ToolStripMenuItem(I18n.Get("Menu.OpenConfig"), null, (s, e) => OpenConfigFile()));
        settingsItem.DropDownItems.Add(new ToolStripMenuItem(I18n.Get("Menu.OpenConfigFolder"), null, (s, e) => OpenConfigFolder()));
        _menu.Items.Add(settingsItem);

        _menu.Items.Add(new ToolStripSeparator());

        var aboutItem = new ToolStripMenuItem(I18n.Get("Menu.About"));
        aboutItem.Click += (s, e) => _context.ShowAboutWindow();
        _menu.Items.Add(aboutItem);

        var exitItem = new ToolStripMenuItem(I18n.Get("Menu.Exit"));
        exitItem.Click += (s, e) => _context.Exit();
        _menu.Items.Add(exitItem);
    }

    public void RecreateMenu()
    {
        BuildMenu();
    }

    public void UpdateEnableState()
    {
        _enableItem.Checked = _context.IsEnabled;
    }

    private void RebuildPromptSubmenu(ToolStripMenuItem parent)
    {
        parent.DropDownItems.Clear();

        var cfg = _context.Config.Data;
        foreach (var p in cfg.Prompts)
        {
            var item = new ToolStripMenuItem(p.Name)
            {
                Checked = p.Id == cfg.CurrentPromptId,
            };
            var id = p.Id;
            item.Click += (s, e) => _context.SelectPrompt(id);
            parent.DropDownItems.Add(item);
        }
    }

    private void RebuildHistorySubmenu(ToolStripMenuItem parent)
    {
        parent.DropDownItems.Clear();

        var entries = _context.GetHistory().ToList();
        if (entries.Count == 0)
        {
            parent.DropDownItems.Add(new ToolStripMenuItem(I18n.Get("Menu.Empty")) { Enabled = false });
            return;
        }

        foreach (var entry in entries)
        {
            string prefix;
            switch (entry.Status)
            {
                case EntryStatus.Success: prefix = "✓"; break;
                case EntryStatus.Failed: prefix = "✗"; break;
                default: prefix = "⋯"; break;
            }

            var display = Truncate(entry.OriginalText, 45);
            var item = new ToolStripMenuItem($"{prefix} {display}");
            item.ToolTipText = $"[{entry.PromptName}]  {entry.Timestamp:yyyy-MM-dd HH:mm:ss}";

            if (entry.Status != EntryStatus.Processing)
            {
                var eRef = entry;
                item.Click += (s, e) => _context.ShowHistoryDetail(eRef);
            }
            else
            {
                item.Enabled = false;
            }

            parent.DropDownItems.Add(item);
        }

        parent.DropDownItems.Add(new ToolStripSeparator());
        var clearItem = new ToolStripMenuItem(I18n.Get("Menu.ClearHistory"));
        clearItem.Click += (s, e) => _context.ClearHistory();
        parent.DropDownItems.Add(clearItem);
    }

    private static void OpenConfigFile()
    {
        var path = Path.Combine(ConfigManager.DataFolderPath, "config.json");
        if (File.Exists(path))
        {
            try { System.Diagnostics.Process.Start("notepad.exe", path); }
            catch { }
        }
    }

    private static void OpenConfigFolder()
    {
        try { System.Diagnostics.Process.Start("explorer.exe", ConfigManager.DataFolderPath); }
        catch { }
    }

    private static void CopyToClipboard(string text)
    {
        try { System.Windows.Clipboard.SetText(text); }
        catch { }
    }

    private static string Truncate(string s, int maxLen)
    {
        if (string.IsNullOrEmpty(s)) return "";
        return s.Length <= maxLen ? s : s[..maxLen] + "...";
    }

    public void ShowNotification(string text, ToolTipIcon icon = ToolTipIcon.Info)
    {
        if (!_context.Config.Data.ShowNotifications) return;
        try
        {
            _notifyIcon.ShowBalloonTip(3000, "ClipboardAI", text, icon);
        }
        catch { }
    }

    public void SetStatusText(string text)
    {
        _notifyIcon.Text = text;
    }

    public void RestoreStatusText()
    {
        _notifyIcon.Text = "ClipboardAI";
    }

    public void Dispose()
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        _menu.Dispose();
    }
}
