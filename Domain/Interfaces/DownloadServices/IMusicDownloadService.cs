using Domain.Models;

namespace Domain.Interfaces;

public interface  IMusicDownloadService
{
    Task<IEnumerable<Music>> FindMusicsAsync(string musicName);
    Task<IEnumerable<string>?> DownloadMusicsAsync(List<Music> music);
}