namespace PublicDomain.Gutenberg;

public sealed class GutenbergClient : ApiClientBase
{
    private readonly string _base;

    public GutenbergClient(string baseUrl, HttpClient? http = null) : base(http)
    {
        _base = baseUrl.TrimEnd('/');
    }

    public Task<GutenbergPaged<GutenbergBook>> SearchAsync(string query, int pageSize = 20, int page = 1, CancellationToken ct = default)
    {
        var q = Uri.EscapeDataString(query);
        var url = $"{_base}/api/books?q={q}&page_size={pageSize}&page={page}";
        return GetJsonAsync<GutenbergPaged<GutenbergBook>>(url, ct);
    }

    public Task<GutenbergBook> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var url = $"{_base}/api/books/{id}";
        return GetJsonAsync<GutenbergBook>(url, ct);
    }
}
