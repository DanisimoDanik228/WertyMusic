using Infrastructure.Options;
using WertyMusic;
using WertyMusic.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false) 
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Services.AddCustomOptions(builder.Configuration);
builder.Services.AddCustomServices(builder.Configuration);
builder.Services.AddCustomStorageDirectory(builder.Configuration);

var generalSettings = builder.Configuration.Get<GeneralSettings>();
if (generalSettings.AvailableSwagger)
{
    builder.Services.AddCustomSwagger(builder.Configuration);
}

var app = builder.Build();

app.UseCustomMiddlewares();
app.UseCustomStorageDirectory();
app.UseCustomRouting();

if (generalSettings.AvailableSwagger)
{
    app.UseCustomSwagger();
}
if (generalSettings.AvailableMigration)
{
    app.RunCustomMigration();
}

app.Run();