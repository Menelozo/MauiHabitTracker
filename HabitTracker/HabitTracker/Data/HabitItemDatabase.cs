using HabitTracker.Models;
using SQLite;

namespace HabitTracker.Data;

public class HabitItemDatabase
{
    SQLiteAsyncConnection Database;

    public HabitItemDatabase()
    {
    }

    async Task Init()
    {
        if (Database is not null)
            return;

        Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        var result = await Database.CreateTableAsync<HabitItem>();
    }

    public async Task<List<HabitItem>> GetItemsAsync()
    {
        await Init();
        return await Database.Table<HabitItem>().ToListAsync();
    }

    public async Task<HabitItem> GetItemAsync(int id)
    {
        await Init();
        return await Database.Table<HabitItem>().Where(i => i.ID == id).FirstOrDefaultAsync();
    }

    public async Task<int> SaveItemAsync(HabitItem item)
    {
        await Init();
        if (item.ID != 0)
        {
            return await Database.UpdateAsync(item);
        }
        else
        {
            return await Database.InsertAsync(item);
        }
    }

    public async Task<int> DeleteItemAsync(HabitItem item)
    {
        await Init();
        return await Database.DeleteAsync(item);
    }

    public async Task<int> DeleteAllItemsAsync()
    {
        await Init();
        return await Database.DeleteAllAsync<HabitItem>();
    }


    public async Task<HabitItem> GetHabitItemByTodoItemIdAsync(int todoItemId)
    {
        await Init();
        return await Database.Table<HabitItem>().Where(i => i.TodoItemId == todoItemId).FirstOrDefaultAsync();
    }
}
