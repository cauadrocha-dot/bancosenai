using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class Documento : Controller
    {
        private readonly string _caminhoRaiz = Path.Combine(Directory.GetCurrentDirectory(), "ClienteArquivos");
        private static List<Models.DocumentoMetadados> _documentoMetadados = new List<Models.DocumentoMetadados>();
        private static int _nextId = 1;

        [HttpPost("upload/{codigoCliente}")]
        public async Task<IActionResult> AnexarArquivo(int codigoCliente, IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
            {
                return BadRequest("Nenhum arquivo foi enviado");
            }
            if (arquivo.Length > 2 * 1024 * 1024) 
            {
                return BadRequest("O arquivo não pode ter mais de 2 MB.");
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
                id = _nextId++,
                name = nomeOriginal,
                Extensao = extensao,
                Caminho = caminhoFinal,
                CodigoCliente = codigoCliente,
            };
            _documentoMetadados.Add(documentoMetadados);
            return Ok(new { mensagem = "Documento anexado com sucesso", arquivoSalvo = novoNome });

        }
        [HttpGet("listar/{codigoCliente}")]
        public IActionResult listarDocumentos(int codigoCliente)
        {
            var documentos = _documentoMetadados
            .Where (d => d.CodigoCliente == codigoCliente)
            .ToList();

            if (!documentos.Any())
            {
                return NotFound("Nenhum Documento foi encontrado para este cliente");
            }
            return Ok(documentos);
        }
        [HttpGet("download/id")]
        public IActionResult DownloadDocumentos(int id)
        {
            var documentos = _documentoMetadados
            .FirstOrDefault(d => d.id == id);

            if (documentos == null)
            {
                return NotFound("Documento não encontrado");
            }

            byte[] arquivo = System.IO.File.ReadAllBytes(documentos.Caminho);
            return File(arquivo, "application/octet-stream", documentos.name + documentos.Extensao);

        }
        [HttpDelete("excluir/{id}")]
        public IActionResult ExcluirDocumentos(int id)
        {
            var documento = _documentoMetadados
            .FirstOrDefault (d => d.id == id);

            if ( documento == null)
            {
                return NotFound("Arquivo não encontrado");
            }
            System.IO.File.Delete(documento.Caminho);
            _documentoMetadados.Remove(documento);

            return Ok("Documento Excluído com sucesso.");
        }

    }
}    

