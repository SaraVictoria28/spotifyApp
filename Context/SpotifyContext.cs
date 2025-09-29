using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AppSpotify.Models;
using Microsoft.EntityFrameworkCore;

namespace AppSpotify.Context
{
    public class SpotifyContext : DbContext
    {
        public SpotifyContext(DbContextOptions<SpotifyContext> options) : base(options) { }

        public DbSet<Musica> Musicas { get; set; }
        // public DbSet<Usuario> Usuarios { get; set; }
    }
}