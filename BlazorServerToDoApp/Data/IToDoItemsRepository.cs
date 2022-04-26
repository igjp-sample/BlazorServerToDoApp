using BlazorServerToDoApp.Models;

namespace BlazorServerToDoApp.Data;

public interface IToDoItemRepository
{
    IEnumerable<ToDoItem> ToDoItems { get; }

    event EventHandler? Changed;

    ValueTask RefreshAsync();

    ValueTask AddAsync(ToDoItem newItem);

    ValueTask UpdateAsync(ToDoItem item);

    ValueTask DeleteAsync(ToDoItem item);
}