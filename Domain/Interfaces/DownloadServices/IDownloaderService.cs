using Domain.Models;

namespace Domain.Interfaces.DownloadServices;

public interface IDownloaderService
{
    Task<IEnumerable<string>?> DownloadMusicsAsync(List<Music> music,string destination);
    Task<string?> DownloadMusicAsync(Music music,string destination);
}