using Domain.Interfaces.Repository;
using Domain.Models;
using MongoDB.Driver;

namespace Infrastructure.Repository;

public class MusicMongoDbRepository : IMusicRepository
{
    private readonly IMongoCollection<Music> _musicCollection;
    
    public MusicMongoDbRepository(IMongoDatabase db)
    {
        _musicCollection = db.GetCollection<Music>("musics");
    }
    public async Task<Music?> GetMusicByIdAsync(Guid id)
    {
        return await _musicCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<Music>> GetMusicsByIdsAsync(IEnumerable<Guid> ids)
    {
        return await _musicCollection.Find(x => ids.Contains(x.Id)).ToListAsync();
    }

    public async Task<IEnumerable<Music>> GetMusicsAsync()
    {
        return await _musicCollection.Find(_ => true).ToListAsync();
    }

    public async Task<IEnumerable<Music>> GetMusicsBySourceNameAsync(string sourceName)
    {
        return await _musicCollection.Find(x => x.SourceName == sourceName).ToListAsync();
    }

    public async Task AddMusicAsync(Music music)
    {
        await _musicCollection.InsertOneAsync(music);
    }

    public async Task AddMusicRangeAsync(IEnumerable<Music> music)
    {
        await _musicCollection.InsertManyAsync(music);
    }
}