using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace AppSpotify.Models
{
    [ApiController]
    [Route("[Controller]")]
    public class Musica
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public float Duracao { get; set; }
        public string Artista { get; set; }
        public string Imagem { get; set; }
        public string Link { get; set; }
    }
}