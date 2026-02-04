using System.Diagnostics;
using System.Windows;
using ListenUp.App.Models;
using ListenUp.App.Services;
using ListenUp.App.ViewModels;

namespace ListenUp.App;

public partial class MainWindow : Window
{
    private readonly SearchService _service = new();
    private readonly MainViewModel _vm;

    public MainWindow()
    {
        InitializeComponent();
        _vm = new MainViewModel(_service);
        DataContext = _vm;
        Closed += (_, _) => _service.Dispose();
    }

    private async void OnSearch(object sender, RoutedEventArgs e)
    {
        await _vm.SearchAsync();
    }

    private void OnRead(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is not AggregatedResult item) return;
        var url = item.HtmlUrl ?? item.PdfUrl ?? item.EpubUrl;
        if (string.IsNullOrWhiteSpace(url)) return;

        // Use WebView2 for HTML/PDF when possible; fall back to default handler otherwise.
        if (!string.IsNullOrWhiteSpace(item.HtmlUrl) || !string.IsNullOrWhiteSpace(item.PdfUrl))
        {
            var reader = new ReaderWindow(url) { Owner = this };
            reader.Show();
        }
        else
        {
            Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
        }
    }

    private void OnListen(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is not AggregatedResult item) return;
        if (string.IsNullOrWhiteSpace(item.AudioUrl)) return;

        var player = new PlayerWindow(item.Title, item.AudioUrl) { Owner = this };
        player.Show();
    }
}