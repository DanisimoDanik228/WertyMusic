using Domain.Models;

namespace WertyMusic.Requests;

public class DonwloadRequest
{
    public IEnumerable<Guid> musicsId { get; set; }
}