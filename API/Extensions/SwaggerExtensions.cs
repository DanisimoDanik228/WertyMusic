using System.Reflection;
using Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace WertyMusic;

public static class SwaggerExtensions
{
    public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSwaggerGen(options =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });
        
        return services;
    }
    
    public static WebApplication UseCustomSwagger(this WebApplication app)
    {
        if (true)
        {
            app.UseSwagger();  
            app.UseSwaggerUI(); 
        }
        
        return app;
    }
}