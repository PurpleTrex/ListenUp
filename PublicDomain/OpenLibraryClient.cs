using System.Text.Json;
namespace PublicDomain.OpenLibrary;

public sealed class OpenLibraryClient : ApiClientBase
{
    private const string Base = "https://openlibrary.org";

    public OpenLibraryClient(HttpClient? http = null) : base(http) { }

    public Task<OLSearchResult> SearchAsync(string query, int page = 1, CancellationToken ct = default)
    {
        var q = Uri.EscapeDataString(query);
        var url = $"{Base}/search.json?q={q}&page={page}";
        return GetJsonAsync<OLSearchResult>(url, ct);
    }

    public Task<OLEdition> GetEditionAsync(string editionId, CancellationToken ct = default)
    {
        var url = $"{Base}/books/{editionId}.json";
        return GetJsonAsync<OLEdition>(url, ct);
    }

    public Task<JsonElement> GetWorkRawAsync(string workId, CancellationToken ct = default)
    {
        var url = $"{Base}/works/{workId}.json";
        return GetJsonAsync<JsonElement>(url, ct);
    }
}
