using HabitTracker.Models;

namespace HabitTracker.Managers.Helpers
{
    public class DayItemsHelper
    {
        DatabaseManager databaseManager;
        public DayItemsHelper(DatabaseManager _databaseManager)
        {
            databaseManager = _databaseManager;
        }

        public async Task<DayHistory> GetOrCreateDayItemAsync(DateTime dateTime)
        {
            var allDayItems = await databaseManager.GetDayHistoriesAsync();
            DayHistory dayItem = allDayItems.FirstOrDefault(i => DateOnly.FromDateTime(i.Day) == DateOnly.FromDateTime(dateTime));

            if (dayItem == null)
            {
                dayItem = new DayHistory { Day = dateTime, RegisteredHabits = new Dictionary<int, bool>() };
                await databaseManager.SaveDayHistoryAsync(dayItem);
            }

            return dayItem;
        }

        public Dictionary<int, bool> UpdateRegisteredHabit(Dictionary<int, bool> registeredHabits, DateTime dateTime, HabitItem habitItem, bool isScheduled)
        {
            var updatedRegisteredHabits = new Dictionary<int, bool>(registeredHabits);

            if (isScheduled)
            {
                bool isHabitDone = habitItem.DoneOn.Contains(DateOnly.FromDateTime(dateTime));

                if (registeredHabits.ContainsKey(habitItem.ID))
                {
                    updatedRegisteredHabits[habitItem.ID] = isHabitDone;
                }
                else
                {
                    updatedRegisteredHabits.Add(habitItem.ID, isHabitDone);
                }
            }
            else
            {
                if (registeredHabits.ContainsKey(habitItem.ID))
                {
                    updatedRegisteredHabits.Remove(habitItem.ID);
                }
            }

            return updatedRegisteredHabits;
        }

        public async Task RefreshDayItemAsync(DateTime dateTime, DayHistory dayItem)
        {
            var todoItems = await databaseManager.GetTodoItemsAsync();
            var todoItemsToday = todoItems.Where(i =>
            {
                if (i.DaysOfWeek.TryGetValue(dateTime.DayOfWeek.ToString(), out bool isScheduled))
                {
                    return isScheduled;
                }
                return false;
            });

            foreach (var item in todoItemsToday)
            {
                var habitItem = await databaseManager.GetHabitItemByTodoItemIdAsync(item.ID);

                dayItem.RegisteredHabits = UpdateRegisteredHabit(dayItem.RegisteredHabits, dayItem.Day, habitItem, true);
            }

            await databaseManager.SaveDayHistoryAsync(dayItem);
        }

        public async Task SaveDayItemAsync(DayHistory dayItem)
        {
            await databaseManager.SaveDayHistoryAsync(dayItem);
        }

        public async Task<IEnumerable<DayHistory>> GetDayItemsAsync()
        {
            return await databaseManager.GetDayHistoriesAsync();
        }

        public async Task<HabitItem> GetHabitItemByTodoItemIdAsync(int todoItemId)
        {
            return await databaseManager.GetHabitItemByTodoItemIdAsync(todoItemId);
        }
    }
}
