using Domain.Interfaces.Repository;
using Domain.Interfaces.Repository.UnitOfWork;

namespace Infrastructure.Repository.UnitOfWork;

public class UnitOfWorkMongoDb : IUnitOfWork
{
    public IMusicRepository Music { get; }

    public UnitOfWorkMongoDb(IMusicRepository music)
    {
        Music = music;
    }
    
    public async Task<int> SaveChangesAsync()
    {
        await Task.CompletedTask;
        return 1;
    }
}