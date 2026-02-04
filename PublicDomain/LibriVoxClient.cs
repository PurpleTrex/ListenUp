namespace PublicDomain.LibriVox;

public sealed class LibriVoxClient : ApiClientBase
{
    private const string BaseUrl = "https://librivox.org/api/feed";

    public LibriVoxClient(HttpClient? http = null) : base(http) { }

    public Task<LibriVoxResponse> SearchAudiobooksAsync(
        string? title = null,
        string? authorLastName = null,
        string? genre = null,
        int limit = 50,
        int offset = 0,
        bool extended = true,
        CancellationToken ct = default)
    {
        var query = new List<string> { "format=json" };
        if (!string.IsNullOrWhiteSpace(title)) query.Add($"title={Uri.EscapeDataString(title)}");
        if (!string.IsNullOrWhiteSpace(authorLastName)) query.Add($"author={Uri.EscapeDataString(authorLastName)}");
        if (!string.IsNullOrWhiteSpace(genre)) query.Add($"genre={Uri.EscapeDataString(genre)}");
        if (extended) query.Add("extended=1");
        if (limit > 0) query.Add($"limit={limit}");
        if (offset > 0) query.Add($"offset={offset}");

        var url = $"{BaseUrl}/audiobooks/?{string.Join("&", query)}";
        return GetJsonAsync<LibriVoxResponse>(url, ct);
    }
}
