Console.WriteLine("GERADOR DE TOKEN - Versão 1.0.0 | 23/06/2026");

string pathDesktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
string pastaDesktop = Path.Combine(pathDesktop, "Token");

if (!Directory.Exists(pastaDesktop)) Directory.CreateDirectory(pastaDesktop);

string arquivo = Path.Combine(pastaDesktop, "TokenClientes.txt");


Console.Write("Em quantos minutos você deseja que o token expire (digite um numero - representa minutos): ");
int minutos = Convert.ToInt32(Console.ReadLine());

string dataHoraExpiracao = DateTime.Now.AddMinutes(minutos).ToString();

Console.WriteLine($"Tokens Gerados expiram em: {dataHoraExpiracao}");

List<string> dados = new List<string> {
 $"01702765562;bs_01702765562_{dataHoraExpiracao};{dataHoraExpiracao}",
 $"02002002033;bs_02002002033_{dataHoraExpiracao};{dataHoraExpiracao}",
 $"44444444444;bs_44444444444_{dataHoraExpiracao};{dataHoraExpiracao}",

};

try
{
	File.WriteAllLines(arquivo, dados);

	if (File.Exists(arquivo))
	{ Console.WriteLine("Arquivo Gerado com Sucesso na Pasta que está o executável");}
	else { Console.WriteLine("Atquivo não gerado, ou não encontrado"); }

}
catch (Exception ex)
{

    Console.WriteLine($"Falha na hora de gerar o arquivo. Erro:  {ex.Message}");
}

Console.ReadKey();