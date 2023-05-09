using AutoMapper;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using ToDoWebApp.Configuration;
using ToDoWebApp.DataAccess.Contexts;
using ToDoWebApp.DataAccess.Repositories;
using ToDoWebApp.Services;

// Set the limit to 256 MB
const int maxLimit = 268435456;

var builder = WebApplication.CreateBuilder(args);

builder.Host.ConfigureAppConfiguration(x => x.AddJsonFile("appsettings.development.json"));

// Add services to the container.
builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = maxLimit;
});

builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = maxLimit;
    options.MultipartBodyLengthLimit = maxLimit;
    options.MultipartHeadersLengthLimit = maxLimit;
});

builder.Services.AddSingleton(
    _ => new MapperConfiguration(cfg => { cfg.AddProfile(new AutoMapping()); }).CreateMapper());
builder.Services.AddDbContext<ToDoDbContext>(optionsBuilder =>
    optionsBuilder.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<ITasksRepository, TasksRepository>();
builder.Services.AddScoped<ITasksService, TasksService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();