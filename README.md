# Gerenciador de Chamados

Projeto de estudo para praticar fundamentos de C#/.NET e Angular por meio de um fluxo simples de abertura, consulta e atualização de chamados. O repositório reúne uma aplicação de console, uma API HTTP, testes de domínio e uma interface web.

> **Estado:** projeto em desenvolvimento e adequado como demonstração de aprendizado. A API e o console compilam; o build de produção do Angular passa. Os testes de domínio xUnit permanecem inconclusivos neste ambiente; os 6 testes Angular passam quando seus 5 arquivos são executados sequencialmente, mas o comando completo apresenta erros de acesso a arquivos temporários entre workers. Consulte [Testes](#testes) e [Pontos a evoluir](#pontos-a-evoluir) antes de avaliar o estado atual.

## Estrutura

```text
GerenciadorChamados.slnx
├── GerenciadorChamados.Console/   Aplicação de terminal independente
├── GerenciadorChamados.Api/       API Minimal e persistência SQLite
├── GerenciadorChamados.Tests/     Testes xUnit das regras de status
└── gerenciador-chamados-web/      Interface Angular
```

A solução `.slnx` inclui a API e o console. O projeto de testes e a aplicação Angular são executados separadamente.

## Tecnologias

- **C# / .NET 10** — API e aplicação de console.
- **ASP.NET Core Minimal APIs** — endpoints HTTP.
- **Entity Framework Core 10 + SQLite** — acesso e armazenamento local dos chamados; o esquema inicial está registrado em uma migration.
- **xUnit** — testes unitários do domínio da API.
- **Angular 22, TypeScript 6 e RxJS 7** — interface web; os testes usam Vitest.
- **Node.js 24 e npm 12** — versões usadas na verificação deste repositório. Use versões compatíveis com as dependências listadas no `package.json`.

## Funcionalidades implementadas

- Criar chamados com título não vazio.
- Consultar todos os chamados e consultar um chamado por identificador.
- Expor a lista de status cadastrados.
- Alterar o status por `PATCH`, com validação de transição: um chamado precisa passar por **Em andamento** antes de ser concluído, e chamados concluídos não podem ser alterados.
- Armazenar chamados e status em SQLite por meio do EF Core.
- Na interface Angular, carregar e criar chamados, exibir totais, buscar por título, filtrar por status, solicitar alteração de status e tentar sincronizar novamente após conflito de atualização.
- No console, criar, listar e alterar status de chamados mantidos em memória durante a execução.

## Requisitos

- .NET SDK 10.
- Node.js e npm compatíveis com as versões do `package.json`.
- Para preparar um banco novo, a ferramenta de linha de comando `dotnet-ef` compatível com o EF Core 10.

## Como executar

### API

No primeiro uso, restaure as dependências e aplique a migration inicial. O banco é criado no diretório de trabalho em que a API for iniciada; execute os comandos abaixo no diretório da API para manter o arquivo local junto a ela:

```powershell
cd GerenciadorChamados.Api
dotnet restore
dotnet ef database update
dotnet run --launch-profile http
```

A API inicia em `http://localhost:5051`, origem configurada para a interface Angular. Rotas disponíveis:

| Método | Rota | Uso |
| --- | --- | --- |
| `GET` | `/chamados` | Lista chamados |
| `GET` | `/chamados/{id}` | Consulta um chamado |
| `POST` | `/chamados` | Cria um chamado |
| `PATCH` | `/chamados/{id}/status` | Altera o status |
| `GET` | `/status` | Lista status |

O `dotnet-ef` é uma ferramenta necessária para aplicar migrations, mas não está fixada em um manifesto de ferramentas do repositório. Se ainda não estiver disponível, instale uma versão compatível com EF Core 10 seguindo a documentação oficial do .NET antes de executar `dotnet ef database update`.

### Interface Angular

Em outro terminal, na raiz do repositório:

```powershell
cd gerenciador-chamados-web
npm ci
npm start
```

Abra `http://localhost:4200`. A interface aponta para a API em `http://localhost:5051`; mantenha a API em execução.

### Aplicação de console

Na raiz do repositório:

```powershell
dotnet run --project GerenciadorChamados.Console/gerenciador-chamado.csproj
```

Os dados do console são mantidos apenas em memória e não são compartilhados com a API.

## Build e testes

Na raiz do repositório:

```powershell
dotnet build GerenciadorChamados.slnx
dotnet test GerenciadorChamados.Tests/GerenciadorChamados.Tests.csproj
```

Para compilar a interface e executar seus testes:

```powershell
cd gerenciador-chamados-web
npm run build
npm test -- --watch=false
```

**Verificação realizada em 10/11/2026:** `dotnet build GerenciadorChamados.slnx --no-restore -m:1 /p:UseSharedCompilation=false` concluiu sem avisos ou erros; `npm run build` concluiu. Os cinco testes xUnit foram executados individualmente com limite externo de 12 segundos; todos ficaram inconclusivos depois que o runner iniciou `testhost.exe` e aguardou, sem conexão, o canal local de comunicação. Nenhum método de teste foi relatado como iniciado. No Angular, `npm test -- --watch=false` executado com todos os arquivos ainda falha ao carregar dois deles por `ENOENT` em caminhos temporários do ambiente (3 arquivos passaram, totalizando 3 testes executados). Executados individualmente, os cinco specs passaram: 6 testes no total. O spec do cartão agora fornece os inputs requeridos; o spec da tela mocka a chamada HTTP e verifica texto presente na interface. Revalide a execução completa em um ambiente limpo.

## Pontos a evoluir

Estes itens descrevem o código atual e podem servir de pauta para uma próxima etapa de estudo; não foram alterados nesta preparação:

- Consolidar ou explicar a separação entre as duas implementações: o console possui seu próprio modelo e regras em memória, enquanto a API contém a versão persistida e testada.
- Incluir o projeto de testes na solução, se a intenção for executar build e testes por um único ponto de entrada.
- Corrigir os testes Angular: configurar os inputs obrigatórios nos testes dos componentes e investigar os erros de arquivos temporários nas suítes que não chegaram a executar.
- Investigar por que a execução xUnit não avança após a descoberta dos testes neste ambiente.
- Revisar o componente `ChamadoFiltros`, que permanece como scaffold sem implementação e não é usado pela tela atual (os filtros da tela estão implementados diretamente no componente principal).
- Revisar a resposta da rota `/status`: o contrato atual chama o campo de texto de `Titulo`, embora represente o nome do status.
- Tornar a URL da API e a localização do banco configuráveis. Atualmente a URL do frontend e a origem CORS estão fixadas para execução local, e a conexão SQLite usa caminho relativo ao diretório de trabalho.
- Remover o atraso artificial de três segundos na criação do chamado antes de qualquer demonstração ao vivo; ele está no handler `POST /chamados`.
- Adicionar testes de integração HTTP para os endpoints e persistência, além dos testes unitários atuais das regras de status.

## Preparação para publicação

O `.gitignore` na raiz exclui saídas de build, dependências locais, configurações de usuário do IDE, arquivos de ambiente e bancos SQLite locais. As migrations e o `package-lock.json` permanecem no repositório. Na inspeção, o Git reconheceu a raiz, mas sinalizou diferença de proprietário; os comandos de leitura usaram uma exceção `safe.directory` somente por execução, sem alterar a configuração global. O estado reportou a solução, os quatro diretórios de projeto e os documentos como não rastreados. Nenhum banco SQLite, pasta de dependências ou saída gerada apareceu no índice; os artefatos existentes são cobertos pelo `.gitignore`. Antes de publicar, revise `git status` e o diff, confirme a ausência de segredos e valide os testes em ambiente limpo.
