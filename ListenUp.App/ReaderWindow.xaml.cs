using System.Windows;

namespace ListenUp.App;

public partial class ReaderWindow : Window
{
    private readonly string _url;

    public ReaderWindow(string url)
    {
        InitializeComponent();
        _url = url;
        Loaded += OnLoaded;
    }

    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            await Web.EnsureCoreWebView2Async();
            Web.Source = new Uri(_url);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Failed to load", MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
        }
    }
}
