using System.ComponentModel.DataAnnotations;

namespace BancoSENAIAPI.Models

{
    public class DocumentoMetadados
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string name { get; set; }
        [Required]
        public string Extensao { get; set; }
        [Required]
        public string Caminho { get; set; }
        [Required]
        public int CodigoCliente { get; set; }

    }
}
