using Domain.Interfaces;
using Domain.Interfaces.File;
using Domain.Models;

namespace Infrastructure.Services.Files;

public class FileSender : IFileSender
{
    public async Task<Stream?> GetFileAsync(Music music)
    {
        if (!File.Exists(music.Url))
            return null;

        return new FileStream(music.Url, FileMode.Open, FileAccess.Read, FileShare.Read);
    }
}