using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using AppSpotify.Context;
using AppSpotify.Models;
using Microsoft.AspNetCore.Mvc;

namespace AppSpotify.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    //cpf e data de nascimento 
    public class MusicasController : ControllerBase
    {
        private readonly SpotifyContext _context;

        public MusicasController(SpotifyContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get() => Ok(_context.Musicas);

        [HttpGet("{id}")]
        public IActionResult BuscarPorId(int id)
        {
            var musica = _context.Musicas.Find(id);
            if (musica == null)
                return NotFound($"Música não encontrada");

            return Ok(_context.Musicas);
        }

        [HttpPost]
        public async Task<IActionResult> CadastrarMusica([FromForm] Musica musica)
        {
            _context.Add(musica);
            _context.SaveChanges();

            return CreatedAtAction(
            nameof(BuscarPorId),
            new { id = musica.Id },
            musica
            );
        }

        [HttpGet("BuscarTitulo")]
        public IActionResult BuscarPorTitulo([FromQuery] string titulo)
        {
            var musica = _context.Musicas.
            Where(m => m.Titulo.Contains(titulo)).
            ToList();

            if (musica == null)
                return NotFound($"Música não encontrada");

            return Ok(_context.Musicas);
        }

        [HttpDelete]
        public IActionResult Deletar(int id)
        {
            var bancoMusica = _context.Musicas.Find(id);
            if (bancoMusica == null)
            {
                return NotFound("Música não encontrada");
            }
            _context.Musicas.Remove(bancoMusica);
            _context.SaveChanges();

            return NoContent();
        }

        


    }
}