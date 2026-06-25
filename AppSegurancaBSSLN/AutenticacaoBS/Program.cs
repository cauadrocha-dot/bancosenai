using System;
using System.IO;
        
string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
string filePath = Path.Combine(desktopPath, "Token", "TokenClientes.txt");

string[] logCpf = new string[3];
string[] logDataHora = new string[3];
int logContador = 0;

bool rodando = true;

while (rodando)
{
    if (logContador >= 3)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("\n[FALHA] Usuário bloqueado!");
        Console.ResetColor();
        rodando = false;

    }
    Console.Clear();
    Console.WriteLine("--- SISTEMA BANCÁRIO (AUTENTICAÇÃO) ---");
    Console.Write("Digite a agência: ");
    string agencia = Console.ReadLine();
    Console.Write("Digite a conta: ");
    string conta = Console.ReadLine();
    Console.Write("Digite o CPF (apenas números): ");
    string cpf = Console.ReadLine();

    if (logContador < 3)
    {
        logCpf[logContador] = cpf;
        logDataHora[logContador] = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
        logContador++;
    } 

    string conteudoArquivo = File.ReadAllText(filePath);

    if (!conteudoArquivo.Contains(cpf))
    {
        Console.WriteLine("\n[RESULTADO] Credenciais inválidas!");
        continue;
    }
    else if (conteudoArquivo.Contains(cpf))
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n[SUCESSO] Usuário autenticado com sucesso!");
        Console.ResetColor();
        rodando = false;
    }
    Console.WriteLine("\nPressione qualquer tecla para prosseguir...");
    Console.ReadKey();
}
Console.WriteLine("");
Console.WriteLine("Tentativa | CPF         | Data Hora");

for (int i = 0; i < logContador; i++)
{
    string numeroTentativa = (i + 1).ToString("D2");
    Console.WriteLine($"{numeroTentativa}        | {logCpf[i]} | {logDataHora[i]}");
}

Console.WriteLine("\nAplicação encerrada. Pressione qualquer tecla para sair.");
Console.ReadKey();
