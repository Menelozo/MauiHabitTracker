using HabitTracker.Data;
using HabitTracker.Managers;
using HabitTracker.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HabitTracker.Pages;
public partial class ListingPage : ContentPage
{
    private readonly DatabaseManager databaseManager;

    public ICommand FrameTappedCommand { get; }

    public ObservableCollection<TodoItem> Items { get; set; } = new();
    public ListingPage(DatabaseManager _databaseManager)
    {
        InitializeComponent();
        databaseManager = _databaseManager;
        BindingContext = this;
        FrameTappedCommand = new Command<TodoItem>(OnFrameTapped);
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
    }

    async void OnItemAdded(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(TodoItemPage), true, new Dictionary<string, object>
        {
            ["Item"] = new TodoItem()
        });
    }

    private async void OnFrameTapped(TodoItem tappedItem)
    {
        if (Items.Contains(tappedItem))
        {
            collectionView.SelectedItem = tappedItem;
        }
    }

    private async void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is not TodoItem item)
            return;

        await Shell.Current.GoToAsync(nameof(TodoItemPage), true, new Dictionary<string, object>
        {
            ["Item"] = item
        });
    }

    private async void Delete_Clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is TodoItem item)
        {
            if (item.ID == 0)
                return;

            HabitItem habitItem = await databaseManager.GetHabitItemByTodoItemIdAsync(item.ID);

            DayItemsManager dayItemsManager = new(databaseManager);
            await dayItemsManager.DeleteHabitFromRegisteredHabitsAsync(habitItem.ID);

            if (habitItem != null)
            {
                await databaseManager.DeleteTodoItemAsync(habitItem);
            }

            await databaseManager.DeleteTodoItemAsync(item);
            Items.Remove(item);
        }
    }

    private async void DeleteAllTapped(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Habits Deleted", "Do you want to delete all habits?", "Yes", "No");
        if (!answer)
        {
            return;
        }

        await databaseManager.DeleteAllHabitItemsAsync();
        await databaseManager.DeleteAllTodoItemsAsync();

        DayItemsManager registerDayManager = new(databaseManager);
        await registerDayManager.DeleteAllRegisteredHabitsAsync();

        var items = await databaseManager.GetTodoItemsAsync();
        MainThread.BeginInvokeOnMainThread(() =>
        {
            Items.Clear();
            foreach (var item in items)
                Items.Add(item);
        });
    }
}