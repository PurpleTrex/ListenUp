using System.IO;
using System.Windows;
using VersOne.Epub;

namespace ListenUp.App.Windows;

public partial class EpubReaderWindow : Window
{
    private EpubBook? _book;
    private List<EpubLocalTextContentFile> _chapters = new();
    private int _currentChapterIndex;

    public EpubReaderWindow(string epubPath)
    {
        InitializeComponent();
        Loaded += async (s, e) => await LoadEpubAsync(epubPath);
    }

    private async Task LoadEpubAsync(string path)
    {
        try
        {
            // Download if URL
            string localPath = path;
            if (path.StartsWith("http", StringComparison.OrdinalIgnoreCase))
            {
                var tempPath = Path.Combine(Path.GetTempPath(), $"listenup_{Guid.NewGuid()}.epub");
                using var client = new HttpClient();
                var bytes = await client.GetByteArrayAsync(path);
                await File.WriteAllBytesAsync(tempPath, bytes);
                localPath = tempPath;
            }

            _book = await EpubReader.ReadBookAsync(localPath);
            
            // Set metadata
            TitleText.Text = _book.Title ?? "Unknown Title";
            AuthorText.Text = _book.Author ?? "Unknown Author";

            // Get reading order
            _chapters = _book.ReadingOrder.ToList();
            
            if (_chapters.Count > 0)
            {
                _currentChapterIndex = 0;
                DisplayChapter(_currentChapterIndex);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Failed to load EPUB: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Close();
        }
    }

    private void DisplayChapter(int index)
    {
        if (index < 0 || index >= _chapters.Count) return;

        try
        {
            var chapter = _chapters[index];
            
            // Simple HTML stripping
            ContentText.Text = System.Text.RegularExpressions.Regex.Replace(
                chapter.Content ?? string.Empty,
                "<.*?>",
                string.Empty
            );

            ChapterInfo.Text = $"Chapter {index + 1} of {_chapters.Count}";
            PrevButton.IsEnabled = index > 0;
            NextButton.IsEnabled = index < _chapters.Count - 1;
        }
        catch (Exception ex)
        {
            ContentText.Text = $"Error loading chapter: {ex.Message}";
        }
    }

    private void OnPrevious(object sender, RoutedEventArgs e)
    {
        if (_currentChapterIndex > 0)
        {
            _currentChapterIndex--;
            DisplayChapter(_currentChapterIndex);
        }
    }

    private void OnNext(object sender, RoutedEventArgs e)
    {
        if (_currentChapterIndex < _chapters.Count - 1)
        {
            _currentChapterIndex++;
            DisplayChapter(_currentChapterIndex);
        }
    }

    private void OnShowToc(object sender, RoutedEventArgs e)
    {
        if (_book == null) return;

        TocList.Items.Clear();
        for (int i = 0; i < _chapters.Count; i++)
        {
            var chapter = _chapters[i];
            var title = $"Chapter {i + 1}";
            TocList.Items.Add(new { Index = i, Title = title });
        }
        TocList.DisplayMemberPath = "Title";
        TocPanel.Visibility = Visibility.Visible;
    }

    private void OnTocSelection(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
    {
        if (TocList.SelectedItem is { } item)
        {
            var index = (int)item.GetType().GetProperty("Index")!.GetValue(item)!;
            _currentChapterIndex = index;
            DisplayChapter(_currentChapterIndex);
            TocPanel.Visibility = Visibility.Collapsed;
        }
    }

    private void OnCloseToc(object sender, RoutedEventArgs e)
    {
        TocPanel.Visibility = Visibility.Collapsed;
    }
}
