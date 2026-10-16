namespace GerenciadorChamados.Api.Models;

public class Chamado
{
    public int Id { get; set; }

    public required string Titulo { get; set; }

    public Status? Status { get; set; }

    public int StatusId { get; set; }
}
