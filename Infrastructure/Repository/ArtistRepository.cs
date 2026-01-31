using Domain.Interfaces.Repository;
using Domain.Models;

namespace Infrastructure.Repository;

public class ArtistRepository :IArtistRepository
{
    private readonly List<Artist> _storage;

    public ArtistRepository()
    {
        _storage = new List<Artist>();
    }
    
    public Task<Artist?> GetArtistcByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Artist>?> GetArtistAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Artist?> AddArtistAsync(Artist artist)
    {
        throw new NotImplementedException();
    }

    public Task<Artist?> DeleteArtistByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}