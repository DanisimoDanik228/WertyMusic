using ClassLibrary1.Interfaces;
using ClassLibrary1.Services;
using Domain.Interfaces.DownloadServices;
using Domain.Interfaces.File;
using Domain.Interfaces.Repository;
using Infrastructure.DBContext;
using Infrastructure.Options;
using Infrastructure.Repository;
using Infrastructure.Services.DownloadService;
using Infrastructure.Services.DownloadServices;
using Infrastructure.Services.Files;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");
builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IMusicRepository,MusicDBRepository>();
builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("MusicOptions"));
builder.Services.AddScoped<IFileSender,FileSender>();

builder.Services.AddScoped<IMusicService,MusicService>();
builder.Services.AddScoped<IDownloaderService,MusicDownloader>();
builder.Services.AddScoped<IMusicFindService, SefonFindMusic>();
// builder.Services.AddScoped<IMusicFindService, SiteBDownloadService>();
// builder.Services.AddScoped<IMusicFindService, SiteCDownloadService>();

var app = builder.Build();

app.UseRouting();
app.MapControllers();
app.MapRazorPages();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=BasiMusic}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();