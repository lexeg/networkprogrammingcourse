using AutoMapper;
using ToDoWebApp.Client.Models;
using ToDoWebApp.DataAccess.Entities;
using ToDoWebApp.DataAccess.Repositories;

namespace ToDoWebApp.Services;

public class TasksService : ITasksService
{
    private readonly ITasksRepository _tasksRepository;
    private readonly IMapper _mapper;

    public TasksService(ITasksRepository tasksRepository, IMapper mapper)
    {
        _tasksRepository = tasksRepository;
        _mapper = mapper;
    }

    public async Task<List<ToDoTask>> GetTasks()
    {
        var tasks = (await _tasksRepository.GetTasks()).Select(t => _mapper.Map<ToDoTask>(t)).ToList();
        return tasks;
    }

    public async Task<ToDoTask> GetById(int id)
    {
        return _mapper.Map<ToDoTask>(await _tasksRepository.GetById(id));
    }

    public Task Create(ToDoTask task)
    {
        return _tasksRepository.Create(_mapper.Map<ToDoTaskEntity>(task));
    }

    public Task Update(int id, UpdateToDoTask task)
    {
        return _tasksRepository.Update(id, task.Description, task.IsCompleted);
    }

    public Task Delete(int id)
    {
        return _tasksRepository.Delete(id);
    }

    public async Task<bool> UploadFile(IFormFile file)
    {
        try
        {
            if (file.Length <= 0) return false;
            var path = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, "UploadedFiles"));
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            await using var fileStream = new FileStream(Path.Combine(path, file.FileName), FileMode.Create);
            await file.CopyToAsync(fileStream);
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception("File Copy Failed", ex);
        }
    }
}