# Gerenciador de Chamados

Este projeto acompanha meu aprendizado com C# e desenvolvimento web. Comecei criando um programa de console para praticar orientação a objetos. Depois, levei as regras para uma API com ASP.NET Core e SQLite. Por fim, desenvolvi uma interface Angular para trabalhar com os chamados pela API.

Ainda uso o projeto para aprender e experimentar com .NET, Angular e testes automatizados.

## Executar o projeto

É necessário ter o .NET 10 SDK, Node.js e npm instalados.

### Preparar e iniciar a API

A primeira vez, crie o banco aplicando as migrations. Se ainda não tiver a ferramenta `dotnet-ef`, instale a versão compatível com o EF Core 10:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.13
```

Em seguida, execute os comandos dentro da pasta da API:

```powershell
cd GerenciadorChamados.Api
dotnet restore
dotnet ef database update
dotnet run --launch-profile http
```

A API fica disponível em `http://localhost:5051`.

### Iniciar a interface

Em outro terminal, na raiz do projeto:

```powershell
cd gerenciador-chamados-web
npm ci
npm start
```

Acesse `http://localhost:4200`. Mantenha a API em execução enquanto usa a interface.

### Console e testes

Na raiz do projeto, inicie a versão de console ou rode os testes .NET:

```powershell
dotnet run --project GerenciadorChamados.Console/gerenciador-chamado.csproj
dotnet test GerenciadorChamados.Tests/GerenciadorChamados.Tests.csproj
```

Para testar a interface Angular:

```powershell
cd gerenciador-chamados-web
npm test -- --watch=false
```
