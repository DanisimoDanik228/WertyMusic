using Domain.Models;

namespace Domain.Interfaces.Repository;

public interface IArtistRepository
{
    Task<Artist?> GetArtistcByIdAsync(Guid id);
    Task<IEnumerable<Artist>?> GetArtistAsync();
    Task<Artist?> AddArtistAsync(Artist artist);
    Task<Artist?> DeleteArtistByIdAsync(Guid id);
}