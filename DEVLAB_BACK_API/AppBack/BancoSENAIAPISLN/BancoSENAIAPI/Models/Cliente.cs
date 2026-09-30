using System.ComponentModel.DataAnnotations;

namespace BancoSENAIAPI.Models
{
    public class Cliente
    {
        [Key]
        public int CodigoCliente { get; set; }
        [Required]
        public required string NomeCliente { get; set; }
        [Required]
        public required string CPF { get; set; }
        [Required]
        public int NumeroAgencia { get; set; } = 10;
        [Required]
        public int SaldoTotal { get; set; } = 0;
       
    }
}
