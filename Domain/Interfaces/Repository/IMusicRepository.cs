using Domain.Models;

namespace Domain.Interfaces.Repository;

public interface IMusicRepository
{
    Task<Music?> GetMusicByIdAsync(Guid id);
    Task<IEnumerable<Music>?> GetMusicsAsync();
    Task<Music?> GetMusicsByNameAsync(string songName);
    Task<Music?> AddMusicAsync(Music music);
    Task<Music?> UpdateMusicAsync(Guid id, Music music);
    Task<Music?> DeleteMusicByIdAsync(Guid id);
    Task<Music?> ExistsMusicAsync(string songName);
}