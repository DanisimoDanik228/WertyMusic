using Domain.Enum;
using Domain.Interfaces.DownloadServices;
using Domain.Models;
using HtmlAgilityPack;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using PuppeteerSharp;

namespace Infrastructure.Services.DownloadServices;

public class HitmoFindMusic  : BaseMusicFind, IMusicFindService
{
    protected override int _maxCountSongForSearchSong { get; }
    
    public HitmoFindMusic(IOptions<StorageOptions> storageOptions,IOptions<SeleniumOptions> selenuimOptions) 
        : base(storageOptions,selenuimOptions)
    {        
        this._maxCountSongForSearchSong = 5;
    }
    private static string CreateUrlForSearch(string inputName)
    {
        return $"https://eu.hitmo-top.com/search?q={inputName.Replace(" ", "%20")}";
    }

    private async Task<string> FindArtistUrl(string musicUrl)
    {
        string htmlContent;
        using (IWebDriver driver = CreateDriver(_seleniumOptions))
        {
            driver.Navigate().GoToUrl(musicUrl);

            htmlContent = driver.PageSource;
        }
        
        var document = new HtmlDocument();
        document.LoadHtml(htmlContent);

        var artistTag = document.DocumentNode.SelectSingleNode(".//a[@class='breadcrumbs__link play-attr__link']");
        string artistUrl = "https://eu.hitmo-top.com" + artistTag?.GetAttributeValue("href", "") ?? "";
        
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

    public async IAsyncEnumerable<Music> FindMusicsAsync(string musicName)
    {
        string htmlContent;
        using (IWebDriver driver = CreateDriver(_seleniumOptions))
        {
            driver.Navigate().GoToUrl(CreateUrlForSearch(musicName));

            htmlContent = driver.PageSource;
        }

        var document = new HtmlDocument();
        document.LoadHtml(htmlContent);
        
        var liElements = document.DocumentNode.SelectNodes("//ul[@class='tracks__list']/li");

        if (liElements == null)
        {
            yield break;
        }

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
         
            yield return music;
        }
    }
}