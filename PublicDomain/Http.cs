using System.Net.Http.Headers;
using System.Text.Json;

namespace PublicDomain;

public static class JsonDefaults
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };
}

public abstract class ApiClientBase : IDisposable
{
    protected readonly HttpClient _http;

    protected ApiClientBase(HttpClient? http = null)
    {
        _http = http ?? new HttpClient();
        _http.Timeout = TimeSpan.FromSeconds(30);
        _http.DefaultRequestHeaders.UserAgent.Add(
            new ProductInfoHeaderValue("FOSClientPortalPDReader", "1.0"));
    }

    protected async Task<T> GetJsonAsync<T>(string url, CancellationToken ct = default)
    {
        using var res = await _http.GetAsync(url, ct).ConfigureAwait(false);
        res.EnsureSuccessStatusCode();
        await using var stream = await res.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
        var obj = await JsonSerializer.DeserializeAsync<T>(stream, JsonDefaults.Options, ct).ConfigureAwait(false);
        return obj!;
    }

    public void Dispose() => _http.Dispose();
}
