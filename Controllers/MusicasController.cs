using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using AppSpotify.Context;
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
        

    }
}