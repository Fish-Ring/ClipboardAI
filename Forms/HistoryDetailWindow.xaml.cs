namespace ClipboardAI.Forms;

public partial class HistoryDetailWindow
{
    private readonly HistoryEntry _entry;

    public HistoryDetailWindow(HistoryEntry entry)
    {
        _entry = entry;
        InitializeComponent();
        ApplyI18n();
        LoadEntry();
    }

    private void ApplyI18n()
    {
        Title = I18n.Get("History.DetailTitle");
        grpOriginal.Header = I18n.Get("History.Original");
        grpResult.Header = I18n.Get("History.Response");
        grpError.Header = I18n.Get("History.Error");
        btnCopyOriginal.Content = I18n.Get("History.CopyOriginal");
        btnCopyResult.Content = I18n.Get("History.CopyResponse");
        btnCopyError.Content = I18n.Get("History.CopyError");
        btnClose.Content = I18n.Get("History.Close");
    }

    private void LoadEntry()
    {
        var status = _entry.Status switch
        {
            EntryStatus.Success => I18n.Get("History.Success"),
            EntryStatus.Failed => I18n.Get("History.Failed"),
            _ => I18n.Get("History.Processing")
        };
        lblHeader.Text = $"[{_entry.PromptName}]  {_entry.Timestamp:yyyy-MM-dd HH:mm:ss}  —  {status}";
        txtOriginal.Text = _entry.OriginalText;

        if (_entry.Status == EntryStatus.Failed)
        {
            txtResult.Text = string.IsNullOrEmpty(_entry.ResultText) ? I18n.Get("History.NoResponse") : _entry.ResultText;
            txtError.Text = _entry.ErrorMessage ?? I18n.Get("History.UnknownError");
            grpError.Visibility = System.Windows.Visibility.Visible;
        }
        else if (_entry.Status == EntryStatus.Processing)
        {
            txtResult.Text = I18n.Get("History.ProcessingStatus");
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
