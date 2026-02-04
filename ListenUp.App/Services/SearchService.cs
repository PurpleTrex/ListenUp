using ListenUp.App.Models;
using PublicDomain.Gutenberg;
using PublicDomain.LibriVox;
using PublicDomain.OpenLibrary;

namespace ListenUp.App.Services;

public sealed class SearchService : IDisposable
{
    private readonly GutenbergClient? _gutenberg;
    private readonly LibriVoxClient _libriVox;
    private readonly OpenLibraryClient _openLibrary;
    private readonly HttpClient? _gutenbergHttp;

    public SearchService()
    {
        if (!string.IsNullOrWhiteSpace(AppConfig.GutenbergBaseUrl))
        {
            _gutenbergHttp = new HttpClient();
            AppConfig.ApplyGutenbergHeaders(_gutenbergHttp);
            _gutenberg = new GutenbergClient(AppConfig.GutenbergBaseUrl!, _gutenbergHttp);
        }

        _libriVox = new LibriVoxClient();
        _openLibrary = new OpenLibraryClient();
    }

    public async Task<List<AggregatedResult>> SearchAsync(string query, CancellationToken ct = default)
    {
        var trimmed = query?.Trim();
        if (string.IsNullOrWhiteSpace(trimmed)) return new List<AggregatedResult>();

        var gutenbergTask = _gutenberg != null
            ? _gutenberg.SearchAsync(trimmed, pageSize: 12, page: 1, ct)
            : Task.FromResult<GutenbergPaged<GutenbergBook>?>(null);

        var libriTask = _libriVox.SearchAudiobooksAsync(title: trimmed, limit: 12, extended: true, ct: ct);
        var olTask = _openLibrary.SearchAsync(trimmed, page: 1, ct: ct);

        await Task.WhenAll(gutenbergTask, libriTask, olTask);

        var results = new Dictionary<string, AggregatedResult>(StringComparer.OrdinalIgnoreCase);

        void AddOrUpdate(string key, Action<AggregatedResult> mutator)
        {
            if (!results.TryGetValue(key, out var existing))
            {
                existing = new AggregatedResult();
                results[key] = existing;
            }
            mutator(existing);
        }

        var gutenberg = gutenbergTask.Result;
        if (gutenberg?.results != null)
        {
            foreach (var book in gutenberg.results)
            {
                var title = book.title ?? book.alternative_title ?? "Untitled";
                var author = book.authors.FirstOrDefault()?.name ?? string.Empty;
                var key = NormalizeKey(title, author);
                AddOrUpdate(key, r =>
                {
                    r.Title = title;
                    r.AuthorDisplay = author;
                    r.Gutenberg = book;
                    r.HtmlUrl = r.HtmlUrl ?? SelectFormat(book, "html");
                    r.EpubUrl = r.EpubUrl ?? SelectFormat(book, "epub");
                    r.PdfUrl = r.PdfUrl ?? SelectFormat(book, "pdf");
                });
            }
        }

        var libri = libriTask.Result;
        foreach (var book in libri.books)
        {
            var title = book.title ?? "Untitled";
            var author = book.authors.FirstOrDefault() is { } person
                ? string.Join(' ', new[] { person.first_name, person.last_name }.Where(s => !string.IsNullOrWhiteSpace(s)))
                : string.Empty;
            var key = NormalizeKey(title, author);
            AddOrUpdate(key, r =>
            {
                r.Title = r.Title.Length == 0 ? title : r.Title;
                r.AuthorDisplay = r.AuthorDisplay.Length == 0 ? author : r.AuthorDisplay;
                r.LibriVox = book;
                r.AudioUrl ??= book.sections.FirstOrDefault()?.listen_url ?? book.url_zip_file;
                r.CoverUrl ??= book.coverart?.coverart_thumbnail ?? book.coverart?.coverart_jpg;
            });
        }

        var ol = olTask.Result;
        foreach (var doc in ol.docs)
        {
            var title = doc.title ?? "Untitled";
            var author = doc.author_name.FirstOrDefault() ?? string.Empty;
            var key = NormalizeKey(title, author);
            AddOrUpdate(key, r =>
            {
                r.Title = r.Title.Length == 0 ? title : r.Title;
                r.AuthorDisplay = r.AuthorDisplay.Length == 0 ? author : r.AuthorDisplay;
                r.OpenLibrary = doc;
                r.CoverUrl ??= doc.edition_key.FirstOrDefault() is { } olid
                    ? $"https://covers.openlibrary.org/b/olid/{olid}-M.jpg"
                    : null;
            });
        }

        return results.Values
            .OrderByDescending(r => r.HasText)
            .ThenByDescending(r => r.HasAudio)
            .ThenBy(r => r.Title)
            .ToList();
    }

    private static string NormalizeKey(string title, string author)
    {
        var normalizedTitle = title.Trim().ToLowerInvariant();
        var normalizedAuthor = author.Trim().ToLowerInvariant();
        return $"{normalizedTitle}|{normalizedAuthor}";
    }

    private static string? SelectFormat(GutenbergBook book, string contains)
    {
        return book.formats.FirstOrDefault(f => (f.type ?? string.Empty).Contains(contains, StringComparison.OrdinalIgnoreCase))?.url;
    }

    public void Dispose()
    {
        _gutenbergHttp?.Dispose();
    }
}
