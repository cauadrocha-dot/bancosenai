using FintechWEB.Infra;
using FintechWEB.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;

namespace FintechWEB.Controllers
{


    public class AgenciaController : Controller
    {
        private readonly HttpClient _httpClient;
        private const string UrlLoginSenai = "https://localhost:7081/api/v1/Login";
        private const string UrlAgenciaSenai = "https://localhost:7081/api/v1/Agencia";
        public AgenciaController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        private async Task<string?> ObterTokenAsync()
        {
            var dadosLogin = new { Login = "reginaldo", Senha = "123", Perfil = "Admin" };


            var response = await _httpClient.PostAsJsonAsync(UrlLoginSenai, dadosLogin);

            if (!response.IsSuccessStatusCode) return null;

            var resultado = await response.Content.ReadFromJsonAsync<TokenResponse>();
            return resultado?.Token;
        }

        public async Task<IActionResult> Index()
        {
            var token = await ObterTokenAsync();
            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { message = "Falha na autenticação com o provedor interno." });

            // Cria uma nova mensagem de requisição HTTP para isolar os headers por chamada
            var request = new HttpRequestMessage(HttpMethod.Get, UrlAgenciaSenai);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                // Consome da API A e já retorna os dados tipados para quem chamou a Fintech
                var agencias = await response.Content.ReadFromJsonAsync<List<Agencia>>();
                
                return View(agencias);
            }

            return null;
        }

        [HttpGet]
        public async Task<IActionResult> Cadastrar()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar(string NumeroAgencia, string Cidade, string SiglaEstado)
        {
            Agencia novaAgencia = new Agencia()
            {
                NumeroAgencia =Convert.ToInt32(NumeroAgencia),
                Cidade = Cidade,
                SiglaEstado = SiglaEstado

            };
            var token = await ObterTokenAsync();
            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { message = "Falha na autenticação com o provedor interno." });

            var request = new HttpRequestMessage(HttpMethod.Post, UrlAgenciaSenai);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = JsonContent.Create(novaAgencia); // Passa o objeto recebido adiante

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var token = await ObterTokenAsync();
            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { message = "Falha na autenticação com o provedor interno." });

            // Busca a agência específica na API para preencher a View de edição
            var request = new HttpRequestMessage(HttpMethod.Get, $"{UrlAgenciaSenai}/{id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var agencia = await response.Content.ReadFromJsonAsync<Agencia>();
                return View(agencia);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Editar(string NumeroAgencia, string Cidade, string SiglaEstado)
        {
            Agencia agenciaAtualizada = new Agencia()
            {
                NumeroAgencia = Convert.ToInt32(NumeroAgencia),
                Cidade = Cidade,
                SiglaEstado = SiglaEstado
            };

            var token = await ObterTokenAsync();
            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { message = "Falha na autenticação com o provedor interno." });

            // Monta a URL com o ID/Número da agência no final: https://localhost:7081/api/v1/Agencia/20
            var request = new HttpRequestMessage(HttpMethod.Put, $"{UrlAgenciaSenai}/{NumeroAgencia}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = JsonContent.Create(agenciaAtualizada);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            // Se der erro, devolve o objeto para a View não perder os dados digitados
            ModelState.AddModelError(string.Empty, "Erro ao atualizar a agência.");
            return View(agenciaAtualizada);
        }

        [HttpPost]
        public async Task<IActionResult> Excluir(int id)
        {
            var token = await ObterTokenAsync();
            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { message = "Falha na autenticação com o provedor interno." });

            // Monta a URL final apontando para o ID da agência: https://localhost:7081/api/v1/Agencia/30
            var request = new HttpRequestMessage(HttpMethod.Delete, $"{UrlAgenciaSenai}/{id}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                // Se deletou com sucesso na API, recarrega a listagem
                return RedirectToAction("Index");
            }

            // Caso dê algum erro na API, você pode redirecionar para a Index ou tratar o erro
            TempData["ErroExclusao"] = "Não foi possível excluir a agência.";
            return RedirectToAction("Index");
        }


    }
}
