using BlazorServerToDoApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorServerToDoApp.Data;


public class DatabaseToDoItemRepository : IToDoItemRepository
{
    public IEnumerable<ToDoItem> ToDoItems => _ToDoItems;

    public event EventHandler? Changed;

    private List<ToDoItem> _ToDoItems = new();

    private IDbContextFactory<ToDoDbContext> DbContextFactory { get; }

    private DateTime _LstRefreshedUTC = DateTime.MinValue;

    public DatabaseToDoItemRepository(IDbContextFactory<ToDoDbContext> dbContextFactory)
    {
        DbContextFactory = dbContextFactory;
        using var dbContext = DbContextFactory.CreateDbContext();
        _ToDoItems = dbContext.ToDoItems.OrderBy(item => item.Id).ToList();
    }

    public async ValueTask RefreshAsync()
    {
        if ((DateTime.UtcNow - _LstRefreshedUTC).TotalSeconds < 5) return;
        _LstRefreshedUTC = DateTime.UtcNow;

        await using var dbContext = await DbContextFactory.CreateDbContextAsync();
        var toDoItems = await dbContext.ToDoItems.OrderBy(item => item.Id).ToListAsync();
        if (toDoItems.Count != _ToDoItems.Count || toDoItems.Zip(_ToDoItems).Any(pair =>
            pair.First.Id != pair.Second.Id ||
            pair.First.Description != pair.Second.Description ||
            pair.First.Done != pair.Second.Done ||
            !Enumerable.SequenceEqual(pair.First.RowVersion ?? Array.Empty<byte>(), pair.Second.RowVersion ?? Array.Empty<byte>())
            )
        )
        {
            _ToDoItems = toDoItems;
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }

    public async ValueTask AddAsync(ToDoItem newItem)
    {
        await using var dbContext = await DbContextFactory.CreateDbContextAsync();
        await dbContext.ToDoItems.AddAsync(newItem);
        await dbContext.SaveChangesAsync();
        _ToDoItems.Add(newItem);
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public async ValueTask UpdateAsync(ToDoItem item)
    {
        await using var dbContext = await DbContextFactory.CreateDbContextAsync();
        var targetItem = await dbContext.ToDoItems.FindAsync(item.Id);
        if (targetItem == null) return;

        targetItem.Done = item.Done;
        dbContext.Entry(targetItem).Property(m => m.RowVersion).OriginalValue = item.RowVersion;
        await dbContext.SaveChangesAsync();

        item.RowVersion = targetItem.RowVersion;
        Changed?.Invoke(this, EventArgs.Empty);
    }

    public async ValueTask DeleteAsync(ToDoItem item)
    {
        await using var dbContext = await DbContextFactory.CreateDbContextAsync();
        var targetItem = await dbContext.ToDoItems.FindAsync(item.Id);
        if (targetItem == null) return;

        dbContext.ToDoItems.Remove(targetItem);
        await dbContext.SaveChangesAsync();
        _ToDoItems.Remove(item);

        Changed?.Invoke(this, EventArgs.Empty);
    }
}