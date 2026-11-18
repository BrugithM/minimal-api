using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;
using minimal_api.Domain.Services;
using minimal_api.Infrastructure.Db;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorService, AdministradorService>();

builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("mysql"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql")));
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdministradorService administradorService) =>
{
    if(administradorService.Login(loginDTO)!=null)
    {
        return Results.Ok("Login com sucesso");
    }
    return Results.Unauthorized();
});

app.Run();

