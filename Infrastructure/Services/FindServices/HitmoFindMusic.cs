using Domain.Enum;
using Domain.Interfaces.DownloadServices;
using Domain.Models;
using HtmlAgilityPack;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using PuppeteerSharp;

namespace Infrastructure.Services.DownloadServices;

public class HitmoFindMusic  : BaseMusicFind, IMusicFindService
{
    private IBrowser? _browser;
    protected override int _maxCountSongForSearchSong { get; }
    
    
    public HitmoFindMusic(IOptions<StorageOptions> options) : base(options)
    {        
        this._maxCountSongForSearchSong = 5;
    }
    private static string CreateUrlForSearch(string inputName)
    {
        return $"https://eu.hitmo-top.com/search?q={inputName.Replace(" ", "%20")}";
    }

    private static async Task<string> FindArtistUrl(string musicUrl)
    {
        var chromeOptions = new ChromeOptions();
        chromeOptions.AddArgument("--headless");

        string artistUrl;
        using (IWebDriver driver = new ChromeDriver(chromeOptions))
        {
            
            driver.Navigate().GoToUrl(musicUrl);

            string htmlContent = driver.PageSource;

            var document = new HtmlDocument();
            document.LoadHtml(htmlContent);

            var artistTag = document.DocumentNode.SelectSingleNode(".//a[@class='breadcrumbs__link play-attr__link']");
            artistUrl = "https://eu.hitmo-top.com" + artistTag?.GetAttributeValue("href", "") ?? "";
        }
        
        return artistUrl;
    }

    private static string FixSongName(string inputName)
    {
        int startIndex = 0;

        while (startIndex < inputName.Length && (inputName[startIndex] == ' ' || inputName[startIndex] == '\r'|| inputName[startIndex] == '\n'))
        {
            startIndex++;
        }
        
        int  endIndex = inputName.Length - 1;
        while (endIndex > startIndex && (inputName[endIndex] == ' ' || inputName[endIndex] == '\r'|| inputName[endIndex] == '\n'))
        {
            endIndex--;
        }
        
        return inputName.Substring(startIndex, endIndex - startIndex);
    }

    public async Task<IEnumerable<Music>> FindMusicsAsync(string musicName)
    {
        List<Music> res = new();
        var chromeOptions = new ChromeOptions();
        chromeOptions.AddArgument("--headless"); 
        
        using (IWebDriver driver = new ChromeDriver(chromeOptions))
        {        
            driver.Navigate().GoToUrl(CreateUrlForSearch(musicName));
            
            string htmlContent = driver.PageSource;
            
            var document = new HtmlDocument();
            document.LoadHtml(htmlContent);
            
            var liElements = document.DocumentNode.SelectNodes("//ul[@class='tracks__list']/li");

            for (int i = 0; i < liElements.Count && i < _maxCountSongForSearchSong; i++)
            {
                Music music = new();
                    
                var liElement  = liElements[i];
                music.ArtistName = liElement.SelectSingleNode(".//div[@class='track__desc']")?.InnerText; 
                var firstMusicName = liElement.SelectSingleNode(".//div[@class='track__title']")?.InnerText;
                music.MusicName = FixSongName(firstMusicName);
                
                var songUrl = liElement.SelectSingleNode(".//a[@class='track__info-l']").GetAttributeValue("href", "");
                music.ArtistUrl = await FindArtistUrl("https://eu.hitmo-top.com" + songUrl);
                
                var musicLinks = liElement.SelectSingleNode(".//a[contains(@href, '/get/music/')]");
                music.DownloadUrl = musicLinks.GetAttributeValue("href", "");
                music.SiteSource = SiteSource.B;
                music.SourceName = musicName;
                music.CreationDate = DateTime.UtcNow;
                
                res.Add(music);
            }
        }
        
        return res;
    }
}