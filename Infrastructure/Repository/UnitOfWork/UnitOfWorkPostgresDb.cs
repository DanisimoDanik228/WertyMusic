using Domain.Interfaces.Repository;
using Domain.Interfaces.Repository.UnitOfWork;
using Infrastructure.DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace Infrastructure.Repository.UnitOfWork;

public class UnitOfWorkPostgresDb : IUnitOfWork
{
    private readonly AppDbContext _context;
    public IMusicRepository Music { get; }
    
    public UnitOfWorkPostgresDb(IDbContextFactory<AppDbContext> dbContextFactory)
    {
        _context = dbContextFactory.CreateDbContext();
        Music = new MusicPostgresDbRepository(_context);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}