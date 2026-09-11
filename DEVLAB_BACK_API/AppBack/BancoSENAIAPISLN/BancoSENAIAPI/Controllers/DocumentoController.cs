using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> AnexarArquivo(int codigoCliente, IFormFile arquiv) 
        {
        
        }
    }
}
