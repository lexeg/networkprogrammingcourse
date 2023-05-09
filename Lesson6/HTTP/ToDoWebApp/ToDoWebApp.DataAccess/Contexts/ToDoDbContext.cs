using Microsoft.EntityFrameworkCore;
using ToDoWebApp.DataAccess.Entities;

namespace ToDoWebApp.DataAccess.Contexts;

public sealed class ToDoDbContext : DbContext
{
    public ToDoDbContext(DbContextOptions<ToDoDbContext> options) : base(options)
    {
        Database.EnsureCreated();
    }

    public DbSet<ToDoTaskEntity> ToDoTasks { get; set; }
}