using CommunityToolkit.Maui;
using HabitTracker.Data;
using HabitTracker.Managers;
using HabitTracker.Pages;
using Microsoft.Extensions.Logging;

namespace HabitTracker
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
                })
                .UseMauiCommunityToolkit();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            AddViewModels(builder.Services);
            return builder.Build();
        }
        private static IServiceCollection AddViewModels(IServiceCollection services)
        {
            services.AddSingleton<TodoItemDatabase>();
            services.AddSingleton<HabitItemDatabase>();
            services.AddSingleton<DayHistoryDatabase>();

            services.AddSingleton<MainPage>();
            services.AddSingleton<HomePage>();
            services.AddSingleton<ListingPage>();
            services.AddSingleton<ProfilePage>();
            services.AddTransient<TodoItemPage>();

            services.AddSingleton<DatabaseManager>();

            return services;
        }
    }
}
