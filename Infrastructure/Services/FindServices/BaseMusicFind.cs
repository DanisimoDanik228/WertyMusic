using System.Net;
using System.Text;
using Domain.Models;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;

namespace Infrastructure.Services.DownloadServices;

public abstract class BaseMusicFind
{
    protected abstract int _maxCountSongForSearchSong { get; }

    protected IWebDriver CreateDriver(SeleniumOptions options)
    {
        var chromeOptions = new ChromeOptions();
        chromeOptions.AddArgument("--headless=new");
        chromeOptions.AddArgument("--no-sandbox");
        chromeOptions.AddArgument("--disable-dev-shm-usage");
        chromeOptions.AddArgument("--disable-gpu");


        if (String.IsNullOrEmpty(options.SeleniumUrl))
        {
            var service = ChromeDriverService.CreateDefaultService();
            return new ChromeDriver(service, chromeOptions); 
        }
        else
        {
            return new RemoteWebDriver(
                new Uri(options.SeleniumUrl),
                chromeOptions
            );
        }
    }
    protected StorageOptions _storageOptions;
    protected SeleniumOptions _seleniumOptions;
    
    public BaseMusicFind(IOptions<StorageOptions> storageOptions,IOptions<SeleniumOptions> selenuimOptions)
    {
        _storageOptions = storageOptions.Value;
        _seleniumOptions = selenuimOptions.Value;
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