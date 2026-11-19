using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Domain.Enums;

namespace minimal_api.Domain.DTOs;

public class AdministradorDTO
{
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public Perfil? Perfil { get; set; } = default!;
}