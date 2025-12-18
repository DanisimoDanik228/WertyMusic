using System.Net;
using System.Net.Http.Json;
using Domain.Models;
using Infrastructure.DBContext;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WertyMusic.Requests;

namespace WertyMusic.Tests;


public class CustomWebApplicationFactory
    : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // ❌ удаляем реальную БД
            services.RemoveAll<DbContextOptions<AppDbContext>>();

            // ✅ добавляем InMemory БД
            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });
        });
    }
}

public class TestControllerApi: IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TestControllerApi(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAllMusics_Returns200Ok()
    {
        // Act
        var response = await _client.GetAsync("/api/music/musics");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var musics = await response.Content.ReadFromJsonAsync<List<Music>>();
        Assert.NotNull(musics);
    }

    [Fact]
    public async Task FindMusics_ReturnsFilteredResult()
    {
        // Arrange
        var request = new FindRequest
        {
            musicName = "test"
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/music/musics",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var musics = await response.Content.ReadFromJsonAsync<List<Music>>();
        Assert.NotNull(musics);
    }

    [Fact]
    public async Task Download_ReturnsZipFile()
    {
        // Arrange
        var request = new DonwloadRequest
        {
            musicsId = new List<Guid>()
        };

        // Act
        var response = await _client.PostAsJsonAsync(
            "/api/music/download",
            request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("application/zip", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task GetMusicFile_ReturnsMp3()
    {
        // Arrange
        var fakeId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/music/music/{fakeId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("audio/mpeg", response.Content.Headers.ContentType?.MediaType);
    }
}