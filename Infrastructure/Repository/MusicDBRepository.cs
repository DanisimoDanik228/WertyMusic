using Domain.Interfaces.Repository;
using Domain.Models;
using Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class MusicDbRepository : IMusicRepository
{
    private readonly AppDbContext _context;
    private IMusicRepository _musicRepositoryImplementation;

    public MusicDbRepository(AppDbContext context)
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

    public async Task AddMusicRangeAsync(IEnumerable<Music> music)
    {
        await _context.AddRangeAsync(music);
        await _context.SaveChangesAsync();
    }

    public async Task<Music?> UpdateMusicUrlAsync(Guid id, string url)
    {
        var m = await _context.Musics.Where(m => m.Id == id).FirstOrDefaultAsync();

        m.Url = url;
        
        await _context.SaveChangesAsync();
        
        return m;
    }
    
    public async Task<IEnumerable<Music>?> GetMusicsBySourceNameAsync(string sourceMusicName)
    {
        return await _context.Musics.AsNoTracking().Where(m => m.SourceName == sourceMusicName).ToListAsync();
    }
}
