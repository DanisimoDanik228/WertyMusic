using System.Net;
using System.Text;
using Domain.Models;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Infrastructure.Services.DownloadServices;

public abstract class BaseSongDowloader
{
    protected abstract string _storageFolder { get; }
    protected abstract int _maxCountSongForSearchSong { get; }

    protected StorageOptions _storageOptions;
    public BaseSongDowloader(IOptions<StorageOptions> options)
    {
        _storageOptions = options.Value;
    }

    private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();
  
    protected static string SanitizeFileName(string fileName, char replacementChar = ' ')
    {
        if (string.IsNullOrEmpty(fileName))
            return fileName;

        var sanitized = new StringBuilder(fileName.Length);

        foreach (char c in fileName)
        {
            if (Array.IndexOf(InvalidFileNameChars, c) >= 0)
                sanitized.Append(replacementChar);
            else
                sanitized.Append(c);
        }

        return sanitized.ToString();
    }
}