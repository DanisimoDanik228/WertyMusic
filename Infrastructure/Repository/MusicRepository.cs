using Domain.Enum;
using Domain.Interfaces.Repository;
using Domain.Models;

namespace Infrastructure.Repository;

public class MusicRepository : IMusicRepository
{
    private readonly List<Music> _storage;

    public MusicRepository()
    {
        var music = new Music()
        {
            Id = Guid.Parse("18f5eac0-3f0f-475a-baf8-b4e0bdd254b7"),
            Name = "lol",
            ArtistName = "werty",
            CreationDate = DateTime.Now,
            SiteSource = SiteSource.A,
            Url =
                @"C:\Users\Werty\source\repos\Code\C#\WertyMusic\Presentation\bin\Debug\net9.0\Storage\Source_A\Eminem feat Dina Rae - Superman (The Eminem Show 2002).mp3",
            DownloadUrl = "www.werty",
            SourceName = "lol"
        };
        
        _storage = new List<Music>();
        _storage.Add(music);
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
        return _storage.FirstOrDefault(x => x.Name == songName);
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
        index.Name = music.Name;
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

    public async Task<Music?> ExistsMusicAsync(string songName)
    {
        var res = _storage.Where(m => {
            return m.Name == songName;
        });
        
        if (res.Count() == 0)
        {
            return null;
        }
        else
        {
            return res.First();
        }
    }
}