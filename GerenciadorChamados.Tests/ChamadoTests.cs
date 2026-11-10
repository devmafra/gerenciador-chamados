using GerenciadorChamados.Api.Models;
using GerenciadorChamados.Api.Models.Enums;
using Xunit;

namespace GerenciadorChamados.Tests;

public class ChamadoTests
{
    [Fact]
    public void EditarStatus_QuandoChamadoJaPossuiEsseValor_DeveRejeitarAlteracao()
    {
        // Arrange
        Chamado chamado = new() { Titulo = "teste" };

        var inicialStatus = chamado.StatusId;
        var novoStatus = StatusEnum.Aberto;
        var expected = ResultadoAlteracaoStatus.StatusJaPossuiEsseValor;

        // Act
        var actual = chamado.EditarStatus(novoStatus);
        // Assert
        Assert.Equal(expected, actual);

        Assert.Equal(inicialStatus, chamado.StatusId);
    }

    [Fact]
    public void EditarStatus_QuandoChamadoConcluido_DeveRejeitarAlteracao()
    {
        // Arrange
        Chamado chamado = new() { Titulo = "teste" };

        chamado.EditarStatus(StatusEnum.EmAndamento);
        chamado.EditarStatus(StatusEnum.Concluido);

        var novoStatus = StatusEnum.Aberto;
        var expected = ResultadoAlteracaoStatus.ChamadoJaConcluido;

        // Act
        var actual = chamado.EditarStatus(novoStatus);
        // Assert
        Assert.Equal(expected, actual);

        Assert.Equal((int)StatusEnum.Concluido, chamado.StatusId);
    }

    [Fact]
    public void EditarStatus_QuandoAbertoTentaConcluir_DeveRejeitarAlteracao()
    {
        // Arrange
        Chamado chamado = new() { Titulo = "teste" };

        var novoStatus = StatusEnum.Concluido;
        var expected = ResultadoAlteracaoStatus.PrecisaPassarPorEmAndamento;

        // Act
        var actual = chamado.EditarStatus(novoStatus);
        // Assert
        Assert.Equal(expected, actual);

        Assert.Equal((int)StatusEnum.Aberto, chamado.StatusId);
    }

    [Fact]
    public void EditarStatus_QuandoChamadoAberto_DeveAceitarAlteracao()
    {
        // Arrange
        Chamado chamado = new() { Titulo = "teste" };

        var novoStatus = StatusEnum.EmAndamento;
        var expected = ResultadoAlteracaoStatus.Sucesso;

        // Act
        var actual = chamado.EditarStatus(novoStatus);
        // Assert
        Assert.Equal(expected, actual);

        Assert.Equal((int)novoStatus, chamado.StatusId);
    }

    [Fact]
    public void EditarStatus_QuandoEmAndamento_DevePermitirConclusao()
    {
        // Arrange
        Chamado chamado = new() { Titulo = "teste" };

        chamado.EditarStatus(StatusEnum.EmAndamento);

        var novoStatus = StatusEnum.Concluido;
        var expected = ResultadoAlteracaoStatus.Sucesso;

        // Act
        var actual = chamado.EditarStatus(novoStatus);
        // Assert
        Assert.Equal(expected, actual);

        Assert.Equal((int)StatusEnum.Concluido, chamado.StatusId);
    }
}
