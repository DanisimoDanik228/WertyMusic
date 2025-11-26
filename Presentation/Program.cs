using ClassLibrary1.Interfaces;
using ClassLibrary1.Services;
using Domain.Interfaces;
using Domain.Interfaces.DownloadServices;
using Domain.Interfaces.File;
using Domain.Interfaces.Repository;
using Infrastructure.DBContext;
using Infrastructure.Options;
using Infrastructure.Repository;
using Infrastructure.Services;
using Infrastructure.Services.DownloadServices;
using Infrastructure.Services.Files;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");

builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IMusicRepository,MusicDBRepository>();
builder.Services.Configure<StorageOptions>(builder.Configuration.GetSection("MusicOptions"));

builder.Services.AddScoped<IFileSender,FileSender>();

builder.Services.AddScoped<IMusicService,MusicService>();
builder.Services.AddScoped<IMusicDownloadService, SiteADownloadService>();
// builder.Services.AddScoped<IMusicDownloadService, SiteBDownloadService>();
// builder.Services.AddScoped<IMusicDownloadService, SiteCDownloadService>();

var app = builder.Build();

app.UseRouting();
app.MapControllers();

app.MapGet("/", () => "Hello Werty");

app.Run();