using HabitTracker.Managers;
using HabitTracker.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace HabitTracker.Pages;

public partial class ProfilePage : ContentPage, INotifyPropertyChanged
{
    private readonly DatabaseManager databaseManager;

    public ObservableCollection<DayHistory> DaysHistory { get; set; } = new();
    public ObservableCollection<TodoItem> Items { get; set; } = new();


    public ProfilePage(DatabaseManager _databaseManager)
    {
        InitializeComponent();
        databaseManager = _databaseManager;
        accountCreatedOn.Text += Preferences.Get("accountCreatedOn", "");
        BindingContext = this;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        var items = await databaseManager.GetTodoItemsAsync();
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Items.Clear();
            foreach (var item in items)
                Items.Add(item);

        });

        LoadDataAsync();
    }

    async Task LoadDataAsync()
    {
        var allTodos = await databaseManager.GetTodoItemsAsync();
        var allHabits = await databaseManager.GetHabitItemsAsync();
        var allDaysHistory = await databaseManager.GetDayHistoriesAsync();

        allTodosCount.Text = allTodos.Count().ToString();
        allHabitsCount.Text = allHabits.Count().ToString();
        allDaysHistoryCount.Text = allDaysHistory.Count().ToString();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            DaysHistory.Clear();
            foreach (var dayHistory in allDaysHistory)
                DaysHistory.Add(dayHistory);
        });
    }

    private async void ResetAccountSettings_Tapped(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert("Confirm Reset", "Are you sure you want to reset your account settings?", "Yes", "No");
        if (confirm)
        {
            Preferences.Clear();
            Preferences.Set("firstLaunch", false);
            Preferences.Set("accountCreatedOn", DateOnly.FromDateTime(DateTime.Now).ToString());
            accountCreatedOn.Text = "Account reset performed on: ";
            accountCreatedOn.Text += Preferences.Get("accountCreatedOn", "");

            await databaseManager.DeleteAllTodoItemsAsync();
            await databaseManager.DeleteAllHabitItemsAsync();
            await databaseManager.DeleteAllDayHistoriesAsync();

            LoadDataAsync();

            await DisplayAlert("Reset Complete", "Your account settings have been reset.", "OK");
        }
    }
}