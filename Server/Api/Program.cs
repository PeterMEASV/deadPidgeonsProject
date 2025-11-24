using DataAccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MyDbContext>(conf =>
{
    conf.UseNpgsql();
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
