using Domain.Interfaces.Repository;
using Domain.Interfaces.Repository.UnitOfWork;
using Infrastructure.DBContext;

namespace Infrastructure.Repository.UnitOfWork;

public class UnitOfWorkPostgresDb : IUnitOfWork
{
    private readonly AppDbContext _context;
    public IMusicRepository Music { get; }
    
    public UnitOfWorkPostgresDb(AppDbContext context,IMusicRepository musicRepository)
    {
        Music = musicRepository;
        _context = context;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}