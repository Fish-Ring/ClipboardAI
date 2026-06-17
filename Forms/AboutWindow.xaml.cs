using System.Net.Http;
using System.Text.Json;

namespace ClipboardAI.Forms;

public partial class AboutWindow
{
    private const string CurrentVersion = "0.0.5";
    private static readonly HttpClient Http = new();
    private string? _latestVersion;

    public AboutWindow()
    {
        InitializeComponent();
        ApplyI18n();
    }

    private void ApplyI18n()
    {
        Title = I18n.Get("About.Title");
        lblVersion.Text = I18n.Format("About.Version", CurrentVersion);
        btnGitHub.Content = I18n.Get("About.GitHub");
        btnCheckUpdate.Content = I18n.Get("About.CheckUpdate");
        btnClose.Content = I18n.Get("About.Close");
    }

    private async void BtnCheckUpdate_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        btnCheckUpdate.IsEnabled = false;
        lblUpdateResult.Text = I18n.Get("About.Checking");
        lblUpdateResult.Visibility = System.Windows.Visibility.Visible;

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get,
                "https://api.github.com/repos/Fish-Ring/ClipboardAI/releases/latest");
            request.Headers.UserAgent.ParseAdd("ClipboardAI");
            using var response = await Http.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var tag = doc.RootElement.GetProperty("tag_name").GetString() ?? "";
            _latestVersion = tag.TrimStart('v');

            var compare = string.Compare(_latestVersion, CurrentVersion, StringComparison.Ordinal);
            if (compare > 0)
            {
                lblUpdateResult.Text = I18n.Format("About.NewVersion", _latestVersion);
                btnGitHub.Content = I18n.Get("About.Download");
            }
            else
            {
                lblUpdateResult.Text = I18n.Get("About.UpToDate");
            }
        }
        catch
        {
            lblUpdateResult.Text = I18n.Get("About.CheckFailed");
        }

        btnCheckUpdate.IsEnabled = true;
    }

    private void BtnGitHub_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        try
        {
            var url = _latestVersion != null && string.Compare(_latestVersion, CurrentVersion, StringComparison.Ordinal) > 0
                ? $"https://github.com/Fish-Ring/ClipboardAI/releases/tag/v{_latestVersion}"
                : "https://github.com/Fish-Ring/ClipboardAI";
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(url)
            {
                UseShellExecute = true
            });
        }
        catch { }
    }

    private void BtnClose_Click(object sender, System.Windows.RoutedEventArgs e) => Close();
}
