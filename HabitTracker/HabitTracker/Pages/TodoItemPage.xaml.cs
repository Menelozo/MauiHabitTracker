using HabitTracker.Data;
using HabitTracker.Managers;
using HabitTracker.Models;
using Newtonsoft.Json;

namespace HabitTracker.Pages
{
    [QueryProperty("Item", "Item")]
    public partial class TodoItemPage : ContentPage
    {
        TodoItem item;
        public TodoItem Item
        {
            get => BindingContext as TodoItem;
            set => BindingContext = value;
        }

        private readonly DatabaseManager databaseManager;

        public TodoItemPage(DatabaseManager _databaseManager)
        {
            InitializeComponent();
            databaseManager = _databaseManager;
        }

        protected override void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            CheckAllDaysTapped();
        }

        async void OnSaveClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Item.Name))
            {
                await DisplayAlert("Name Required", "Please enter a name for the todo item.", "OK");
                return;
            }

            var items = await databaseManager.GetTodoItemsAsync();
            bool nameUsed = items.Any(i => i.Name.ToLower() == Item.Name.ToLower() && i.ID != Item.ID);
            if (nameUsed)
            {
                await DisplayAlert("Name Used", "Please enter a name that was not used before.", "OK");
                return;
            }

            bool newHabit = false;
            if (Item.ID == 0)
            {
                newHabit = true;
            }

            await databaseManager.SaveTodoItemAsync(Item);

            if (newHabit)
            {
                HabitItem habit = new HabitItem { TodoItemId = Item.ID, Name = Item.Name, CreatedOn = DateTime.Now };
                await databaseManager.SaveHabitItemAsync(habit);
            }
            else
            {
                HabitItem habit = await databaseManager.GetHabitItemByTodoItemIdAsync(Item.ID);
                habit.Name = Item.Name;
                await databaseManager.SaveHabitItemAsync(habit);
            }

            DayItemsManager registerDayManager = new DayItemsManager(databaseManager);
            await registerDayManager.UpdateDayForHabitChangesAsync(Item);

            await Shell.Current.GoToAsync("..");
        }

        async void OnCancelClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        void OnDayTapped(object sender, EventArgs e)
        {
            if (sender is Border border && border.GestureRecognizers[0] is TapGestureRecognizer tapGestureRecognizer)
            {
                string day = tapGestureRecognizer.CommandParameter as string;

                if (Item.DaysOfWeek.ContainsKey(day))
                {
                    Item.DaysOfWeek[day] = !Item.DaysOfWeek[day];
                    Item.DaysOfWeekSerialized = JsonConvert.SerializeObject(Item.DaysOfWeek);
                }
            }
            CheckAllDaysTapped();
        }

        private async void SelectAllDays_Tapped(object sender, TappedEventArgs e)
        {
            if (sender is Label label && label.GestureRecognizers[0] is TapGestureRecognizer tapGestureRecognizer)
            {
                if (SelectAllDays.Text == "Select All Days")
                {
                    foreach (var key in Item.DaysOfWeek.Keys)
                    {
                        Item.DaysOfWeek[key] = true;
                    }
                }
                else if (SelectAllDays.Text == "Unselect All Days")
                {
                    foreach (var key in Item.DaysOfWeek.Keys)
                    {
                        Item.DaysOfWeek[key] = false;
                    }
                }


                Item.DaysOfWeekSerialized = JsonConvert.SerializeObject(Item.DaysOfWeek);
                CheckAllDaysTapped();
            }
        }

        private async void CheckAllDaysTapped()
        {
            bool allDaysSelected = true;
            foreach (var key in Item.DaysOfWeek.Keys)
            {
                if (Item.DaysOfWeek[key] == false)
                {
                    allDaysSelected = false;
                }
            }
            if (allDaysSelected == false)
            {
                SelectAllDays.Text = "Select All Days";
            }
            else
            {
                SelectAllDays.Text = "Unselect All Days";
            }
        }
    }
}