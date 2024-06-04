using HabitTracker.Managers.Helpers;
using HabitTracker.Models;

namespace HabitTracker.Managers
{
    public class DayItemsManager
    {
        DayItemsHelper dayItemsHelper;
        public DayItemsManager(DatabaseManager _databaseManager)
        {
            dayItemsHelper = new DayItemsHelper(_databaseManager);
        }

        public async void RefreshTodayItemAsync()
        {
            DateTime todayDateTime = DateTime.Now;

            DayHistory todayDayItem = await dayItemsHelper.GetOrCreateDayItemAsync(todayDateTime);
            await dayItemsHelper.RefreshDayItemAsync(todayDateTime, todayDayItem);
        }

        public async Task UpdateHabitDoneAsync(DateOnly selectedDateOnly, HabitItem habitItem)
        {
            DateTime selectedDateTime = new DateTime(selectedDateOnly.Year, selectedDateOnly.Month, selectedDateOnly.Day);

            DayHistory dayItem = await dayItemsHelper.GetOrCreateDayItemAsync(selectedDateTime);

            dayItem.RegisteredHabits = dayItemsHelper.UpdateRegisteredHabit(dayItem.RegisteredHabits, dayItem.Day, habitItem, true);

            await dayItemsHelper.SaveDayItemAsync(dayItem);
        }

        public async Task UpdateDayForHabitChangesAsync(TodoItem todoItem)
        {
            DateTime todayDateTime = DateTime.Now;

            DayHistory dayItem = await dayItemsHelper.GetOrCreateDayItemAsync(todayDateTime);

            bool isTodoScheduledForToday = false;
            if (todoItem.DaysOfWeek.TryGetValue(todayDateTime.DayOfWeek.ToString(), out bool isScheduled))
            {
                isTodoScheduledForToday = isScheduled;
            }

            HabitItem habitItem = await dayItemsHelper.GetHabitItemByTodoItemIdAsync(todoItem.ID);

            dayItem.RegisteredHabits = dayItemsHelper.UpdateRegisteredHabit(dayItem.RegisteredHabits, dayItem.Day, habitItem, isTodoScheduledForToday);
            await dayItemsHelper.SaveDayItemAsync(dayItem);
        }

        public async Task DeleteAllRegisteredHabitsAsync()
        {
            var dayItems = await dayItemsHelper.GetDayItemsAsync();

            foreach (var dayItem in dayItems)
            {
                var updatedRegisteredHabits = new Dictionary<int, bool>(dayItem.RegisteredHabits);
                updatedRegisteredHabits.Clear();
                dayItem.RegisteredHabits = updatedRegisteredHabits;
                await dayItemsHelper.SaveDayItemAsync(dayItem);
            }
        }

        public async Task DeleteHabitFromRegisteredHabitsAsync(int habitId)
        {
            var dayItems = await dayItemsHelper.GetDayItemsAsync();
            foreach (var dayItem in dayItems)
            {
                if (dayItem.RegisteredHabits.ContainsKey(habitId))
                {
                    var updatedRegisteredHabits = new Dictionary<int, bool>(dayItem.RegisteredHabits);
                    updatedRegisteredHabits.Remove(habitId);
                    dayItem.RegisteredHabits = updatedRegisteredHabits;
                    await dayItemsHelper.SaveDayItemAsync(dayItem);
                }
            }
        }
    }
}
