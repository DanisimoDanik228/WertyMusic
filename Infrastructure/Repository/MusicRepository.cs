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
        return CreateMusic(_storage.FirstOrDefault(x => x.Id == id));
    }

    public async Task<IEnumerable<Music>?> GetMusicsAsync()
    {
        return new List<Music>(_storage);
    }

    public async Task<Music?> AddMusicAsync(Music music)
    {
        music.Id = Guid.NewGuid();
        
        _storage.Add(music);
        
        return CreateMusic(music);
    }

    public async Task<Music?> UpdateMusicUrlAsync(Guid id, string url)
    {
        var index =  _storage.FindIndex(x => x.Id == id);
        _storage[index].Url = url;
        
        return CreateMusic(_storage[index]);
    }

    public async Task<IEnumerable<Music>?> GetMusicsBySourceNameAsync(string songName)
    {
         return new List<Music>(_storage.Where(x => x.MusicName == songName));
    }
    

    private static Music CreateMusic(Music music)
    {
        return new Music
        {
            Id = Guid.NewGuid(),
            MusicName = music.MusicName,
            ArtistName = music.ArtistName,
            Url = music.Url,
            ArtistUrl = music.ArtistUrl,
            DownloadUrl = music.DownloadUrl,
            CreationDate = DateTime.UtcNow, 
            SiteSource = music.SiteSource,
            SourceName = music.SourceName
        };
    }
}