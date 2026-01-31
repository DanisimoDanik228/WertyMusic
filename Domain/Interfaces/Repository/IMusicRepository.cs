using Domain.Models;

namespace Domain.Interfaces.Repository;

public interface IMusicRepository
{
    // If the method changes the BD states it returns Task.
    // Otherwise Data is returned. 
    
    Task<Music?> GetMusicByIdAsync(Guid id);
    
    Task<IEnumerable<Music>> GetMusicsByIdsAsync(IEnumerable<Guid> ids);
    
    Task<IEnumerable<Music>> GetMusicsAsync();
    
    Task<IEnumerable<Music>> GetMusicsBySourceNameAsync(string sourceName);
    
    Task AddMusicAsync(Music music);
    
    Task AddMusicRangeAsync(IEnumerable<Music> music);
    
}