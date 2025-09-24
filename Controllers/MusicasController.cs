using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using AppSpotify.Context;
using AppSpotify.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace AppSpotify.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class MusicasController : ControllerBase
    {
        private readonly SpotifyContext _context;
        private readonly IWebHostEnvironment _env;

        public MusicasController(SpotifyContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public IActionResult Get() => Ok(_context.Musicas);

        [HttpGet("{id}")]
        public IActionResult BuscarPorId(int id)
        {
            var musica = _context.Musicas.Find(id);
            if (musica == null)
                return NotFound("Música não encontrada");
            
            var musicaDTO = new MusicaOutputDTO
            {
                Id = musica.Id,
                Titulo = musica.Titulo,
                Artista = musica.Artista,
                Link = musica.Link,
                Imagem = musica.Imagem
            };

            return Ok(musicaDTO);
        }

        [HttpGet("BuscarTitulo")]
        public IActionResult BuscarPorTitulo([FromQuery] string titulo)
        {
            var musica = _context.Musicas.Where(m => m.Titulo.Contains(titulo)).ToList();

            if (!musica.Any())
                return NotFound("Música não encontrada");

            return Ok(musica);
        }

        [HttpDelete("{id}")]
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

        [HttpPost("upload")]
        public async Task<IActionResult> UploadMusicaAsync([FromForm] MusicaInputDTO musicaDTO)
        {
            if (musicaDTO == null)
            {
                return BadRequest("Dados da música inválidos.");
            }
            
            if (string.IsNullOrWhiteSpace(musicaDTO.Titulo) || string.IsNullOrWhiteSpace(musicaDTO.Artista))
            {
                return BadRequest("Título e artista são obrigatórios.");
            }

            if (musicaDTO.MusicaFile == null || musicaDTO.MusicaFile.Length == 0)
            {
                return BadRequest("Nenhum arquivo de música foi enviado.");
            }

            if (musicaDTO.Imagem == null || musicaDTO.Imagem.Length == 0)
            {
                return BadRequest("Nenhum arquivo de imagem foi enviado.");
            }

            try
            {
                var musica = new Musica
                {
                    Titulo = musicaDTO.Titulo,
                    Artista = musicaDTO.Artista
                };

                // Lógica para salvar a imagem
                var imagemNome = $"{Guid.NewGuid()}{Path.GetExtension(musicaDTO.Imagem.FileName)}";
                var imagemPath = Path.Combine(_env.WebRootPath, "images", imagemNome);
                Directory.CreateDirectory(Path.Combine(_env.WebRootPath, "images"));
                using (var stream = new FileStream(imagemPath, FileMode.Create))
                {
                    await musicaDTO.Imagem.CopyToAsync(stream);
                }
                musica.Imagem = $"/images/{imagemNome}";

                // Lógica para salvar o arquivo de música
                var musicaNome = $"{Guid.NewGuid()}{Path.GetExtension(musicaDTO.MusicaFile.FileName)}";
                var musicaPath = Path.Combine(_env.WebRootPath, "musicas", musicaNome);
                Directory.CreateDirectory(Path.Combine(_env.WebRootPath, "musicas"));
                using (var stream = new FileStream(musicaPath, FileMode.Create))
                {
                    await musicaDTO.MusicaFile.CopyToAsync(stream);
                }
                musica.Link = $"/musicas/{musicaNome}";

                _context.Musicas.Add(musica);
                await _context.SaveChangesAsync();
                
                var musicaOutputDTO = new MusicaOutputDTO
                {
                    Id = musica.Id,
                    Titulo = musica.Titulo,
                    Artista = musica.Artista,
                    Link = musica.Link,
                    Imagem = musica.Imagem
                };

                return CreatedAtAction(nameof(BuscarPorId), new { id = musica.Id }, musicaOutputDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao processar a música: {ex.Message}");
            }
        }
    }
}