using Domain.Models;

namespace Domain.Interfaces.File;

public interface IZipCreator
{
    Task<byte[]> CreateZipFromFileListAsync(IEnumerable<Music> musics);
}