using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BlazorServerToDoApp.Data;
using BlazorServerToDoApp.Models;

namespace BlazorServerToDoApp.Internals;

internal class MockToDoItemRepository : IToDoItemRepository
{
    public IEnumerable<ToDoItem> ToDoItems => _ToDoItems;

    public event EventHandler? Changed;

    private List<ToDoItem> _ToDoItems = new List<ToDoItem>();

    public MockToDoItemRepository(ToDoItem[] toDoItems)
    {
        _ToDoItems.AddRange(toDoItems);
    }

    public ValueTask AddAsync(ToDoItem newItem)
    {
        _ToDoItems.Add(newItem);
        Changed?.Invoke(this, EventArgs.Empty);
        return ValueTask.CompletedTask;
    }

    public ValueTask DeleteAsync(ToDoItem item)
    {
        _ToDoItems.Remove(item);
        Changed?.Invoke(this, EventArgs.Empty);
        return ValueTask.CompletedTask;
    }

    public ValueTask RefreshAsync()
    {
        return ValueTask.CompletedTask;
    }

    public ValueTask UpdateAsync(ToDoItem item)
    {
        throw new NotImplementedException();
    }
}