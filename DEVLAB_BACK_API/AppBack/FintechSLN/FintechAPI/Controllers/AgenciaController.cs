using FintechAPI.Infra;
using FintechAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace FintechAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
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
            var dadosLogin = new { Login = "reginaldo", Senha = "123", Perfil="Admin" };

            
            var response = await _httpClient.PostAsJsonAsync(UrlLoginSenai, dadosLogin);

            if (!response.IsSuccessStatusCode) return null;

            var resultado = await response.Content.ReadFromJsonAsync<TokenResponse>();
            return resultado?.Token;
        }

        [HttpGet]
        public async Task<IActionResult> ListarTodas()
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
                var agencias = await response.Content.ReadFromJsonAsync<List<AgenciaDto>>();
                return Ok(agencias);
            }

            return StatusCode((int)response.StatusCode, new { message = "Erro ao buscar agências no Banco SENAI." });
        }

        [HttpPost]
        public async Task<IActionResult> Cadastrar([FromBody] AgenciaDto novaAgencia)
        {
            var token = await ObterTokenAsync();
            if (string.IsNullOrEmpty(token))
                return Unauthorized(new { message = "Falha na autenticação com o provedor interno." });

            var request = new HttpRequestMessage(HttpMethod.Post, UrlAgenciaSenai);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = JsonContent.Create(novaAgencia); // Passa o objeto recebido adiante

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var agenciaCriada = await response.Content.ReadFromJsonAsync<AgenciaDto>();

                // Retorna 201 Created para o cliente da Fintech
                return Created("", agenciaCriada);
            }

            // Se a API A retornar BadRequest (Ex: agência duplicada), repassa o erro
            var erroConteudo = await response.Content.ReadAsStringAsync();
            return StatusCode((int)response.StatusCode, erroConteudo);
        }




    }
}
