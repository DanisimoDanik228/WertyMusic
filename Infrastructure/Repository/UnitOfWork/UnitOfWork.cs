using Domain.Interfaces.Repository;
using Domain.Interfaces.Repository.UnitOfWork;

namespace Infrastructure.Repository.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    public IMusicRepository Music { get; }
    
    public UnitOfWork()
    {
        Music = new MusicRepository();
    }

    public async Task<int> SaveChangesAsync()
    {
        await Task.CompletedTask;
        return 1;
    }
}