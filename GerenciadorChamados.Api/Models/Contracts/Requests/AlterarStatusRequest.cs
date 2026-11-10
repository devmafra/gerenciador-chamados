using GerenciadorChamados.Api.Models.Enums;

namespace GerenciadorChamados.Api.Models.Contracts.Requests;

public record AlterarStatusRequest(StatusEnum? Status);
