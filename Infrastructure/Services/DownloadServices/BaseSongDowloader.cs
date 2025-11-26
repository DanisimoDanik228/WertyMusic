using System.Net;
using System.Text;
using Domain.Models;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Infrastructure.Services;

public abstract class BaseSongDowloader
{
    protected abstract string storageFolder { get; }
    protected abstract int _maxCountSongForSearchSong { get; }
    protected abstract string CreateUrlForSearch(string inputName);

    protected StorageOptions _storageOptions;
    public BaseSongDowloader(IOptions<StorageOptions> options)
    {
        _storageOptions = options.Value;
    }

    private static readonly char[] InvalidFileNameChars = Path.GetInvalidFileNameChars();
    protected static IWebDriver SetupDriver()
    {
        ChromeDriverService service = ChromeDriverService.CreateDefaultService();
        service.EnableVerboseLogging = false;
        service.SuppressInitialDiagnosticInformation = true;
        service.HideCommandPromptWindow = true;

        ChromeOptions options = new ChromeOptions();

        options.PageLoadStrategy = PageLoadStrategy.Normal;

        options.AddArgument("--window-size=1920,1080");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--headless");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--disable-crash-reporter");
        options.AddArgument("--disable-extensions");
        options.AddArgument("--disable-in-process-stack-traces");
        options.AddArgument("--disable-logging");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--log-level=3");
        options.AddArgument("--output=/dev/null");
        options.AddExcludedArgument("enable-logging");

        return new ChromeDriver(options);
    }
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