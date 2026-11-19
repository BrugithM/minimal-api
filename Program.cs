using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Enums;
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
    if (administradorService.Login(loginDTO) != null)
    {
        return Results.Ok("Login com sucesso");
    }
    return Results.Unauthorized();
}).WithTags("Administrador");

app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorService administradorService) =>
{
    var validacao = new ErrosDeValidacao
    {
        Mensagens = new List<string>()
    };

    if (string.IsNullOrEmpty(administradorDTO.Email))
        validacao.Mensagens.Add("O email não pode ser vazio");
    if (string.IsNullOrEmpty(administradorDTO.Password))
        validacao.Mensagens.Add("A senha não pode ser vazia");
    if (administradorDTO.Perfil == null)
        validacao.Mensagens.Add("O perfil não pode ser vazio");

    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

        var administrador = new Administrador
        {
            Email = administradorDTO.Email,
            Password = administradorDTO.Password,
            Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString(),
        };
        administradorService.Salvar(administrador);

        return Results.Created($"/administrador/{administrador.Id}", new AdministradorModelView
        {
            Id = administrador.Id,
            Email = administrador.Email,
            Perfil = administrador.Perfil
        });

}).WithTags("Administrador");

app.MapGet("/administradores", ([FromQuery] int? pagina, IAdministradorService administradorService) =>
{
    var adms = new List<AdministradorModelView>();
    var administradores = administradorService.Todos(pagina);
    foreach(var adm in administradores)
    {
        adms.Add(new AdministradorModelView
        {
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
        });
    }
    return Results.Ok(adms);
}).WithTags("Administrador");

app.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorService administradorService) =>
{
    var adm = administradorService.BuscarPorId(id);
    if(adm == null ) return Results.NotFound();
    return Results.Ok(new AdministradorModelView
        {
            Id = adm.Id,
            Email = adm.Email,
            Perfil = adm.Perfil
        });
}).WithTags("Administrador");

#endregion

#region Veiculos
ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO)
{
    var validacao = new ErrosDeValidacao
    {
        Mensagens = new List<string>()
    };

    if (string.IsNullOrEmpty(veiculoDTO.Nome))
        validacao.Mensagens.Add("O nome não pode ser vazio");
    if (string.IsNullOrEmpty(veiculoDTO.Marca))
        validacao.Mensagens.Add("A marca não pode ser vazia");
    if (veiculoDTO.Ano < 1950)
        validacao.Mensagens.Add("Veiculo muito antigo, aceito somente anos superiores a 1950");

    return validacao;
}

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculosService veiculosService) =>
{
    var validacao = validaDTO(veiculoDTO);
    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano,
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
    if (veiculo == null) return Results.NotFound();

    return Results.Ok(veiculo);
}).WithTags("Veiculo");

app.MapPut("/veiculos/{id}", ([FromRoute] int id, [FromBody] VeiculoDTO veiculoDTO, IVeiculosService veiculosService) =>
{
    var veiculo = veiculosService.BuscaPorId(id);
    if (veiculo == null) return Results.NotFound();

    var validacao = validaDTO(veiculoDTO);
    if (validacao.Mensagens.Count > 0)
        return Results.BadRequest(validacao);

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculosService.Atualizar(veiculo);

    return Results.Ok(veiculo);
}).WithTags("Veiculo");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculosService veiculosService) =>
{
    var veiculo = veiculosService.BuscaPorId(id);
    if (veiculo == null) return Results.NotFound();

    veiculosService.Apagar(veiculo);

    return Results.NoContent();
}).WithTags("Veiculo");

#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.Run();
#endregion