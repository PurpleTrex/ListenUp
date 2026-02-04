using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ListenUp.App.Models;
using ListenUp.App.Services;

namespace ListenUp.App.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private readonly SearchService _service;
    private bool _isBusy;
    private string _query = string.Empty;
    private string? _status;

    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservableCollection<AggregatedResult> Results { get; } = new();

    public bool IsBusy
    {
        get => _isBusy;
        set { _isBusy = value; OnPropertyChanged(); }
    }

    public string Query
    {
        get => _query;
        set { _query = value; OnPropertyChanged(); }
    }

    public string? Status
    {
        get => _status;
        set { _status = value; OnPropertyChanged(); }
    }

    public MainViewModel(SearchService service)
    {
        _service = service;
    }

    public async Task SearchAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        Status = "Searching…";

        try
        {
            Results.Clear();
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var items = await _service.SearchAsync(Query, cts.Token);
            foreach (var item in items)
            {
                Results.Add(item);
            }
            Status = items.Count == 0 ? "No results" : $"Found {items.Count} items";
        }
        catch (Exception ex)
        {
            Status = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
