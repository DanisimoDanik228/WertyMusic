using Domain.Enum;

namespace Domain.Models;

public class Music
{
    public Guid Id { get; set; }
    public string MusicName { get; set; }
    public string ArtistName { get; set; }
    public string? Url { get; set; }
    public string? ArtistUrl { get; set; }
    public string DownloadUrl { get; set; }
    public DateTime CreationDate  { get; set; }
    public SiteSource SiteSource { get; set; }
    
    public string SourceName { get; set; }
}