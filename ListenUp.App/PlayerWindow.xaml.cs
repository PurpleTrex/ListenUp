using System.Windows;
using System.Windows.Threading;

namespace ListenUp.App;

public partial class PlayerWindow : Window
{
    private readonly string _audioUrl;
    private bool _isPlaying;
    private readonly DispatcherTimer _timer;

    public PlayerWindow(string title, string audioUrl)
    {
        InitializeComponent();
        _audioUrl = audioUrl;
        TrackLabel.Text = title;
        
        _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(500) };
        _timer.Tick += OnTimerTick;
        
        Loaded += OnLoaded;
        Closed += OnClosed;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        try
        {
            Player.Source = new Uri(_audioUrl);
            Player.Volume = VolumeSlider.Value;
            StatusText.Text = "Ready";
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Failed to load audio", MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
        }
    }

    private void OnMediaOpened(object sender, RoutedEventArgs e)
    {
        if (Player.NaturalDuration.HasTimeSpan)
        {
            var duration = Player.NaturalDuration.TimeSpan;
            ProgressSlider.Maximum = duration.TotalSeconds;
            TotalTime.Text = FormatTime(duration);
            StatusText.Text = "Loaded";
        }
    }

    private void OnPlayPause(object sender, RoutedEventArgs e)
    {
        if (_isPlaying)
        {
            Player.Pause();
            _isPlaying = false;
            _timer.Stop();
            PlayPauseButton.Content = "▶ Play";
            StatusText.Text = "Paused";
        }
        else
        {
            Player.Play();
            _isPlaying = true;
            _timer.Start();
            PlayPauseButton.Content = "⏸ Pause";
            StatusText.Text = "Playing";
        }
    }

    private void OnStop(object sender, RoutedEventArgs e)
    {
        Player.Stop();
        _isPlaying = false;
        _timer.Stop();
        PlayPauseButton.Content = "▶ Play";
        CurrentTime.Text = "0:00";
        ProgressSlider.Value = 0;
        StatusText.Text = "Stopped";
    }

    private void OnSeekBack(object sender, RoutedEventArgs e)
    {
        var newPos = Player.Position - TimeSpan.FromSeconds(10);
        if (newPos < TimeSpan.Zero) newPos = TimeSpan.Zero;
        Player.Position = newPos;
    }

    private void OnSeekForward(object sender, RoutedEventArgs e)
    {
        var newPos = Player.Position + TimeSpan.FromSeconds(10);
        if (Player.NaturalDuration.HasTimeSpan && newPos > Player.NaturalDuration.TimeSpan)
            newPos = Player.NaturalDuration.TimeSpan;
        Player.Position = newPos;
    }

    private void OnProgressChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        // Allow manual seeking via slider
        if (Player.NaturalDuration.HasTimeSpan && !_isPlaying)
        {
            Player.Position = TimeSpan.FromSeconds(ProgressSlider.Value);
        }
    }

    private void OnVolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (Player != null)
        {
            Player.Volume = VolumeSlider.Value;
        }
    }

    private void OnTimerTick(object? sender, EventArgs e)
    {
        if (Player.NaturalDuration.HasTimeSpan)
        {
            ProgressSlider.Value = Player.Position.TotalSeconds;
            CurrentTime.Text = FormatTime(Player.Position);
        }
    }

    private void OnMediaEnded(object sender, RoutedEventArgs e)
    {
        _isPlaying = false;
        _timer.Stop();
        PlayPauseButton.Content = "▶ Play";
        StatusText.Text = "Ended";
        Player.Position = TimeSpan.Zero;
        ProgressSlider.Value = 0;
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        _timer.Stop();
        Player.Stop();
    }

    private static string FormatTime(TimeSpan time)
    {
        return time.TotalHours >= 1
            ? $"{(int)time.TotalHours}:{time.Minutes:D2}:{time.Seconds:D2}"
            : $"{time.Minutes}:{time.Seconds:D2}";
    }
}
