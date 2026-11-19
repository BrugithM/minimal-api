using minimal_api.Domain.DTOs;
using minimal_api.Domain.Entities;

namespace minimal_api.Domain.Interfaces;

public interface IAdministradorService
{
    Administrador? Login(LoginDTO loginDTO);
    Administrador Salvar(Administrador administrador);
    List<Administrador> Todos(int? pagina);
    Administrador? BuscarPorId(int id);

}