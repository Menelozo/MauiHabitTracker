using HabitTracker.Data;
using HabitTracker.Models;

namespace HabitTracker.Managers
{
    public class DatabaseManager
    {
        private readonly TodoItemDatabase todoDatabase;
        private readonly HabitItemDatabase habitDatabase;
        private readonly DayHistoryDatabase dayHistoryDatabase;

        public DatabaseManager(TodoItemDatabase _todoDatabase, HabitItemDatabase _habitDatabase, DayHistoryDatabase _dayHistoryDatabase)
        {
            todoDatabase = _todoDatabase;
            habitDatabase = _habitDatabase;
            dayHistoryDatabase = _dayHistoryDatabase;
        }

        public async Task<IEnumerable<TodoItem>> GetTodoItemsAsync()
        {
            return await todoDatabase.GetItemsAsync();
        }

        public async Task<IEnumerable<HabitItem>> GetHabitItemsAsync()
        {
            return await habitDatabase.GetItemsAsync();
        }

        public async Task<HabitItem> GetHabitItemByTodoItemIdAsync(int todoItemId)
        {
            return await habitDatabase.GetHabitItemByTodoItemIdAsync(todoItemId);
        }

        public async Task SaveHabitItemAsync(HabitItem habitItem)
        {
            await habitDatabase.SaveItemAsync(habitItem);
        }

        public async Task SaveTodoItemAsync(TodoItem todoItem)
        {
            await todoDatabase.SaveItemAsync(todoItem);
        }

        public async Task<IEnumerable<DayHistory>> GetDayHistoriesAsync()
        {
            return await dayHistoryDatabase.GetItemsAsync();
        }

        public async Task SaveDayHistoryAsync(DayHistory dayHistory)
        {
            await dayHistoryDatabase.SaveItemAsync(dayHistory);
        }

        public async Task<HabitItem> GetHabitItemAsync(int id)
        {
            return await habitDatabase.GetItemAsync(id);
        }

        public async Task DeleteTodoItemAsync(TodoItem item)
        {
            await todoDatabase.DeleteItemAsync(item);
        }

        public async Task DeleteTodoItemAsync(HabitItem habitItem)
        {
            await habitDatabase.DeleteItemAsync(habitItem);
        }

        public async Task DeleteAllTodoItemsAsync()
        {
            var todoItems = await GetTodoItemsAsync();
            await Task.WhenAll(todoItems.Select(item => todoDatabase.DeleteItemAsync(item)));
        }

        public async Task DeleteAllHabitItemsAsync()
        {
            var habitItems = await GetHabitItemsAsync();
            await Task.WhenAll(habitItems.Select(item => habitDatabase.DeleteItemAsync(item)));
        }

        public async Task DeleteAllDayHistoriesAsync()
        {
            var dayHistories = await GetDayHistoriesAsync();
            await Task.WhenAll(dayHistories.Select(item => dayHistoryDatabase.DeleteItemAsync(item)));
        }
    }
}
