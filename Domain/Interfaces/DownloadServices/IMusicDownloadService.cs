using Domain.Models;

namespace Domain.Interfaces;

public interface  IMusicDownloadService
{
    Task<IEnumerable<Music>> DownloadMusicsAsync(string musicName);
    Task<IEnumerable<Music>> FindMusicsAsync(string musicName);
}