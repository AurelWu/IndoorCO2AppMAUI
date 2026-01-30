using Syncfusion.Maui.Core.Hosting;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
using Mapsui.UI.Maui;
using SkiaSharp.Views.Maui.Controls.Hosting;

namespace IndoorCO2App_Multiplatform
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {

            var builder = MauiApp.CreateBuilder();      

            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseSkiaSharp()
                .ConfigureSyncfusionCore()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
