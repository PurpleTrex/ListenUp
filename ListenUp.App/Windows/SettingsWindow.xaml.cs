using System.Windows;

namespace ListenUp.App.Windows;

public partial class SettingsWindow : Window
{
    public SettingsWindow()
    {
        InitializeComponent();
        LoadSettings();
    }

    private void LoadSettings()
    {
        GutenbergUrl.Text = AppConfig.GutenbergBaseUrl ?? string.Empty;
        GutenbergKeyName.Text = AppConfig.GutenbergApiKeyName ?? string.Empty;
        if (!string.IsNullOrEmpty(AppConfig.GutenbergApiKeyValue))
        {
            GutenbergKeyValue.Password = AppConfig.GutenbergApiKeyValue;
        }
        GutenbergHost.Text = AppConfig.GutenbergHostHeaderValue ?? string.Empty;
    }

    private void OnSave(object sender, RoutedEventArgs e)
    {
        AppConfig.GutenbergBaseUrl = string.IsNullOrWhiteSpace(GutenbergUrl.Text) ? null : GutenbergUrl.Text;
        AppConfig.GutenbergApiKeyName = string.IsNullOrWhiteSpace(GutenbergKeyName.Text) ? null : GutenbergKeyName.Text;
        AppConfig.GutenbergApiKeyValue = string.IsNullOrWhiteSpace(GutenbergKeyValue.Password) ? null : GutenbergKeyValue.Password;
        AppConfig.GutenbergHostHeaderValue = string.IsNullOrWhiteSpace(GutenbergHost.Text) ? null : GutenbergHost.Text;
        
        MessageBox.Show(this, "Settings saved. Restart the application for changes to take effect.", "Settings", MessageBoxButton.OK, MessageBoxImage.Information);
        DialogResult = true;
        Close();
    }

    private void OnCancel(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    private void OnClearCache(object sender, RoutedEventArgs e)
    {
        try
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var cacheDb = System.IO.Path.Combine(appData, "ListenUp", "cache.db");
            if (System.IO.File.Exists(cacheDb))
            {
                System.IO.File.Delete(cacheDb);
                MessageBox.Show(this, "Cache cleared successfully.", "Cache", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(this, "No cache found.", "Cache", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Failed to clear cache: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
