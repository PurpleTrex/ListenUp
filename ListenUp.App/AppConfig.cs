using System.Net.Http.Headers;

namespace ListenUp.App;

public static class AppConfig
{
    // TODO: set your actual Gutenberg API base URL and auth headers. Leave empty to skip Gutenberg calls.
    public static string? GutenbergBaseUrl { get; set; } = "https://project-gutenberg-books-api.p.rapidapi.com";
    public static string? GutenbergApiKeyName { get; set; } = "X-RapidAPI-Key";
    public static string? GutenbergApiKeyValue { get; set; } = null; // set your key here
    public static string? GutenbergHostHeaderName { get; set; } = "X-RapidAPI-Host";
    public static string? GutenbergHostHeaderValue { get; set; } = "project-gutenberg-books-api.p.rapidapi.com";

    public static void ApplyGutenbergHeaders(HttpClient client)
    {
        if (!string.IsNullOrWhiteSpace(GutenbergApiKeyName) && !string.IsNullOrWhiteSpace(GutenbergApiKeyValue))
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation(GutenbergApiKeyName, GutenbergApiKeyValue);
        }

        if (!string.IsNullOrWhiteSpace(GutenbergHostHeaderName) && !string.IsNullOrWhiteSpace(GutenbergHostHeaderValue))
        {
            client.DefaultRequestHeaders.TryAddWithoutValidation(GutenbergHostHeaderName, GutenbergHostHeaderValue);
        }

        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("ListenUpWpf", "0.1"));
    }
}
