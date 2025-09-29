using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppSpotify.Models
{
    public class MusicaOutputDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Artista { get; set; }
        public string Link { get; set; }
        public string Imagem { get; set; }
    }
}