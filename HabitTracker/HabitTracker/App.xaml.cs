namespace HabitTracker
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            bool isFirstLaunch = Preferences.Get("firstLaunch", true);

            if (isFirstLaunch)
            {
                Preferences.Set("firstLaunch", false);
                Preferences.Set("accountCreatedOn", DateOnly.FromDateTime(DateTime.Now).ToString());
            }

            MainPage = new AppShell();
        }
    }
}
