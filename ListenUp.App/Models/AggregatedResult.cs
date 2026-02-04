using PublicDomain.Gutenberg;
using PublicDomain.LibriVox;
using PublicDomain.OpenLibrary;

namespace ListenUp.App.Models;

public sealed class AggregatedResult
{
    public string Title { get; set; } = string.Empty;
    public string AuthorDisplay { get; set; } = string.Empty;
    public GutenbergBook? Gutenberg { get; set; }
    public LibriVoxBook? LibriVox { get; set; }
    public OLDoc? OpenLibrary { get; set; }
    public string? HtmlUrl { get; set; }
    public string? EpubUrl { get; set; }
    public string? PdfUrl { get; set; }
    public string? AudioUrl { get; set; }
    public string? CoverUrl { get; set; }
    public bool HasText => !string.IsNullOrWhiteSpace(HtmlUrl) || !string.IsNullOrWhiteSpace(EpubUrl) || !string.IsNullOrWhiteSpace(PdfUrl);
    public bool HasAudio => !string.IsNullOrWhiteSpace(AudioUrl);
}
