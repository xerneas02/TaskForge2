using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using pokemon.Data;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace pokemon.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    
    public class PokemonController : ControllerBase
    {
        private readonly DataContext _context;
        

        public PokemonController(DataContext context)
        {
            _context = context;
        }

        // GET: api/<UserController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Entities.Pokemon>>> GetPokemons()
        {
            return Ok(await _context.Pokemons.ToListAsync());
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Entities.Pokemon>> GetPokemon(int id)
        {
             return Ok(await _context.Pokemons.FirstOrDefaultAsync(user => user.Id == id));
        }

        [HttpGet("trainer/{trainerId}")]
        public async Task<ActionResult<IEnumerable<Entities.Pokemon>>> GetTrainerPokemons(int trainerId)
        {
             var trainerPokemons = await _context.Pokemons
                                    .Where(pokemon => pokemon.TrainerId == trainerId)
                                    .ToListAsync();

            return Ok(trainerPokemons);
        }

        // POST api/<UserController>
        [HttpPost]
        public async Task<ActionResult<Entities.Pokemon>> PostPokemon(Entities.Pokemon pokemon)
        {
            // On ajoute notre utilisateur dans la base
            _context.Pokemons.Add(pokemon);
            // On sauvegarde la modification
            await _context.SaveChangesAsync();
            // On retourne l'utilisateur nouvellement crée en appelant la fonction CreatedAtAction
            return CreatedAtAction(nameof(GetPokemon), new { id = pokemon.Id }, pokemon);
        }

        [HttpPost("AddRandomPokemon/{trainerId}")]
        public async Task<ActionResult<Entities.Pokemon>> AddRandomPokemon(int trainerId)
        {
            //try
            //{
                // Retrieve a random Pokemon template from the PokemonTemplate microservice
                Entities.Pokemon randomTemplate = GetRandomPokemonTemplate();

                IEnumerable<Entities.Pokemon> allPokemons = await _context.Pokemons.ToListAsync();

                int maxId = allPokemons.Any() ? allPokemons.Max(p => p.Id) : 0;

                int newId = maxId + 1;

                // Create a new Pokemon using the retrieved template and provided trainer ID
                Entities.Pokemon newPokemon = new Entities.Pokemon(
                    newId,
                    trainerId,
                    randomTemplate.Nom,
                    randomTemplate.Type,
                    ShouldPokemonBeShiny()
                );

                // Add the new Pokemon to the Pokemon microservice database
                _context.Pokemons.Add(newPokemon);
                await _context.SaveChangesAsync();

                // Return the newly created Pokemon
                return CreatedAtAction(nameof(GetPokemon), new { id = newPokemon.Id }, newPokemon);
            //}
            //catch (Exception ex)
            //{
                // Handle any exceptions that might occur during the process
            //    return BadRequest($"An error occurred: {ex.Message}");
            //}
        }

        private Entities.Pokemon GetRandomPokemonTemplate()
        {
            //try
            //{
                // Make an HTTP request to the PokemonTemplate microservice endpoint
                using (HttpClient httpClient = new HttpClient())
                {
                    string pokemonTemplateApiUrl = "http://localhost:5227/api/pokemonTemplate/random";
                    HttpResponseMessage response = httpClient.GetAsync(pokemonTemplateApiUrl).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResult = response.Content.ReadAsStringAsync().Result;

                        var templateData = new { Nom = "", Type = Entities.PokemonType.Normal};
                        var randomTemplate = JsonConvert.DeserializeAnonymousType(jsonResult, templateData);

                        // Create a new Pokemon using the retrieved template and provided trainer ID
                        if (randomTemplate == null)
                        {
                            throw new Exception($"Failed to retrieve random Pokemon template. Status code: {response.StatusCode}");
                        }

                        Entities.Pokemon newPokemon = new Entities.Pokemon
                        (
                            0,
                            -1,
                            randomTemplate!.Nom,
                            randomTemplate!.Type,
                            false
                        );
                        return newPokemon;
                    }
                    else
                    {
                        throw new Exception($"Failed to retrieve random Pokemon template. Status code: {response.StatusCode}");
                    }
                }
            //}
            //catch (Exception ex)
            //{
                // Handle any exceptions that might occur during the process
            //    throw new Exception($"An error occurred while getting a random Pokemon template: {ex.Message}");
            //}
        }

        [HttpPost("AddPokemonFromTemplate")]
        public async Task<ActionResult<Entities.Pokemon>> AddPokemonFromTemplate(int trainerId, int templateId)
        {
            try
            {
                // Retrieve the Pokemon template with the specified templateId from the PokemonTemplate microservice
                Entities.Pokemon randomTemplate = GetPokemonTemplateById(templateId);

                IEnumerable<Entities.Pokemon> allPokemons = await _context.Pokemons.ToListAsync();

                int maxId = allPokemons.Any() ? allPokemons.Max(p => p.Id) : 0;

                int newId = maxId + 1;

                // Create a new Pokemon using the retrieved template and provided trainer ID
                Entities.Pokemon newPokemon = new Entities.Pokemon(
                    newId,
                    trainerId,
                    randomTemplate.Nom,
                    randomTemplate.Type,
                    ShouldPokemonBeShiny()
                );

                // Add the new Pokemon to the Pokemon microservice database
                _context.Pokemons.Add(newPokemon);
                await _context.SaveChangesAsync();

                // Return the newly created Pokemon
                return CreatedAtAction(nameof(GetPokemon), new { id = newPokemon.Id }, newPokemon);
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur during the process
                return BadRequest($"An error occurred: {ex.Message}");
            }
        }

        private Entities.Pokemon GetPokemonTemplateById(int templateId)
        {
            try
            {
                // Make an HTTP request to the PokemonTemplate microservice endpoint with the specified templateId
                using (HttpClient httpClient = new HttpClient())
                {
                    string pokemonTemplateApiUrl = $"http://localhost:5227/api/pokemonTemplate/{templateId}";
                    HttpResponseMessage response = httpClient.GetAsync(pokemonTemplateApiUrl).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        string jsonResult = response.Content.ReadAsStringAsync().Result;

                        var templateData = new { Nom = "", Type = Entities.PokemonType.Normal };
                        var pokemonTemplate = JsonConvert.DeserializeAnonymousType(jsonResult, templateData);

                        if (pokemonTemplate == null)
                        {
                            throw new Exception($"Failed to retrieve random Pokemon template. Status code: {response.StatusCode}");
                        }

                        // Create a new Pokemon using the retrieved template
                        Entities.Pokemon templatePokemon = new Entities.Pokemon(
                            0,
                            -1,
                            pokemonTemplate!.Nom,
                            pokemonTemplate!.Type,
                            false // Set the Shiny property as needed
                        );

                        return templatePokemon;
                    }
                    else
                    {
                        throw new Exception($"Failed to retrieve Pokemon template with ID {templateId}. Status code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that might occur during the process
                throw new Exception($"An error occurred while getting Pokemon template with ID {templateId}: {ex.Message}");
            }

        }

        private bool ShouldPokemonBeShiny()
        {
            Random random = new Random();
            return random.Next(512) == 0; // Probability of 1/512
        }
    }
}
