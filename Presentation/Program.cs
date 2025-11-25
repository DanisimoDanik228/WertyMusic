using ClassLibrary1.Interfaces;
using ClassLibrary1.Services;
using Domain.Interfaces;
using Domain.Interfaces.Repository;
using Infrastructure.Repository;
using Infrastructure.Services;
using Infrastructure.Services.Files;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddSingleton<IMusicRepository,MusicRepository>();

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