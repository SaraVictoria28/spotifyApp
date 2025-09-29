using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using AppSpotify.Context;
using AppSpotify.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using spotifyApp.Models;
using System.IO;

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

        [HttpDelete("DeletarPorTitulo")]
        public async Task<IActionResult> DeletarPorTitulo([FromQuery] string titulo)
        {
            if (string.IsNullOrWhiteSpace(titulo))
            {
                return BadRequest("O título da música é obrigatório.");
            }

            var musicasParaDeletar = await _context.Musicas
                .Where(m => m.Titulo == titulo)
                .ToListAsync();

            if (musicasParaDeletar.Count == 0)
            {
                return NotFound("Nenhuma música encontrada com esse título.");
            }

            if (musicasParaDeletar.Count > 1)
            {
                return BadRequest("Mais de uma música foi encontrada com esse título. Por favor, seja mais específico (por exemplo, com o nome do artista).");
            }

            var musica = musicasParaDeletar.First();

            _context.Musicas.Remove(musica);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("cadastrar")]
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

                var imagemNome = $"{Guid.NewGuid()}{Path.GetExtension(musicaDTO.Imagem.FileName)}";
                var imagemPath = Path.Combine(_env.WebRootPath, "images", imagemNome);
                Directory.CreateDirectory(Path.Combine(_env.WebRootPath, "images"));
                using (var stream = new FileStream(imagemPath, FileMode.Create))
                {
                    await musicaDTO.Imagem.CopyToAsync(stream);
                }
                musica.Imagem = $"/images/{imagemNome}";

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

                var novaMusica = new MusicaOutputDTO
                {
                    Id = musica.Id,
                    Titulo = musica.Titulo,
                    Artista = musica.Artista,
                    Link = musica.Link,
                    Imagem = musica.Imagem
                };

                return CreatedAtAction(nameof(BuscarPorId), new { id = musica.Id }, novaMusica);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Erro interno ao processar a música: {ex.Message}");
            }
        }

        [HttpPut]
        [Route("titulo")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AtualizarMusica(
       [FromQuery] string tituloAntigo,
       [FromForm] MusicaUpdateDTO musicaDTO
       )
        {
            // A validação 'ModelState.IsValid' é ignorada implicitamente aqui para campos opcionais.

            if (string.IsNullOrWhiteSpace(tituloAntigo))
            {
                return BadRequest("O título da música a ser editada (tituloAntigo) é obrigatório.");
            }

            // 1. Busca a música existente
            var tituloBuscaPadronizado = tituloAntigo.ToLower();
            var musicaExistente = await _context.Musicas
                .FirstOrDefaultAsync(m => m.Titulo.ToLower() == tituloBuscaPadronizado);

            if (musicaExistente == null)
            {
                return NotFound($"Música com o título '{tituloAntigo}' não encontrada.");
            }

            // 2. Atualiza campos de texto (se fornecidos)
            // Se musicaDTO.Titulo for null ou whitespace (não enviado/deixado vazio), o valor antigo é mantido.
            if (!string.IsNullOrWhiteSpace(musicaDTO.Titulo))
            {
                musicaExistente.Titulo = musicaDTO.Titulo;
            }

            if (!string.IsNullOrWhiteSpace(musicaDTO.Artista))
            {
                musicaExistente.Artista = musicaDTO.Artista;
            }

            // 3. Atualiza o arquivo MP3 (se for fornecido)
            if (musicaDTO.MusicaFile != null && musicaDTO.MusicaFile.Length > 0)
            {
                var extensaoArquivo = Path.GetExtension(musicaDTO.MusicaFile.FileName).ToLowerInvariant();

                if (extensaoArquivo != ".mp3")
                    return BadRequest("Formato de música não suportado. Use MP3.");

                try
                {
                    // Deleta o antigo
                    if (!string.IsNullOrEmpty(musicaExistente.Link))
                    {
                        var caminhoAntigo = Path.Combine(_env.WebRootPath, musicaExistente.Link.TrimStart('/'));
                        if (System.IO.File.Exists(caminhoAntigo))
                        {
                            System.IO.File.Delete(caminhoAntigo);
                        }
                    }

                    // Salva o novo
                    var pastaMusicas = Path.Combine(_env.WebRootPath, "musicas");
                    Directory.CreateDirectory(pastaMusicas);
                    var novoNomeArquivo = $"{Guid.NewGuid()}{extensaoArquivo}";
                    var caminhoNovo = Path.Combine(pastaMusicas, novoNomeArquivo);

                    using (var stream = new FileStream(caminhoNovo, FileMode.Create))
                    {
                        await musicaDTO.MusicaFile.CopyToAsync(stream);
                    }

                    musicaExistente.Link = $"/musicas/{novoNomeArquivo}";
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Erro ao substituir o arquivo de música: {ex.Message}");
                }
            }

            // 4. Atualiza a Imagem (se for fornecida)
            if (musicaDTO.Imagem != null && musicaDTO.Imagem.Length > 0)
            {
                var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png" };
                var extensaoArquivo = Path.GetExtension(musicaDTO.Imagem.FileName).ToLowerInvariant();

                if (!extensoesPermitidas.Contains(extensaoArquivo))
                    return BadRequest("Formato de imagem não suportado. Use JPG, JPEG ou PNG.");

                try
                {
                    // Deleta a antiga
                    if (!string.IsNullOrEmpty(musicaExistente.Imagem))
                    {
                        var caminhoAntigo = Path.Combine(_env.WebRootPath, musicaExistente.Imagem.TrimStart('/'));
                        if (System.IO.File.Exists(caminhoAntigo))
                        {
                            System.IO.File.Delete(caminhoAntigo);
                        }
                    }

                    // Salva a nova
                    var pastaImagens = Path.Combine(_env.WebRootPath, "images");
                    Directory.CreateDirectory(pastaImagens);
                    var novoNomeArquivo = $"{Guid.NewGuid()}{extensaoArquivo}";
                    var caminhoNovo = Path.Combine(pastaImagens, novoNomeArquivo);

                    using (var stream = new FileStream(caminhoNovo, FileMode.Create))
                    {
                        await musicaDTO.Imagem.CopyToAsync(stream);
                    }

                    musicaExistente.Imagem = $"/images/{novoNomeArquivo}";
                }
                catch (Exception ex)
                {
                    return StatusCode(500, $"Erro ao substituir o arquivo de imagem: {ex.Message}");
                }
            }

            // 5. Salva as alterações no banco de dados
            _context.Musicas.Update(musicaExistente);
            await _context.SaveChangesAsync();

            // 6. Retorna o resultado
            var musicaOutput = new MusicaOutputDTO
            {
                Id = musicaExistente.Id,
                Titulo = musicaExistente.Titulo,
                Artista = musicaExistente.Artista,
                Link = musicaExistente.Link,
                Imagem = musicaExistente.Imagem
            };

            return Ok(musicaOutput);
        }
    }
}