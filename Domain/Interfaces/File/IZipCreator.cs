using Domain.Models;

namespace Domain.Interfaces.File;

public interface IZipCreator
{
    void CreateZipFromFileList(IEnumerable<Music> musics, string zipPath);
}