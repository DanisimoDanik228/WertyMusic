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

var generalSettings = builder.Configuration.GetSection("GeneralSettings").Get<GeneralSettings>();
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

var dbSettings = builder.Configuration.GetSection("DatabaseOptions").Get<DatabaseOptions>();
if (dbSettings.Db == "Postgres" && generalSettings.AvailableMigration)
{
    app.RunCustomMigration();
}

app.Run();