using Domain.Models;

namespace WertyMusic.Requests;

public class DownloadRequest
{
    public IEnumerable<Guid> musicsId { get; set; }
    public string ConnectionId { get; set; }
}