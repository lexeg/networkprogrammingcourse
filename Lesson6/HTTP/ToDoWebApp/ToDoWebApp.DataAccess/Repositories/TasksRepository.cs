using Microsoft.EntityFrameworkCore;
using ToDoWebApp.DataAccess.Contexts;
using ToDoWebApp.DataAccess.Entities;

namespace ToDoWebApp.DataAccess.Repositories;

public class TasksRepository : ITasksRepository
{
    private readonly ToDoDbContext _dbContext;

    public TasksRepository(ToDoDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<List<ToDoTaskEntity>> GetTasks()
    {
        return _dbContext.ToDoTasks.ToListAsync();
    }

    public Task<ToDoTaskEntity> GetById(int id)
    {
        var task = _dbContext.ToDoTasks.FirstOrDefault(t => t.Id == id);
        return Task.FromResult(task);
    }

    public Task Create(ToDoTaskEntity task)
    {
        _dbContext.ToDoTasks.Add(task);
        _dbContext.SaveChanges();
        return Task.CompletedTask;
    }

    public async Task Update(int id, string description, bool isCompleted)
    {
        var foundTask = await GetById(id);
        if (foundTask == null) return;
        foundTask.Description = description;
        foundTask.IsCompleted = isCompleted;
        await _dbContext.SaveChangesAsync();
    }

    public async Task Delete(int id)
    {
        var task = await GetById(id);
        _dbContext.ToDoTasks.Remove(task);
        await _dbContext.SaveChangesAsync();
    }
}