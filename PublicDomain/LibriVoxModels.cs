namespace PublicDomain.LibriVox;

public sealed class LibriVoxResponse
{
    public List<LibriVoxBook> books { get; set; } = new();
}

public sealed class LibriVoxBook
{
    public int id { get; set; }
    public string? title { get; set; }
    public string? description { get; set; }
    public string? language { get; set; }
    public string? url_librivox { get; set; }
    public string? url_zip_file { get; set; }
    public string? url_project { get; set; }
    public string? url_iarchive { get; set; }
    public string? url_rss { get; set; }
    public string? totaltime { get; set; }
    public int? totaltimesecs { get; set; }
    public List<LibriVoxPerson> authors { get; set; } = new();
    public List<LibriVoxSection> sections { get; set; } = new();
    public LibriVoxCoverArt? coverart { get; set; }
}

public sealed class LibriVoxPerson
{
    public int id { get; set; }
    public string? first_name { get; set; }
    public string? last_name { get; set; }
}

public sealed class LibriVoxSection
{
    public int? section_number { get; set; }
    public string? title { get; set; }
    public string? language { get; set; }
    public string? playtime { get; set; }
    public int? playtime_seconds { get; set; }
    public string? listen_url { get; set; }
}

public sealed class LibriVoxCoverArt
{
    public string? coverart_thumbnail { get; set; }
    public string? coverart_jpg { get; set; }
    public string? coverart_pdf { get; set; }
}
