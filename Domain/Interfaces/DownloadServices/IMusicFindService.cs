using Domain.Models;

namespace Domain.Interfaces.DownloadServices;

public interface  IMusicFindService
{
    IAsyncEnumerable<Music> FindMusicsAsync(string musicName);

}