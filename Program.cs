// Variaveis globais
using GerenciadorChamados.Api.Data;

const string GetChamadosEndpointName = "GetChamados";

// Inicializa configurações e serviços
var builder = WebApplication.CreateBuilder(args);

// Variable of connection
var connString = "Data Source=ChamadosDb.db";
builder.Services.AddSqlite<ChamadoDbContext>(connString);

// Constrói o objeto WebApplication
var app = builder.Build();

// Dados locais
var status = new { online = true, aplicacao = "Ticket Manager" };
var chamado1 = new Chamado("Problema na impressora");
var chamado2 = new Chamado("Computador lento");
chamado2.EditarStatus(StatusEnum.EmAndamento);

List<Chamado> listaChamados = [chamado1, chamado2];

// Registra o endpoint e seu handler
app.MapGet("/", () => "Hello Ticket Manager App!");

app.MapGet("/status", () => status);

app.MapGet("/saudacao/{nome}", (string nome) => $"Olá, {nome}!");

app.MapGet("/chamados", () => listaChamados);

app.MapGet(
        "/chamados/{id}",
        (int id) =>
        {
            Chamado? result = listaChamados.Find(c => c.Id == id);

            if (result != null)
                return Results.Ok(result);
            else
                return Results.NotFound();
        }
    )
    .WithName(GetChamadosEndpointName);

app.MapPost(
    "/chamados",
    (NovoChamado novoChamado) =>
    {
        string? titulo = novoChamado.Titulo;

        if (string.IsNullOrWhiteSpace(titulo))
        {
            return Results.BadRequest(new { erro = "Título não pode ser vazio!" });
        }

        Chamado chamado = new(titulo.Trim());

        listaChamados.Add(chamado);

        return Results.CreatedAtRoute(GetChamadosEndpointName, new { id = chamado.Id }, chamado);
    }
);

app.MapPatch(
    "/chamados/{id}/status",
    (int id, AlterarStatusRequest request) =>
    {
        Chamado? result = listaChamados.Find(c => c.Id == id);

        if (result is null)
            return Results.NotFound();

        if (request.Status is null)
            return Results.BadRequest("Status inválido!");

        StatusEnum novoStatus = request.Status.Value;

        switch (result.EditarStatus(novoStatus))
        {
            case ResultadoAlteracaoStatus.Sucesso:
                return Results.Ok(result);
            case ResultadoAlteracaoStatus.ChamadoJaConcluido:
                return Results.Conflict("Não é possivel alterar um chamado concluído!");
            case ResultadoAlteracaoStatus.StatusInvalido:
                return Results.BadRequest("Status inválido!");
            case ResultadoAlteracaoStatus.PrecisaPassarPorEmAndamento:
                return Results.Conflict(
                    "Para ser marcado como conclúido o chamado precisa passar por Em andamento!"
                );
            default:
                return Results.BadRequest("Ocorreu um erro e o status não pode ser alterado!");
        }
        ;
    }
);

// Inicia o servidor HTTP
app.Run();

public class Chamado
{
    private static int IdRef { get; set; }
    public int Id { get; }
    public string Titulo { get; set; }
    public StatusEnum Status { get; private set; }

    public Chamado(string titulo)
    {
        IdRef++;
        Id = IdRef;
        Titulo = titulo;
        Status = StatusEnum.Aberto;
    }

    public ResultadoAlteracaoStatus EditarStatus(StatusEnum status)
    {
        if (!Enum.IsDefined(status))
            return ResultadoAlteracaoStatus.StatusInvalido;

        if (Status == StatusEnum.Concluido)
            return ResultadoAlteracaoStatus.ChamadoJaConcluido;

        if (status == StatusEnum.Concluido && Status != StatusEnum.EmAndamento)
        {
            return ResultadoAlteracaoStatus.PrecisaPassarPorEmAndamento;
        }

        Status = status;
        return ResultadoAlteracaoStatus.Sucesso;
    }
}

public class NovoChamado(string titulo)
{
    public string Titulo { get; set; } = titulo;
}

public class AlterarStatusRequest(StatusEnum? status)
{
    public StatusEnum? Status { get; set; } = status;
}

public enum ResultadoAlteracaoStatus
{
    Sucesso,
    ChamadoJaConcluido,
    StatusInvalido,
    PrecisaPassarPorEmAndamento,
}

public enum StatusEnum
{
    Aberto = 0,
    EmAndamento = 1,
    Concluido = 2,
}
