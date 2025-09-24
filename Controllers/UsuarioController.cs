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
            if (await _context.Usuarios.AnyAsync(u => u.Nome == usuario.Nome || u.Email == usuario.Email))
            {
                return BadRequest("Usuário ou e-mail já cadastrado.");
            }

            var newUser = new Usuario
            {
                Nome = usuario.Nome,
                Email = usuario.Email,
                SenhaHast = usuario.SenhaHast,
            };
            _context.Usuarios.Add(newUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Usuário cadastrado com sucesso!" });
        }
    }
}