﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pokemonTemplate.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace pokemonTemplate.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    
    public class PokemonTemplateController : ControllerBase
    {
        private readonly DataContext _context;

        public PokemonTemplateController(DataContext context)
        {
            _context = context;
        }

        // GET: api/<UserController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Entities.PokemonTemplate>>> GetPokemonTemplates()
        {
            return Ok(await _context.PokemonTemplates.ToListAsync());
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Entities.PokemonTemplate>> GetPokemonTemplate(int id)
        {
             return Ok(await _context.PokemonTemplates.FirstOrDefaultAsync(user => user.Id == id));
        }

        [HttpGet("random")]
        public async Task<ActionResult<Entities.PokemonTemplate>> Random()
        {
            int totalCount = await _context.PokemonTemplates.CountAsync();
            int randomIndex = new Random().Next(0, totalCount);
            
            return Ok(await _context.PokemonTemplates.FirstOrDefaultAsync(user => user.Id == randomIndex));
        }

        // POST api/<UserController>
        [HttpPost]
        public async Task<ActionResult<Entities.PokemonTemplate>> PostPokemonTemplate(Entities.PokemonTemplate pokemon)
        {
            // On ajoute notre utilisateur dans la base
            _context.PokemonTemplates.Add(pokemon);
            // On sauvegarde la modification
            await _context.SaveChangesAsync();
            // On retourne l'utilisateur nouvellement crée en appelant la fonction CreatedAtAction
            return CreatedAtAction(nameof(GetPokemonTemplate), new { id = pokemon.Id }, pokemon);
        }
    }
}
