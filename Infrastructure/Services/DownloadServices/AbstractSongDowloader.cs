using System.Net;
using System.Text;
using Domain.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Infrastructure.Services;

public abstract class AbstractSongDowloader
    {
        protected string _webStorage =  @"C:\Users\Werty\source\repos\Code\C#\WertyMusic\Presentation\bin\Debug\net9.0\Storage\Source_A";
        
        protected const int MaxCountSongForSearchSong = 1;

        protected abstract List<BasicInfoMusic> GetInfoSong(string inputName);
        protected abstract string CreateUrlForSearch(string inputName);

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