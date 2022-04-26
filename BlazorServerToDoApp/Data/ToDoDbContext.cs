using BlazorServerToDoApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BlazorServerToDoApp.Data;

public class ToDoDbContext : DbContext
{
    public DbSet<ToDoItem> ToDoItems => Set<ToDoItem>();

    public ToDoDbContext(DbContextOptions<ToDoDbContext> options) : base(options)
    {
    }
}