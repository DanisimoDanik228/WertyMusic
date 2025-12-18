using Infrastructure.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace WertyMusic.Extensions;

public static class ApplicationBuilderExtensions
{
    public static WebApplication UseStorageDirectory(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var options = scope.ServiceProvider.GetRequiredService<IOptions<StorageOptions>>();
        var directoryPath = options.Value.LocalStorage;

        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        return app;
    }

    public static WebApplication UseCustomRouting(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();  
            app.UseSwaggerUI(); 
        }
        
        app.UseRouting();
        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.MapStaticAssets();
        app.MapControllerRoute(
                name: "default",
                pattern: "{controller=BasiMusic}/{action=Index}/{id?}")
            .WithStaticAssets();
        
        app.MapRazorPages();

        return app;
    }
}