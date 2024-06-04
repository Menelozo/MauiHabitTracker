using HabitTracker.Data;
using HabitTracker.Managers;

namespace HabitTracker.Pages
{
    public partial class MainPage : ContentPage
    {
        public MainPage(DatabaseManager databaseManager)
        {
            InitializeComponent();

            InitializeAsync(databaseManager);
        }

        private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
        }

        private async Task InitializeAsync(DatabaseManager databaseManager)
        {
            //await EmergancyDelete();

            DayItemsManager dayItemsManager = new(databaseManager);
            dayItemsManager.RefreshTodayItemAsync();
        }

        private async Task EmergancyDelete(DatabaseManager databaseManager)
        {
            await databaseManager.DeleteAllTodoItemsAsync();
            await databaseManager.DeleteAllHabitItemsAsync();
            await databaseManager.DeleteAllDayHistoriesAsync();
        }
    }
}
