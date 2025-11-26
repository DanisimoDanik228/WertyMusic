using Domain.Interfaces.Repository;
using Domain.Models;
using Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class MusicDBRepository : IMusicRepository
{
    private readonly AppDbContext _context;
    private IMusicRepository _musicRepositoryImplementation;

    public MusicDBRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Music?> GetMusicByIdAsync(Guid id)
    {
        return await _context.Musics.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<Music>?> GetMusicsAsync()
    {
        return await _context.Musics.AsNoTracking().ToListAsync();
    }

    public async Task<Music?> AddMusicAsync(Music music)
    {
        var m = await _context.Musics.AddAsync(music);

        await _context.SaveChangesAsync();
        
        return m.Entity;
    }

    public async Task<Music?> UpdateMusicAsync(Guid id, Music music)
    {
        var m = await _context.Musics.FirstOrDefaultAsync(m => m.Id == id);
        
        m.ArtistName = music.ArtistName;
        m.CreationDate = music.CreationDate;
        m.DownloadUrl = music.DownloadUrl;
        m.MusicName = music.MusicName;
        m.SiteSource = music.SiteSource;
        m.Url = music.Url;
        
        _context.Musics.Update(m);
        
        await _context.SaveChangesAsync();

        return m;
    }

    public async Task<Music?> DeleteMusicByIdAsync(Guid id)
    {
        var m = await _context.Musics.FirstOrDefaultAsync(m => m.Id == id);
        _context.Musics.Remove(m);

        await _context.SaveChangesAsync();
        return m;
    }

    public async Task<IEnumerable<Music>?> GetMusicsBySourceNameAsync(string sourceMusicName)
    {
        return await _context.Musics.AsNoTracking().Where(m => m.SourceName == sourceMusicName).ToListAsync();
    }

    public Task<bool> ExistsMusicByIdAsync(Guid id)
    {
        return _context.Musics.AsNoTracking().AnyAsync(m => m.Id == id);
    }

    public async Task<Music?> UpdateFieldMusicAsync(Guid id, Music music)
    {
        var m = await _context.Musics.FirstOrDefaultAsync(m => m.Id == id);
        if (m == null)
        {
            return null;
        }
        
        m.ArtistName = (music.ArtistName == null) ? m.ArtistName : music.ArtistName;
        m.Url = (music.Url == null) ? m.Url : music.Url;
        m.CreationDate = (music.CreationDate == null) ? m.CreationDate : music.CreationDate;
        m.DownloadUrl = (music.DownloadUrl == null) ? m.DownloadUrl : music.DownloadUrl;
        m.MusicName = (music.MusicName == null) ? m.MusicName : music.MusicName;
        m.SourceName = (music.SourceName == null) ? m.SourceName : music.SourceName;
        
        await _context.SaveChangesAsync();
        return m;
    }
}
