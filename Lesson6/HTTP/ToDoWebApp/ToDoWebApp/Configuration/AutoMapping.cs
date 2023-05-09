using AutoMapper;
using ToDoWebApp.Client.Models;
using ToDoWebApp.DataAccess.Entities;

namespace ToDoWebApp.Configuration;

public class AutoMapping : Profile
{
    public AutoMapping()
    {
        CreateMap<ToDoTaskEntity, ToDoTask>().ReverseMap();
    }
}