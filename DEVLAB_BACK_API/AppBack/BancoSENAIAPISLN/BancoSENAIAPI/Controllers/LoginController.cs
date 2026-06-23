using BancoSENAIAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BancoSENAIAPI.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class LoginController : ControllerBase
    {
        private static List<Usuario> _usuarios = new List<Usuario>
        {
            new Usuario { Login = "reginaldo", Senha = "123", Perfil = "Admin" },
            new Usuario { Login = "gerente", Senha = "456", Perfil = "Gerente" },
            new Usuario { Login = "aluno", Senha = "789", Perfil = "User" }
        };

        [HttpPost]
        public IActionResult Autenticar([FromBody] Usuario dadosLogin)
        {

            var usuario = _usuarios.FirstOrDefault(u =>
                u.Login == dadosLogin.Login &&
                u.Senha == dadosLogin.Senha);


            if (usuario == null)
                return Unauthorized(new { message = "Login ou senha inválidos." });


            var tokenHandler = new JwtSecurityTokenHandler();
            var chaveBytes = Encoding.ASCII.GetBytes("ChaveSecretaSuperSecretaDoSenai2026!");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, usuario.Login),
                    new Claim(ClaimTypes.Role, usuario.Perfil)
                }),
                Expires = DateTime.UtcNow.AddHours(2), // Expira em 2 horas
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(chaveBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenCriado = tokenHandler.CreateToken(tokenDescriptor);
            var tokenGerado = tokenHandler.WriteToken(tokenCriado);
            // -----------------------------

            return Ok(new
            {
                user = usuario.Login,
                perfil = usuario.Perfil,
                token = tokenGerado
            });
        }

    }
}
