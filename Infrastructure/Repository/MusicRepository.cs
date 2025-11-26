using Domain.Enum;
using Domain.Interfaces.Repository;
using Domain.Models;

namespace Infrastructure.Repository;

public class MusicRepository //: IMusicRepository
{
    private readonly List<Music> _storage;

    public MusicRepository()
    {
        _storage = new List<Music>();
    }
    
    public async Task<Music?> GetMusicByIdAsync(Guid id)
    {
        return _storage.FirstOrDefault(x => x.Id == id);
    }

    public async Task<IEnumerable<Music>?> GetMusicsAsync()
    {
        return _storage;
    }

    public async Task<Music?> GetMusicsByNameAsync(string songName)
    {
        return _storage.FirstOrDefault(x => x.MusicName == songName);
    }

    public async Task<Music?> AddMusicAsync(Music music)
    {
        music.Id = Guid.NewGuid();
        
        _storage.Add(music);
        
        return music;
    }

    public async Task<Music?> UpdateMusicAsync(Guid id, Music music)
    {
        var index = _storage.Find(x => x.Id == id);
        
        if(index == null)
            return null;
        
        index.Id = id;
        index.ArtistName =  music.ArtistName;
        index.CreationDate = music.CreationDate;
        index.MusicName = music.MusicName;
        index.SiteSource =  music.SiteSource;
        index.Url = music.Url;
        index.DownloadUrl = music.DownloadUrl;
        
        return index;
    }

    public async Task<Music?> DeleteMusicByIdAsync(Guid id)
    {
        var t =  _storage.FirstOrDefault(x => x.Id == id);
        
        _storage.RemoveAll(x => x.Id == id);

        return t;
    }

    public async Task<IEnumerable<Music>?> GetMusicsBySourceNameAsync(string songName)
    {
         return _storage.Where(m => m.MusicName == songName);
    }

    public async Task<bool> ExistsMusicByIdAsync(Guid id)
    {
        return _storage.Any(x => x.Id == id);
    }
}