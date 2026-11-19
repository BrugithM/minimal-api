using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Interfaces;
using minimal_api.Domain.ModelViews;
using minimal_api.Domain.Services;
using minimal_api.Infrastructure.Db;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IAdministradorService, AdministradorService>();
builder.Services.AddScoped<IVeiculosService, VeiculoService>();

builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseMySql(builder.Configuration.GetConnectionString("mysql"),
    ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql")));
});

var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administradores
app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorService administradorService) =>
{
    if(administradorService.Login(loginDTO)!=null)
    {
        return Results.Ok("Login com sucesso");
    }
    return Results.Unauthorized();
}).WithTags("Administrador");
#endregion

#region Veiculos
app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculosService veiculosService) =>
{
    var veiculo = new Veiculo
    {
        Nome=veiculoDTO.Nome,
        Marca=veiculoDTO.Marca,
        Ano=veiculoDTO.Ano,
    };
    veiculosService.Salvar(veiculo);
    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
}).WithTags("Veiculo");

app.MapGet("/veiculos", ([FromQuery] int? pagina, IVeiculosService veiculosService) =>
{
    var veiculos = veiculosService.TodosVeiculos(pagina);
    return Results.Ok(veiculos);
}).WithTags("Veiculo");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculosService veiculosService) =>
{
    var veiculo = veiculosService.BuscaPorId(id);
    if(veiculo == null) return Results.NotFound();
    
    return Results.Ok(veiculo);
}).WithTags("Veiculo");

app.MapPut("/veiculos/{id}", ([FromRoute] int id, [FromBody] VeiculoDTO veiculoDTO, IVeiculosService veiculosService) =>
{
    var veiculo = veiculosService.BuscaPorId(id);
    if(veiculo == null) return Results.NotFound();
    
    veiculo.Nome=veiculoDTO.Nome;
    veiculo.Marca=veiculoDTO.Marca;
    veiculo.Ano=veiculoDTO.Ano;

    veiculosService.Atualizar(veiculo);

    return Results.Ok(veiculo);
}).WithTags("Veiculo");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculosService veiculosService) =>
{
    var veiculo = veiculosService.BuscaPorId(id);
    if(veiculo == null) return Results.NotFound();
    
    veiculosService.Apagar(veiculo);

    return Results.NoContent();
}).WithTags("Veiculo");

#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion