using System.Diagnostics.CodeAnalysis;
using GerenciadorChamados.Api.Models.Enums;

namespace GerenciadorChamados.Api.Models;

public class Chamado()
{
    public int Id { get; set; }

    public required string Titulo { get; set; }

    public Status? Status { get; set; }

    public int StatusId { get; private set; } = (int)StatusEnum.Aberto;

    public ResultadoAlteracaoStatus EditarStatus(StatusEnum novoStatus)
    {
        if (!Enum.IsDefined(novoStatus))
            return ResultadoAlteracaoStatus.StatusInvalido;

        if (StatusId == (int)StatusEnum.Concluido)
            return ResultadoAlteracaoStatus.ChamadoJaConcluido;

        if (novoStatus == StatusEnum.Concluido && StatusId != (int)StatusEnum.EmAndamento)
        {
            return ResultadoAlteracaoStatus.PrecisaPassarPorEmAndamento;
        }

        if (StatusId == (int)novoStatus)
            return ResultadoAlteracaoStatus.StatusJaPossuiEsseValor;

        StatusId = (int)novoStatus;
        return ResultadoAlteracaoStatus.Sucesso;
    }
}
