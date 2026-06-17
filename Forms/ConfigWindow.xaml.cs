using ClipboardAI.Models;

namespace ClipboardAI.Forms;

public partial class ConfigWindow
{
    private readonly ConfigManager _config;
    private PromptPreset? _lastSelectedPreset;

    public ConfigWindow(ConfigManager config)
    {
        _config = config;
        InitializeComponent();
        ApplyI18n();

        chkHotkeyEnabled.Checked += (s, e) => UpdateHotkeyControls();
        chkHotkeyEnabled.Unchecked += (s, e) => UpdateHotkeyControls();
        cboLanguage.SelectionChanged += OnLanguageChanged;

        LoadConfig();
    }

    private void ApplyI18n()
    {
        Title = I18n.Get("Config.Title");
        tabApi.Header = I18n.Get("Config.TabApi");
        tabPrompts.Header = I18n.Get("Config.TabPrompts");
        tabHotkeys.Header = I18n.Get("Config.TabHotkeys");
        tabOther.Header = I18n.Get("Config.TabOther");

        lblApiUrl.Text = I18n.Get("Config.ApiUrl");
        lblApiKey.Text = I18n.Get("Config.ApiKey");
        lblModel.Text = I18n.Get("Config.Model");
        lblMaxTokens.Text = I18n.Get("Config.MaxTokens");
        lblTemperature.Text = I18n.Get("Config.Temperature");

        btnAddPrompt.Content = I18n.Get("Config.AddPrompt");
        btnDeletePrompt.Content = I18n.Get("Config.DeletePrompt");
        lblPromptId.Text = I18n.Get("Config.Id");
        lblPromptName.Text = I18n.Get("Config.Name");
        lblSystemPrompt.Text = I18n.Get("Config.SystemPrompt");

        lblEnableHotkey.Text = I18n.Get("Config.EnableHotkey");
        chkHotkeyEnabled.Content = I18n.Get("Config.EnableHotkeyCheck");
        lblModifiers.Text = I18n.Get("Config.Modifiers");
        chkHotkeyAlt.Content = I18n.Get("Config.Alt");
        chkHotkeyCtrl.Content = I18n.Get("Config.Ctrl");
        chkHotkeyShift.Content = I18n.Get("Config.Shift");
        lblKey.Text = I18n.Get("Config.Key");

        lblMaxHistory.Text = I18n.Get("Config.MaxHistory");
        lblNotifications.Text = I18n.Get("Config.Notifications");
        chkShowNotifications.Content = I18n.Get("Config.NotificationsCheck");
        lblClipboardMonitor.Text = I18n.Get("Config.ClipboardMonitor");
        chkClipboardMonitor.Content = I18n.Get("Config.ClipboardMonitorCheck");
        lblPlaySound.Text = I18n.Get("Config.PlaySound");
        chkPlaySound.Content = I18n.Get("Config.PlaySoundCheck");

        lblLanguage.Text = I18n.Get("Config.Language");
        btnSave.Content = I18n.Get("Config.Save");
        btnCancel.Content = I18n.Get("Config.Cancel");
    }

    private void LoadConfig()
    {
        var data = _config.Data;
        txtApiBase.Text = data.ApiBase;
        txtApiKey.Password = data.ApiKey;
        txtModel.Text = data.Model;
        txtMaxTokens.Text = data.MaxTokens.ToString();
        sldTemperature.Value = data.Temperature;
        lblTemperatureVal.Text = data.Temperature.ToString("F1");

        chkHotkeyEnabled.IsChecked = data.HotkeyEnabled;
        chkHotkeyAlt.IsChecked = data.HotkeyModifiers?.Contains("Alt") ?? false;
        chkHotkeyCtrl.IsChecked = data.HotkeyModifiers?.Contains("Ctrl") ?? false;
        chkHotkeyShift.IsChecked = data.HotkeyModifiers?.Contains("Shift") ?? false;

        cboHotkeyKey.Items.Clear();
        for (char c = 'A'; c <= 'Z'; c++)
            cboHotkeyKey.Items.Add(c.ToString());
        var hotkey = data.HotkeyKey?.ToUpper();
        cboHotkeyKey.SelectedItem = !string.IsNullOrEmpty(hotkey) && cboHotkeyKey.Items.Contains(hotkey)
            ? hotkey : "C";

        txtMaxHistory.Text = data.MaxHistory.ToString();
        chkShowNotifications.IsChecked = data.ShowNotifications;
        chkClipboardMonitor.IsChecked = data.ClipboardMonitoringEnabled;
        chkPlaySound.IsChecked = data.PlaySoundOnComplete;

        lstPrompts.ItemsSource = data.Prompts;
        if (data.Prompts.Count > 0)
        {
            var selected = data.Prompts.FirstOrDefault(p => p.Id == data.CurrentPromptId);
            lstPrompts.SelectedItem = selected ?? data.Prompts[0];
        }

        cboLanguage.Items.Clear();
        cboLanguage.Items.Add("中文");
        cboLanguage.Items.Add("English");
        cboLanguage.SelectedItem = I18n.Lang == "en" ? "English" : "中文";

        UpdateHotkeyControls();
    }

