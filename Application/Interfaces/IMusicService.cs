using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace ClassLibrary1.Interfaces;

public interface IMusicService
{
    // Main function
    Task<string?> DownloadMusicAsync(Guid id);
    Task<byte[]?> DownloadMusicsAsync(IEnumerable<Guid> id);
    Task<IEnumerable<Music>> FindMusicsAsync(string sourceMusicName);
    
    
    Task<Stream?>  GetFileMusicAsync(Guid id);
    Task<Music?> GetMusicByIdAsync(Guid id);
    Task<IEnumerable<Music>?> GetMusicsAsync();
    
    
    Task<Music?> AddMusicAsync(Music music);
    
    Task<Music?> UpDateMusicAsync(Guid id,Music music);
}