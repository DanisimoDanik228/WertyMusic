using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace ClassLibrary1.Interfaces;

public interface IMusicService
{
    // Main function
    Task<IEnumerable<Music>> DownloadMusicsAsync(string musicName);
    Task<Stream?>  GetFileMusicAsync(Guid id);
    Task<Music?> GetMusicByNameAsync(string  musicName);
    Task<Music?> GetMusicByIdAsync(Guid id);
    
    Task<Music?> AddMusicAsync(Music music);
}