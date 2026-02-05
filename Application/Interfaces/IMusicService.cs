using Domain.Models;
using Microsoft.AspNetCore.Http;

namespace ClassLibrary1.Interfaces;

public interface IMusicService
{
    Task<byte[]?> DownloadMusicsAsync(IEnumerable<Guid> id);
    IAsyncEnumerable<Music> FindMusicsAsync(string sourceMusicName);
    
    Task<Stream?>  GetFileMusicAsync(Guid id);
    Task<IEnumerable<Music>?> GetMusicsAsync();
}