namespace PublicDomain.Gutenberg;

public sealed class GutenbergPaged<T>
{
    public string? next { get; set; }
    public string? previous { get; set; }
    public List<T> results { get; set; } = new();
}

public sealed class GutenbergBook
{
    public int id { get; set; }
    public string? title { get; set; }
    public string? alternative_title { get; set; }
    public List<GutenbergAuthor> authors { get; set; } = new();
    public List<string> subjects { get; set; } = new();
    public string? language { get; set; }
    public string? media_type { get; set; }
    public int? download_count { get; set; }
    public string? gutenberg_url { get; set; }
    public List<GutenbergFormat> formats { get; set; } = new();
}

public sealed class GutenbergAuthor
{
    public int id { get; set; }
    public string? name { get; set; }
}

public sealed class GutenbergFormat
{
    public string? type { get; set; }
    public string? url { get; set; }
}
