List<Chamado> listaChamados = [];

while (true)
{
    Console.Clear();
    Console.WriteLine("=== GERENCIADOR DE CHAMADOS ===");
    int userInput = MenuInputValidation();

    switch (userInput)
    {
        case 1:
            OpenTicket(listaChamados);
            break;
        case 2:
            ListTickets(listaChamados);
            break;
        case 3:
            UpdateTicket(listaChamados);
            break;
        case 0:
            Console.WriteLine("\nFim do programa!");
            return;
        default:
            Console.WriteLine("\tFunção inválida");
            break;
    }
}

static void OpenTicket(List<Chamado> listaChamados)
{
    Console.Clear();
    Console.WriteLine("=== ABRIR CHAMADO ===\n");

    string titulo = TitleValidation();
    Chamado novoChamado = new Chamado(titulo);
    listaChamados.Add(novoChamado);
    Console.Clear();
    Console.WriteLine("\tChamado adicionado com sucesso!");
    AwaitKeyPress();
}

static void ListTickets(List<Chamado> listaChamados)
{
    Console.Clear();
    Console.WriteLine("=== LISTAR CHAMADOS ===\n");

    if (listaChamados.Count == 0)
    {
        Console.WriteLine("\nNão existe nenhum chamado cadastrado!");
        AwaitKeyPress();
        return;
    }
    foreach (var c in listaChamados)
    {
        c.MostrarInfo();
    }
    AwaitKeyPress();
}

static void UpdateTicket(List<Chamado> listaChamados)
{
    Console.Clear();
    Console.WriteLine("=== ATUALIZAR CHAMADOS ===\n");

    int id = InputNumberValidation("ID: ", "ID deve ser numérico!");

    Chamado? chamado = listaChamados.Find(c => c.Id == id);

    if (chamado == null)
    {
        Console.Clear();
        Console.WriteLine("\n\tId do chamado não encontrado!");
        AwaitKeyPress();
    }
    else
    {
        chamado.MostrarInfo();
        Console.WriteLine("\n\tChamado encontrado!");

        Console.WriteLine(
            "\nAltere o estado do chamado:\n1 - Aberto\n2 - Concluido\n3 - EmAndamento\n0 - Sair sem editar\n"
        );
        int option = InputNumberValidation("Opção: ", "Opção deve ser numérica!");
        if (option == 0)
        {
            return;
        }
        Console.Clear();
        switch (UpdateStatus(option, chamado))
        {
            case ResultadoAlteracaoStatus.Sucesso:
                Console.WriteLine("\n\tEstado do chamado alterado com sucesso!");
                break;
            case ResultadoAlteracaoStatus.ChamadoJaConcluido:
                Console.WriteLine("\n\tNão é possivel alterar um status de um chamado concluído!");
                break;
            case ResultadoAlteracaoStatus.StatusInvalido:
                Console.WriteLine("\n\tEstado inválido!");
                break;
            case ResultadoAlteracaoStatus.PrecisaPassarPorEmAndamento:
                Console.WriteLine(
                    "\n\tUm chamado só pode ser concluído se antes estiver em andamento!"
                );
                break;
            default:
                Console.WriteLine("\n\tOcorreu um erro e o estado não pode ser atualizado!");
                break;
        }
        AwaitKeyPress();
        return;
    }
}

static string TitleValidation()
{
    bool isValid;
    string title;

    do
    {
        Console.Write("Título do chamado: ");
        title = Console.ReadLine() ?? "";
        isValid = !string.IsNullOrWhiteSpace(title);

        if (isValid)
            return title.Trim();
        else
            Console.WriteLine("\tO título não pode ser vazio");
    } while (true);
}

static int InputNumberValidation(string promptMessage, string errorMessage)
{
    bool isValid;
    int result;
    do
    {
        Console.Write(promptMessage);
        string input = Console.ReadLine() ?? "";
        isValid = int.TryParse(input, out result);
        if (!isValid)
            Console.WriteLine(errorMessage);
    } while (!isValid);

    return result;
}

static int MenuInputValidation()
{
    do
    {
        Console.WriteLine(
            "\n1 - Abrir chamado\n2 - Listar chamados\n3 - Atualizar status\n0 - Sair\n"
        );
        Console.Write("Escolha uma opção: ");

        switch (Console.ReadLine())
        {
            case "1":
                return 1;
            case "2":
                return 2;
            case "3":
                return 3;
            case "0":
                return 0;
            default:
                Console.WriteLine("\tFunção inválida");
                break;
        }
    } while (true);
}

static ResultadoAlteracaoStatus UpdateStatus(int option, Chamado chamado)
{
    StatusEnum status;

    switch (option)
    {
        case 1:
            status = StatusEnum.Aberto;
            break;
        case 2:
            status = StatusEnum.Concluido;
            break;
        case 3:
            status = StatusEnum.EmAndamento;
            break;
        default:
            return ResultadoAlteracaoStatus.StatusInvalido;
    }

    return chamado.EditarStatus(status);
}

static void AwaitKeyPress()
{
    Console.WriteLine("\n\tAperte uma tecla para voltar...");
    Console.ReadKey();
}

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

    public void MostrarInfo()
    {
        Console.WriteLine($"Id: {Id}\t||\tTítulo: {Titulo}\t\t||\tStatus: {Status}");
    }

    public ResultadoAlteracaoStatus EditarStatus(StatusEnum status)
    {
        if (Status == StatusEnum.Concluido)
            return ResultadoAlteracaoStatus.ChamadoJaConcluido;
        else if (status == StatusEnum.Concluido && Status != StatusEnum.EmAndamento)
        {
            return ResultadoAlteracaoStatus.PrecisaPassarPorEmAndamento;
        }
        else
        {
            Status = status;
            return ResultadoAlteracaoStatus.Sucesso;
        }
    }
}

public enum StatusEnum
{
    Aberto,
    EmAndamento,
    Concluido,
}

public enum ResultadoAlteracaoStatus
{
    Sucesso,
    ChamadoJaConcluido,
    StatusInvalido,
    PrecisaPassarPorEmAndamento,
}
