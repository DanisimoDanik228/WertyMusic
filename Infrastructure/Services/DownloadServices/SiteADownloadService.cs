using Domain.Enum;
using Domain.Interfaces;
using Domain.Models;

namespace Infrastructure.Services;

public class SiteADownloadService : IMusicDownloadService
{
    public Task<IEnumerable<Music>> DownloadMusicsAsync(string musicName)
    {
        // must set the value of SourceName
        throw new NotImplementedException();
    }
}