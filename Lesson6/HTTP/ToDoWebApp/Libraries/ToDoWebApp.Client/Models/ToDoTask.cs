namespace ToDoWebApp.Client.Models;

public class ToDoTask
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime DeadlineDate { get; set; }
    public bool IsCompleted { get; set; }
}