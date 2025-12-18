using Domain.Models;

namespace WertyMusic.Tests.Controllers.Fixtures;

public static class MusicTestData
{
    public static readonly Music Music1 = new()
    {
        Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
        MusicName = "Test Song 1"
    };

    public static readonly Music Music2 = new()
    {
        Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
        MusicName = "Test Song 2"
    };

    public static List<Music> GetMusicList() => new()
    {
        Music1,
        Music2
    };
}
