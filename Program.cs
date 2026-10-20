// Variaveis globais
using GerenciadorChamados.Api.Data;
using GerenciadorChamados.Api.Models;
using GerenciadorChamados.Api.Models.Contracts.Requests;
using GerenciadorChamados.Api.Models.Enums;
using Microsoft.EntityFrameworkCore;

const string GetChamadosEndpointName = "GetChamados";

// Inicializa configurações e serviços
var builder = WebApplication.CreateBuilder(args);

// Criação do DbContext
string connString = "Data Source=ChamadosDb.db";

builder.Services.AddDbContext<ChamadoDbContext>(options => options.UseSqlite(connString));

// Constrói o objeto WebApplication
var app = builder.Build();

// Registra o endpoint e seu handler
app.MapGet("/", () => "Hello Ticket Manager App!");

app.MapGet("/saudacao/{nome}", (string nome) => $"Olá, {nome}!");

// Endpoints
app.MapGet(
    "/chamados",
    (ChamadoDbContext db) =>
    {
        return db.Chamados.Include(c => c.Status).ToListAsync();
    }
);

app.MapGet(
        "/chamados/{id}",
        async (int id, ChamadoDbContext db) =>
        {
            Chamado? result = await db
                .Chamados.Include(c => c.Status)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (result != null)
                return Results.Ok(result);
            else
                return Results.NotFound();
        }
    )
    .WithName(GetChamadosEndpointName);

app.MapPost(
    "/chamados",
    async (NovoChamado novoChamado, ChamadoDbContext db) =>
    {
        string? titulo = novoChamado.Titulo.Trim();

        if (string.IsNullOrWhiteSpace(titulo))
        {
            return Results.BadRequest(new { erro = "Título não pode ser vazio!" });
        }

        Chamado chamado = new() { Titulo = titulo };

        db.Chamados.Add(chamado);

        await db.SaveChangesAsync();

        return Results.CreatedAtRoute(GetChamadosEndpointName, new { id = chamado.Id }, chamado);
    }
);

app.MapPatch(
    "/chamados/{id}/status",
    async (int id, AlterarStatusRequest request, ChamadoDbContext db) =>
    {
        Chamado? result = await db.Chamados.FirstOrDefaultAsync(c => c.Id == id);

        if (result is null)
            return Results.NotFound();

        if (request.Status is null)
            return Results.BadRequest("Status inválido!");

        StatusEnum novoStatus = request.Status.Value;

        switch (result.EditarStatus(novoStatus))
        {
            case ResultadoAlteracaoStatus.Sucesso:
                await db.SaveChangesAsync();

                result = await db
                    .Chamados.Include(c => c.Status)
                    .FirstOrDefaultAsync(c => c.Id == id);

                return Results.Ok(result);
            case ResultadoAlteracaoStatus.ChamadoJaConcluido:
                return Results.Conflict("Não é possivel alterar um chamado concluído!");
            case ResultadoAlteracaoStatus.StatusInvalido:
                return Results.BadRequest("Status inválido!");
            case ResultadoAlteracaoStatus.PrecisaPassarPorEmAndamento:
                return Results.Conflict(
                    "Para ser marcado como conclúido o chamado precisa passar por Em andamento!"
                );
            case ResultadoAlteracaoStatus.StatusJaPossuiEsseValor:
                return Results.Ok("O status já possui esse valor definido!");
            default:
                return Results.BadRequest("Ocorreu um erro e o status não pode ser alterado!");
        }
        ;
    }
);

// Inicia o servidor HTTP
app.Run();
