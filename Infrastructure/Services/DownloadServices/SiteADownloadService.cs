using Domain.Enum;
using Domain.Interfaces;
using Domain.Models;
using OpenQA.Selenium;

namespace Infrastructure.Services;

public class SiteADownloadService : AbstractSongDowloader, IMusicDownloadService
{
    public async Task<IEnumerable<Music>> DownloadMusicsAsync(string musicName)
    {
        // must set the value of SourceName
        var musics = GetInfoSong(musicName,10);

        return musics.Select(m => new Music()
        {
            MusicName = m.MusicName,
            ArtistName = m.ArtistName,
            DownloadUrl = m.DownloadUrl,
            Url = m.Url,

        });
        
        throw new NotImplementedException();
    }
    
    private static BasicInfoMusic FindApi(string url)
    {
        var driver = SetupDriver();
        try
        {
            driver.Navigate().GoToUrl(url);
            BasicInfoMusic info = new();

            var h1 = driver.FindElement(By.TagName("h1"));
            string[] parts = SanitizeFileName(h1.Text).Trim().Split(new[] { " - " }, 2, StringSplitOptions.None);
            info.ArtistName = parts[0].Trim();
            info.MusicName = parts.Length > 1 ? parts[1].Trim() : "";
            info.ArtistUrl = h1.FindElement(By.CssSelector("a")).GetAttribute("href");
            info.DownloadUrl = driver.FindElement(By.CssSelector("a.b_btn.download.no-ajix[href*='/api/']")).GetAttribute("href");

            
            
            return info;
        }
        finally
        {
            driver.Quit();
        }
    }
    protected override List<BasicInfoMusic> GetInfoSong(string inputName, int count)
    {
        var driver = SetupDriver();
        try
        {
            List<BasicInfoMusic> res = new();
            driver.Navigate().GoToUrl(CreateUrlForSearch(inputName));

            var mainSection = driver.FindElements(By.CssSelector("div.main"));

            if (mainSection.Count == 0)
                return [];

            var songPages = mainSection[0].FindElements(By.CssSelector("a[href*='/mp3/']"));

            for (int i = 0; i < count * 2 && i < songPages.Count() && i < 2 * MaxCountSongForSearchSong; i += 2)
            {
                var t = FindApi(songPages[i].GetAttribute("href"));
                t.Url = DowloadMusic(t,_webStorage);
                res.Add(t);
            }

            return res;
        }
        finally
        {
            driver.Quit();
        }
    }

    protected override string CreateUrlForSearch(string inputName)
    {
        return $"https://sefon.pro/search/{inputName.Replace(" ", "%20")}";
    }
}