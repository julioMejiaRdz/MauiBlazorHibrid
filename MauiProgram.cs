using Microsoft.Extensions.Logging;
using MauiBlazorHibrid.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MauiBlazorHibrid
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Configurar appsettings.json
            using var stream = FileSystem.OpenAppPackageFileAsync("appsettings.json").Result;

            var config = new ConfigurationBuilder()
                .AddJsonStream(stream)
                .Build();

            builder.Configuration.AddConfiguration(config);

            // Obtener cadena de conexión desde configuración
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
