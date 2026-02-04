namespace PublicDomain.OpenLibrary;

public sealed class OLSearchResult
{
    public int numFound { get; set; }
    public List<OLDoc> docs { get; set; } = new();
}

public sealed class OLDoc
{
    public string? key { get; set; }
    public string? title { get; set; }
    public List<string> author_name { get; set; } = new();
    public List<string> language { get; set; } = new();
    public int? first_publish_year { get; set; }
    public List<string> edition_key { get; set; } = new();
}

public sealed class OLEdition
{
    public string? key { get; set; }
    public string? title { get; set; }
    public List<string> publishers { get; set; } = new();
    public string? publish_date { get; set; }
    public List<string> languages { get; set; } = new();
    public Dictionary<string, object>? identifiers { get; set; }
    public Dictionary<string, object>? links { get; set; }
    public Dictionary<string, object>? formats { get; set; }
}
