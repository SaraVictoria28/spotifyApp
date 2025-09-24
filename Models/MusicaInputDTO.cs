using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppSpotify.Models
{
    public class MusicaInputDTO
    {
        public string Titulo { get; set; }
        public string Artista { get; set; }
        public IFormFile MusicaFile { get; set; }
        public IFormFile Imagem { get; set; }
    }
}