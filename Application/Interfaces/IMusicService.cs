using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace ClassLibrary1.Interfaces;

public interface IMusicService
{
    // Main function
    Task<IEnumerable<string>?> DownloadMusicsAsync(IEnumerable<Guid> id);
    Task<string?> DownloadMusicAsync(Guid id);
    Task<IEnumerable<Music>> FindMusicsAsync(string sourceMusicName);
    
    
    Task<Stream?>  GetFileMusicAsync(Guid id);
    Task<Music?> GetMusicByIdAsync(Guid id);
    Task<IEnumerable<Music>?> GetMusicsAsync();
    
    
    Task<Music?> AddMusicAsync(Music music);
}