using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DBContext;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
        // Database.EnsureDeleted();
        // Database.EnsureCreated();
    }
    
    public DbSet<Music> Musics => Set<Music>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
    
}
