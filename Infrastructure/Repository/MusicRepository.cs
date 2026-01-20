using Domain.Enum;
using Domain.Interfaces.Repository;
using Domain.Models;

namespace Infrastructure.Repository;

public class MusicRepository : IMusicRepository
{
    private readonly List<Music> _storage;

    public MusicRepository()
    {
        _storage = new List<Music>();
    }

    public async Task<Music?> GetMusicByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Music>?> GetMusicsByIdsAsync(IEnumerable<Guid> ids)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Music>> GetMusicsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<Music>> GetMusicsBySourceNameAsync(string sourceName)
    {
        throw new NotImplementedException();
    }

    public async Task AddMusicAsync(Music music)
    {
        throw new NotImplementedException();
    }

    public async Task AddMusicRangeAsync(IEnumerable<Music> music)
    {
        throw new NotImplementedException();
    }
}