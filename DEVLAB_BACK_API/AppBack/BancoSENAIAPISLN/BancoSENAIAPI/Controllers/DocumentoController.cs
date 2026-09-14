using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Route("api/v1/documento/listar/[codigoCliente]")]
    public class Documento : Controller
    {
       private readonly string _caminhoRaiz = Path.Combine(Directory.GetCurrentDirectory(), "ClienteArquivos");
       private static List<Models.DocumentoMetadados> _documentoMetadados = new List<Models.DocumentoMetadados>();
        private static int _nextId = 1;

        [HttpPost("upload/{codigoCliente}")]
        public async Task<IActionResult> AnexarArquivo(int codigoCliente, IFormFile arquivo) 
        {
         if (arquivo ==  null || arquivo.Length == 0)
         {
                return BadRequest("Nenhum arquivo foi enviado");
         }

            string pastaCliente = Path.Combine(_caminhoRaiz, codigoCliente.ToString());
           if (!Directory.Exists(pastaCliente))
           {
                Directory.CreateDirectory(pastaCliente);
           }
            string extensao = Path.GetExtension(arquivo.FileName);
            string nomeOriginal = Path.GetFileNameWithoutExtension(arquivo.FileName);
            string novoNome = $"{codigoCliente}_{nomeOriginal}_{Guid.NewGuid()}{extensao}";
            string caminhoFinal = Path.Combine(pastaCliente, novoNome);

            using (var stream = new FileStream(caminhoFinal, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }
            var documentoMetadados = new Models.DocumentoMetadados { 
            id = _nextId ++,
            name = nomeOriginal,
            Extensao = extensao,
            Caminho = caminhoFinal,
            CodigoCliente = codigoCliente,
            };
            _documentoMetadados.Add(documentoMetadados);
            return Ok(new { mensagem = "Documento anexado com sucesso", arquivoSalvo = novoNome });

        }
    }    
}
