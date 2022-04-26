using System.ComponentModel.DataAnnotations;

namespace BlazorServerToDoApp.Models;

public class ToDoItem
{
    public int Id { get; set; }

    [Required]
    public string Description { get; set; } = "";

    public bool Done { get; set; }

    [Timestamp]
    public byte[]? RowVersion { get; set; }
}