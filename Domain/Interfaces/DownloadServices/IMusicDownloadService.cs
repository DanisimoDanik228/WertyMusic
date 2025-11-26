using Domain.Models;

namespace Domain.Interfaces.DownloadServices;

public interface  IMusicDownloadService
{
    Task<IEnumerable<Music>> FindMusicsAsync(string musicName);
    Task<IEnumerable<string>?> DownloadMusicsAsync(List<Music> music);
    Task<string?> DownloadMusicAsync(Music music);
}