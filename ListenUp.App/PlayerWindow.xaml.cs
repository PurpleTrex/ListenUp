using System.Windows;

namespace ListenUp.App;

public partial class PlayerWindow : Window
{
    private readonly string _audioUrl;

    public PlayerWindow(string title, string audioUrl)
    {
        InitializeComponent();
        _audioUrl = audioUrl;
        TrackLabel.Text = title;
        Loaded += OnLoaded;
        Closed += OnClosed;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            Player.Source = new Uri(_audioUrl);
            Player.Play();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Playback failed", MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
        }
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        Player.Stop();
    }
}
