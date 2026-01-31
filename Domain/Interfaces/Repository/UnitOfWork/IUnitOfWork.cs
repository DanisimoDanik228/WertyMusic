namespace Domain.Interfaces.Repository.UnitOfWork;

public interface IUnitOfWork
{
    IMusicRepository Music { get; }
    Task<int> SaveChangesAsync();
}