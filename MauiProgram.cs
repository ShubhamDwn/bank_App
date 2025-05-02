using bank_demo.ViewModels;
using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;

namespace bank_demo
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
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            }).UseMauiCommunityToolkitCamera();
#if DEBUG
            builder.Logging.AddDebug();
            // Register ViewModels for Dependency Injection
            builder.Services.AddSingleton<HomeViewModel>(); // Single instance for the app's lifetime
            builder.Services.AddSingleton<LoginViewModel>(); // Registering LoginViewModel
            // Register Pages (if needed)
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<LoginPage>();
#endif
            return builder.Build();
        }
    }
}