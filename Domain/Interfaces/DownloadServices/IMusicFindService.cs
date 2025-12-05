using Domain.Models;

namespace Domain.Interfaces.DownloadServices;

public interface  IMusicFindService
{
    Task<IEnumerable<Music>> FindMusicsAsync(string musicName);

}