    private void OnLanguageChanged(object? sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (cboLanguage.SelectedItem is string lang)
        {
            I18n.Lang = lang == "English" ? "en" : "zh-CN";
            ApplyI18n();
        }
    }

    private void UpdateHotkeyControls()
    {
        bool enabled = chkHotkeyEnabled.IsChecked == true;
        chkHotkeyAlt.IsEnabled = enabled;
        chkHotkeyCtrl.IsEnabled = enabled;
        chkHotkeyShift.IsEnabled = enabled;
        cboHotkeyKey.IsEnabled = enabled;
    }

    private void OnPromptSelected(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (_lastSelectedPreset != null)
        {
            var idx = _config.Data.Prompts.IndexOf(_lastSelectedPreset);
            if (idx >= 0)
            {
                _config.Data.Prompts[idx].Name = txtPromptName.Text;
                _config.Data.Prompts[idx].SystemPrompt = txtSystemPrompt.Text;
            }
        }

        var preset = lstPrompts.SelectedItem as PromptPreset;
        btnDeletePrompt.IsEnabled = preset != null && _config.Data.Prompts.Count > 1;
        if (preset != null)
        {
            txtPromptId.Text = preset.Id;
            txtPromptName.Text = preset.Name;
            txtSystemPrompt.Text = preset.SystemPrompt;
            _lastSelectedPreset = preset;
        }
    }

    private void BtnAddPrompt_OnClick(object sender, System.Windows.RoutedEventArgs e)
    {
        var newId = "prompt_" + Guid.NewGuid().ToString("N")[..8];
        var preset = new PromptPreset { Id = newId, Name = I18n.Get("Config.AddPrompt"), SystemPrompt = "" };
        _config.Data.Prompts.Add(preset);
        lstPrompts.ItemsSource = null;
        lstPrompts.ItemsSource = _config.Data.Prompts;
        lstPrompts.SelectedItem = preset;
    }

    private void BtnDeletePrompt_OnClick(object sender, System.Windows.RoutedEventArgs e)
    {
        var preset = lstPrompts.SelectedItem as PromptPreset;
        if (preset != null && _config.Data.Prompts.Count > 1)
        {
            _config.Data.Prompts.Remove(preset);
            lstPrompts.ItemsSource = null;
            lstPrompts.ItemsSource = _config.Data.Prompts;
            lstPrompts.SelectedIndex = 0;
        }
    }

    private void SldTemperature_OnValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
    {
        lblTemperatureVal.Text = e.NewValue.ToString("F1");
    }

    private void SaveConfig()
    {
        var data = _config.Data;

        var currentPreset = lstPrompts.SelectedItem as PromptPreset;
        if (currentPreset != null)
        {
            currentPreset.Id = txtPromptId.Text;
            currentPreset.Name = txtPromptName.Text;
            currentPreset.SystemPrompt = txtSystemPrompt.Text;
        }

        data.ApiBase = txtApiBase.Text.Trim();
        data.ApiKey = txtApiKey.Password;
        data.Model = txtModel.Text.Trim();

        int.TryParse(txtMaxTokens.Text, out var maxTokens);
        data.MaxTokens = Math.Clamp(maxTokens, 1, 128000);
        data.Temperature = sldTemperature.Value;

        data.HotkeyEnabled = chkHotkeyEnabled.IsChecked == true;
        var mods = new List<string>();
        if (chkHotkeyAlt.IsChecked == true) mods.Add("Alt");
        if (chkHotkeyCtrl.IsChecked == true) mods.Add("Ctrl");
        if (chkHotkeyShift.IsChecked == true) mods.Add("Shift");
        data.HotkeyModifiers = string.Join("+", mods);
        data.HotkeyKey = cboHotkeyKey.SelectedItem?.ToString() ?? "C";

        int.TryParse(txtMaxHistory.Text, out var maxHistory);
        data.MaxHistory = Math.Clamp(maxHistory, 1, 100);
        data.ShowNotifications = chkShowNotifications.IsChecked == true;
        data.ClipboardMonitoringEnabled = chkClipboardMonitor.IsChecked == true;
        data.PlaySoundOnComplete = chkPlaySound.IsChecked == true;
        data.Language = I18n.Lang;

        _config.Save();
    }

    private void BtnSave_OnClick(object sender, System.Windows.RoutedEventArgs e) => SaveConfig();

    private void BtnCancel_OnClick(object sender, System.Windows.RoutedEventArgs e) => Close();
}
