using ToDoWebApp.Client.Models;

namespace ToDoWebApp.Services;

public interface ITasksService
{
    Task<List<ToDoTask>> GetTasks();
    Task<ToDoTask> GetById(int id);
    Task Create(ToDoTask task);
    Task Update(int id, UpdateToDoTask task);
    Task Delete(int id);
    Task<bool> UploadFile(IFormFile file);
}