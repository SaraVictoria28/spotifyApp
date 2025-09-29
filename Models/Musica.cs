using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppSpotify.Models
{
    public class Musica
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Artista { get; set; }
        public string Imagem { get; set; }
        public string Link { get; set; }
    }
}