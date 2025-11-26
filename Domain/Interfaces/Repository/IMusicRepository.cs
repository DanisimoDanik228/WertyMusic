using Domain.Models;

namespace Domain.Interfaces.Repository;

public interface IMusicRepository
{
    Task<Music?> GetMusicByIdAsync(Guid id);
    Task<IEnumerable<Music>?> GetMusicsAsync();
    
    Task<Music?> AddMusicAsync(Music music);
    
    Task<Music?> UpdateMusicAsync(Guid id, Music music);
    
    Task<Music?> DeleteMusicByIdAsync(Guid id);
    
    Task<IEnumerable<Music>?> GetMusicsBySourceNameAsync(string sourceMusicName);
    Task<bool> ExistsMusicByIdAsync(Guid id);
    
    /// <summary>
    ///  update only not null value in music param
    /// </summary>
    /// <param name="id"></param> 
    /// <param name="music"></param>
    /// <returns></returns>
    Task<Music?> UpdateFieldMusicAsync(Guid id,Music music);
}