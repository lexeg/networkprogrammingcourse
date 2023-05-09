using Microsoft.AspNetCore.Mvc;
using ToDoWebApp.Client.Models;
using ToDoWebApp.Services;

namespace ToDoWebApp.Controllers;

[ApiController]
[Route("[controller]")]
public class ToDoController : ControllerBase
{
    private readonly ITasksService _tasksService;

    public ToDoController(ITasksService tasksService)
    {
        _tasksService = tasksService;
    }

    [HttpGet]
    public Task<List<ToDoTask>> GetTasks()
    {
        return _tasksService.GetTasks();
    }

    [HttpGet("{id}")]
    public Task<ToDoTask> GetById([FromRoute] int id)
    {
        return _tasksService.GetById(id);
    }

    [HttpPost]
    public Task Create([FromBody] ToDoTask task)
    {
        return _tasksService.Create(task);
    }

    [HttpPut("{id}")]
    public Task Update([FromRoute] int id, [FromBody] UpdateToDoTask task)
    {
        return _tasksService.Update(id, task);
    }

    [HttpDelete("{id}")]
    public Task Delete([FromRoute] int id)
    {
        return _tasksService.Delete(id);
    }

    [HttpPost("file")]
    public async Task<IActionResult> UploadFile(IFormFile formFile)
    {
        if (await _tasksService.UploadFile(formFile))
        {
            return Ok();
        }

        return BadRequest();
    }
}