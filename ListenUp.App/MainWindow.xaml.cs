using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using ListenUp.App.Models;
using ListenUp.App.Services;
using ListenUp.App.ViewModels;
using ListenUp.App.Windows;

namespace ListenUp.App;

public partial class MainWindow : Window
{
    private readonly SearchService _service = new();
    private readonly CacheService _cache = new();
    private readonly MainViewModel _vm;
    private bool _showingFavorites;

    public MainWindow()
    {
        InitializeComponent();
        _vm = new MainViewModel(_service, _cache);
        DataContext = _vm;
        Closed += (_, _) =>
        {
            _service.Dispose();
            _cache.Dispose();
        };
        SearchBox.Focus();
    }

    private async void OnSearch(object sender, RoutedEventArgs e)
    {
        _showingFavorites = false;
        await _vm.SearchAsync();
    }

    private void OnSearchKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            OnSearch(sender, e);
        }
    }

    private void OnRead(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is not AggregatedResult item) return;
        
        try
        {
            // Prefer EPUB for better experience, then HTML, then PDF
            if (!string.IsNullOrWhiteSpace(item.EpubUrl))
            {
                var reader = new EpubReaderWindow(item.EpubUrl) { Owner = this };
                reader.Show();
            }
            else if (!string.IsNullOrWhiteSpace(item.HtmlUrl))
            {
                var reader = new ReaderWindow(item.HtmlUrl) { Owner = this };
                reader.Show();
            }
            else if (!string.IsNullOrWhiteSpace(item.PdfUrl))
            {
                var reader = new ReaderWindow(item.PdfUrl) { Owner = this };
                reader.Show();
            }
            else
            {
                MessageBox.Show(this, "No readable format available.", "Not Available", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Failed to open reader: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void OnListen(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is not AggregatedResult item) return;
        
        try
        {
            if (string.IsNullOrWhiteSpace(item.AudioUrl))
            {
                MessageBox.Show(this, "No audio available.", "Not Available", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var player = new PlayerWindow(item.Title, item.AudioUrl) { Owner = this };
            player.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Failed to open player: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void OnToggleFavorite(object sender, RoutedEventArgs e)
    {
        if ((sender as FrameworkElement)?.DataContext is not AggregatedResult item) return;

        try
        {
            if (_cache.IsFavorite(item))
            {
                _cache.RemoveFavorite(item);
                ((System.Windows.Controls.Button)sender).Content = "⭐ Favorite";
                MessageBox.Show(this, "Removed from favorites", "Favorites", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                _cache.AddFavorite(item);
                ((System.Windows.Controls.Button)sender).Content = "★ Favorited";
                MessageBox.Show(this, "Added to favorites", "Favorites", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            if (_showingFavorites)
            {
                OnShowFavorites(sender, e);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Failed to update favorites: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void OnShowFavorites(object sender, RoutedEventArgs e)
    {
        try
        {
            _showingFavorites = true;
            _vm.ShowFavorites(_cache.GetFavorites());
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Failed to load favorites: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void OnShowSearch(object sender, RoutedEventArgs e)
    {
        _showingFavorites = false;
        SearchBox.Focus();
    }

    private void OnSettings(object sender, RoutedEventArgs e)
    {
        try
        {
            var settings = new SettingsWindow { Owner = this };
            settings.ShowDialog();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Failed to open settings: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void OnAbout(object sender, RoutedEventArgs e)
    {
        MessageBox.Show(this,
            "ListenUp v1.0.0\n\n" +
            "A public domain book reader and audiobook player.\n\n" +
            "Data sources:\n" +
            "• Project Gutenberg (text)\n" +
            "• LibriVox (audio)\n" +
            "• Open Library (metadata)\n\n" +
            "Built with .NET 10 and WPF",
            "About ListenUp",
            MessageBoxButton.OK,
            MessageBoxImage.Information);
    }

    private void OnExit(object sender, RoutedEventArgs e)
    {
        Close();
    }
}