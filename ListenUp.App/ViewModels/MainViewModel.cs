using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ListenUp.App.Models;
using ListenUp.App.Services;

namespace ListenUp.App.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private readonly SearchService _service;
    private readonly CacheService _cache;
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

    public MainViewModel(SearchService service, CacheService cache)
    {
        _service = service;
        _cache = cache;
    }

    public async Task SearchAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        Status = "Searching…";

        try
        {
            Results.Clear();

            // Try cache first
            if (_cache.TryGetCachedSearch(Query, out var cachedResults, TimeSpan.FromHours(24)) && cachedResults != null)
            {
                Status = $"Found {cachedResults.Count} cached results";
                foreach (var item in cachedResults)
                {
                    Results.Add(item);
                }
                return;
            }

            // Perform search with retry logic
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(45));
            var items = await ResilienceService.ExecuteWithRetryAsync(
                async () => await _service.SearchAsync(Query, cts.Token),
                maxRetries: 2
            );

            // Cache results
            _cache.CacheSearch(Query, items);

            foreach (var item in items)
            {
                Results.Add(item);
            }
            Status = items.Count == 0 ? "No results found" : $"Found {items.Count} items";
        }
        catch (OperationCanceledException)
        {
            Status = "Search timed out";
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

    public void ShowFavorites(List<AggregatedResult> favorites)
    {
        Results.Clear();
        foreach (var item in favorites)
        {
            Results.Add(item);
        }
        Status = favorites.Count == 0 ? "No favorites yet" : $"{favorites.Count} favorites";
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
