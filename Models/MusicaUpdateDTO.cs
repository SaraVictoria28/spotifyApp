using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; 

namespace spotifyApp.Models
{
    public class MusicaUpdateDTO
    {
        public string? Titulo { get; set; }
        public string? Artista { get; set; }
        public IFormFile? MusicaFile { get; set; }
        public IFormFile? Imagem { get; set; }

    }
}