using GerenciadorChamados.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GerenciadorChamados.Api.Data;

public class ChamadoDbContext(DbContextOptions<ChamadoDbContext> options) : DbContext(options)
{
    public DbSet<Models.Chamado> Chamados => Set<Models.Chamado>();
    public DbSet<Status> Status => Set<Status>();
}
