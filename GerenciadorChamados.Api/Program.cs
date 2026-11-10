// Variaveis globais
using GerenciadorChamados.Api.Data;
using GerenciadorChamados.Api.Models;
using GerenciadorChamados.Api.Models.Contracts.Requests;
using GerenciadorChamados.Api.Models.Contracts.Responses;
using GerenciadorChamados.Api.Models.Enums;
using Microsoft.EntityFrameworkCore;

const string GetChamadosEndpointName = "GetChamados";

// Inicializa configurações e serviços
var builder = WebApplication.CreateBuilder(args);

// Criação do DbContext
string connString = "Data Source=ChamadosDb.db";

builder.Services.AddDbContext<ChamadoDbContext>(options => options.UseSqlite(connString));

// Configuração CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AngularLocal",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200").AllowAnyHeader().AllowAnyMethod();
        }
    );
});

// Constrói o objeto WebApplication
var app = builder.Build();

// Habilitar políticas CORS
app.UseCors("AngularLocal");

// Registra o endpoint e seu handler
app.MapGet("/", () => "Hello Ticket Manager App!");

app.MapGet("/saudacao/{nome}", (string nome) => $"Olá, {nome}!");

// Endpoints
app.MapGet(
    "/chamados",
    (ChamadoDbContext db) =>
    {
        return db
            .Chamados.Select(c => new ChamadoResponse(c.Id, c.Titulo, c.Status!.Name))
            .ToListAsync();
    }
);

app.MapGet(
    "/status",
    (ChamadoDbContext db) =>
    {
        return db.Status.Select(s => new StatusResponse(s.Id, s.Name));
    }
);

app.MapGet(
        "/chamados/{id}",
        async (int id, ChamadoDbContext db) =>
        {
            ChamadoResponse? result = await db
                .Chamados.Where(c => c.Id == id)
                .Select(c => new ChamadoResponse(c.Id, c.Titulo, c.Status!.Name))
                .FirstOrDefaultAsync();

            if (result is null)
                return Results.NotFound();

            return Results.Ok(result);
        }
    )
    .WithName(GetChamadosEndpointName);

app.MapPost(
    "/chamados",
    async (NovoChamado novoChamado, ChamadoDbContext db) =>
    {
        await Task.Delay(3000);

        string? titulo = novoChamado.Titulo;

        if (string.IsNullOrWhiteSpace(titulo))
        {
            return Results.BadRequest(new { erro = "Título não pode ser vazio!" });
        }

        titulo = titulo.Trim();

        Chamado chamado = new() { Titulo = titulo };

        db.Chamados.Add(chamado);

        await db.SaveChangesAsync();

        string? statusName = await db
            .Status.Where(s => s.Id == chamado.StatusId)
            .Select(s => s.Name)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(statusName))
        {
            return Results.InternalServerError("Erro interno do servidor!");
        }

        return Results.CreatedAtRoute(
            GetChamadosEndpointName,
            new { id = chamado.Id },
            new ChamadoResponse(chamado.Id, chamado.Titulo, statusName)
        );
    }
);

app.MapPatch(
    "/chamados/{id}/status",
    async (int id, AlterarStatusRequest request, ChamadoDbContext db) =>
    {
        // await Task.Delay(3000);

        Chamado? chamado = await db.Chamados.FirstOrDefaultAsync(c => c.Id == id);

        if (chamado is null)
            return Results.NotFound();

        if (request.Status is null)
            return Results.BadRequest("Status inválido!");

        StatusEnum novoStatus = request.Status.Value;

        ResultadoAlteracaoStatus resultadoAlteracaoStatus = chamado.EditarStatus(novoStatus);

        switch (resultadoAlteracaoStatus)
        {
            case ResultadoAlteracaoStatus.Sucesso:
                await db.SaveChangesAsync();
                break;
            case ResultadoAlteracaoStatus.StatusJaPossuiEsseValor:
                break;
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

        string? statusName = await db
            .Status.Where(s => s.Id == chamado.StatusId)
            .Select(s => s.Name)
            .FirstOrDefaultAsync();

        if (string.IsNullOrEmpty(statusName))
        {
            return Results.InternalServerError("Erro interno do servidor!");
        }

        var result = new ChamadoResponse(chamado.Id, chamado.Titulo, statusName);

        return Results.Ok(result);
    }
);

// Inicia o servidor HTTP
app.Run();
