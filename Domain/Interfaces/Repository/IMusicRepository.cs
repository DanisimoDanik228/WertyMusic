using Domain.Models;

namespace Domain.Interfaces.Repository;

public interface IMusicRepository
{
    Task<Music?> GetMusicByIdAsync(Guid id);
    Task<IEnumerable<Music>?> GetMusicsAsync();
    
    Task<Music?> AddMusicAsync(Music music);
    
    Task<Music?> UpdateMusicUrlAsync(Guid id, string url);
    
    Task<IEnumerable<Music>?> GetMusicsBySourceNameAsync(string sourceMusicName);
}