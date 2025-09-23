using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppSpotify.Context;
using AppSpotify.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AppSpotify.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsuarioController : ControllerBase
    {
         private readonly SpotifyContext _context;

        public UsuarioController(SpotifyContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] Usuario usuario)
        {
            // 1. Verifique se o usuário ou e-mail já existem
            if (await _context.Usuarios.AnyAsync(u => u.Nome == usuario.Nome || u.Email == usuario.Email))
            {
                return BadRequest("Usuário ou e-mail já cadastrado.");
            }

            // 3. Crie o novo objeto de usuário
            var newUser = new Usuario
            {
                Nome = usuario.Nome,
                Email = usuario.Email,
                SenhaHast = usuario.SenhaHast,
            };

            // 4. Salve o novo usuário no banco de dados
            _context.Usuarios.Add(newUser);
            await _context.SaveChangesAsync();

            // 5. Retorne uma resposta de sucesso
            return Ok(new { message = "Usuário cadastrado com sucesso!" });
        }
    }
}