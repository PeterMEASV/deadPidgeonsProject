using api;
using Api.Security;
using Api.Services.Classes;
using Api.Services.Interfaces;
using DataAccess;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

var appOptions = builder.Services.AddAppOptions(builder.Configuration);

builder.Services.AddDbContext<MyDbContext>(conf =>
{
    conf.UseNpgsql(appOptions.DBConnectionString);
});

builder.Services.AddScoped<KonciousArgon2idPasswordHasher>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "Dead Pidgeons API";
    config.Version = "v0.1";
});

var app = builder.Build();

app.UseCors(config => config.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.MapControllers();

app.UseOpenApi();
app.UseSwaggerUi();

app.Run();
