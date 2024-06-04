using HabitTracker.Models;
using System.Collections.ObjectModel;
using static HabitTracker.Pages.HomePage;

namespace HabitTracker.Managers
{
    public partial class HomePageManager : ContentPage
    {
        private readonly DatabaseManager databaseManager;
        private readonly ContentPage contentPage;

        public HomePageManager(DatabaseManager _databaseManager, ContentPage _contentPage)
        {
            databaseManager = _databaseManager;
            contentPage = _contentPage;
        }

        public async Task UpdateHabitDone(HabitItem item, bool isCompleted, ObservableCollection<HabitItem> Items, ObservableCollection<HabitItem>
            ItemsDone, DateTime SelectedDate, SelectedDateToToday selectedDateToToday,
            CollectionView collectionView1, Label dayCompleted)
        {
            DateOnly selectedDate = DateOnly.FromDateTime(SelectedDate);
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            if (today < selectedDate)
            {
                await contentPage.DisplayAlert("Future Activity", "You can not edit future activity!", "OK");
                return;
            }

            if (today > selectedDate)
            {
                bool answer = await contentPage.DisplayAlert("Historical Activity", "This is past activity, are you sure you want to edit?", "Yes", "No");
                if (!answer)
                {
                    return;
                }
            }

            try
            {
                var updatedDoneOn = new List<DateOnly>(item.DoneOn);

                if (!isCompleted)
                {
                    if (!updatedDoneOn.Contains(selectedDate))
                    {
                        updatedDoneOn.Add(selectedDate);
                    }
                }
                else
                {
                    if (updatedDoneOn.Contains(selectedDate))
                    {
                        updatedDoneOn.Remove(selectedDate);
                    }
                }

                item.DoneOn = updatedDoneOn;
                await databaseManager.SaveHabitItemAsync(item);

                DayItemsManager dayItemsManager = new DayItemsManager(databaseManager);
                await dayItemsManager.UpdateHabitDoneAsync(selectedDate, await databaseManager.GetHabitItemAsync(item.ID));

                await LoadItemsForSelectedDate(Items, ItemsDone, SelectedDate, selectedDateToToday,
                                                            collectionView1, dayCompleted);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        public async Task LoadItemsForSelectedDate(ObservableCollection<HabitItem> Items, ObservableCollection<HabitItem>
            ItemsDone, DateTime SelectedDate, SelectedDateToToday selectedDateToToday,
            CollectionView collectionView1, Label dayCompleted)
        {
            Items.Clear();
            ItemsDone.Clear();

            if (selectedDateToToday == SelectedDateToToday.Future)
            {
                var selectedDayOfWeek = SelectedDate.DayOfWeek.ToString();

                var allItems = await databaseManager.GetTodoItemsAsync();
                var itemsForSelectedDay = allItems.Where(i => i.DaysOfWeek[selectedDayOfWeek]);

                foreach (var item in itemsForSelectedDay)
                {
                    var habitItem = await databaseManager.GetHabitItemByTodoItemIdAsync(item.ID);

                    if (DateOnly.FromDateTime(habitItem.CreatedOn) <= DateOnly.FromDateTime(SelectedDate))
                    {
                        if (!habitItem.DoneOn.Contains(DateOnly.FromDateTime(SelectedDate)))
                        {
                            Items.Add(habitItem);
                        }
                    }
                }

                ToggleCollectionViewVisibility(Items, collectionView1, dayCompleted);
                return;
            }

            var allDaysItems = await databaseManager.GetDayHistoriesAsync();
            DayHistory todayItem = allDaysItems.FirstOrDefault(i => DateOnly.FromDateTime(i.Day) == DateOnly.FromDateTime(SelectedDate));

            if (todayItem == null)
            {
                ToggleCollectionViewVisibility(Items, collectionView1, dayCompleted);
                return;
            }

            var allHabits = await databaseManager.GetHabitItemsAsync();
            List<HabitItem>? selectedDayHabits = new List<HabitItem>();

            foreach (var habit in allHabits)
            {
                if (todayItem.RegisteredHabits.ContainsKey(habit.ID))
                {
                    if (!habit.DoneOn.Contains(DateOnly.FromDateTime(SelectedDate)))
                    {
                        Items.Add(habit);
                    }
                    else
                    {
                        ItemsDone.Add(habit);
                    }
                }
            }

            ToggleCollectionViewVisibility(Items, collectionView1, dayCompleted);
        }

        public void ToggleCollectionViewVisibility(ObservableCollection<HabitItem> Items, CollectionView collectionView1, Label dayCompleted)
        {
            if (Items.Count == 0)
            {
                dayCompleted.IsVisible = true;
                collectionView1.IsVisible = false;
            }
            else
            {
                dayCompleted.IsVisible = false;
                collectionView1.IsVisible = true;
            }
        }

        public void EditViewBasedOnSelectedDate(Label label1, Label label2, CollectionView collectionView2, Label dayCompleted, SelectedDateToToday selectedDateToToday)
        {
            if (selectedDateToToday == SelectedDateToToday.Present)
            {
                label1.Text = "Habits for Today";
                label2.IsVisible = true;
                collectionView2.IsVisible = true;
                dayCompleted.Text = "Day Completed!";
            }
            else if (selectedDateToToday == SelectedDateToToday.Future)
            {
                label1.Text = "Future Habits";
                label2.IsVisible = false;
                collectionView2.IsVisible = false;
                dayCompleted.Text = "No Habits Yet!";
            }
            else
            {
                label1.Text = "Past Habits";
                label2.IsVisible = true;
                collectionView2.IsVisible = true;
                dayCompleted.Text = "Day Completed!";
            }
        }

        public SelectedDateToToday DefineSelectedDateToToday(DateTime SelectedDate, SelectedDateToToday selectedDateToToday)
        {
            DateOnly selectedDate = DateOnly.FromDateTime(SelectedDate);
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            if (today == selectedDate)
            {
                selectedDateToToday = SelectedDateToToday.Present;
            }
            else if (today < selectedDate)
            {
                selectedDateToToday = SelectedDateToToday.Future;
            }
            else
            {
                selectedDateToToday = SelectedDateToToday.Past;
            }

            return selectedDateToToday;
        }
    }
}
