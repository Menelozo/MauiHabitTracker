using HabitTracker.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HabitTracker.Data;

public class DayHistoryDatabase
{
    SQLiteAsyncConnection Database;

    public DayHistoryDatabase()
    {
    }

    async Task Init()
    {
        if (Database is not null)
            return;

        Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        var result = await Database.CreateTableAsync<DayHistory>();
    }

    public async Task<List<DayHistory>> GetItemsAsync()
    {
        await Init();
        return await Database.Table<DayHistory>().ToListAsync();
    }

    public async Task<DayHistory> GetItemAsync(int id)
    {
        await Init();
        return await Database.Table<DayHistory>().Where(i => i.ID == id).FirstOrDefaultAsync();
    }

    public async Task<int> SaveItemAsync(DayHistory item)
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

    public async Task<int> DeleteItemAsync(DayHistory item)
    {
        await Init();
        return await Database.DeleteAsync(item);
    }

    public async Task<int> DeleteAllItemsAsync()
    {
        await Init();
        return await Database.DeleteAllAsync<DayHistory>();
    }
}
