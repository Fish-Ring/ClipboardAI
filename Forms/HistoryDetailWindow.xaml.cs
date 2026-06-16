namespace ClipboardAI.Forms;

public partial class HistoryDetailWindow
{
    private readonly HistoryEntry _entry;

    public HistoryDetailWindow(HistoryEntry entry)
    {
        _entry = entry;
        InitializeComponent();
        LoadEntry();
    }

    private void LoadEntry()
    {
        var status = _entry.Status switch
        {
            EntryStatus.Success => "✓ 成功",
            EntryStatus.Failed => "✗ 失败",
            _ => "⋯ 处理中"
        };
        lblHeader.Text = $"[{_entry.PromptName}]  {_entry.Timestamp:yyyy-MM-dd HH:mm:ss}  —  {status}";
        txtOriginal.Text = _entry.OriginalText;

        if (_entry.Status == EntryStatus.Failed)
        {
            txtResult.Text = string.IsNullOrEmpty(_entry.ResultText) ? "(无返回)" : _entry.ResultText;
            txtError.Text = _entry.ErrorMessage ?? "未知错误";
            grpError.Visibility = System.Windows.Visibility.Visible;
        }
        else if (_entry.Status == EntryStatus.Processing)
        {
            txtResult.Text = "(处理中...)";
            grpError.Visibility = System.Windows.Visibility.Collapsed;
        }
        else
        {
            txtResult.Text = _entry.ResultText;
            grpError.Visibility = System.Windows.Visibility.Collapsed;
        }
    }

    private void BtnCopyOriginal_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        try { System.Windows.Clipboard.SetText(txtOriginal.Text); } catch { }
    }

    private void BtnCopyResult_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        try { System.Windows.Clipboard.SetText(txtResult.Text); } catch { }
    }

    private void BtnCopyError_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        try { System.Windows.Clipboard.SetText(txtError.Text); } catch { }
    }

    private void BtnClose_Click(object sender, System.Windows.RoutedEventArgs e) => Close();
}
