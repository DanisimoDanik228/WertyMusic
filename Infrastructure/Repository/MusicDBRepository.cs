using Domain.Interfaces.Repository;
using Domain.Models;
using Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repository;

public class MusicDbRepository : IMusicRepository
{
    private readonly AppDbContext _context;

    public MusicDbRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Music?> GetMusicByIdAsync(Guid id)
    {
        return await _context.Musics.FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<IEnumerable<Music>> GetMusicsByIdsAsync(IEnumerable<Guid> ids)
    {
        return _context.Musics.Where(m => ids.Contains(m.Id)).AsEnumerable();
    }

    public async Task<IEnumerable<Music>> GetMusicsAsync()
    {
        return _context.Musics;
    }

    public async Task<IEnumerable<Music>> GetMusicsBySourceNameAsync(string sourceName)
    {
        return _context.Musics.Where(m => m.SourceName == sourceName).AsEnumerable();
    }

    public async Task AddMusicAsync(Music music)
    {
        await _context.Musics.AddAsync(music);
        await _context.SaveChangesAsync();
    }

    public async Task AddMusicRangeAsync(IEnumerable<Music> music)
    {
        await _context.Musics.AddRangeAsync(music);
        await _context.SaveChangesAsync();
    }
}
